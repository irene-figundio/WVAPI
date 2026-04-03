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

        public async Task<IActionResult> Index([FromQuery] int langId = 1, string? search = null)
        {
            ViewData["CurrentLangId"] = langId;
            ViewData["SearchTerm"] = search;
            string sessionToken = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
            string userAgent = GetUserAgent() ?? string.Empty;
            var result = await _eventsService.GetEventsAsync(langId, sessionToken, userAgent);

            if (!string.IsNullOrEmpty(search))
            {
                result = result.Where(e => (e.Title?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                         (e.Description?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false)).ToList();
            }

            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> Create([FromQuery] int langId = 1)
        {
            string sessionToken = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
            string userAgent = GetUserAgent() ?? string.Empty;
            ViewBag.Categories = await _eventsService.GetEventCategoriesAsync(langId, sessionToken, userAgent);
            return View(new CreateEventRequest { LangID = langId });
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

            ViewBag.Categories = await _eventsService.GetEventCategoriesAsync(langId, sessionToken, userAgent);

            var model = new UpdateEventRequest
            {
                id = eventDto.Id,
                Title = eventDto.Title,
                Description = eventDto.Description,
                EventDate = eventDto.EventDate,
                EndDate = eventDto.EndDate,
                EndTime = eventDto.EndTime,
                CoverImage = eventDto.CoverImage,
                BookingEndDate = eventDto.BookingEndDate,
                Location = eventDto.Location,
                Organizer = eventDto.Organizer,
                ContactInfo = eventDto.ContactInfo,
                Price = eventDto.Price,
                IsOnline = eventDto.IsOnline,
                LangID = eventDto.LangID,
                Subtitle = eventDto.Subtitle,
                CategoryId = eventDto.CategoryId,
                Coordinates = eventDto.Coordinates,
                HeroImage = eventDto.HeroImage,
                HasNeeds = eventDto.HasNeeds,
                HasVariantPrice = eventDto.HasVariantPrice,
                ProgramPdf = eventDto.ProgramPdf
            };

            return View(model);
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

        [HttpGet]
        public async Task<JsonResult> GetEventsByLang(int langId)
        {
            string sessionToken = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
            string userAgent = GetUserAgent() ?? string.Empty;
            var events = await _eventsService.GetEventsAsync(langId, sessionToken, userAgent);
            return Json(events.Select(e => new { id = e.Id, title = e.Title, categoryId = e.CategoryId }));
        }

        [HttpGet]
        public async Task<JsonResult> GetCategoriesByLang(int langId)
        {
            string sessionToken = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
            string userAgent = GetUserAgent() ?? string.Empty;
            var categories = await _eventsService.GetEventCategoriesAsync(langId, sessionToken, userAgent);
            return Json(categories.Select(c => new { id = c.Id, name = c.Name }));
        }

        [HttpGet]
        public async Task<IActionResult> VariantPrices(int eventId)
        {
            ViewBag.EventId = eventId;
            var token = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
            var userAgent = GetUserAgent() ?? string.Empty;
            var list = await _eventsService.GetVariantPricesAsync(eventId, token, userAgent);
            return View(list);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddVariantPrice(VariantPriceDto model)
        {
            if (ModelState.IsValid)
            {
                var token = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
                var userAgent = GetUserAgent() ?? string.Empty;
                await _eventsService.CreateVariantPriceAsync(model, token, userAgent);
            }
            return RedirectToAction(nameof(VariantPrices), new { eventId = model.EventId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVariantPrice(int id, int eventId)
        {
            var token = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
            var userAgent = GetUserAgent() ?? string.Empty;
            await _eventsService.DeleteVariantPriceAsync(id, token, userAgent);
            return RedirectToAction(nameof(VariantPrices), new { eventId });
        }

        [HttpGet]
        public async Task<IActionResult> EventNeeds(int eventId)
        {
            ViewBag.EventId = eventId;
            var token = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
            var userAgent = GetUserAgent() ?? string.Empty;
            var list = await _eventsService.GetEventNeedsAsync(eventId, token, userAgent);
            return View(list);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEventNeed(EventNeedDto model)
        {
            if (ModelState.IsValid)
            {
                var token = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
                var userAgent = GetUserAgent() ?? string.Empty;
                await _eventsService.CreateEventNeedAsync(model, token, userAgent);
            }
            return RedirectToAction(nameof(EventNeeds), new { eventId = model.EventId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEventNeed(int id, int eventId)
        {
            var token = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
            var userAgent = GetUserAgent() ?? string.Empty;
            await _eventsService.DeleteEventNeedAsync(id, token, userAgent);
            return RedirectToAction(nameof(EventNeeds), new { eventId });
        }

        protected string? GetUserAgent()
        {
            return Request.Headers["User-Agent"].ToString();
        }
    }
}
