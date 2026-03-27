using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VITBO.Models;
using VITBO.Services.Interfaces;

namespace VITBO.Controllers
{
    [Authorize]
    public class ContentImagesController : Controller
    {
        private readonly IMediaService _mediaService;

        public ContentImagesController(IMediaService mediaService)
        {
            _mediaService = mediaService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? contentId)
        {
            ViewBag.ContentId = contentId;
            var token = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
            var userAgent = GetUserAgent() ?? string.Empty;
            var list = await _mediaService.GetContentImagesByContentIdAsync(contentId, token, userAgent);
            return View(list);
        }

        [HttpGet]
        public IActionResult Create(int? contentId)
        {
            return View(new ContentImageDto { ContentId = contentId ?? 0, LangID = 1 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ContentImageDto model)
        {
            if (ModelState.IsValid)
            {
                var token = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
                var userAgent = GetUserAgent() ?? string.Empty;
                var success = await _mediaService.CreateContentImageAsync(model, token, userAgent);
                if (success)
                {
                    return RedirectToAction(nameof(Index), new { contentId = model.ContentId });
                }
                ModelState.AddModelError("", "Failed to add image.");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int contentId)
        {
            var token = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
            var userAgent = GetUserAgent() ?? string.Empty;
            await _mediaService.DeleteContentImageAsync(id, token, userAgent);
            return RedirectToAction(nameof(Index), new { contentId });
        }

        protected string? GetUserAgent()
        {
            return Request.Headers["User-Agent"].ToString();
        }
    }
}
