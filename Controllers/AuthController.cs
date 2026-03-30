using AI_Integration.DataAccess;
using AI_Integration.DataAccess.Database.Models;
using AI_Integration.DataAccess.Database.Repositories.interfaces;
using AI_Integration.Helpers;
using AI_Integration.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AI_Integration.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private IHttpClientFactory? _httpClientFactory;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthService _authService;
        private readonly JwtOptions _jwtOptions;
        private DateTime expires;
        public AuthController(IHttpClientFactory httpClientFactory, 
            IUnitOfWork unitOfWork,
            IAuthService authService,
            IOptions<JwtOptions> jwtOptions)
        {
            _httpClientFactory = httpClientFactory;
            _unitOfWork = unitOfWork;
            _authService = authService;
            _jwtOptions = jwtOptions.Value;
        }

         [HttpGet("GenerateTestToken")]
        public IActionResult Encode(string input)
        {
            //Token test : V!tin3rar!0@2024-{{pwa}}${{20250505T124457000Z}}
            if (string.IsNullOrEmpty(input))
            {
                return BadRequest("Input cannot be empty.");
            }

            // Converti la stringa in Base64
            var base64String = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(input));

            return Ok(base64String);
        }

        // POST api/auth/token  (token exchange base64 → JWT)
        [HttpPost("token")]
        public async Task<IActionResult> Login([FromBody] TokenRequest tokenRequest,
            [FromHeader(Name = "User-Agent")] string userAgent)
        {
            var log = new WebAPILog
            {
                DateTimeStamp = DateTime.Now,
                RequestMethod = "POST",
                RequestUrl = "api/auth/token",
                RequestBody = tokenRequest?.Token ?? "",
                UserAgent = userAgent,
                AdditionalInfo = "Login API - Autenticazione con Token"
            };

            if (tokenRequest == null || string.IsNullOrWhiteSpace(tokenRequest.Token))
            {
                log.ResponseCode = 400;
                log.ResponseMessage = "Bad Request";
                log.ResponseBody = "{ success = false, message = 'Token is required' }";
                await _unitOfWork.InsertAsync(log);
                await _unitOfWork.SaveChangesAsync();
                return BadRequest(new { success = false, message = "Token is required" });
            }

            string decodedToken;
            try
            {
                decodedToken = DecodeBase64(tokenRequest.Token);
            }
            catch
            {
                log.ResponseCode = 400;
                log.ResponseMessage = "Bad Request";
                log.ResponseBody = "{ success = false, message = 'Invalid base64 token' }";
                await _unitOfWork.InsertAsync(log);
                await _unitOfWork.SaveChangesAsync();
                return BadRequest(new { success = false, message = "Invalid base64 token" });
            }

            // Estrai {{platform}} e {{timestamp}}
            const string pattern = @"\{\{([^}]*)\}\}";
            string platform = "";
            var matches = Regex.Matches(decodedToken, pattern);
            if (matches.Count < 2)
            {
                log.ResponseCode = 401;
                log.ResponseMessage = "Unauthorized";
                log.ResponseBody = "{ success = false, message = 'Token non valido' }";
                await _unitOfWork.InsertAsync(log);
                await _unitOfWork.SaveChangesAsync();
                return Unauthorized();
            }

            try
            {
                platform = matches[0].Groups[1].Value;
                string timestamp = matches[1].Groups[1].Value;

                if (string.IsNullOrWhiteSpace(platform) || string.IsNullOrWhiteSpace(timestamp))
                {
                    log.ResponseCode = 401;
                    log.ResponseMessage = "Unauthorized";
                    log.ResponseBody = "{ success = false, message = 'Token non valido' }";
                    await _unitOfWork.InsertAsync(log);
                    await _unitOfWork.SaveChangesAsync();
                    return Unauthorized();
                }

                var p = platform.ToLowerInvariant();
                if (p != "android" && p != "ios" && p != "pwa")
                {
                    log.ResponseCode = 401;
                    log.ResponseMessage = "Unauthorized";
                    log.ResponseBody = "{ success = false, message = 'Token non valido' }";
                    await _unitOfWork.InsertAsync(log);
                    await _unitOfWork.SaveChangesAsync();
                    return Unauthorized();
                }

                if (!TokenService.IsIso8601Timestamp(timestamp))
                {
                    log.ResponseCode = 401;
                    log.ResponseMessage = "Unauthorized";
                    log.ResponseBody = "{ success = false, message = 'Token non valido' }";
                    await _unitOfWork.InsertAsync(log);
                    await _unitOfWork.SaveChangesAsync();
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                log.ResponseCode = 401;
                log.ResponseMessage = "Unauthorized";
                log.ResponseBody = "{ success = false, message = '" + ex.Message + "'}";
                await _unitOfWork.InsertAsync(log);
                await _unitOfWork.SaveChangesAsync();
                return Unauthorized();
            }

            // Token valido → genera JWT
            var newToken = GenerateJwtToken(platform);

            // salva JWT a DB (APIToken) con UoW generico
            var dbToken = new APIToken
            {
                DataCreation = DateTime.Now,
                Token = newToken,
                DataExpiration = expires,
                Platform = platform
            };
            await _unitOfWork.InsertAsync(dbToken);

            log.ResponseCode = 200;
            log.ResponseMessage = "OK";
            log.ResponseBody = "{ token = '" + newToken + "' }";

            await _unitOfWork.InsertAsync(log);
            await _unitOfWork.SaveChangesAsync();

            return Ok(new { token = newToken, expires });
        }


        private string DecodeBase64(string base64String)
        {
            var bytes = Convert.FromBase64String(base64String);
            return Encoding.UTF8.GetString(bytes);
        }

        private string GenerateJwtToken(string platform)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtOptions.SecretKey);
            this.expires = DateTime.Now.AddHours(4);
            var newClaims = new ClaimsIdentity(new[]
            {
                new Claim("custom_claim", "V!tin3rar!0@2024-allowed") // Nuovo claim personalizzato da aggiungere
                // Puoi aggiungere altri nuovi claim personalizzati se necessario
            });
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("platform", platform) }),
                Expires = this.expires,
                Issuer = "Vitinerario_API",
                Audience = "Vitinerario_API",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var existingClaims = new ClaimsIdentity();
            existingClaims.AddClaims(tokenDescriptor.Subject.Claims);
            existingClaims.AddClaims(newClaims.Claims);
            tokenDescriptor.Subject = existingClaims;
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        // POST api/auth/login  (username/password → JWT via AuthService)
        [HttpPost("login")]
        public async Task<IActionResult> LoginWithCredentials([FromBody] LoginRequest loginRequest,
            [FromHeader(Name = "User-Agent")] string userAgent)
        {
            var log = new WebAPILog
            {
                DateTimeStamp = DateTime.Now,
                RequestMethod = "POST",
                RequestUrl = "api/auth/login",
                RequestBody = loginRequest?.Username ?? "",
                UserAgent = userAgent,
                AdditionalInfo = "Login API - Autenticazione con username/password"
            };

            try
            {
                var (token, exp) = await _authService.LoginWithCredentialsAsync(loginRequest, userAgent);

                log.ResponseCode = 200;
                log.ResponseMessage = "OK";
                log.ResponseBody = "{ token = '" + token + "' }";
                await _unitOfWork.InsertAsync(log);
                await _unitOfWork.SaveChangesAsync();

                return Ok(new { token, expires = exp });
            }
            catch (ArgumentException ex)
            {
                log.ResponseCode = 400;
                log.ResponseMessage = "Bad Request";
                log.ResponseBody = ex.Message;
                await _unitOfWork.InsertAsync(log);
                await _unitOfWork.SaveChangesAsync();
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                log.ResponseCode = 401;
                log.ResponseMessage = "Unauthorized";
                log.ResponseBody = ex.Message;
                await _unitOfWork.InsertAsync(log);
                await _unitOfWork.SaveChangesAsync();
                return Unauthorized(new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                log.ResponseCode = 403;
                log.ResponseMessage = "Forbidden";
                log.ResponseBody = ex.Message;
                await _unitOfWork.InsertAsync(log);
                await _unitOfWork.SaveChangesAsync();
                return StatusCode(403, new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                log.ResponseCode = 500;
                log.ResponseMessage = "Internal Server Error";
                log.ResponseBody = ex.Message;
                await _unitOfWork.InsertAsync(log);
                await _unitOfWork.SaveChangesAsync();
                return StatusCode(500, new { success = false, message = "Error: " + ex.Message });
            }
        }
    }

    public class TokenRequest
    {
        public string Token { get; set; }
    }


}