namespace AI_Integration.DataAccess.Database.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!; // contiene hash (es. BCrypt)
        public string Email { get; set; } = null!;

        public int StatusId { get; set; }
        public DateTime StatusTime { get; set; }

        public DateTime? LastLoginTime { get; set; }
        public DateTime? LastPasswordModificationTime { get; set; }

        public DateTime CreationTime { get; set; }
        public int? CreationUserId { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public int? LastModificationUserId { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime? DeletionTime { get; set; }
        public int? DeletionUserId { get; set; }

        public bool SuperAdmin { get; set; }

        public string VerificationToken { get; set; } = null!;
        public string? ResetPasswordCode { get; set; }
        public DateTime? ResetPasswordCodeExpiration { get; set; }

        // Navigation properties
        public virtual UserStatus Status { get; set; } = null!;

        public virtual User? CreationUser { get; set; }
        public virtual User? LastModificationUser { get; set; }
        public virtual User? DeletionUser { get; set; }

        public virtual ICollection<User> CreatedUsers { get; set; } = new List<User>();
        public virtual ICollection<User> ModifiedUsers { get; set; } = new List<User>();
        public virtual ICollection<User> DeletedUsers { get; set; } = new List<User>();
    }
}
