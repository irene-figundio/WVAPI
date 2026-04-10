using System.ComponentModel.DataAnnotations;

namespace VITBO.Models
{
    public class PodcastDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime PublishDate { get; set; }
        public string? CoverImage { get; set; }
        public string? YoutubeUrl { get; set; }
        public string? SpotifyUrl { get; set; }
        public int LangID { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class CreatePodcastRequest
    {
        [Required]
        public string Title { get; set; } = null!;
        [Required]
        public string Description { get; set; } = null!;
        [Required]
        public DateTime PublishDate { get; set; } = DateTime.Now;
        public string? YoutubeUrl { get; set; }
        public string? SpotifyUrl { get; set; }
        public int LangID { get; set; } = 1;

        public Microsoft.AspNetCore.Http.IFormFile? CoverImageFile { get; set; }
    }
}
