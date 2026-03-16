using AI_Integration.DataAccess.Database.Repositories.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AI_Integration.DataAccess.Database.Models;
using Microsoft.EntityFrameworkCore;
using AI_Integration.Helpers;
using System.Diagnostics;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AI_Integration.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AdAnalyticController : ControllerBase
    {

        private readonly IWebHostEnvironment _environment;
        private readonly IUnitOfWork _unitOfWork;

        public AdAnalyticController(IWebHostEnvironment environment, IUnitOfWork unitOfWork)
        {
            _environment = environment;
            _unitOfWork = unitOfWork;
        }
        // GET: api/<AdAnalyticController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<AdAnalyticController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<AdAnalyticController>
        [HttpPost("create")]
        public async Task<IActionResult> Post([FromBody] AdAnalytics analytic, [FromHeader(Name = "User-Agent")] string userAgent = "")
        {
            var log = WebApiLogHelper.NewLog("POST", "api/adanalytic/create", analytic?.ToString(), userAgent, "Create analytic via SP");
            var sw = Stopwatch.StartNew();
            if (analytic == null) return BadRequest(new { success = false, message = "Analytic info is required." });
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var sql = "EXEC [dbo].[sp_CreaAdAnalytics] @SessionId={0}, @VideoId={1}, @NumViews={2}, @NumClick={3}, @Platform={4}, @SendDate={5}, @LangID={6}";
                var results = await _unitOfWork.Context.AdAnalyticsCreationResults.FromSqlRaw(sql,
                    analytic.SessionId,
                    analytic.VideoId,
                    analytic.NumViews ?? (object)DBNull.Value,
                    analytic.NumClick ?? (object)DBNull.Value,
                    analytic.Platform,
                    analytic.SendDate,
                    1
                ).ToListAsync();

                sw.Stop();
                var result = results.FirstOrDefault();
                if (result != null)
                {
                    await WebApiLogHelper.LogOkAsync(_unitOfWork, log, "{ success = true }", $"ElapsedMs={sw.ElapsedMilliseconds}");
                    return Ok(new { success = true, id = result.AdAnalyticsId });
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

        // PUT api/<AdAnalyticController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<AdAnalyticController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
