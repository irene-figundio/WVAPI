using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VITBO.Services.Interfaces;
using VITBO.Models;

namespace VITBO.Controllers
{
    [Authorize]
    public class AiVideoController : Controller
    {
        private readonly IAiVideoService _aiVideoService;

        public AiVideoController(IAiVideoService aiVideoService)
        {
            _aiVideoService = aiVideoService;
        }

        public async Task<IActionResult> Index([FromQuery] string? q = null, [FromQuery] int page = 1)
        {
            ViewData["CurrentFilter"] = q;
            var result = await _aiVideoService.GetVideosAsync(q, page);
            return View(result);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateVideoRequest());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateVideoRequest model)
        {
            if (ModelState.IsValid)
            {
                var success = await _aiVideoService.CreateVideoAsync(model);
                if (success)
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Failed to create video. Please try again.");
            }
            return View(model);

        }
    }
}
