using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VITBO.Services.Interfaces;
using VITBO.Models;

namespace VITBO.Controllers
{
    [Authorize]
    public class EventsController : Controller
    {
        private readonly IEventsService _eventsService;

        public EventsController(IEventsService eventsService)
        {
            _eventsService = eventsService;
        }

        public async Task<IActionResult> Index([FromQuery] int langId = 1)
        {
            ViewData["CurrentLangId"] = langId;
            var result = await _eventsService.GetEventsAsync(langId);
            return View(result);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateEventRequest());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEventRequest model)
        {
            if (ModelState.IsValid)
            {
                var success = await _eventsService.CreateEventAsync(model);
                if (success)
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Failed to create event. Please try again.");
            }
            return View(model);

        }
    }
}
