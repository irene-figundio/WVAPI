using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VITBO.Models;
using VITBO.Services.Interfaces;

namespace VITBO.Controllers
{
    [Authorize]
    public class ContentExpertsController : Controller
    {
        private readonly IMediaService _mediaService;
        private readonly IExpertsService _expertsService;

        public ContentExpertsController(IMediaService mediaService, IExpertsService expertsService)
        {
            _mediaService = mediaService;
            _expertsService = expertsService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int contentId, int langId = 1)
        {
            ViewBag.ContentId = contentId;
            ViewBag.LangId = langId;
            var token = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
            var userAgent = GetUserAgent() ?? string.Empty;
            var list = await _mediaService.GetContentExpertsAsync(contentId, token, userAgent);

            // Get ALL experts to match details
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
        public async Task<IActionResult> Add(ContentExpertDto model)
        {
            if (ModelState.IsValid)
            {
                var token = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
                var userAgent = GetUserAgent() ?? string.Empty;
                await _mediaService.AddContentExpertAsync(model, token, userAgent);
            }
            return RedirectToAction(nameof(Index), new { contentId = model.ContentId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int contentId)
        {
            var token = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
            var userAgent = GetUserAgent() ?? string.Empty;
            await _mediaService.RemoveContentExpertAsync(id, token, userAgent);
            return RedirectToAction(nameof(Index), new { contentId });
        }

        protected string? GetUserAgent()
        {
            return Request.Headers["User-Agent"].ToString();
        }
    }
}
