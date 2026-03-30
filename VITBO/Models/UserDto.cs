namespace VITBO.Models
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = default!;
        public string Email { get; set; } = default!;
        public int StatusId { get; set; }
        public DateTime StatusTime { get; set; }
        public DateTime? LastLoginTime { get; set; }
        public bool SuperAdmin { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
