using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AI_Integration.DataAccess.Database.Models
{
    public class Content
    {
        public int Id { get; set; }
        public int LangID { get; set; }
        [Required]
        [StringLength(255)]
        public string Title { get; set; } = null!;
        [Required]
        public string Text { get; set; } = null!;
        public DateTime PublishDate { get; set; }
        [StringLength(500)]
        public string? CoverImage { get; set; }
        [Required]
        [StringLength(20)]
        public string ContentType { get; set; } = null!;
        public bool? IsPublished { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("LangID")]
        public virtual Language? Language { get; set; }

        public virtual ICollection<ContentImage> ContentImages { get; set; } = new List<ContentImage>();
        public virtual ICollection<ContentLink> ContentLinks { get; set; } = new List<ContentLink>();
    }
}