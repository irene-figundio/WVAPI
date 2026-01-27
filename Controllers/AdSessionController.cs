using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AI_Integration.DataAccess;
using AI_Integration.DataAccess.Database.Models;
using AI_Integration.DataAccess.Database.Repositories.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // AnyAsync, ToListAsync, Include
using AI_Integration.Helpers;

namespace AI_Integration.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AdSessionController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IUnitOfWork _unitOfWork;
        private readonly VideoHelper _videoHelper;

        public AdSessionController(IWebHostEnvironment environment, IUnitOfWork unitOfWork,
            VideoHelper videoHelper)
        {
            _environment = environment;
            _unitOfWork = unitOfWork;
            _videoHelper = videoHelper;
        }

        // GET: api/AdSession
        // Opzionale: filtro per campaignId
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int? campaignId = null,
            [FromHeader(Name = "User-Agent")] string userAgent = "")
        {
            var url = $"api/adsession{(campaignId.HasValue ? $"?campaignId={campaignId.Value}" : string.Empty)}";
            var log = WebApiLogHelper.NewLog("GET", url, "", userAgent, "List sessions");
            var sw = Stopwatch.StartNew();

            try
            {
                var q = _unitOfWork.Query<AdSession>()
                                   .Where(s => s.IsDeleted != true);

                if (campaignId.HasValue)
                    q = q.Where(s => s.ID_Campaing == campaignId.Value);

                var sessions = await q.OrderBy(s => s.StartDate).ToListAsync();

                foreach (var session in sessions)
                {
                    var campaign = await _unitOfWork.GetByIdAsync<AdCampaign>(session.ID_Campaing);
                    session.Campaign = campaign;
                    session.Videos = await _unitOfWork.Query<AIVideo>()
                            .Where(v => v.ID_Session == session.Id && v.IsDeleted != true)
                            .ToListAsync();
                }

                sw.Stop();
                await WebApiLogHelper.LogOkAsync(_unitOfWork, log,
                    $"{{ success = true, count = {sessions.Count} }}",
                    $"ElapsedMs={sw.ElapsedMilliseconds}");

                return Ok(sessions);
            }
            catch (Exception ex)
            {
                sw.Stop();
                await WebApiLogHelper.LogErrorAsync(_unitOfWork, log, ex, $"ElapsedMs={sw.ElapsedMilliseconds}");
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // GET: api/AdSession/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id,
            [FromHeader(Name = "User-Agent")] string userAgent = "")
        {
            var log = WebApiLogHelper.NewLog("GET", $"api/adsession/{id}", "", userAgent, "Get session by id");
            var sw = Stopwatch.StartNew();

            try
            {
                if (id <= 0)
                {
                    sw.Stop();
                    await WebApiLogHelper.LogBadRequestAsync(_unitOfWork, log,
                        "{ success = false, message = 'Invalid session ID.' }",
                        $"ElapsedMs={sw.ElapsedMilliseconds}");
                    return BadRequest(new { success = false, message = "Invalid session ID." });

                }

                var q = _unitOfWork.Query<AdSession>()
                                   .Where(s => s.IsDeleted != true && s.Id == id);

                var result = await q
                    .OrderBy(s => s.StartDate)
                    .Include(s => s.Campaign)
                    .Include(s => s.Videos.Where(v => v.IsDeleted != true))
                    .Select(s => new
                    {
                        s.Id,
                        s.StartDate,
                        s.EndDate,
                        Campaign = new { s.Campaign.Id, s.Campaign.Name },
                        Videos = s.Videos.Select(v => new { v.Id, v.Title, Url_Video = _videoHelper.BuildPublicUrl(v.Url_Video, v.IsLandscape ?? false) })
                    })
                    .ToListAsync();

                sw.Stop();

                if (result == null || result.Count == 0)
                {
                    await WebApiLogHelper.LogNotFoundAsync(_unitOfWork, log,
                        "{ success = false, message = 'Session not found.' }",
                        $"ElapsedMs={sw.ElapsedMilliseconds}");
                    return NotFound(new { success = false, message = "Session not found." });
                }

                await WebApiLogHelper.LogOkAsync(_unitOfWork, log,
                    "{ success = true }",
                    $"ElapsedMs={sw.ElapsedMilliseconds}");
                return Ok(result.FirstOrDefault());
            }
            catch (Exception ex)
            {
                sw.Stop();
                await WebApiLogHelper.LogErrorAsync(_unitOfWork, log, ex, $"ElapsedMs={sw.ElapsedMilliseconds}");
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // POST: api/AdSession/add
        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] AdSession session,
            [FromHeader(Name = "User-Agent")] string userAgent)
        {
            var log = WebApiLogHelper.NewLog("POST", "api/adsession/add", session?.ToString(), userAgent, "Create session");
            var sw = Stopwatch.StartNew();

            // Validazioni base
            if (session == null)
            {
                sw.Stop();
                await WebApiLogHelper.LogBadRequestAsync(_unitOfWork, log,
                    "{ success = false, message = 'Session information is required.' }",
                    $"ElapsedMs={sw.ElapsedMilliseconds}");
                return BadRequest(new { success = false, message = "Session information is required." });
            }

            // ID_Campaing è int NOT NULL: controlliamo esistenza campagna
            var campaignExists = await _unitOfWork.Query<AdCampaign>()
                                                  .AnyAsync(c => c.Id == session.ID_Campaing && c.IsDeleted != true);
            if (!campaignExists)
            {
                sw.Stop();
                await WebApiLogHelper.LogBadRequestAsync(_unitOfWork, log,
                    "{ success = false, message = 'Invalid or missing Campaign ID.' }",
                    $"ElapsedMs={sw.ElapsedMilliseconds}");
                return BadRequest(new { success = false, message = "Invalid or missing Campaign ID." });
            }

            try
            {
                await _unitOfWork.InsertAsync(session);
                await _unitOfWork.SaveChangesAsync();

                sw.Stop();
                await WebApiLogHelper.LogOkAsync(_unitOfWork, log,
                    "{ success = true, message = 'Session added successfully.' }",
                    $"ElapsedMs={sw.ElapsedMilliseconds}");

                return Ok(new { success = true, message = "Session added successfully." });
            }
            catch (Exception ex)
            {
                sw.Stop();
                await WebApiLogHelper.LogErrorAsync(_unitOfWork, log, ex, $"ElapsedMs={sw.ElapsedMilliseconds}");
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // PUT: api/AdSession/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] AdSession changes,
            [FromHeader(Name = "User-Agent")] string userAgent)
        {
            var log = WebApiLogHelper.NewLog("PUT", $"api/adsession/{id}", changes?.ToString(), userAgent, "Update session");
            var sw = Stopwatch.StartNew();

            try
            {
                var session = await _unitOfWork.GetByIdAsync<AdSession>(id);
                if (session == null || session.IsDeleted == true)
                {
                    sw.Stop();
                    await WebApiLogHelper.LogNotFoundAsync(_unitOfWork, log,
                        "{ success = false, message = 'Session not found.' }",
                        $"ElapsedMs={sw.ElapsedMilliseconds}");
                    return NotFound(new { success = false, message = "Session not found." });
                }

                if (changes == null)
                {
                    sw.Stop();
                    await WebApiLogHelper.LogBadRequestAsync(_unitOfWork, log,
                        "{ success = false, message = 'No changes provided.' }",
                        $"ElapsedMs={sw.ElapsedMilliseconds}");
                    return BadRequest(new { success = false, message = "No changes provided." });
                }
                // Applica modifiche minime (non cambiamo ID_Campaing qui; se vuoi farlo, aggiungi validazioni come in Add)
                if (changes.StartDate != default) session.StartDate = changes.StartDate;
                if (changes.EndDate != default) session.EndDate = changes.EndDate;

                session.LastModificationTime = DateTime.Now;
                _unitOfWork.Update(session);

                sw.Stop();
                await WebApiLogHelper.LogOkAsync(_unitOfWork, log,
                    "{ success = true }",
                    $"ElapsedMs={sw.ElapsedMilliseconds}");
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                sw.Stop();
                await WebApiLogHelper.LogErrorAsync(_unitOfWork, log, ex, $"ElapsedMs={sw.ElapsedMilliseconds}");
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // DELETE: api/AdSession/5  (soft delete)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id,
            [FromHeader(Name = "User-Agent")] string userAgent)
        {
            var log = WebApiLogHelper.NewLog("DELETE", $"api/adsession/{id}", "", userAgent, "Soft delete session");
            var sw = Stopwatch.StartNew();

            try
            {
                var session = await _unitOfWork.GetByIdAsync<AdSession>(id);
                if (session == null || session.IsDeleted == true)
                {
                    sw.Stop();
                    await WebApiLogHelper.LogNotFoundAsync(_unitOfWork, log,
                        "{ success = false, message = 'Session not found.' }",
                        $"ElapsedMs={sw.ElapsedMilliseconds}");
                    return NotFound(new { success = false, message = "Session not found." });
                }

                session.IsDeleted = true;
                session.DeletionTime = DateTime.Now;
                _unitOfWork.Update(session);

                sw.Stop();
                await WebApiLogHelper.LogNoContentAsync(_unitOfWork, log,
                    "{ success = true }",
                    $"ElapsedMs={sw.ElapsedMilliseconds}");
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
