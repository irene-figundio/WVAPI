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
    public class PodcastsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public PodcastsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int langId = 1, [FromHeader(Name = "User-Agent")] string userAgent = "")
        {
            var log = WebApiLogHelper.NewLog("GET", "api/podcasts", $"langId={langId}", userAgent, "List podcasts");
            var sw = Stopwatch.StartNew();
            try
            {
                var podcasts = await _unitOfWork.Query<Podcast>().Where(e => e.IsDeleted != true)
                    .Where(p => p.LangID == langId)
                    .OrderByDescending(p => p.PublishDate)
                    .ToListAsync();
                sw.Stop();
                await WebApiLogHelper.LogOkAsync(_unitOfWork, log, $"{{ success = true, count = {podcasts.Count} }}", $"ElapsedMs={sw.ElapsedMilliseconds}");
                return Ok(podcasts);
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
            var log = WebApiLogHelper.NewLog("GET", $"api/podcasts/{id}", $"langId={langId}", userAgent, "Get podcast by id");
            var sw = Stopwatch.StartNew();
            try
            {
                var podcast = await _unitOfWork.Query<Podcast>().Where(e => e.IsDeleted != true)
                    .Where(p => p.Id == id && p.LangID == langId)
                    .FirstOrDefaultAsync();
                if (podcast == null)
                {
                    sw.Stop();
                    await WebApiLogHelper.LogNotFoundAsync(_unitOfWork, log, "{ success = false, message = 'Podcast not found.' }", $"ElapsedMs={sw.ElapsedMilliseconds}");
                    return NotFound(new { success = false, message = "Podcast not found." });
                }
                sw.Stop();
                await WebApiLogHelper.LogOkAsync(_unitOfWork, log, "{ success = true }", $"ElapsedMs={sw.ElapsedMilliseconds}");
                return Ok(podcast);
            }
            catch (Exception ex)
            {
                sw.Stop();
                await WebApiLogHelper.LogErrorAsync(_unitOfWork, log, ex, $"ElapsedMs={sw.ElapsedMilliseconds}");
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Podcast podcast, [FromHeader(Name = "User-Agent")] string userAgent = "")
        {
            var log = WebApiLogHelper.NewLog("POST", "api/podcasts", podcast?.ToString(), userAgent, "Create podcast");
            var sw = Stopwatch.StartNew();
            if (podcast == null) return BadRequest(new { success = false, message = "Podcast info is required." });
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                podcast.CreatedAt = DateTime.Now;
                await _unitOfWork.InsertAsync(podcast);
                await _unitOfWork.SaveChangesAsync();
                sw.Stop();
                await WebApiLogHelper.LogOkAsync(_unitOfWork, log, "{ success = true }", $"ElapsedMs={sw.ElapsedMilliseconds}");
                return Ok(new { success = true, id = podcast.Id });
            }
            catch (Exception ex)
            {
                sw.Stop();
                await WebApiLogHelper.LogErrorAsync(_unitOfWork, log, ex, $"ElapsedMs={sw.ElapsedMilliseconds}");
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] Podcast changes, [FromHeader(Name = "User-Agent")] string userAgent = "")
        {
            var log = WebApiLogHelper.NewLog("PUT", $"api/podcasts/{id}", changes?.ToString(), userAgent, "Update podcast");
            var sw = Stopwatch.StartNew();
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var podcast = await _unitOfWork.GetByIdAsync<Podcast>(id);
                if (podcast == null) return NotFound(new { success = false, message = "Podcast not found." });

                podcast.Title = changes.Title ?? podcast.Title;
                podcast.Description = changes.Description ?? podcast.Description;
                podcast.PublishDate = changes.PublishDate != default ? changes.PublishDate : podcast.PublishDate;
                podcast.CoverImage = changes.CoverImage ?? podcast.CoverImage;
                podcast.YoutubeUrl = changes.YoutubeUrl ?? podcast.YoutubeUrl;
                podcast.SpotifyUrl = changes.SpotifyUrl ?? podcast.SpotifyUrl;
                podcast.LangID = changes.LangID != 0 ? changes.LangID : podcast.LangID;

                _unitOfWork.Update(podcast);
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
            var log = WebApiLogHelper.NewLog("DELETE", $"api/podcasts/{id}", "", userAgent, "Delete podcast");
            var sw = Stopwatch.StartNew();
            try
            {
                var podcast = await _unitOfWork.GetByIdAsync<Podcast>(id);
                if (podcast == null) return NotFound(new { success = false, message = "Podcast not found." });
                podcast.IsDeleted = true;
                podcast.DeletionDate = DateTime.Now;
                _unitOfWork.Update(podcast);
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
