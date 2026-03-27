using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AI_Integration.DataAccess.Database.Models;
using AI_Integration.DataAccess.Database.Repositories.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AI_Integration.Helpers;

namespace AI_Integration.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class StaysController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public StaysController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public sealed class StayDto
        {
            public int Id { get; set; }
            public int TripId { get; set; }
            public string Name { get; set; } = null!;
            public string? Description { get; set; }
            public string? Image { get; set; }
            public string? Location { get; set; }
            public int OrderIndex { get; set; }
            public int? ItineraryDayId { get; set; }
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int? tripId, [FromHeader(Name = "User-Agent")] string userAgent = "")
        {
            var log = WebApiLogHelper.NewLog("GET", "api/stays", $"tripId={tripId}", userAgent, "List stays");
            var sw = Stopwatch.StartNew();
            try
            {
                var query = _unitOfWork.Query<Stay>().Where(s => s.IsDeleted != true);
                if (tripId.HasValue)
                {
                    query = query.Where(s => s.TripId == tripId.Value);
                }

                var stays = await query.OrderBy(s => s.OrderIndex).Select(s => new StayDto
                {
                    Id = s.Id,
                    TripId = s.TripId,
                    Name = s.Name,
                    Description = s.Description,
                    Image = s.Image,
                    Location = s.Location,
                    OrderIndex = s.OrderIndex,
                    ItineraryDayId = s.ItineraryDayId
                }).ToListAsync();

                sw.Stop();
                await WebApiLogHelper.LogOkAsync(_unitOfWork, log, $"{{ success = true, count = {stays.Count} }}", $"ElapsedMs={sw.ElapsedMilliseconds}");
                return Ok(stays);
            }
            catch (Exception ex)
            {
                sw.Stop();
                await WebApiLogHelper.LogErrorAsync(_unitOfWork, log, ex, $"ElapsedMs={sw.ElapsedMilliseconds}");
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, [FromHeader(Name = "User-Agent")] string userAgent = "")
        {
            var log = WebApiLogHelper.NewLog("GET", $"api/stays/{id}", "", userAgent, "Get stay by id");
            var sw = Stopwatch.StartNew();
            try
            {
                var stay = await _unitOfWork.Query<Stay>().Where(s => s.IsDeleted != true && s.Id == id)
                    .Select(s => new StayDto
                    {
                        Id = s.Id,
                        TripId = s.TripId,
                        Name = s.Name,
                        Description = s.Description,
                        Image = s.Image,
                        Location = s.Location,
                        OrderIndex = s.OrderIndex,
                        ItineraryDayId = s.ItineraryDayId
                    }).FirstOrDefaultAsync();

                if (stay == null)
                {
                    sw.Stop();
                    await WebApiLogHelper.LogNotFoundAsync(_unitOfWork, log, "{ success = false, message = 'Stay not found.' }", $"ElapsedMs={sw.ElapsedMilliseconds}");
                    return NotFound(new { success = false, message = "Stay not found." });
                }
                sw.Stop();
                await WebApiLogHelper.LogOkAsync(_unitOfWork, log, "{ success = true }", $"ElapsedMs={sw.ElapsedMilliseconds}");
                return Ok(stay);
            }
            catch (Exception ex)
            {
                sw.Stop();
                await WebApiLogHelper.LogErrorAsync(_unitOfWork, log, ex, $"ElapsedMs={sw.ElapsedMilliseconds}");
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Stay stay, [FromHeader(Name = "User-Agent")] string userAgent = "")
        {
            var log = WebApiLogHelper.NewLog("POST", "api/stays", stay?.ToString(), userAgent, "Create stay");
            var sw = Stopwatch.StartNew();
            if (stay == null) return BadRequest(new { success = false, message = "Stay info is required." });

            if (stay.TripId <= 0) ModelState.AddModelError("TripId", "TripId is required.");
            if (string.IsNullOrWhiteSpace(stay.Name)) ModelState.AddModelError("Name", "Name is required.");

            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                stay.CreationTime = DateTime.Now;
                await _unitOfWork.InsertAsync(stay);
                await _unitOfWork.SaveChangesAsync();
                sw.Stop();
                await WebApiLogHelper.LogOkAsync(_unitOfWork, log, "{ success = true }", $"ElapsedMs={sw.ElapsedMilliseconds}");
                return Ok(new { success = true, id = stay.Id });
            }
            catch (Exception ex)
            {
                sw.Stop();
                await WebApiLogHelper.LogErrorAsync(_unitOfWork, log, ex, $"ElapsedMs={sw.ElapsedMilliseconds}");
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] Stay changes, [FromHeader(Name = "User-Agent")] string userAgent = "")
        {
            var log = WebApiLogHelper.NewLog("PUT", $"api/stays/{id}", changes?.ToString(), userAgent, "Update stay");
            var sw = Stopwatch.StartNew();
            if (changes == null) return BadRequest(new { success = false, message = "Stay info is required." });

            try
            {
                var stay = await _unitOfWork.GetByIdAsync<Stay>(id);
                if (stay == null || stay.IsDeleted == true) return NotFound(new { success = false, message = "Stay not found." });

                stay.TripId = changes.TripId != 0 ? changes.TripId : stay.TripId;
                stay.Name = changes.Name ?? stay.Name;
                stay.Description = changes.Description ?? stay.Description;
                stay.Image = changes.Image ?? stay.Image;
                stay.Location = changes.Location ?? stay.Location;
                stay.OrderIndex = changes.OrderIndex;
                stay.ItineraryDayId = changes.ItineraryDayId;

                stay.LastModificationTime = DateTime.Now;

                _unitOfWork.Update(stay);
                await _unitOfWork.SaveChangesAsync();
                sw.Stop();
                await WebApiLogHelper.LogOkAsync(_unitOfWork, log, "{ success = true }", $"ElapsedMs={sw.ElapsedMilliseconds}");
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                sw.Stop();
                await WebApiLogHelper.LogErrorAsync(_unitOfWork, log, ex, $"ElapsedMs={sw.ElapsedMilliseconds}");
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, [FromHeader(Name = "User-Agent")] string userAgent = "")
        {
            var log = WebApiLogHelper.NewLog("DELETE", $"api/stays/{id}", "", userAgent, "Soft delete stay");
            var sw = Stopwatch.StartNew();
            try
            {
                var stay = await _unitOfWork.GetByIdAsync<Stay>(id);
                if (stay == null || stay.IsDeleted == true) return NotFound(new { success = false, message = "Stay not found." });

                stay.IsDeleted = true;
                stay.DeletionTime = DateTime.Now;

                _unitOfWork.Update(stay);
                await _unitOfWork.SaveChangesAsync();
                sw.Stop();
                await WebApiLogHelper.LogNoContentAsync(_unitOfWork, log, "{ success = true }", $"ElapsedMs={sw.ElapsedMilliseconds}");
                return NoContent();
            }
            catch (Exception ex)
            {
                sw.Stop();
                await WebApiLogHelper.LogErrorAsync(_unitOfWork, log, ex, $"ElapsedMs={sw.ElapsedMilliseconds}");
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }
    }
}
