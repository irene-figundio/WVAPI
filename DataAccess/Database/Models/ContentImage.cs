using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AI_Integration.DataAccess.Database.Models
{
    public class ContentImage
    {
        public bool? IsDeleted { get; set; } = false;
        public DateTime? DeletionDate { get; set; }
        public int Id { get; set; }
        public int ContentId { get; set; }
        [Required]
        [StringLength(500)]
        public string ImageUrl { get; set; } = null!;
        [StringLength(255)]
        public string? Caption { get; set; }
        public int? Position { get; set; }
        public DateTime? CreatedAt { get; set; }

        public int LangID { get; set; }
        [ForeignKey("LangID")]
        public virtual Language? Language { get; set; }

        [NotMapped]
        public virtual Content? Content { get; set; } = null!;
    }
}