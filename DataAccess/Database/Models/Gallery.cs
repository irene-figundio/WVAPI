using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AI_Integration.DataAccess.Database.Models
{
    public class Gallery
    {
        public bool? IsDeleted { get; set; }
        public DateTime? DeletionDate { get; set; }
        public int Id { get; set; }
        public int EventId { get; set; }
        [StringLength(255)]
        public string? Title { get; set; }
        public DateTime? CreatedAt { get; set; }

        public int LangID { get; set; }
        [ForeignKey("LangID")]
        public virtual Language? Language { get; set; }

        [NotMapped]
        public virtual Event? Event { get; set; } = null!;
        public virtual ICollection<PhotoGallery> Photos { get; set; } = new List<PhotoGallery>();
    }
}