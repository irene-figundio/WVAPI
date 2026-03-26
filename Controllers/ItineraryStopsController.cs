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
    public class ItineraryStopsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ItineraryStopsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public sealed class ItineraryStopDto
        {
            public int Id { get; set; }
            public int DayId { get; set; }
            public TimeSpan? Time { get; set; }
            public string Title { get; set; } = null!;
            public string? Description { get; set; }
            public string Type { get; set; } = null!;
            public int OrderIndex { get; set; }
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int? dayId, [FromHeader(Name = "User-Agent")] string userAgent = "")
        {
            var log = WebApiLogHelper.NewLog("GET", "api/itinerarystops", $"dayId={dayId}", userAgent, "List itinerary stops");
            var sw = Stopwatch.StartNew();
            try
            {
                var query = _unitOfWork.Query<ItineraryStop>().Where(s => s.IsDeleted != true);
                if (dayId.HasValue)
                {
                    query = query.Where(s => s.DayId == dayId.Value);
                }

                var stops = await query.OrderBy(s => s.OrderIndex).ThenBy(s => s.Time).Select(s => new ItineraryStopDto
                {
                    Id = s.Id,
                    DayId = s.DayId,
                    Time = s.Time,
                    Title = s.Title,
                    Description = s.Description,
                    Type = s.Type,
                    OrderIndex = s.OrderIndex
                }).ToListAsync();

                sw.Stop();
                await WebApiLogHelper.LogOkAsync(_unitOfWork, log, $"{{ success = true, count = {stops.Count} }}", $"ElapsedMs={sw.ElapsedMilliseconds}");
                return Ok(stops);
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
            var log = WebApiLogHelper.NewLog("GET", $"api/itinerarystops/{id}", "", userAgent, "Get itinerary stop by id");
            var sw = Stopwatch.StartNew();
            try
            {
                var stop = await _unitOfWork.Query<ItineraryStop>().Where(s => s.IsDeleted != true && s.Id == id)
                    .Select(s => new ItineraryStopDto
                    {
                        Id = s.Id,
                        DayId = s.DayId,
                        Time = s.Time,
                        Title = s.Title,
                        Description = s.Description,
                        Type = s.Type,
                        OrderIndex = s.OrderIndex
                    }).FirstOrDefaultAsync();

                if (stop == null)
                {
                    sw.Stop();
                    await WebApiLogHelper.LogNotFoundAsync(_unitOfWork, log, "{ success = false, message = 'Itinerary stop not found.' }", $"ElapsedMs={sw.ElapsedMilliseconds}");
                    return NotFound(new { success = false, message = "Itinerary stop not found." });
                }
                sw.Stop();
                await WebApiLogHelper.LogOkAsync(_unitOfWork, log, "{ success = true }", $"ElapsedMs={sw.ElapsedMilliseconds}");
                return Ok(stop);
            }
            catch (Exception ex)
            {
                sw.Stop();
                await WebApiLogHelper.LogErrorAsync(_unitOfWork, log, ex, $"ElapsedMs={sw.ElapsedMilliseconds}");
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] ItineraryStop stop, [FromHeader(Name = "User-Agent")] string userAgent = "")
        {
            var log = WebApiLogHelper.NewLog("POST", "api/itinerarystops", stop?.ToString(), userAgent, "Create itinerary stop");
            var sw = Stopwatch.StartNew();
            if (stop == null) return BadRequest(new { success = false, message = "Itinerary stop info is required." });

            if (stop.DayId <= 0) ModelState.AddModelError("DayId", "DayId is required.");
            if (string.IsNullOrWhiteSpace(stop.Title)) ModelState.AddModelError("Title", "Title is required.");

            var validTypes = new[] { "activity", "meal", "transfer", "experience" };
            if (!validTypes.Contains(stop.Type)) ModelState.AddModelError("Type", "Invalid type.");

            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                stop.CreationTime = DateTime.Now;
                await _unitOfWork.InsertAsync(stop);
                await _unitOfWork.SaveChangesAsync();
                sw.Stop();
                await WebApiLogHelper.LogOkAsync(_unitOfWork, log, "{ success = true }", $"ElapsedMs={sw.ElapsedMilliseconds}");
                return Ok(new { success = true, id = stop.Id });
            }
            catch (Exception ex)
            {
                sw.Stop();
                await WebApiLogHelper.LogErrorAsync(_unitOfWork, log, ex, $"ElapsedMs={sw.ElapsedMilliseconds}");
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] ItineraryStop changes, [FromHeader(Name = "User-Agent")] string userAgent = "")
        {
            var log = WebApiLogHelper.NewLog("PUT", $"api/itinerarystops/{id}", changes?.ToString(), userAgent, "Update itinerary stop");
            var sw = Stopwatch.StartNew();
            if (changes == null) return BadRequest(new { success = false, message = "Itinerary stop info is required." });

            try
            {
                var stop = await _unitOfWork.GetByIdAsync<ItineraryStop>(id);
                if (stop == null || stop.IsDeleted == true) return NotFound(new { success = false, message = "Itinerary stop not found." });

                stop.DayId = changes.DayId != 0 ? changes.DayId : stop.DayId;
                stop.Time = changes.Time;
                stop.Title = changes.Title ?? stop.Title;
                stop.Description = changes.Description ?? stop.Description;
                stop.Type = changes.Type ?? stop.Type;
                stop.OrderIndex = changes.OrderIndex;

                stop.LastModificationTime = DateTime.Now;

                _unitOfWork.Update(stop);
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
            var log = WebApiLogHelper.NewLog("DELETE", $"api/itinerarystops/{id}", "", userAgent, "Soft delete itinerary stop");
            var sw = Stopwatch.StartNew();
            try
            {
                var stop = await _unitOfWork.GetByIdAsync<ItineraryStop>(id);
                if (stop == null || stop.IsDeleted == true) return NotFound(new { success = false, message = "Itinerary stop not found." });

                stop.IsDeleted = true;
                stop.DeletionTime = DateTime.Now;

                _unitOfWork.Update(stop);
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
