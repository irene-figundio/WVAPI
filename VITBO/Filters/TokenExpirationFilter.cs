using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace VITBO.Filters
{
    public class TokenExpirationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Skip filter for AllowAnonymous endpoints (like Login)
            if (context.ActionDescriptor.EndpointMetadata.Any(em => em.GetType() == typeof(Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute)) ||
                context.HttpContext.Request.Path.StartsWithSegments("/Account/Login"))
            {
                await next();
                return;
            }

            var token = context.HttpContext.User.FindFirst("JWToken")?.Value ?? context.HttpContext.Session.GetString("JWToken");

            if (!string.IsNullOrEmpty(token))
            {
                var handler = new JwtSecurityTokenHandler();
                try
                {
                    if (handler.CanReadToken(token))
                    {
                        var jwtToken = handler.ReadJwtToken(token);
                        var expClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp)?.Value;

                        if (expClaim != null && long.TryParse(expClaim, out long expTime))
                        {
                            var expirationTime = DateTimeOffset.FromUnixTimeSeconds(expTime).UtcDateTime;
                            if (expirationTime <= DateTime.UtcNow)
                            {
                                // Token is expired. Trigger logout and redirect to login with ReturnUrl
                                await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                                context.HttpContext.Session.Remove("JWToken");

                                var returnUrl = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;
                                context.Result = new RedirectToActionResult("Login", "Account", new { ReturnUrl = returnUrl });
                                return;
                            }
                        }
                    }
                }
                catch
                {
                    // Invalid token structure
                    await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    context.HttpContext.Session.Remove("JWToken");

                    var returnUrl = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;
                    context.Result = new RedirectToActionResult("Login", "Account", new { ReturnUrl = returnUrl });
                    return;
                }
            }

            await next();
        }
    }
}
