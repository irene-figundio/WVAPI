using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VITBO.Services.Interfaces;
using VITBO.Models;

namespace VITBO.Controllers
{
    [Authorize]
    public class ContentsController : Controller
    {
        private readonly IContentsService _contentsService;
        private readonly IMediaService _mediaService;

        public ContentsController(IContentsService contentsService, IMediaService mediaService)
        {
            _contentsService = contentsService;
            _mediaService = mediaService;
        }

        public async Task<IActionResult> Articles([FromQuery] int langId = 1, string? search = null)
        {
            ViewData["CurrentLangId"] = langId;
            ViewData["SearchTerm"] = search;
            var token = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
            var result = await _contentsService.GetContentsByTypeAsync("blog", langId, token, userAgent);

            if (!string.IsNullOrEmpty(search))
            {
                result = result.Where(c => (c.Title?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                         (c.Subtitle?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false)).ToList();
            }

            return View(result);
        }

        //[HttpGet]
        //public IActionResult CreateArticle()
        //{
        //    return View(new CreateContentRequest { ContentType = "Article" });
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> CreateArticle(CreateContentRequest model)
        //{
        //    model.ContentType = "Article";
        //    if (ModelState.IsValid)
        //    {
        //        var success = await _contentsService.CreateContentAsync(model);
        //        if (success)
        //        {
        //            return RedirectToAction(nameof(Articles));
        //        }
        //        ModelState.AddModelError("", "Failed to create article. Please try again.");
        //    }
        //    return View(model);
        //}

        //[HttpGet]
        //public IActionResult CreateNews()
        //{
        //    return View(new CreateContentRequest { ContentType = "News" });
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> CreateNews(CreateContentRequest model)
        //{
        //    model.ContentType = "News";
        //    if (ModelState.IsValid)
        //    {
        //        var success = await _contentsService.CreateContentAsync(model);
        //        if (success)
        //        {
        //            return RedirectToAction(nameof(News));
        //        }
        //        ModelState.AddModelError("", "Failed to create news. Please try again.");
        //    }
        //    return View(model);

        //}

        public async Task<IActionResult> News([FromQuery] int langId = 1, string? search = null)
        {
            ViewData["CurrentLangId"] = langId;
            ViewData["SearchTerm"] = search;
            var token = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
            var result = await _contentsService.GetContentsByTypeAsync("News", langId, token, userAgent);

            if (!string.IsNullOrEmpty(search))
            {
                result = result.Where(c => (c.Title?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                         (c.Subtitle?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false)).ToList();
            }

            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> CreateArticle()
        {
            var token = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
            var userAgent = GetUserAgent() ?? string.Empty;
            ViewBag.Categories = await _contentsService.GetContentCategoriesAsync(1, token, userAgent);
            ViewBag.HeroImages = await _mediaService.GetHeroImagesAsync(token, userAgent);
            return View(new CreateContentRequest { ContentType = "blog" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateArticle(CreateContentRequest model)
        {
            model.ContentType = "blog";
            var token = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
            var userAgent = GetUserAgent() ?? string.Empty;

            if (ModelState.IsValid)
            {
                var newId = await _contentsService.CreateContent(model, token, userAgent);
                if (newId.HasValue)
                {
                    if (model.CoverImageFile != null)
                    {
                        using var content = new MultipartFormDataContent();
                        var fileContent = new StreamContent(model.CoverImageFile.OpenReadStream());
                        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(model.CoverImageFile.ContentType);
                        content.Add(fileContent, "File", model.CoverImageFile.FileName);
                        content.Add(new StringContent("Content"), "ParentType");
                        content.Add(new StringContent(newId.Value.ToString()), "ParentId");
                        content.Add(new StringContent("ContentCoverImage"), "UploadType");
                        await _mediaService.UploadFileAsync(content, token, userAgent);
                    }
                    return RedirectToAction(nameof(Edit), new { id = newId.Value });
                }
                ModelState.AddModelError("", "Failed to create article. Please try again.");
            }
            ViewBag.Categories = await _contentsService.GetContentCategoriesAsync(model.LangID, token, userAgent);
            ViewBag.HeroImages = await _mediaService.GetHeroImagesAsync(token, userAgent);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> CreateNews()
        {
            var token = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
            var userAgent = GetUserAgent() ?? string.Empty;
            ViewBag.Categories = await _contentsService.GetContentCategoriesAsync(1, token, userAgent);
            ViewBag.HeroImages = await _mediaService.GetHeroImagesAsync(token, userAgent);
            return View(new CreateContentRequest { ContentType = "news" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNews(CreateContentRequest model)
        {
            model.ContentType = "news";
            var token = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
            var userAgent = GetUserAgent() ?? string.Empty;

            if (ModelState.IsValid)
            {
                var newId = await _contentsService.CreateContent(model, token, userAgent);
                if (newId.HasValue)
                {
                    if (model.CoverImageFile != null)
                    {
                        using var content = new MultipartFormDataContent();
                        var fileContent = new StreamContent(model.CoverImageFile.OpenReadStream());
                        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(model.CoverImageFile.ContentType);
                        content.Add(fileContent, "File", model.CoverImageFile.FileName);
                        content.Add(new StringContent("Content"), "ParentType");
                        content.Add(new StringContent(newId.Value.ToString()), "ParentId");
                        content.Add(new StringContent("ContentCoverImage"), "UploadType");
                        await _mediaService.UploadFileAsync(content, token, userAgent);
                    }
                    return RedirectToAction(nameof(Edit), new { id = newId.Value });
                }
                ModelState.AddModelError("", "Failed to create news. Please try again.");
            }
            ViewBag.Categories = await _contentsService.GetContentCategoriesAsync(model.LangID, token, userAgent);
            ViewBag.HeroImages = await _mediaService.GetHeroImagesAsync(token, userAgent);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, [FromQuery] int langId = 1)
        {
            var token = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
            var contentDto = await _contentsService.GetContentByIdAsync(id, langId, token, userAgent);
            if (contentDto == null)
            {
                return NotFound();
            }
            var categories = await _contentsService.GetContentCategoriesAsync(langId, token, userAgent);
            ViewBag.Categories = categories;
            ViewBag.HeroImages = await _mediaService.GetHeroImagesAsync(token, userAgent);

            var model = new EditContentRequest
            {
                Id = contentDto.Id,
                Title = contentDto.Title,
                Text = contentDto.Text,
                PublishDate = contentDto.PublishDate,
                CoverImage = contentDto.CoverImage,
                ContentType = contentDto.ContentType,
                IsPublished = contentDto.IsPublished,
                Subtitle = contentDto.Subtitle,
                CategoryId = contentDto.CategoryId,
                Preview = contentDto.Preview,
                HeroImage = contentDto.HeroImage
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditContentRequest model)
        {
            if (ModelState.IsValid)
            {
                var token = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
                var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
                var request = new ContentDto
                {
                    Id = model.Id,
                    Title = model.Title,
                    Text = model.Text,
                    PublishDate = model.PublishDate,
                    CoverImage = model.CoverImage,
                    ContentType = model.ContentType,
                    IsPublished = model.IsPublished,
                    Subtitle = model.Subtitle,
                    CategoryId = model.CategoryId,
                    Preview = model.Preview,
                    HeroImage = model.HeroImage
                };
                var success = await _contentsService.UpdateContentAsync(model.Id,request, token, userAgent);
                if (success)
                {
                    return RedirectToAction(model.ContentType == "news" ? nameof(News) : nameof(Articles));
                }
                ModelState.AddModelError("", "Failed to update content. Please try again.");
            }
            return View(model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, string contentType)
        {
            var token = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
            var success = await _contentsService.DeleteContentAsync(id, token, userAgent);
            if (success)
            {
                return RedirectToAction(contentType.Equals("News", StringComparison.OrdinalIgnoreCase) ? nameof(News) : nameof(Articles));
            }
            ModelState.AddModelError("", "Failed to delete content. Please try again.");
            return RedirectToAction(contentType.Equals("News", StringComparison.OrdinalIgnoreCase) ? nameof(News) : nameof(Articles));
        }

        [HttpGet]
        public async Task<JsonResult> GetCategoriesByLang(int langId)
        {
            var token = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
            var userAgent = GetUserAgent() ?? string.Empty;
            var categories = await _contentsService.GetContentCategoriesAsync(langId, token, userAgent);
            return Json(categories.Select(c => new { id = c.Id, name = c.Name }));
        }

        [HttpPost]
        public async Task<IActionResult> AjaxUpload()
        {
            var file = Request.Form.Files["File"];
            if (file == null) return Json(new { success = false, message = "No file" });

            var token = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
            var userAgent = GetUserAgent() ?? string.Empty;

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

        protected string? GetUserAgent()
        {
            return Request.Headers["User-Agent"].ToString();
        }
    }

    public class EditContentRequest
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime PublishDate { get; set; }
        public string CoverImage { get; set; }
        public string ContentType { get; set; }
        public bool IsPublished { get; set; }
        public string Subtitle { get; set; }
        public int? CategoryId { get; set; }
        public string Preview { get; set; }
        public string HeroImage { get; set; }
    }
}
