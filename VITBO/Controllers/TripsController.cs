using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public async Task<IActionResult> Index(int? eventId)
        {
            string token = GetToken();
            string ua = GetUserAgent();
            var trips = await _tripsService.GetTripsAsync(eventId, token, ua);
            ViewBag.EventId = eventId;
            return View(trips);
        }

        public async Task<IActionResult> Details(int id)
        {
            string token = GetToken();
            string ua = GetUserAgent();
            var trip = await _tripsService.GetTripByIdAsync(id, token, ua);
            if (trip == null) return NotFound();

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
            return RedirectToAction(nameof(Details), new { id = model.TripId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteStay(int id, int tripId)
        {
            await _tripsService.DeleteStayAsync(id, GetToken(), GetUserAgent());
            return RedirectToAction(nameof(Details), new { id = tripId });
        }

        // Days
        [HttpPost]
        public async Task<IActionResult> AddDay(CreateItineraryDayRequest model)
        {
            if (ModelState.IsValid)
            {
                await _tripsService.CreateItineraryDayAsync(model, GetToken(), GetUserAgent());
            }
            return RedirectToAction(nameof(Details), new { id = model.TripId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDay(int id, int tripId)
        {
            await _tripsService.DeleteItineraryDayAsync(id, GetToken(), GetUserAgent());
            return RedirectToAction(nameof(Details), new { id = tripId });
        }

        // Stops
        [HttpPost]
        public async Task<IActionResult> AddStop(CreateItineraryStopRequest model, int tripId)
        {
            if (ModelState.IsValid)
            {
                await _tripsService.CreateItineraryStopAsync(model, GetToken(), GetUserAgent());
            }
            return RedirectToAction(nameof(Details), new { id = tripId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteStop(int id, int tripId)
        {
            await _tripsService.DeleteItineraryStopAsync(id, GetToken(), GetUserAgent());
            return RedirectToAction(nameof(Details), new { id = tripId });
        }

        // Musts
        [HttpPost]
        public async Task<IActionResult> AddMust(CreateTripMustRequest model)
        {
            if (ModelState.IsValid)
            {
                await _tripsService.CreateTripMustAsync(model, GetToken(), GetUserAgent());
            }
            return RedirectToAction(nameof(Details), new { id = model.TripId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMust(int id, int tripId)
        {
            await _tripsService.DeleteTripMustAsync(id, GetToken(), GetUserAgent());
            return RedirectToAction(nameof(Details), new { id = tripId });
        }

        private string GetToken() => HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
        private string GetUserAgent() => Request.Headers["User-Agent"].ToString();
    }
}
