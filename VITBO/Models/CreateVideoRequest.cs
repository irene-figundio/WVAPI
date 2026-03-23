using System.ComponentModel.DataAnnotations;

namespace VITBO.Models
{
    public class CreateVideoRequest
    {
        [Required]
        public string Title { get; set; } = default!;

        public string? Url_Video { get; set; }

        public IFormFile? File { get; set; }

        public bool? IsLandscape { get; set; } = false;

        public int Play_Priority { get; set; } = 0;

        public int ID_Session { get; set; } = 1;
    }
}
