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

        [HttpPost("CollegaExpert")]
        public async Task<IActionResult> CollegaExpert([FromBody] EventExpert item, [FromHeader(Name = "User-Agent")] string userAgent = "")
        {
            var log = WebApiLogHelper.NewLog("POST", "api/eventexperts/CollegaExpert", item?.ToString(), userAgent, "Link expert to event via SP");
            var sw = Stopwatch.StartNew();
            if (item == null) return BadRequest();
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var sql = "EXEC [dbo].[sp_CollegaExpertAEvent] @EventId={0}, @ExpertId={1}";
                var results = await _unitOfWork.Context.EventExpertCreationResults.FromSqlRaw(sql,
                    item.EventId,
                    item.ExpertId
                ).ToListAsync();

                sw.Stop();
                await WebApiLogHelper.LogOkAsync(_unitOfWork, log, "{ success = true }", $"ElapsedMs={sw.ElapsedMilliseconds}");

                var result = results.FirstOrDefault();
                if (result != null)
                {
                    if (result.EventExpertId == 0)
                        return BadRequest(new { success = false, message = "This expert is already linked to this event." });

                    return Ok(new { success = true, id = result.EventExpertId });
                }
                return Ok(new { success = true });
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
