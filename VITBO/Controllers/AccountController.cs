using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using VITBO.Models;
using VITBO.Services;
using VITBO.Services.Interfaces;

namespace VITBO.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly HttpService _httpService;
        private IConfiguration _configuration;
        private readonly string _apiBaseUrl;

        public AccountController(IAuthService authService, HttpService httpService,IConfiguration configuration)
        {
            _authService = authService;
            _httpService = httpService;
            _configuration = configuration;
            _apiBaseUrl = _configuration["ApiBaseAddress"] ?? "https://localhost:7275";
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
              var requestBody = new PostModel
        {
            Username = model.Username,
            Password = model.Password
        };

        var response = await _httpService.SendHttpRequestAsync(HttpMethod.Post, $"{_apiBaseUrl}auth/login", null, requestBody, userAgent: GetUserAgent());


        var (isValid, error) = this.ValidateHttpResponse(
            response,
            HttpStatusCode.OK
        );

        if (!isValid)
        {
            if (response != null)
            {
                    ModelState.AddModelError(string.Empty, $"Login validation failed. HTTP status: {response.StatusCode}");
                }
            else
            {
                ModelState.AddModelError(string.Empty, "Login validation failed. Response is null.");
            }
            return View();
        }

        var session = await _httpService.GetBodyFromHttpResponseAsync<PostModelResponse>(response);

            try
            {
               
               var token = session?.Token;
                if (!string.IsNullOrEmpty(token))
                {
                    HttpContext.Session.SetString("JWToken", token);

                    var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, model.Username),
                            new Claim("JWToken", token)
                        };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Connection Error: {ex.Message}");
            }
            //}

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Remove("JWToken");
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
        protected string? GetUserAgent()
        {
            return Request.Headers["User-Agent"].ToString();
        }
        protected (bool, string) ValidateHttpResponse(HttpResponseMessage? response, HttpStatusCode expectedStatus)
        {
            if (response == null)
            {
                return (false, "Unable to connect to the server.");
            }

            if (response.StatusCode != expectedStatus)
            {
                return (false, "The request could not be processed.");
            }

            return (true, "Request processed successfully.");
        }
    }
}
