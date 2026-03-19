using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VITBO.Models;
using VITBO.Services;

namespace VITBO.Controllers
{
    [Authorize]
    public class AiVideoController : Controller
    {
        private readonly ApiService _apiService;

        public AiVideoController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index([FromQuery] string? q = null, [FromQuery] int page = 1)
        {
            var endpoint = $"api/Video?page={page}&pageSize=20";
            if (!string.IsNullOrEmpty(q))
            {
                endpoint += $"&q={Uri.EscapeDataString(q)}";
                ViewData["CurrentFilter"] = q;
            }

            var result = await _apiService.GetAsync<PagedResult<VideoListItemDto>>(endpoint);
            if (result == null)
            {
                ModelState.AddModelError("", "Error retrieving videos from API.");
                result = new PagedResult<VideoListItemDto> { Items = new List<VideoListItemDto>() };
            }

            return View(result);
        }
    }
}
