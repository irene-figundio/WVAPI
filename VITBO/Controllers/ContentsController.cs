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

        public ContentsController(IContentsService contentsService)
        {
            _contentsService = contentsService;
        }

        public async Task<IActionResult> Articles([FromQuery] int langId = 1)
        {

            ViewData["CurrentLangId"] = langId;
            var token = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
            var result = await _contentsService.GetContentsByTypeAsync("Article", langId, token, userAgent);
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

        public async Task<IActionResult> News([FromQuery] int langId = 1)
        {
            var token = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
            ViewData["CurrentLangId"] = langId;
            var result = await _contentsService.GetContentsByTypeAsync("News", langId, token, userAgent);
            return View(result);
        }

        [HttpGet]
        public IActionResult CreateArticle()
        {
            return View(new CreateContentRequest { ContentType = "Article" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateArticle(CreateContentRequest model)
        {
            if (ModelState.IsValid)
            {
                var token = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
                var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
                model.ContentType = "blog";
                var success = await _contentsService.CreateContent(model, token, userAgent);
                if (success)
                {
                    return RedirectToAction(nameof(Articles));
                }
                ModelState.AddModelError("", "Failed to create article. Please try again.");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult CreateNews()
        {
            return View(new CreateContentRequest { ContentType = "News" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNewsAsync(CreateContentRequest model)
        {
     
            if (ModelState.IsValid)
            {
                var token = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
                                var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
                model.ContentType = "news";
                var success = await _contentsService.CreateContent(model, token, userAgent);
                if (success)
                {
                    return RedirectToAction(nameof(News));
                }
                ModelState.AddModelError("", "Failed to create news. Please try again.");
            }
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
                return RedirectToAction(contentType == "News" ? nameof(News) : nameof(Articles));
            }
            ModelState.AddModelError("", "Failed to delete content. Please try again.");
            return RedirectToAction(contentType == "News" ? nameof(News) : nameof(Articles));
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
