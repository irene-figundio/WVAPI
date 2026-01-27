using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AI_Integration.DataAccess.Database.Models
{
    public class Gallery
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        [StringLength(255)]
        public string? Title { get; set; }
        public DateTime? CreatedAt { get; set; }

        public virtual Event Event { get; set; } = null!;
        public virtual ICollection<PhotoGallery> Photos { get; set; } = new List<PhotoGallery>();
    }
}