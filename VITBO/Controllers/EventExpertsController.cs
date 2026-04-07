using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VITBO.Models;
using VITBO.Services.Interfaces;

namespace VITBO.Controllers
{
    [Authorize]
    public class EventExpertsController : Controller
    {
        private readonly IMediaService _mediaService;
        private readonly IExpertsService _expertsService;

        public EventExpertsController(IMediaService mediaService, IExpertsService expertsService)
        {
            _mediaService = mediaService;
            _expertsService = expertsService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int eventId, int langId = 1)
        {
            ViewBag.EventId = eventId;
            ViewBag.LangId = langId;
            var token = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
            var userAgent = GetUserAgent() ?? string.Empty;
            var list = await _mediaService.GetEventExpertsAsync(eventId, token, userAgent);

            // Get ALL experts to match details (regardless of lang filtering in the "Link" select)
            var allExperts = new List<ExpertDto>();
            allExperts.AddRange(await _expertsService.GetExpertsAsync(1, token, userAgent));
            allExperts.AddRange(await _expertsService.GetExpertsAsync(2, token, userAgent));
            ViewBag.AllExperts = allExperts.GroupBy(e => e.Id).Select(g => g.First()).ToList();

            var expertsList = await _expertsService.GetExpertsAsync(langId, token, userAgent);
            ViewBag.AvailableExperts = expertsList;
            return View(list);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(EventExpertDto model)
        {
            if (ModelState.IsValid)
            {
                var token = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
                var userAgent = GetUserAgent() ?? string.Empty;
                await _mediaService.AddEventExpertAsync(model, token, userAgent);
            }
            return RedirectToAction(nameof(Index), new { eventId = model.EventId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int eventId)
        {
            var token = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
            var userAgent = GetUserAgent() ?? string.Empty;
            await _mediaService.RemoveEventExpertAsync(id, token, userAgent);
            return RedirectToAction(nameof(Index), new { eventId });
        }

        protected string? GetUserAgent()
        {
            return Request.Headers["User-Agent"].ToString();
        }
    }
}
