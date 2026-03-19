using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using VITBO.Models;

namespace VITBO.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public AccountController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
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

            if (ModelState.IsValid)
            {
                var client = _httpClientFactory.CreateClient();
                // Base address of the API
                client.BaseAddress = new Uri(_configuration["ApiBaseAddress"]);

                var loginData = new { username = model.Username, password = model.Password };
                //var content = new StringContent(JsonSerializer.Serialize(loginData), Encoding.UTF8, "application/json");
                var body = JsonSerializer.Serialize(loginData);
                
                try
                {
                    // var response = await client.PostAsync($"https://www.geordie.app:441/VitinerarioGateTest/api/auth/login", content);
                    var request = new HttpRequestMessage(HttpMethod.Post, "https://www.geordie.app:441/VitinerarioGateTest/api/auth/login");
                    request.Content = new StringContent(body, Encoding.UTF8);
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var tokenResult = JsonSerializer.Deserialize<TokenResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                        if (tokenResult != null && !string.IsNullOrEmpty(tokenResult.Token))
                        {
                            // Store JWT token in session
                            HttpContext.Session.SetString("JWToken", tokenResult.Token);

                            // Create Cookie Authentication
                            var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Name, model.Username),
                                // you can decode JWT here to add more specific claims if needed
                            };

                            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                            var authProperties = new AuthenticationProperties
                            {
                                IsPersistent = model.RememberMe,
                                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
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
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    }
                    else
                    {
                         ModelState.AddModelError(string.Empty, $"API Error: {response.StatusCode}");
                    }
                }
                catch(Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Connection Error: {ex.Message}");
                }
            }

            // If we got this far, something failed, redisplay form
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

        // Helper class for deserializing
        private class TokenResponse
        {
            public string? Token { get; set; }
            public DateTime Expiration { get; set; }
        }
    }
}
