using AI_Integration.DataAccess.Database.Models;
using AI_Integration.DataAccess.Database.Repositories.interfaces;
using AI_Integration.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AI_Integration.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TripsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public TripsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public sealed class TripDto
        {
            public int Id { get; set; }
            public int EventId { get; set; }
            public string DepartureCity { get; set; } = null!;
            public string DepartureCountry { get; set; } = null!;
            public string ArrivalCity { get; set; } = null!;
            public string? ArrivalCountry { get; set; }
            public int DurationDays { get; set; }
            public int DurationNights { get; set; }
            public int MaxGuests { get; set; }
            public string Status { get; set; } = null!;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int? eventId, [FromHeader(Name = "User-Agent")] string userAgent = "")
        {
            var log = WebApiLogHelper.NewLog("GET", "api/trips", $"eventId={eventId}", userAgent, "List trips");
            var sw = Stopwatch.StartNew();
            try
            {
                var query = _unitOfWork.Query<Trip>().Where(t => t.IsDeleted != true);
                if (eventId.HasValue)
                {
                    query = query.Where(t => t.EventId == eventId.Value);
                }

                var trips = await query.Select(t => new TripDto
                {
                    Id = t.Id,
                    EventId = t.EventId,
                    DepartureCity = t.DepartureCity,
                    DepartureCountry = t.DepartureCountry,
                    ArrivalCity = t.ArrivalCity,
                    ArrivalCountry = t.ArrivalCountry,
                    DurationDays = t.DurationDays,
                    DurationNights = t.DurationNights,
                    MaxGuests = t.MaxGuests,
                    Status = t.Status
                }).ToListAsync();

                sw.Stop();
                await WebApiLogHelper.LogOkAsync(_unitOfWork, log, $"{{ success = true, count = {trips.Count} }}", $"ElapsedMs={sw.ElapsedMilliseconds}");
                return Ok(trips);
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
            var log = WebApiLogHelper.NewLog("GET", $"api/trips/{id}", "", userAgent, "Get trip by id");
            var sw = Stopwatch.StartNew();
            try
            {
                var trip = await _unitOfWork.Query<Trip>().Where(t => t.IsDeleted != true && t.Id == id)
                    .Select(t => new TripDto
                    {
                        Id = t.Id,
                        EventId = t.EventId,
                        DepartureCity = t.DepartureCity,
                        DepartureCountry = t.DepartureCountry,
                        ArrivalCity = t.ArrivalCity,
                        ArrivalCountry = t.ArrivalCountry,
                        DurationDays = t.DurationDays,
                        DurationNights = t.DurationNights,
                        MaxGuests = t.MaxGuests,
                        Status = t.Status
                    }).FirstOrDefaultAsync();

                if (trip == null)
                {
                    sw.Stop();
                    await WebApiLogHelper.LogNotFoundAsync(_unitOfWork, log, "{ success = false, message = 'Trip not found.' }", $"ElapsedMs={sw.ElapsedMilliseconds}");
                    return NotFound(new { success = false, message = "Trip not found." });
                }
                sw.Stop();
                await WebApiLogHelper.LogOkAsync(_unitOfWork, log, "{ success = true }", $"ElapsedMs={sw.ElapsedMilliseconds}");
                return Ok(trip);
            }
            catch (Exception ex)
            {
                sw.Stop();
                await WebApiLogHelper.LogErrorAsync(_unitOfWork, log, ex, $"ElapsedMs={sw.ElapsedMilliseconds}");
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] TripDto model, [FromHeader(Name = "User-Agent")] string userAgent = "")
        {
            var log = WebApiLogHelper.NewLog("POST", "api/trips", model?.ToString(), userAgent, "Create trip");
            var sw = Stopwatch.StartNew();
            if (model == null) return BadRequest(new { success = false, message = "Trip info is required." });

            // Manual validation according to requirements
            if (model.EventId <= 0) ModelState.AddModelError("EventId", "EventId is required.");
            if (string.IsNullOrWhiteSpace(model.DepartureCity)) ModelState.AddModelError("DepartureCity", "DepartureCity is required.");
            if (string.IsNullOrWhiteSpace(model.DepartureCountry)) ModelState.AddModelError("DepartureCountry", "DepartureCountry is required.");
            if (string.IsNullOrWhiteSpace(model.ArrivalCity)) ModelState.AddModelError("ArrivalCity", "ArrivalCity is required.");
            if (model.DurationDays < 0) ModelState.AddModelError("DurationDays", "DurationDays must be >= 0.");
            if (model.DurationNights < 0) ModelState.AddModelError("DurationNights", "DurationNights must be >= 0.");
            if (model.MaxGuests < 1) ModelState.AddModelError("MaxGuests", "MaxGuests must be >= 1.");

            var validStatuses = new[] { "done", "cancelled", "booking", "started", "in_progress" };
            if (!validStatuses.Contains(model.Status)) ModelState.AddModelError("Status", "Invalid status.");

            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var trip = new Trip 
                {
                    EventId = model.EventId,
                    DepartureCity = model.DepartureCity,
                    DepartureCountry = model.DepartureCountry,
                    ArrivalCity = model.ArrivalCity,
                    ArrivalCountry = model.ArrivalCountry,
                    DurationDays = model.DurationDays,
                    DurationNights = model.DurationNights,
                    MaxGuests = model.MaxGuests,
                    Status = model.Status
                };
                trip.CreationTime = DateTime.Now;
                trip.LastModificationTime = DateTime.Now;
                trip.IsDeleted = false;
                await _unitOfWork.InsertAsync(trip);
                await _unitOfWork.SaveChangesAsync();
                sw.Stop();
                await WebApiLogHelper.LogOkAsync(_unitOfWork, log, "{ success = true }", $"ElapsedMs={sw.ElapsedMilliseconds}");
                return Ok(new { success = true, id = trip.Id });
            }
            catch (Exception ex)
            {
                sw.Stop();
                await WebApiLogHelper.LogErrorAsync(_unitOfWork, log, ex, $"ElapsedMs={sw.ElapsedMilliseconds}");
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] Trip changes, [FromHeader(Name = "User-Agent")] string userAgent = "")
        {
            var userId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var parsedId) ? parsedId : 0;
            var log = WebApiLogHelper.NewLog("PUT", $"api/trips/{id}", changes?.ToString(), userAgent, "Update trip");
            var sw = Stopwatch.StartNew();
            if (changes == null) return BadRequest(new { success = false, message = "Trip info is required." });

            try
            {
                var trip = await _unitOfWork.GetByIdAsync<Trip>(id);
                if (trip == null || trip.IsDeleted == true) return NotFound(new { success = false, message = "Trip not found." });

                trip.EventId = changes.EventId != 0 ? changes.EventId : trip.EventId;
                trip.DepartureCity = changes.DepartureCity ?? trip.DepartureCity;
                trip.DepartureCountry = changes.DepartureCountry ?? trip.DepartureCountry;
                trip.ArrivalCity = changes.ArrivalCity ?? trip.ArrivalCity;
                trip.ArrivalCountry = changes.ArrivalCountry ?? trip.ArrivalCountry;
                trip.DurationDays = changes.DurationDays >= 0 ? changes.DurationDays : trip.DurationDays;
                trip.DurationNights = changes.DurationNights >= 0 ? changes.DurationNights : trip.DurationNights;
                trip.MaxGuests = changes.MaxGuests >= 1 ? changes.MaxGuests : trip.MaxGuests;
                trip.Status = changes.Status ?? trip.Status;

                trip.LastModificationTime = DateTime.Now;
                trip.LastModification_User = userId;

                _unitOfWork.Update(trip);
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
            var userId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var parsedId) ? parsedId : 0;
            var log = WebApiLogHelper.NewLog("DELETE", $"api/trips/{id}", "", userAgent, "Soft delete trip");
            var sw = Stopwatch.StartNew();
            try
            {
                var trip = await _unitOfWork.GetByIdAsync<Trip>(id);
                if (trip == null || trip.IsDeleted == true) return NotFound(new { success = false, message = "Trip not found." });

                trip.IsDeleted = true;
                trip.DeletionTime = DateTime.Now;
                trip.Deletion_User = userId;
                _unitOfWork.Update(trip);
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
