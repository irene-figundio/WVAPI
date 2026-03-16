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
    public class ContentLinksController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ContentLinksController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromHeader(Name = "User-Agent")] string userAgent = "")
        {
            var log = WebApiLogHelper.NewLog("GET", "api/contentlinks", "", userAgent, "List contentlinks");
            var sw = Stopwatch.StartNew();
            try
            {
                var items = await _unitOfWork.Query<ContentLink>().ToListAsync();
                sw.Stop();
                await WebApiLogHelper.LogOkAsync(_unitOfWork, log, $"{{ success = true, count = {items.Count} }}", $"ElapsedMs={sw.ElapsedMilliseconds}");
                return Ok(items);
            }
            catch (Exception ex)
            {
                sw.Stop();
                await WebApiLogHelper.LogErrorAsync(_unitOfWork, log, ex, $"ElapsedMs={sw.ElapsedMilliseconds}");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, [FromHeader(Name = "User-Agent")] string userAgent = "")
        {
            var log = WebApiLogHelper.NewLog("GET", $"api/contentlinks/{id}", "", userAgent, "Get contentlink by id");
            var sw = Stopwatch.StartNew();
            try
            {
                var item = await _unitOfWork.GetByIdAsync<ContentLink>(id);
                if (item == null)
                {
                    sw.Stop();
                    await WebApiLogHelper.LogNotFoundAsync(_unitOfWork, log, "{ success = false, message = 'Item not found.' }", $"ElapsedMs={sw.ElapsedMilliseconds}");
                    return NotFound();
                }
                sw.Stop();
                await WebApiLogHelper.LogOkAsync(_unitOfWork, log, "{ success = true }", $"ElapsedMs={sw.ElapsedMilliseconds}");
                return Ok(item);
            }
            catch (Exception ex)
            {
                sw.Stop();
                await WebApiLogHelper.LogErrorAsync(_unitOfWork, log, ex, $"ElapsedMs={sw.ElapsedMilliseconds}");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] ContentLink item, [FromHeader(Name = "User-Agent")] string userAgent = "")
        {
            var log = WebApiLogHelper.NewLog("POST", "api/contentlinks/add", item?.ToString(), userAgent, "Create contentlink via SP");
            var sw = Stopwatch.StartNew();
            if (item == null) return BadRequest();
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var sql = "EXEC [dbo].[sp_CreaContentLink] @ContentId={0}, @LinkUrl={1}, @Description={2}, @LangID={3}";
                var results = await _unitOfWork.Context.ContentLinkCreationResults.FromSqlRaw(sql,
                    item.ContentId,
                    item.LinkUrl,
                    item.Description ?? (object)DBNull.Value,
                    item.LangID != 0 ? item.LangID : 1
                ).ToListAsync();

                sw.Stop();
                var result = results.FirstOrDefault();
                if (result != null)
                {
                    await WebApiLogHelper.LogOkAsync(_unitOfWork, log, "{ success = true }", $"ElapsedMs={sw.ElapsedMilliseconds}");
                    return Ok(new { success = true, id = result.ContentLinkId });
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

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] ContentLink changes, [FromHeader(Name = "User-Agent")] string userAgent = "")
        {
            var log = WebApiLogHelper.NewLog("PUT", $"api/contentlinks/{id}", changes?.ToString(), userAgent, "Update contentlink");
            var sw = Stopwatch.StartNew();
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var item = await _unitOfWork.GetByIdAsync<ContentLink>(id);
                if (item == null)
                {
                    sw.Stop();
                    await WebApiLogHelper.LogNotFoundAsync(_unitOfWork, log, "{ success = false, message = 'Item not found.' }", $"ElapsedMs={sw.ElapsedMilliseconds}");
                    return NotFound();
                }

                // Basic mapping (better to use Automapper or similar, but following pattern)
                item.LinkUrl = changes.LinkUrl ?? item.LinkUrl;
                item.Description = changes.Description ?? item.Description;
                item.LangID = changes.LangID != 0 ? changes.LangID : item.LangID;

                _unitOfWork.Update(item);
                await _unitOfWork.SaveChangesAsync();
                sw.Stop();
                await WebApiLogHelper.LogOkAsync(_unitOfWork, log, "{ success = true }", $"ElapsedMs={sw.ElapsedMilliseconds}");
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
            var log = WebApiLogHelper.NewLog("DELETE", $"api/contentlinks/{id}", "", userAgent, "Delete contentlink");
            var sw = Stopwatch.StartNew();
            try
            {
                var item = await _unitOfWork.GetByIdAsync<ContentLink>(id);
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
