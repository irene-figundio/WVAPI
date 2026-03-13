using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AI_Integration.DataAccess;
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
    public class ExpertsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ExpertsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int langId = 1, [FromHeader(Name = "User-Agent")] string userAgent = "")
        {
            var log = WebApiLogHelper.NewLog("GET", "api/experts", $"langId={langId}", userAgent, "List experts");
            var sw = Stopwatch.StartNew();
            try
            {
                var experts = await _unitOfWork.Query<Expert>()
                    .Where(e => e.LangID == langId)
                    .OrderByDescending(e => e.CreatedAt)
                    .ToListAsync();
                sw.Stop();
                await WebApiLogHelper.LogOkAsync(_unitOfWork, log, $"{{ success = true, count = {experts.Count} }}", $"ElapsedMs={sw.ElapsedMilliseconds}");
                return Ok(experts);
            }
            catch (Exception ex)
            {
                sw.Stop();
                await WebApiLogHelper.LogErrorAsync(_unitOfWork, log, ex, $"ElapsedMs={sw.ElapsedMilliseconds}");
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, [FromQuery] int langId = 1, [FromHeader(Name = "User-Agent")] string userAgent = "")
        {
            var log = WebApiLogHelper.NewLog("GET", $"api/experts/{id}", $"langId={langId}", userAgent, "Get expert by id");
            var sw = Stopwatch.StartNew();
            try
            {
                var expert = await _unitOfWork.Query<Expert>()
                    .Where(e => e.Id == id && e.LangID == langId)
                    .FirstOrDefaultAsync();

                if (expert == null)
                {
                    sw.Stop();
                    await WebApiLogHelper.LogNotFoundAsync(_unitOfWork, log, "{ success = false, message = 'Expert not found.' }", $"ElapsedMs={sw.ElapsedMilliseconds}");
                    return NotFound(new { success = false, message = "Expert not found." });
                }

                sw.Stop();
                await WebApiLogHelper.LogOkAsync(_unitOfWork, log, "{ success = true }", $"ElapsedMs={sw.ElapsedMilliseconds}");
                return Ok(expert);
            }
            catch (Exception ex)
            {
                sw.Stop();
                await WebApiLogHelper.LogErrorAsync(_unitOfWork, log, ex, $"ElapsedMs={sw.ElapsedMilliseconds}");
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost("CreateExpert")]
        public async Task<IActionResult> CreateExpert([FromBody] Expert expert, [FromHeader(Name = "User-Agent")] string userAgent = "")
        {
            var log = WebApiLogHelper.NewLog("POST", "api/experts/CreateExpert", expert?.ToString(), userAgent, "Create expert via SP");
            var sw = Stopwatch.StartNew();
            if (expert == null) return BadRequest(new { success = false, message = "Expert info is required." });
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var sql = "EXEC [dbo].[sp_CreaExpert] @Name={0}, @Bio={1}, @PhotoUrl={2}, @Email={3}, @LangID={4}";
                var results = await _unitOfWork.Context.ExpertCreationResults.FromSqlRaw(sql,
                    expert.Name,
                    expert.Bio ?? (object)DBNull.Value,
                    expert.PhotoUrl ?? (object)DBNull.Value,
                    expert.Email ?? (object)DBNull.Value,
                    expert.LangID != 0 ? expert.LangID : 1
                ).ToListAsync();

                sw.Stop();
                await WebApiLogHelper.LogOkAsync(_unitOfWork, log, "{ success = true }", $"ElapsedMs={sw.ElapsedMilliseconds}");
                var result = results.FirstOrDefault();
                if (result != null)
                {
                    return Ok(new { success = true, id = result.ExpertId });
                }
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                sw.Stop();
                await WebApiLogHelper.LogErrorAsync(_unitOfWork, log, ex, $"ElapsedMs={sw.ElapsedMilliseconds}");
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] Expert changes, [FromHeader(Name = "User-Agent")] string userAgent = "")
        {
            var log = WebApiLogHelper.NewLog("PUT", $"api/experts/{id}", changes?.ToString(), userAgent, "Update expert");
            var sw = Stopwatch.StartNew();
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var expert = await _unitOfWork.GetByIdAsync<Expert>(id);
                if (expert == null) return NotFound(new { success = false, message = "Expert not found." });

                expert.Name = changes.Name ?? expert.Name;
                expert.Bio = changes.Bio ?? expert.Bio;
                expert.PhotoUrl = changes.PhotoUrl ?? expert.PhotoUrl;
                expert.Email = changes.Email ?? expert.Email;
                expert.LangID = changes.LangID != 0 ? changes.LangID : expert.LangID;

                _unitOfWork.Update(expert);
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
            var log = WebApiLogHelper.NewLog("DELETE", $"api/experts/{id}", "", userAgent, "Delete expert");
            var sw = Stopwatch.StartNew();
            try
            {
                var expert = await _unitOfWork.GetByIdAsync<Expert>(id);
                if (expert == null) return NotFound(new { success = false, message = "Expert not found." });

                _unitOfWork.Remove(expert);
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
