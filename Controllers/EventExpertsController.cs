using System;
using System.Diagnostics;
using System.Threading.Tasks;
using AI_Integration.DataAccess;
using AI_Integration.DataAccess.Database.Models;
using AI_Integration.DataAccess.Database.Repositories.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AI_Integration.Helpers;
using Microsoft.EntityFrameworkCore;

namespace AI_Integration.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EventExpertsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public EventExpertsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] EventExpert item, [FromHeader(Name = "User-Agent")] string userAgent = "")
        {
            var log = WebApiLogHelper.NewLog("POST", "api/eventexperts", item?.ToString(), userAgent, "Link expert to event");
            var sw = Stopwatch.StartNew();
            if (item == null) return BadRequest();
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                // Check if connection already exists to prevent duplicates
                var existing = await _unitOfWork.Query<EventExpert>()
                    .FirstOrDefaultAsync(ee => ee.EventId == item.EventId && ee.ExpertId == item.ExpertId);

                if (existing != null)
                {
                    sw.Stop();
                    return BadRequest(new { success = false, message = "This expert is already linked to this event." });
                }

                await _unitOfWork.InsertAsync(item);
                await _unitOfWork.SaveChangesAsync();
                sw.Stop();
                await WebApiLogHelper.LogOkAsync(_unitOfWork, log, "{ success = true }", $"ElapsedMs={sw.ElapsedMilliseconds}");
                return Ok(new { success = true, id = item.Id });
            }
            catch (Exception ex)
            {
                sw.Stop();
                await WebApiLogHelper.LogErrorAsync(_unitOfWork, log, ex, $"ElapsedMs={sw.ElapsedMilliseconds}");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, [FromHeader(Name = "User-Agent")] string userAgent = "")
        {
            var log = WebApiLogHelper.NewLog("DELETE", $"api/eventexperts/{id}", "", userAgent, "Unlink expert from event");
            var sw = Stopwatch.StartNew();
            try
            {
                var item = await _unitOfWork.GetByIdAsync<EventExpert>(id);
                if (item == null)
                {
                    sw.Stop();
                    await WebApiLogHelper.LogNotFoundAsync(_unitOfWork, log, "{ success = false, message = 'Item not found.' }", $"ElapsedMs={sw.ElapsedMilliseconds}");
                    return NotFound();
                }
                _unitOfWork.Remove(item);
                await _unitOfWork.SaveChangesAsync();
                sw.Stop();
                await WebApiLogHelper.LogNoContentAsync(_unitOfWork, log, "{ success = true }", $"ElapsedMs={sw.ElapsedMilliseconds}");
                return NoContent();
            }
            catch (Exception ex)
            {
                sw.Stop();
                await WebApiLogHelper.LogErrorAsync(_unitOfWork, log, ex, $"ElapsedMs={sw.ElapsedMilliseconds}");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
