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
using System.Security.Claims;

namespace AI_Integration.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ItineraryDaysController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ItineraryDaysController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public sealed class ItineraryDayDto
        {
            public int Id { get; set; }
            public int TripId { get; set; }
            public int DayNumber { get; set; }
            public string Title { get; set; } = null!;
            public string? Description { get; set; }
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int? tripId, [FromHeader(Name = "User-Agent")] string userAgent = "")
        {
            var log = WebApiLogHelper.NewLog("GET", "api/itinerarydays", $"tripId={tripId}", userAgent, "List itinerary days");
            var sw = Stopwatch.StartNew();
            try
            {
                var query = _unitOfWork.Query<ItineraryDay>().Where(d => d.IsDeleted != true);
                if (tripId.HasValue)
                {
                    query = query.Where(d => d.TripId == tripId.Value);
                }

                var days = await query.OrderBy(d => d.DayNumber).Select(d => new ItineraryDayDto
                {
                    Id = d.Id,
                    TripId = d.TripId,
                    DayNumber = d.DayNumber,
                    Title = d.Title,
                    Description = d.Description
                }).ToListAsync();

                sw.Stop();
                await WebApiLogHelper.LogOkAsync(_unitOfWork, log, $"{{ success = true, count = {days.Count} }}", $"ElapsedMs={sw.ElapsedMilliseconds}");
                return Ok(days);
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
            var log = WebApiLogHelper.NewLog("GET", $"api/itinerarydays/{id}", "", userAgent, "Get itinerary day by id");
            var sw = Stopwatch.StartNew();
            try
            {
                var day = await _unitOfWork.Query<ItineraryDay>().Where(d => d.IsDeleted != true && d.Id == id)
                    .Select(d => new ItineraryDayDto
                    {
                        Id = d.Id,
                        TripId = d.TripId,
                        DayNumber = d.DayNumber,
                        Title = d.Title,
                        Description = d.Description
                    }).FirstOrDefaultAsync();

                if (day == null)
                {
                    sw.Stop();
                    await WebApiLogHelper.LogNotFoundAsync(_unitOfWork, log, "{ success = false, message = 'Itinerary day not found.' }", $"ElapsedMs={sw.ElapsedMilliseconds}");
                    return NotFound(new { success = false, message = "Itinerary day not found." });
                }
                sw.Stop();
                await WebApiLogHelper.LogOkAsync(_unitOfWork, log, "{ success = true }", $"ElapsedMs={sw.ElapsedMilliseconds}");
                return Ok(day);
            }
            catch (Exception ex)
            {
                sw.Stop();
                await WebApiLogHelper.LogErrorAsync(_unitOfWork, log, ex, $"ElapsedMs={sw.ElapsedMilliseconds}");
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] ItineraryDayDto model, [FromHeader(Name = "User-Agent")] string userAgent = "")
        {
            var log = WebApiLogHelper.NewLog("POST", "api/itinerarydays", model?.ToString(), userAgent, "Create itinerary day");
            var sw = Stopwatch.StartNew();
            if (model == null) return BadRequest(new { success = false, message = "Itinerary day info is required." });

            if (model.TripId <= 0) ModelState.AddModelError("TripId", "TripId is required.");
            if (model.DayNumber <= 0) ModelState.AddModelError("DayNumber", "DayNumber must be > 0.");
            if (string.IsNullOrWhiteSpace(model.Title)) ModelState.AddModelError("Title", "Title is required.");
            var userId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var parsedId) ? parsedId : 0;
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var day = new ItineraryDay
                { 
                    TripId = model.TripId,
                    DayNumber = model.DayNumber,
                    Title = model.Title,
                    Description = model.Description
                };

                day.CreationTime = DateTime.Now;
                day.Creation_User = userId;
                day.LastModificationTime = DateTime.Now;
                day.LastModification_User = userId;
                day.IsDeleted = false;

                await _unitOfWork.InsertAsync(day);
                await _unitOfWork.SaveChangesAsync();
                sw.Stop();
                await WebApiLogHelper.LogOkAsync(_unitOfWork, log, "{ success = true }", $"ElapsedMs={sw.ElapsedMilliseconds}");
                return Ok(new { success = true, id = day.Id });
            }
            catch (Exception ex)
            {
                sw.Stop();
                await WebApiLogHelper.LogErrorAsync(_unitOfWork, log, ex, $"ElapsedMs={sw.ElapsedMilliseconds}");
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] ItineraryDay changes, [FromHeader(Name = "User-Agent")] string userAgent = "")
        {
            var log = WebApiLogHelper.NewLog("PUT", $"api/itinerarydays/{id}", changes?.ToString(), userAgent, "Update itinerary day");
            var sw = Stopwatch.StartNew();
            if (changes == null) return BadRequest(new { success = false, message = "Itinerary day info is required." });

            try
            {
                var day = await _unitOfWork.GetByIdAsync<ItineraryDay>(id);
                if (day == null || day.IsDeleted == true) return NotFound(new { success = false, message = "Itinerary day not found." });

                day.TripId = changes.TripId != 0 ? changes.TripId : day.TripId;
                day.DayNumber = changes.DayNumber > 0 ? changes.DayNumber : day.DayNumber;
                day.Title = changes.Title ?? day.Title;
                day.Description = changes.Description ?? day.Description;

                day.LastModificationTime = DateTime.Now;

                _unitOfWork.Update(day);
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
            var log = WebApiLogHelper.NewLog("DELETE", $"api/itinerarydays/{id}", "", userAgent, "Soft delete itinerary day");
            var sw = Stopwatch.StartNew();
            try
            {
                var day = await _unitOfWork.GetByIdAsync<ItineraryDay>(id);
                if (day == null || day.IsDeleted == true) return NotFound(new { success = false, message = "Itinerary day not found." });

                day.IsDeleted = true;
                day.DeletionTime = DateTime.Now;

                _unitOfWork.Update(day);
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
