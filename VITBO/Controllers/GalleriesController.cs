using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VITBO.Models;
using VITBO.Services.Interfaces;

namespace VITBO.Controllers
{
    [Authorize]
    public class GalleriesController : Controller
    {
        private readonly IMediaService _mediaService;

        public GalleriesController(IMediaService mediaService)
        {
            _mediaService = mediaService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? eventId, [FromQuery] int langId = 1, string? search = null)
        {
            ViewData["CurrentLangId"] = langId;
            ViewData["SearchTerm"] = search;
            ViewBag.EventId = eventId;
            var token = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
            var userAgent = GetUserAgent() ?? string.Empty;

            var list = await _mediaService.GetGalleriesAsync(token, userAgent);

            if (eventId.HasValue) {
                list = list.Where(g => g.EventId == eventId.Value).ToList();
            }

            list = list.Where(g => g.LangID == langId).ToList();

            if (!string.IsNullOrEmpty(search))
            {
                list = list.Where(g => g.Title?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false).ToList();
            }

            return View(list);
        }

        [HttpGet]
        public IActionResult Create(int? eventId)
        {
            return View(new GalleryDto { EventId = eventId ?? 0, LangID = 1 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GalleryDto model)
        {
            if (ModelState.IsValid)
            {
                var token = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
                var userAgent = GetUserAgent() ?? string.Empty;
                var success = await _mediaService.CreateGalleryAsync(model, token, userAgent);
                if (success)
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Failed to create gallery.");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int eventId)
        {
            var token = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
            var userAgent = GetUserAgent() ?? string.Empty;
            await _mediaService.DeleteGalleryAsync(id, token, userAgent);
            return RedirectToAction(nameof(Index), new { eventId });
        }

        protected string? GetUserAgent()
        {
            return Request.Headers["User-Agent"].ToString();
        }
    }
}
