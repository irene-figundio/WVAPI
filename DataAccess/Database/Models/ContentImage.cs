using System;
using System.ComponentModel.DataAnnotations;

namespace AI_Integration.DataAccess.Database.Models
{
    public class ContentImage
    {
        public int Id { get; set; }
        public int ContentId { get; set; }
        [Required]
        [StringLength(500)]
        public string ImageUrl { get; set; } = null!;
        [StringLength(255)]
        public string? Caption { get; set; }
        public int? Position { get; set; }
        public DateTime? CreatedAt { get; set; }

        public virtual Content Content { get; set; } = null!;
    }
}