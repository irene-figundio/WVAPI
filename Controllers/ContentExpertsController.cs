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
    public class ContentExpertsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ContentExpertsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost("CollegaExpert")]
        public async Task<IActionResult> CollegaExpert([FromBody] ContentExpert item, [FromHeader(Name = "User-Agent")] string userAgent = "")
        {
            var log = WebApiLogHelper.NewLog("POST", "api/contentexperts/CollegaExpert", item?.ToString(), userAgent, "Link expert to content via SP");
            var sw = Stopwatch.StartNew();
            if (item == null) return BadRequest();
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var sql = "EXEC [dbo].[sp_CollegaExpertAContent] @ContentId={0}, @ExpertId={1}";
                var results = await _unitOfWork.Context.ContentExpertCreationResults.FromSqlRaw(sql,
                    item.ContentId,
                    item.ExpertId
                ).ToListAsync();

                sw.Stop();
                await WebApiLogHelper.LogOkAsync(_unitOfWork, log, "{ success = true }", $"ElapsedMs={sw.ElapsedMilliseconds}");

                var result = results.FirstOrDefault();
                if (result != null)
                {
                    if (result.ContentExpertId == 0)
                        return BadRequest(new { success = false, message = "This expert is already linked to this content." });

                    return Ok(new { success = true, id = result.ContentExpertId });
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
            var log = WebApiLogHelper.NewLog("DELETE", $"api/contentexperts/{id}", "", userAgent, "Unlink expert from content");
            var sw = Stopwatch.StartNew();
            try
            {
                var item = await _unitOfWork.GetByIdAsync<ContentExpert>(id);
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
