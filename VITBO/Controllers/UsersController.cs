using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VITBO.Models;
using VITBO.Services;

namespace VITBO.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly ApiService _apiService;

        public UsersController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index([FromQuery] string? query = null, [FromQuery] int page = 1)
        {
            var endpoint = $"api/Users?page={page}&pageSize=20";
            if (!string.IsNullOrEmpty(query))
            {
                endpoint += $"&query={Uri.EscapeDataString(query)}";
                ViewData["CurrentFilter"] = query;
            }

            var result = await _apiService.GetAsync<PagedResult<UserDto>>(endpoint);
            if (result == null)
            {
                ModelState.AddModelError("", "Error retrieving users from API.");
                result = new PagedResult<UserDto> { Items = new List<UserDto>() };
            }

            return View(result);
        }
    }
}
