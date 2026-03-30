using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using VITBO.Models;
using VITBO.Services.Interfaces;

namespace VITBO.Controllers
{
    [Authorize]
    public class TripsController : Controller
    {
        private readonly ITripsService _tripsService;
        private readonly IEventsService _eventsService;

        public TripsController(ITripsService tripsService, IEventsService eventsService)
        {
            _tripsService = tripsService;
            _eventsService = eventsService;
        }

        public async Task<IActionResult> Index(int? eventId, [FromQuery] int langId = 1, string? search = null)
        {
            ViewData["CurrentLangId"] = langId;
            ViewData["SearchTerm"] = search;
            string token = GetToken();
            string ua = GetUserAgent();
            var trips = await _tripsService.GetTripsAsync(eventId, token, ua);

            // Note: Trips themselves don't have LangID, but they are linked to Events which do.
            // However, we added LangID to CreateTripRequest for UI filtering.
            // If the API doesn't support LangID on Trips, we might need to filter by related Event.
            // For now, let's filter by search term on departure/arrival cities.

            if (!string.IsNullOrEmpty(search))
            {
                trips = trips.Where(t =>
                    (t.DepartureCity?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (t.ArrivalCity?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (t.DepartureCountry?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (t.ArrivalCountry?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false)
                ).ToList();
            }

            ViewBag.EventId = eventId;
            return View(trips);
        }

        public async Task<IActionResult> Details(int id)
        {
            string token = GetToken();
            string ua = GetUserAgent();
            var trip = await _tripsService.GetTripByIdAsync(id, token, ua);
            if (trip == null) return NotFound();
            var evetnt = await _eventsService.GetEventByIdAbsAsync(trip.EventId,token,ua);
            if (evetnt != null)
            {
                ViewBag.title = evetnt.Title;
                ViewBag.description = evetnt.Description;
                ViewBag.imageUrl = evetnt.CoverImage;
                ViewBag.Price = evetnt.Price;
            }
            ViewBag.Stays = await _tripsService.GetStaysByTripIdAsync(id, token, ua);
            var days = await _tripsService.GetItineraryDaysByTripIdAsync(id, token, ua);
            ViewBag.Days = days;
            ViewBag.Musts = await _tripsService.GetTripMustsByTripIdAsync(id, token, ua);
            var stopsByDay = new Dictionary<int, List<ItineraryStopDto>>();
            foreach (var day in days)
            {
                stopsByDay[day.Id] = await _tripsService.GetItineraryStopsByDayIdAsync(day.Id, token, ua);
            }
            ViewBag.StopsByDay = stopsByDay;

            return View(trip);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? eventId)
        {
            string token = GetToken();
            string ua = GetUserAgent();
            ViewBag.Events = await _eventsService.GetEventsAsync(1, token, ua); // Default lang 1
            return View(new CreateTripRequest { EventId = eventId ?? 0 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTripRequest model)
        {
            if (ModelState.IsValid)
            {
                string token = GetToken();
                string ua = GetUserAgent();

                // Validation: prevent multiple trips for the same EventId
                var existingTrips = await _tripsService.GetTripsAsync(model.EventId, token, ua);
                if (existingTrips != null && existingTrips.Any(t => t.EventId == model.EventId))
                {
                    ModelState.AddModelError("EventId", "A trip is already associated with this Event ID.");
                    return View(model);
                }

                var success = await _tripsService.CreateTripAsync(model, token, ua);
                if (success) return RedirectToAction(nameof(Index));
                ModelState.AddModelError("", "Error creating trip.");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            string token = GetToken();
            string ua = GetUserAgent();
            var trip = await _tripsService.GetTripByIdAsync(id, token, ua);
            if (trip == null) return NotFound();

            ViewBag.Events = await _eventsService.GetEventsAsync(1, token, ua);
            var model = new UpdateTripRequest
            {
                Id = trip.Id,
                EventId = trip.EventId,
                DepartureCity = trip.DepartureCity,
                DepartureCountry = trip.DepartureCountry,
                ArrivalCity = trip.ArrivalCity,
                ArrivalCountry = trip.ArrivalCountry,
                DurationDays = trip.DurationDays,
                DurationNights = trip.DurationNights,
                MaxGuests = trip.MaxGuests,
                Status = trip.Status
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateTripRequest model)
        {
            if (ModelState.IsValid)
            {
                string token = GetToken();
                string ua = GetUserAgent();
                var success = await _tripsService.UpdateTripAsync(id, model, token, ua);
                if (success) return RedirectToAction(nameof(Index));
                ModelState.AddModelError("", "Error updating trip.");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            string token = GetToken();
            string ua = GetUserAgent();
            await _tripsService.DeleteTripAsync(id, token, ua);
            return RedirectToAction(nameof(Index));
        }

        // --- Related Entities ---

        // Stays
        [HttpPost]
        public async Task<IActionResult> AddStay(CreateStayRequest model)
        {
            if (ModelState.IsValid)
            {
                await _tripsService.CreateStayAsync(model, GetToken(), GetUserAgent());
            }
            return RedirectToAction(
         nameof(Details),
         null,
         new { id = model.TripId },
         "stays-tab");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteStay(int id, int tripId)
        {
            await _tripsService.DeleteStayAsync(id, GetToken(), GetUserAgent());
            return RedirectToAction(
         nameof(Details),
         null,
         new { id = tripId },
         "stays-tab");
        }

        // Days
        [HttpPost]
        public async Task<IActionResult> AddDay(CreateItineraryDayRequest model)
        {
            if (ModelState.IsValid)
            {
                await _tripsService.CreateItineraryDayAsync(model, GetToken(), GetUserAgent());
            }
            return RedirectToAction(
         nameof(Details),
         null,
         new { id = model.TripId },
         "days-tab");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDay(int id, int tripId)
        {
            await _tripsService.DeleteItineraryDayAsync(id, GetToken(), GetUserAgent());
            return RedirectToAction(
         nameof(Details),
         null,
         new { id = tripId },
         "days-tab");
        }

        // Stops
        [HttpPost]
        public async Task<IActionResult> AddStop(CreateItineraryStopRequest model, int tripId)
        {
            if (ModelState.IsValid)
            {
                await _tripsService.CreateItineraryStopAsync(model, GetToken(), GetUserAgent());
            }
            return RedirectToAction(
        nameof(Details),
        null,
        new { id = tripId },
        "days-tab");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteStop(int id, int tripId)
        {
            await _tripsService.DeleteItineraryStopAsync(id, GetToken(), GetUserAgent());
            return RedirectToAction(
         nameof(Details),
         null,
         new { id = tripId },
         "days-tab");
        }


        // Musts
        [HttpPost]
        public async Task<IActionResult> AddMust(CreateTripMustRequest model)
        {
            if (ModelState.IsValid)
            {
                await _tripsService.CreateTripMustAsync(model, GetToken(), GetUserAgent());
            }
            return RedirectToAction(
        nameof(Details),
        null,
        new { id = model.TripId },
        "musts-tab");
        }


        [HttpPost]
        public async Task<IActionResult> DeleteMust(int id, int tripId)
        {
            await _tripsService.DeleteTripMustAsync(id, GetToken(), GetUserAgent());
            return RedirectToAction(
         nameof(Details),
         null,
         new { id = tripId },
         "musts-tab");
        }

        private string GetToken() => HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
        private string GetUserAgent() => Request.Headers["User-Agent"].ToString();
    }
}
