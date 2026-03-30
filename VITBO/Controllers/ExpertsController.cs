using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VITBO.Models;
using VITBO.Services.Interfaces;

namespace VITBO.Controllers
{
    [Authorize]
    public class ExpertsController : Controller
    {
        private readonly IExpertsService _expertsService;

        public ExpertsController(IExpertsService expertsService)
        {
            _expertsService = expertsService;
        }

        public async Task<IActionResult> Index([FromQuery] int langId = 1, string? search = null)
        {
            ViewData["CurrentLangId"] = langId;
            ViewData["SearchTerm"] = search;
            var token = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
            var userAgent = GetUserAgent() ?? string.Empty;
            var experts = await _expertsService.GetExpertsAsync(langId, token, userAgent);

            if (!string.IsNullOrEmpty(search))
            {
                experts = experts.Where(e => (e.Name?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                         (e.Bio?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false)).ToList();
            }

            return View(experts);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new ExpertDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExpertDto model)
        {
            if (ModelState.IsValid)
            {
                var token = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
                var userAgent = GetUserAgent() ?? string.Empty;
                var success = await _expertsService.CreateExpertAsync(model, token, userAgent);
                if (success)
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Failed to create expert.");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, [FromQuery] int langId = 1)
        {
            var token = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
            var userAgent = GetUserAgent() ?? string.Empty;
            var expert = await _expertsService.GetExpertByIdAsync(id, langId, token, userAgent);
            if (expert == null)
            {
                return NotFound();
            }
            return View(expert);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ExpertDto model)
        {
            if (ModelState.IsValid)
            {
                var token = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
                var userAgent = GetUserAgent() ?? string.Empty;
                var success = await _expertsService.UpdateExpertAsync(id, model, token, userAgent);
                if (success)
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Failed to update expert.");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var token = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
            var userAgent = GetUserAgent() ?? string.Empty;
            await _expertsService.DeleteExpertAsync(id, token, userAgent);
            return RedirectToAction(nameof(Index));
        }

        protected string? GetUserAgent()
        {
            return Request.Headers["User-Agent"].ToString();
        }
    }
}
