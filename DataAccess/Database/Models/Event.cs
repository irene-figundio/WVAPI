using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AI_Integration.DataAccess.Database.Models
{
    public class Event
    {
        public int Id { get; set; }
        [Required]
        [StringLength(255)]
        public string Title { get; set; } = null!;
        [Required]
        public string Description { get; set; } = null!;
        public DateTime EventDate { get; set; }
        [StringLength(500)]
        public string? CoverImage { get; set; }
        public int? GalleryId { get; set; }
        public DateTime? CreatedAt { get; set; }

        public DateTime? BookingEndDate { get; set; }
        [StringLength(500)]
        public string? Location { get; set; }
        [StringLength(255)]
        public string? Organizer { get; set; }
        [StringLength(255)]
        public string? ContactInfo { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public bool IsOnline { get; set; }

        public int LangID { get; set; }
        [ForeignKey("LangID")]
        public virtual Language? Language { get; set; }

        [NotMapped]
        public virtual Gallery? Gallery { get; set; }
        public virtual ICollection<EventLink> EventLinks { get; set; } = new List<EventLink>();

        public virtual ICollection<EventExpert> EventExperts { get; set; } = new List<EventExpert>();
    }
}