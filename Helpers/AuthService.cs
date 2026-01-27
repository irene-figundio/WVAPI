using AI_Integration.DataAccess;
using AI_Integration.DataAccess.Database.Models;
using AI_Integration.DataAccess.Database.Repositories.interfaces;
using AI_Integration.Model;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AI_Integration.Helpers
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly JwtOptions _jwtOptions;
        private DateTime _expires;

        public AuthService(IUnitOfWork unitOfWork, IConfiguration config, IOptions<JwtOptions> jwtOptions)
        {
            _unitOfWork = unitOfWork;
            _jwtOptions = jwtOptions.Value;
        }

        public async Task<(string Token, DateTime Expires)> LoginWithCredentialsAsync(LoginRequest loginRequest, string? userAgent)
        {
            if (loginRequest is null || string.IsNullOrWhiteSpace(loginRequest.Username) || string.IsNullOrWhiteSpace(loginRequest.Password))
                throw new ArgumentException("Username and password are required.");

            var user = _unitOfWork.Users.GetByUsername(loginRequest.Username);
            if (user is null || user.IsDeleted)
                throw new UnauthorizedAccessException("Invalid credentials.");

            if (!_unitOfWork.Users.IsActive(user.StatusId))
                throw new InvalidOperationException("User not active.");

            // Verifica password (BCrypt)
            if (!BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.Password))
                throw new UnauthorizedAccessException("Invalid credentials.");

            var platform = "pwa"; // opzionale: ricavabile dal User-Agent
            var jwt = GenerateJwtTokenWithUser(platform, user);
            var apiToken = new APIToken
            {
                DataCreation = DateTime.Now,
                Token = jwt,
                DataExpiration = _expires,
                Platform = platform
            };

            await _unitOfWork.InsertAsync(apiToken);
            await _unitOfWork.SaveChangesAsync();
            _unitOfWork.Users.UpdateLastLoginTime(user.Id, DateTime.Now);

            return (jwt, _expires);
        }

        private string GenerateJwtTokenWithUser(string platform, User user)
        {
            var handler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtOptions.SecretKey);
            _expires = DateTime.Now.AddHours(_jwtOptions.ExpiryHours);

            var identity = new ClaimsIdentity(new[]
            {
            new Claim("platform", platform),
            new Claim("user_id", user.Id.ToString()),
            new Claim("username", user.Username),
            new Claim("super_admin", user.SuperAdmin ? "true" : "false"),
            // compatibilità con tua logica esistente
            new Claim("custom_claim", "V!tin3rar!0@2024-allowed")
        });

            var descriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = _expires,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = handler.CreateToken(descriptor);
            return handler.WriteToken(token);
        }
    }
}
