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
            var result = await _contentsService.GetContentsByTypeAsync("Article", langId);
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
            ViewData["CurrentLangId"] = langId;
            var result = await _contentsService.GetContentsByTypeAsync("News", langId);
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
            model.ContentType = "Article";
            if (ModelState.IsValid)
            {
                var success = await _contentsService.CreateContent(model);
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
            model.ContentType = "News";
            if (ModelState.IsValid)
            {
                var success = await _contentsService.CreateContent(model);
                if (success)
                {
                    return RedirectToAction(nameof(News));
                }
                ModelState.AddModelError("", "Failed to create news. Please try again.");
            }
            return View(model);

        }
        protected string? GetUserAgent()
        {
            return Request.Headers["User-Agent"].ToString();
        }
    }
}
