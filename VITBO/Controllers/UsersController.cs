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
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
            var result = await _usersService.GetUsersAsync(query, page, token, userAgent);
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
                var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
                var success = await _usersService.CreateUserAsync(model, token, userAgent);
                if (success)
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Failed to create user. Please try again.");
            }
            return View(model);

        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var token = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
            var user = await _usersService.GetUserByIdAsync(id, token, userAgent);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var token = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
            var success = await _usersService.DeleteUserAsync(id, token, userAgent);
            if (success)
            {
                return RedirectToAction(nameof(Index));
            }
            return BadRequest("Failed to delete user. Please try again.");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var token = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
            var user = await _usersService.GetUserByIdAsync(id, token, userAgent);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserDto model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }
            if (ModelState.IsValid)
            {
                var token = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
                var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
                var success = await _usersService.UpdateUserAsync(id,model, token, userAgent);
                if (success)
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Failed to update user. Please try again.");
            }
            return View(model);
        }

        protected string? GetUserAgent()
        {
            return Request.Headers["User-Agent"].ToString();
        }
    }
}
