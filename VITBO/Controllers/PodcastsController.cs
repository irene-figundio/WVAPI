using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VITBO.Models;
using VITBO.Services.Interfaces;

namespace VITBO.Controllers
{
    [Authorize]
    public class PodcastsController : Controller
    {
        private readonly IPodcastsService _podcastsService;
        private readonly IMediaService _mediaService;

        public PodcastsController(IPodcastsService podcastsService, IMediaService mediaService)
        {
            _podcastsService = podcastsService;
            _mediaService = mediaService;
        }

        public async Task<IActionResult> Index([FromQuery] int langId = 1)
        {
            ViewData["CurrentLangId"] = langId;
            string token = GetToken();
            string ua = GetUserAgent();
            var list = await _podcastsService.GetPodcastsAsync(langId, token, ua);
            return View(list);
        }

        [HttpGet]
        public IActionResult Create([FromQuery] int langId = 1)
        {
            return View(new CreatePodcastRequest { LangID = langId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePodcastRequest model)
        {
            if (ModelState.IsValid)
            {
                string token = GetToken();
                string ua = GetUserAgent();
                var newId = await _podcastsService.CreatePodcastAsync(model, token, ua);
                if (newId.HasValue)
                {
                    if (model.CoverImageFile != null)
                    {
                        using var content = new MultipartFormDataContent();
                        var fileContent = new StreamContent(model.CoverImageFile.OpenReadStream());
                        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(model.CoverImageFile.ContentType);
                        content.Add(fileContent, "File", model.CoverImageFile.FileName);
                        content.Add(new StringContent("Podcast"), "ParentType");
                        content.Add(new StringContent(newId.Value.ToString()), "ParentId");
                        content.Add(new StringContent("PodcastCoverImage"), "UploadType");
                        await _mediaService.UploadFileAsync(content, token, ua);
                    }
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Failed to create podcast.");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, [FromQuery] int langId = 1)
        {
            string token = GetToken();
            string ua = GetUserAgent();
            var dto = await _podcastsService.GetPodcastByIdAsync(id, langId, token, ua);
            if (dto == null) return NotFound();
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PodcastDto model)
        {
            if (ModelState.IsValid)
            {
                string token = GetToken();
                string ua = GetUserAgent();
                var success = await _podcastsService.UpdatePodcastAsync(id, model, token, ua);
                if (success) return RedirectToAction(nameof(Index));
                ModelState.AddModelError("", "Failed to update podcast.");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _podcastsService.DeletePodcastAsync(id, GetToken(), GetUserAgent());
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> AjaxUpload()
        {
            var file = Request.Form.Files["File"];
            if (file == null) return Json(new { success = false, message = "No file" });

            var token = GetToken();
            var userAgent = GetUserAgent();

            using var content = new MultipartFormDataContent();
            var fileContent = new StreamContent(file.OpenReadStream());
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
            content.Add(fileContent, "File", file.FileName);

            foreach (var key in Request.Form.Keys)
            {
                if (key == "File") continue;
                content.Add(new StringContent(Request.Form[key].ToString()), key);
            }

            var res = await _mediaService.UploadFileAsync(content, token, userAgent);
            return Json(res);
        }

        private string GetToken() => HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
        private string GetUserAgent() => Request.Headers["User-Agent"].ToString();
    }
}
