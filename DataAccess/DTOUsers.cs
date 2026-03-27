using AI_Integration.DataAccess.Database.Models;

namespace AI_Integration.DataAccess
{
    public class DTOUsers
    {
    }
    // DTO di base
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = default!;
        public string Email { get; set; } = default!;
        public int StatusId { get; set; }           // 0=NotActive, 1=Active
        public DateTime StatusTime { get; set; }
        public DateTime? LastLoginTime { get; set; }
        public bool SuperAdmin { get; set; }
        public DateTime CreationTime { get; set; }
    }

    public class CreateUserRequest
    {
        public string Username { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public bool SuperAdmin { get; set; } = false;
        public int StatusId { get; set; } = 1;       // default Active
    }

    public class UpdateUserRequest
    {
        public string? Email { get; set; }
        public bool? SuperAdmin { get; set; }
        public int? StatusId { get; set; }           // 0/1
    }

    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; } = default!;
        public string NewPassword { get; set; } = default!;
    }

    public class SetStatusRequest
    {
        public int StatusId { get; set; }            // 0/1
    }

    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
    }

    // Per Auth/Login
    public class LoginRequest
    {
        public string Username { get; set; } = default!;
        public string Password { get; set; } = default!;
    }


    public class UpdateVideoRequest
    {
        public string? Title { get; set; }
        public string? Url_Video { get; set; }      // mantengo naming attuale del modello
        public bool? IsLandscape { get; set; }
        public int? Play_Priority { get; set; }
        public int? ID_Session { get; set; }        // se vuoi consentire il cambio sessione
        public bool? IsDeleted { get; set; } = false;
    }
    public class VideoListItemDto
    {
        public int Id { get; set; }
        public int ID_Session { get; set; }
        public string? Title { get; set; }
        public string? Url_Video { get; set; }
        public string PublicUrl { get; set; } = string.Empty;
        public bool? IsLandscape { get; set; }
        public int? Play_Priority { get; set; }
        public DateTime DataCreation { get; set; }

        public List<AdSession> Sessions { get; set; } = new();
    }


}
