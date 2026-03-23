using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VITBO.Services.Interfaces;
using VITBO.Models;
using VITBO.Services;

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
            string sessionToken = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
            string userAgent = GetUserAgent() ?? string.Empty;
            var result = await _eventsService.GetEventsAsync(langId, sessionToken, userAgent);
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
            string sessionToken = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
            string userAgent = GetUserAgent() ?? string.Empty;
            if (ModelState.IsValid)
            {
                var success = await _eventsService.CreateEventAsync(model, sessionToken, userAgent);
                if (success)
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Failed to create event. Please try again.");
            }
            return View(model);

        }

        [HttpGet]
        
        public async Task<IActionResult> Edit(int id, [FromQuery] int langId = 1)
        {
            string sessionToken = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
            string userAgent = GetUserAgent() ?? string.Empty;
            var eventDto = await _eventsService.GetEventByIdAsync(id, langId, sessionToken, userAgent);
            if (eventDto == null)
            {
                return NotFound();
            }
            return View(eventDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateEventRequest model)
        {
            string sessionToken = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
            string userAgent = GetUserAgent() ?? string.Empty;
            if (ModelState.IsValid)
            {
                var success = await _eventsService.UpdateEventAsync(id, model, sessionToken, userAgent);
                if (success)
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Failed to update event. Please try again.");
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            string sessionToken = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
            string userAgent = GetUserAgent() ?? string.Empty;
            var success = await _eventsService.DeleteEventAsync(id, sessionToken, userAgent);
            if (success)
            {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", "Failed to delete event. Please try again.");
            return RedirectToAction(nameof(Index));
        }

        protected string? GetUserAgent()
        {
            return Request.Headers["User-Agent"].ToString();
        }
    }
}
