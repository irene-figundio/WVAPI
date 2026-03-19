using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VITBO.Models;
using VITBO.Services;

namespace VITBO.Controllers
{
    [Authorize]
    public class ContentsController : Controller
    {
        private readonly ApiService _apiService;

        public ContentsController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Articles([FromQuery] int langId = 1)
        {
            var endpoint = $"api/Contents/type/Article?langId={langId}";
            var result = await _apiService.GetAsync<List<ContentDto>>(endpoint);

            if (result == null)
            {
                ModelState.AddModelError("", "Error retrieving articles from API.");
                result = new List<ContentDto>();
            }

            ViewData["CurrentLangId"] = langId;
            return View(result);
        }

        public async Task<IActionResult> News([FromQuery] int langId = 1)
        {
            var endpoint = $"api/Contents/type/News?langId={langId}";
            var result = await _apiService.GetAsync<List<ContentDto>>(endpoint);

            if (result == null)
            {
                ModelState.AddModelError("", "Error retrieving news from API.");
                result = new List<ContentDto>();
            }

            ViewData["CurrentLangId"] = langId;
            return View(result);
        }
    }
}
