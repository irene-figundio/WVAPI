using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AI_Integration.DataAccess;
using AI_Integration.DataAccess.Database.Models;
using AI_Integration.DataAccess.Database.Repositories.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // ToListAsync, FirstOrDefaultAsync
using AI_Integration.Helpers;

namespace AI_Integration.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AdCampaignController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IUnitOfWork _unitOfWork;

        public AdCampaignController(IWebHostEnvironment environment, IUnitOfWork unitOfWork)
        {
            _environment = environment;
            _unitOfWork = unitOfWork;
        }

        // DTO minimalisti per evitare cicli
        public sealed class AdSessionDto
        {
            public int Id { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime? EndDate { get; set; }
        }

        public sealed class AdCampaignDto
        {
            public int Id { get; set; }
            public string? Name { get; set; }
            public string? Description { get; set; }
            public decimal? Budget { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public DateTime CreationTime { get; set; }
            public List<AdSessionDto> Sessions { get; set; } = new();
        }

        // GET: api/AdCampaign
        // Cerca tutte le campagne non eliminate
        [HttpGet]
        public async Task<IActionResult> Get([FromHeader(Name = "User-Agent")] string userAgent = "")
        {
            var log = WebApiLogHelper.NewLog("GET", "api/adcampaign", "", userAgent, "List campaigns (DTO)");
            var sw = Stopwatch.StartNew();

            try
            {
                // Proiezione in DTO: nessuna navigazione circolare
                var campaigns = await _unitOfWork.Query<AdCampaign>().Where(e => e.IsDeleted != true)
                    .Where(c => c.IsDeleted != true)
                    .OrderByDescending(c => c.CreationTime)
                    .Select(c => new AdCampaignDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Description = c.Description,
                        Budget = c.Budget,
                        StartDate = c.StartDate,
                        EndDate = c.EndDate,
                        CreationTime = c.CreationTime ?? DateTime.Now,
                        Sessions = c.Sessions
                            .Where(s => s.IsDeleted != true)
                            .Select(s => new AdSessionDto
                            {
                                Id = s.Id,
                                StartDate = s.StartDate,
                                EndDate = s.EndDate
                            })
                            .ToList()
                    })
                    .ToListAsync();

                sw.Stop();
                await WebApiLogHelper.LogOkAsync(_unitOfWork, log,
                    $"{{ success = true, count = {campaigns.Count} }}",
                    $"ElapsedMs={sw.ElapsedMilliseconds}");

                return Ok(campaigns);
            }
            catch (Exception ex)
            {
                sw.Stop();
                await WebApiLogHelper.LogErrorAsync(_unitOfWork, log, ex, $"ElapsedMs={sw.ElapsedMilliseconds}");
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // GET api/AdCampaign/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id,
            [FromHeader(Name = "User-Agent")] string userAgent = "")
        {
            var log = WebApiLogHelper.NewLog("GET", $"api/adcampaign/{id}", "", userAgent, "Get campaign by id (DTO)");
            var sw = Stopwatch.StartNew();

            try
            {
                if (id <= 0)
                {
                    sw.Stop();
                    await WebApiLogHelper.LogBadRequestAsync(_unitOfWork, log,
                        "{ success = false, message = 'Invalid campaign ID.' }",
                        $"ElapsedMs={sw.ElapsedMilliseconds}");
                    return BadRequest(new { success = false, message = "Invalid campaign ID." });
                }

                // Proiezione diretta in DTO: evita Include + entità
                var dto = await _unitOfWork.Query<AdCampaign>().Where(e => e.IsDeleted != true)
                    .Where(c => c.IsDeleted != true && c.Id == id)
                    .Select(c => new AdCampaignDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Description = c.Description,
                        Budget = c.Budget,
                        StartDate = c.StartDate,
                        EndDate = c.EndDate,
                        CreationTime = c.CreationTime ?? DateTime.Now,
                        Sessions = c.Sessions
                            .Where(s => s.IsDeleted != true)
                            .Select(s => new AdSessionDto
                            {
                                Id = s.Id,
                                StartDate = s.StartDate,
                                EndDate = s.EndDate
                            })
                            .ToList()
                    })
                    .FirstOrDefaultAsync();

                if (dto == null)
                {
                    sw.Stop();
                    await WebApiLogHelper.LogNotFoundAsync(_unitOfWork, log,
                        "{ success = false, message = 'Campaign not found.' }",
                        $"ElapsedMs={sw.ElapsedMilliseconds}");
                    return NotFound(new { success = false, message = "Campaign not found." });
                }

                sw.Stop();
                await WebApiLogHelper.LogOkAsync(_unitOfWork, log,
                    "{ success = true }",
                    $"ElapsedMs={sw.ElapsedMilliseconds}");
                return Ok(dto);
            }
            catch (Exception ex)
            {
                sw.Stop();
                await WebApiLogHelper.LogErrorAsync(_unitOfWork, log, ex, $"ElapsedMs={sw.ElapsedMilliseconds}");
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // POST api/AdCampaign/add
        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] AdCampaign campaign,
            [FromHeader(Name = "User-Agent")] string userAgent = "")
        {
            var log = WebApiLogHelper.NewLog("POST", "api/adcampaign/add", campaign?.ToString(), userAgent, "Create campaign");
            var sw = Stopwatch.StartNew();

            if (campaign == null)
            {
                sw.Stop();
                await WebApiLogHelper.LogBadRequestAsync(_unitOfWork, log,
                    "{ success = false, message = 'Campaign information is required.' }",
                    $"ElapsedMs={sw.ElapsedMilliseconds}");
                return BadRequest(new { success = false, message = "Campaign information is required." });
            }

            try
            {
                campaign.CreationTime = DateTime.Now;
                await _unitOfWork.InsertAsync(campaign);
                await _unitOfWork.SaveChangesAsync();

                sw.Stop();
                await WebApiLogHelper.LogOkAsync(_unitOfWork, log,
                    "{ success = true, message = 'Campaign added successfully.' }",
                    $"ElapsedMs={sw.ElapsedMilliseconds}");
                return Ok(new { success = true, message = "Campaign added successfully." });
            }
            catch (Exception ex)
            {
                sw.Stop();
                await WebApiLogHelper.LogErrorAsync(_unitOfWork, log, ex, $"ElapsedMs={sw.ElapsedMilliseconds}");
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // PUT api/AdCampaign/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] AdCampaign changes,
            [FromHeader(Name = "User-Agent")] string userAgent = "")
        {
            var log = WebApiLogHelper.NewLog("PUT", $"api/adcampaign/{id}", changes?.ToString(), userAgent, "Update campaign");
            var sw = Stopwatch.StartNew();

            try
            {
                if (id <= 0)
                {
                    sw.Stop();
                    await WebApiLogHelper.LogBadRequestAsync(_unitOfWork, log,
                        "{ success = false, message = 'Invalid campaign ID.' }",
                        $"ElapsedMs={sw.ElapsedMilliseconds}");
                    return BadRequest(new { success = false, message = "Invalid campaign ID." });
                }

                var campaign = await _unitOfWork.GetByIdAsync<AdCampaign>(id);
                if (campaign == null || campaign.IsDeleted == true)
                {
                    sw.Stop();
                    await WebApiLogHelper.LogNotFoundAsync(_unitOfWork, log,
                        "{ success = false, message = 'Campaign not found.' }",
                        $"ElapsedMs={sw.ElapsedMilliseconds}");
                    return NotFound(new { success = false, message = "Campaign not found." });
                }

                // Modifiche
                campaign.Name = changes.Name ?? campaign.Name;
                campaign.Description = changes.Description ?? campaign.Description;
                campaign.Budget = changes.Budget ?? campaign.Budget;
                campaign.StartDate = changes.StartDate != default ? changes.StartDate : campaign.StartDate;
                campaign.EndDate = changes.EndDate != default ? changes.EndDate : campaign.EndDate;
                campaign.LastModificationTime = DateTime.Now;

                _unitOfWork.Update(campaign);
                await _unitOfWork.SaveChangesAsync();

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

        // DELETE api/AdCampaign/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id,
            [FromHeader(Name = "User-Agent")] string userAgent = "")
        {
            var log = WebApiLogHelper.NewLog("DELETE", $"api/adcampaign/{id}", "", userAgent, "Soft delete campaign");
            var sw = Stopwatch.StartNew();

            try
            {
                if (id <= 0)
                {
                    sw.Stop();
                    await WebApiLogHelper.LogBadRequestAsync(_unitOfWork, log,
                        "{ success = false, message = 'Invalid campaign ID.' }",
                        $"ElapsedMs={sw.ElapsedMilliseconds}");
                    return BadRequest(new { success = false, message = "Invalid campaign ID." });
                }

                var campaign = await _unitOfWork.GetByIdAsync<AdCampaign>(id);
                if (campaign == null || campaign.IsDeleted == true)
                {
                    sw.Stop();
                    await WebApiLogHelper.LogNotFoundAsync(_unitOfWork, log,
                        "{ success = false, message = 'Campaign not found.' }",
                        $"ElapsedMs={sw.ElapsedMilliseconds}");
                    return NotFound(new { success = false, message = "Campaign not found." });
                }

                // Soft delete
                campaign.IsDeleted = true;
                campaign.DeletionTime = DateTime.Now;

                _unitOfWork.Update(campaign);
                await _unitOfWork.SaveChangesAsync();

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
