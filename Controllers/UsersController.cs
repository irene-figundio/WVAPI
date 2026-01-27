using System;
using System.Linq;
using System.Threading.Tasks;
using AI_Integration.DataAccess;
using AI_Integration.DataAccess.Database.Models;
using AI_Integration.DataAccess.Database.Repositories.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // CountAsync, ToListAsync

namespace AI_Integration.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public UsersController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        // GET /api/users?query=&page=1&pageSize=20
        [HttpGet]
        public async Task<ActionResult<PagedResult<UserDto>>> Get(
            [FromQuery] string? query = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 20 : Math.Min(pageSize, 100);

            var q = _uow.Query<User>()        // IQueryable<Users> (AsNoTracking di default)
                        .Where(u => !u.IsDeleted);

            if (!string.IsNullOrWhiteSpace(query))
            {
                q = q.Where(u => u.Username.Contains(query) || u.Email.Contains(query));
            }

            var total = await q.CountAsync();

            var items = await q.OrderByDescending(u => u.CreationTime)
                               .Skip((page - 1) * pageSize)
                               .Take(pageSize)
                               .Select(u => new UserDto
                               {
                                   Id = u.Id,
                                   Username = u.Username,
                                   Email = u.Email,
                                   StatusId = u.StatusId,
                                   StatusTime = u.StatusTime,
                                   LastLoginTime = u.LastLoginTime,
                                   SuperAdmin = u.SuperAdmin,
                                   CreationTime = u.CreationTime
                               })
                               .ToListAsync();

            return Ok(new PagedResult<UserDto>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                Total = total
            });
        }

        // GET /api/users/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserDto>> GetById(int id)
        {
            var u = await _uow.GetByIdAsync<User>(id);
            if (u == null || u.IsDeleted)
                return NotFound();

            return Ok(new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                StatusId = u.StatusId,
                StatusTime = u.StatusTime,
                LastLoginTime = u.LastLoginTime,
                SuperAdmin = u.SuperAdmin,
                CreationTime = u.CreationTime
            });
        }

        // POST /api/users
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest req,
            [FromHeader(Name = "User-Agent")] string userAgent)
        {
            var log = new WebAPILog
            {
                DateTimeStamp = DateTime.Now,
                RequestMethod = "POST",
                RequestUrl = "api/users",
                RequestBody = req?.Username ?? "",
                UserAgent = userAgent,
                AdditionalInfo = "Create user"
            };

            if (req == null || string.IsNullOrWhiteSpace(req.Username)
                            || string.IsNullOrWhiteSpace(req.Email)
                            || string.IsNullOrWhiteSpace(req.Password))
            {
                log.ResponseCode = 400; log.ResponseMessage = "Bad Request";
                await _uow.InsertAsync(log);
                await _uow.SaveChangesAsync();
                return BadRequest(new { success = false, message = "Username, Email, Password are required." });
            }

            // Uniqueness check (via repo specifico)
            var existing = _uow.Users.GetByUsername(req.Username);
            if (existing != null && !existing.IsDeleted)
            {
                log.ResponseCode = 409; log.ResponseMessage = "Conflict";
                log.ResponseBody = "{ success = false, message = 'Username already exists.' }";
                await _uow.InsertAsync(log);
                await _uow.SaveChangesAsync();
                return Conflict(new { success = false, message = "Username already exists." });
            }

            // Hash password
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(req.Password);

            var user = new User
            {
                Username = req.Username,
                Password = passwordHash,
                Email = req.Email,
                StatusId = req.StatusId,                 // 1 = Active
                StatusTime = DateTime.Now,
                CreationTime = DateTime.Now,
                IsDeleted = false,
                SuperAdmin = req.SuperAdmin,
                VerificationToken = Guid.NewGuid().ToString("N")
            };

            await _uow.InsertAsync(user);
            log.ResponseCode = 201; log.ResponseMessage = "Created";
            await _uow.InsertAsync(log);
            await _uow.SaveChangesAsync();

            return StatusCode(201, new { success = true, id = user.Id });
        }

        // PUT /api/users/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserRequest req,
            [FromHeader(Name = "User-Agent")] string userAgent)
        {
            var log = new WebAPILog
            {
                DateTimeStamp = DateTime.Now,
                RequestMethod = "PUT",
                RequestUrl = $"api/users/{id}",
                RequestBody = req?.ToString() ?? "",
                UserAgent = userAgent,
                AdditionalInfo = "Update user"
            };

            var user = await _uow.GetByIdAsync<User>(id);
            if (user == null || user.IsDeleted)
            {
                log.ResponseCode = 404; log.ResponseMessage = "Not Found";
                await _uow.InsertAsync(log);
                await _uow.SaveChangesAsync();
                return NotFound();
            }

            if (req.Email != null) user.Email = req.Email;
            if (req.SuperAdmin.HasValue) user.SuperAdmin = req.SuperAdmin.Value;
            if (req.StatusId.HasValue)
            {
                user.StatusId = req.StatusId.Value;
                user.StatusTime = DateTime.Now;
            }

            user.LastModificationTime = DateTime.Now;
            // user.LastModification_User = <callerId se disponibile>

            _uow.Update(user);

            log.ResponseCode = 200; log.ResponseMessage = "OK";
            await _uow.InsertAsync(log);
            await _uow.SaveChangesAsync();

            return Ok(new { success = true });
        }

        // PUT /api/users/{id}/password
        [HttpPut("{id:int}/password")]
        public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordRequest req,
            [FromHeader(Name = "User-Agent")] string userAgent)
        {
            var log = new WebAPILog
            {
                DateTimeStamp = DateTime.Now,
                RequestMethod = "PUT",
                RequestUrl = $"api/users/{id}/password",
                RequestBody = "", // non loggare password
                UserAgent = userAgent,
                AdditionalInfo = "Change password"
            };

            var user = await _uow.GetByIdAsync<User>(id);
            if (user == null || user.IsDeleted)
            {
                log.ResponseCode = 404; log.ResponseMessage = "Not Found";
                await _uow.InsertAsync(log);
                await _uow.SaveChangesAsync();
                return NotFound();
            }

            if (!BCrypt.Net.BCrypt.Verify(req.CurrentPassword, user.Password))
                return Unauthorized(new { success = false, message = "Current password is incorrect." });

            user.Password = BCrypt.Net.BCrypt.HashPassword(req.NewPassword);
            user.LastPasswordModificationTime = DateTime.Now;

            _uow.Update(user);
            log.ResponseCode = 200; log.ResponseMessage = "OK";
            await _uow.InsertAsync(log);
            await _uow.SaveChangesAsync();

            return Ok(new { success = true });
        }

        // PUT /api/users/{id}/status
        [HttpPut("{id:int}/status")]
        public async Task<IActionResult> SetStatus(int id, [FromBody] SetStatusRequest req,
            [FromHeader(Name = "User-Agent")] string userAgent)
        {
            var log = new WebAPILog
            {
                DateTimeStamp = DateTime.Now,
                RequestMethod = "PUT",
                RequestUrl = $"api/users/{id}/status",
                RequestBody = $"StatusId={req?.StatusId}",
                UserAgent = userAgent,
                AdditionalInfo = "Set user status"
            };

            var user = await _uow.GetByIdAsync<User>(id);
            if (user == null || user.IsDeleted)
            {
                log.ResponseCode = 404; log.ResponseMessage = "Not Found";
                await _uow.InsertAsync(log);
                await _uow.SaveChangesAsync();
                return NotFound();
            }

            user.StatusId = req.StatusId;
            user.StatusTime = DateTime.Now;

            _uow.Update(user);
            log.ResponseCode = 200; log.ResponseMessage = "OK";
            await _uow.InsertAsync(log);
            await _uow.SaveChangesAsync();

            return Ok(new { success = true });
        }

        // DELETE /api/users/{id} (soft delete)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> SoftDelete(int id,
            [FromHeader(Name = "User-Agent")] string userAgent)
        {
            var log = new WebAPILog
            {
                DateTimeStamp = DateTime.Now,
                RequestMethod = "DELETE",
                RequestUrl = $"api/users/{id}",
                RequestBody = "",
                UserAgent = userAgent,
                AdditionalInfo = "Soft delete user"
            };

            var user = await _uow.GetByIdAsync<User>(id);
            if (user == null || user.IsDeleted)
            {
                log.ResponseCode = 404; log.ResponseMessage = "Not Found";
                await _uow.InsertAsync(log);
                await _uow.SaveChangesAsync();
                return NotFound();
            }

            user.IsDeleted = true;
            user.DeletionTime = DateTime.Now;
            // user.Deletion_User = <callerId se disponibile>

            _uow.Update(user);
            log.ResponseCode = 204; 
            log.ResponseMessage = "No Content";
            await _uow.InsertAsync(log);
            await _uow.SaveChangesAsync();

            return NoContent();
        }
    }
}