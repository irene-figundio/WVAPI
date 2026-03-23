using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VITBO.Models;
using VITBO.Services.Interfaces;

namespace VITBO.Controllers
{
    [Authorize]
    public class PhotoGalleryController : Controller
    {
        private readonly IMediaService _mediaService;

        public PhotoGalleryController(IMediaService mediaService)
        {
            _mediaService = mediaService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int galleryId)
        {
            ViewBag.GalleryId = galleryId;
            var token = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
            var userAgent = GetUserAgent() ?? string.Empty;
            var list = await _mediaService.GetPhotosAsync(galleryId, token, userAgent);
            return View(list);
        }

        [HttpGet]
        public IActionResult Create(int galleryId)
        {
            return View(new PhotoGalleryDto { GalleryId = galleryId, LangID = 1 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PhotoGalleryDto model)
        {
            if (ModelState.IsValid)
            {
                var token = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
                var userAgent = GetUserAgent() ?? string.Empty;
                var success = await _mediaService.CreatePhotoAsync(model, token, userAgent);
                if (success)
                {
                    return RedirectToAction(nameof(Index), new { galleryId = model.GalleryId });
                }
                ModelState.AddModelError("", "Failed to add photo.");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int galleryId)
        {
            var token = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
            var userAgent = GetUserAgent() ?? string.Empty;
            await _mediaService.DeletePhotoAsync(id, token, userAgent);
            return RedirectToAction(nameof(Index), new { galleryId });
        }

        protected string? GetUserAgent()
        {
            return Request.Headers["User-Agent"].ToString();
        }
    }
}
