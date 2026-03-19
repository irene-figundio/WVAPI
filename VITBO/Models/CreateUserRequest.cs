using System.ComponentModel.DataAnnotations;

namespace VITBO.Models
{
    public class CreateUserRequest
    {
        [Required]
        public string Username { get; set; } = default!;

        [Required, EmailAddress]
        public string Email { get; set; } = default!;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = default!;

        public bool SuperAdmin { get; set; } = false;

        public int StatusId { get; set; } = 1;
    }
}
