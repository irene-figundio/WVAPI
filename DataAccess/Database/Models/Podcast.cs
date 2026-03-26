using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AI_Integration.DataAccess.Database.Models
{
    public class Podcast
    {
        public bool? IsDeleted { get; set; }
        public DateTime? DeletionDate { get; set; }
        public int Id { get; set; }
        [Required]
        [StringLength(255)]
        public string Title { get; set; } = null!;
        [Required]
        public string Description { get; set; } = null!;
        public DateTime PublishDate { get; set; }
        [StringLength(500)]
        public string? CoverImage { get; set; }
        [StringLength(500)]
        public string? YoutubeUrl { get; set; }
        [StringLength(500)]
        public string? SpotifyUrl { get; set; }
        public DateTime? CreatedAt { get; set; }

        public int LangID { get; set; }
        [ForeignKey("LangID")]
        public virtual Language? Language { get; set; }
    }
}