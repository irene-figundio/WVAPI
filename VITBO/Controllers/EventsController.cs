using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VITBO.Models;
using VITBO.Services;

namespace VITBO.Controllers
{
    [Authorize]
    public class EventsController : Controller
    {
        private readonly ApiService _apiService;

        public EventsController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index([FromQuery] int langId = 1)
        {
            var endpoint = $"api/Events?langId={langId}";
            var result = await _apiService.GetAsync<List<EventDto>>(endpoint);

            if (result == null)
            {
                ModelState.AddModelError("", "Error retrieving events from API.");
                result = new List<EventDto>();
            }

            ViewData["CurrentLangId"] = langId;
            return View(result);
        }
    }
}
