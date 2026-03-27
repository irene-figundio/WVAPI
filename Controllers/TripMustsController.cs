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
    public class TripMustsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public TripMustsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public sealed class TripMustDto
        {
            public int Id { get; set; }
            public int TripId { get; set; }
            public string Text { get; set; } = null!;
            public int TypeId { get; set; }
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int? tripId, [FromHeader(Name = "User-Agent")] string userAgent = "")
        {
            var log = WebApiLogHelper.NewLog("GET", "api/tripmusts", $"tripId={tripId}", userAgent, "List trip musts");
            var sw = Stopwatch.StartNew();
            try
            {
                var query = _unitOfWork.Query<TripMust>().Where(m => m.IsDeleted != true);
                if (tripId.HasValue)
                {
                    query = query.Where(m => m.TripId == tripId.Value);
                }

                var musts = await query.Select(m => new TripMustDto
                {
                    Id = m.Id,
                    TripId = m.TripId,
                    Text = m.Text,
                    TypeId = m.TypeId
                }).ToListAsync();

                sw.Stop();
                await WebApiLogHelper.LogOkAsync(_unitOfWork, log, $"{{ success = true, count = {musts.Count} }}", $"ElapsedMs={sw.ElapsedMilliseconds}");
                return Ok(musts);
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
            var log = WebApiLogHelper.NewLog("GET", $"api/tripmusts/{id}", "", userAgent, "Get trip must by id");
            var sw = Stopwatch.StartNew();
            try
            {
                var must = await _unitOfWork.Query<TripMust>().Where(m => m.IsDeleted != true && m.Id == id)
                    .Select(m => new TripMustDto
                    {
                        Id = m.Id,
                        TripId = m.TripId,
                        Text = m.Text,
                        TypeId = m.TypeId
                    }).FirstOrDefaultAsync();

                if (must == null)
                {
                    sw.Stop();
                    await WebApiLogHelper.LogNotFoundAsync(_unitOfWork, log, "{ success = false, message = 'Trip must not found.' }", $"ElapsedMs={sw.ElapsedMilliseconds}");
                    return NotFound(new { success = false, message = "Trip must not found." });
                }
                sw.Stop();
                await WebApiLogHelper.LogOkAsync(_unitOfWork, log, "{ success = true }", $"ElapsedMs={sw.ElapsedMilliseconds}");
                return Ok(must);
            }
            catch (Exception ex)
            {
                sw.Stop();
                await WebApiLogHelper.LogErrorAsync(_unitOfWork, log, ex, $"ElapsedMs={sw.ElapsedMilliseconds}");
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] TripMustDto model, [FromHeader(Name = "User-Agent")] string userAgent = "")
        {
            var userId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var parsedId) ? parsedId : 0;
            var log = WebApiLogHelper.NewLog("POST", "api/tripmusts", model?.ToString(), userAgent, "Create trip must");
            var sw = Stopwatch.StartNew();
            if (model == null) return BadRequest(new { success = false, message = "Trip must info is required." }); 
            if (model.TripId <= 0) ModelState.AddModelError("TripId", "TripId is required.");
            if (string.IsNullOrWhiteSpace(model.Text)) ModelState.AddModelError("Text", "Text is required.");
            if (model.TypeId <= 0) ModelState.AddModelError("TypeId", "TypeId is required (1: Inclusion, 2: Exclusion).");

            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var must = new TripMust
                {
                    TripId = model.TripId,
                    Text = model.Text,
                    TypeId = model.TypeId
                };
                must.CreationTime = DateTime.Now;
                must.Creation_User= userId;
                must.LastModificationTime = DateTime.Now;
                must.LastModification_User = userId;
                await _unitOfWork.InsertAsync(must);
                await _unitOfWork.SaveChangesAsync();
                sw.Stop();
                await WebApiLogHelper.LogOkAsync(_unitOfWork, log, "{ success = true }", $"ElapsedMs={sw.ElapsedMilliseconds}");
                return Ok(new { success = true, id = must.Id });
            }
            catch (Exception ex)
            {
                sw.Stop();
                await WebApiLogHelper.LogErrorAsync(_unitOfWork, log, ex, $"ElapsedMs={sw.ElapsedMilliseconds}");
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] TripMust changes, [FromHeader(Name = "User-Agent")] string userAgent = "")
        {
            var userId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var parsedId) ? parsedId : 0;
            var log = WebApiLogHelper.NewLog("PUT", $"api/tripmusts/{id}", changes?.ToString(), userAgent, "Update trip must");
            var sw = Stopwatch.StartNew();
            if (changes == null) return BadRequest(new { success = false, message = "Trip must info is required." });

            try
            {
                var must = await _unitOfWork.GetByIdAsync<TripMust>(id);
                if (must == null || must.IsDeleted == true) return NotFound(new { success = false, message = "Trip must not found." });

                must.TripId = changes.TripId != 0 ? changes.TripId : must.TripId;
                must.Text = changes.Text ?? must.Text;
                must.TypeId = changes.TypeId != 0 ? changes.TypeId : must.TypeId;

                must.LastModificationTime = DateTime.Now;
                must.LastModification_User = userId;

                _unitOfWork.Update(must);
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
            var log = WebApiLogHelper.NewLog("DELETE", $"api/tripmusts/{id}", "", userAgent, "Soft delete trip must");
            var sw = Stopwatch.StartNew();
            try
            {
                var must = await _unitOfWork.GetByIdAsync<TripMust>(id);
                if (must == null || must.IsDeleted == true) return NotFound(new { success = false, message = "Trip must not found." });

                must.IsDeleted = true;
                must.DeletionTime = DateTime.Now;
                must.Deletion_User = userId;

                _unitOfWork.Update(must);
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
