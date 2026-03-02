using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AI_Integration.DataAccess.Database.Models
{
    public class ContentLink
    {
        public int Id { get; set; }
        public int ContentId { get; set; }
        [Required]
        [StringLength(500)]
        public string LinkUrl { get; set; } = null!;
        [StringLength(255)]
        public string? Description { get; set; }
        public DateTime? CreatedAt { get; set; }

        public int LangID { get; set; }
        [ForeignKey("LangID")]
        public virtual Language? Language { get; set; }

        [NotMapped]
        public virtual Content? Content { get; set; } = null!;
    }
}