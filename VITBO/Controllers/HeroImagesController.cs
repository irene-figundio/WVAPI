using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VITBO.Models;
using VITBO.Services.Interfaces;

namespace VITBO.Controllers
{
    [Authorize]
    public class HeroImagesController : Controller
    {
        private readonly IMediaService _mediaService;

        public HeroImagesController(IMediaService mediaService)
        {
            _mediaService = mediaService;
        }

        public async Task<IActionResult> Index()
        {
            var token = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
            var userAgent = Request.Headers["User-Agent"].ToString();
            var list = await _mediaService.GetHeroImagesAsync(token, userAgent);
            return View(list);
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file, string? caption)
        {
            if (file == null || file.Length == 0) return BadRequest("No file");

            var token = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
            var userAgent = Request.Headers["User-Agent"].ToString();

            using var content = new MultipartFormDataContent();
            var fileContent = new StreamContent(file.OpenReadStream());
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);

            content.Add(fileContent, "File", file.FileName);
            content.Add(new StringContent("HeroImage"), "ParentType");
            content.Add(new StringContent("0"), "ParentId");
            content.Add(new StringContent("HeroImage"), "UploadType");
            if (!string.IsNullOrEmpty(caption))
                content.Add(new StringContent(caption), "Caption");

            var res = await _mediaService.UploadFileAsync(content, token, userAgent);
            if (res.Success) return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", res.Message ?? "Upload failed");
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var token = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
            var userAgent = Request.Headers["User-Agent"].ToString();
            await _mediaService.DeleteHeroImageAsync(id, token, userAgent);
            return RedirectToAction(nameof(Index));
        }
    }
}
