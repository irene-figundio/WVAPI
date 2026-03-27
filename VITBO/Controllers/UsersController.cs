using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VITBO.Services.Interfaces;
using VITBO.Models;

namespace VITBO.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly IUsersService _usersService;

        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        public async Task<IActionResult> Index([FromQuery] string? query = null, [FromQuery] int page = 1)
        {
            ViewData["CurrentFilter"] = query;
            var token = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
            var userAgent = GetUserAgent() ?? string.Empty;
            var result = await _usersService.GetUsersAsync(query, page,token,userAgent);
            return View(result);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateUserRequest());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserRequest model)
        {
            if (ModelState.IsValid)
            {
                var token = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
                var userAgent = GetUserAgent() ?? string.Empty;
                var success = await _usersService.CreateUserAsync(model,token,userAgent);
                if (success)
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Failed to create user. Please try again.");
            }
            return View(model);

        }
        protected string? GetUserAgent()
        {
            return Request.Headers["User-Agent"].ToString();
        }
    }
}
