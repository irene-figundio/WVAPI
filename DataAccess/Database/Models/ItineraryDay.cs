using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AI_Integration.DataAccess.Database.Models
{
    public class ItineraryDay
    {
        public int Id { get; set; }
        public int TripId { get; set; }

        public int DayNumber { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        [StringLength(500)]
        public string? Image1 { get; set; }
        [StringLength(500)]
        public string? Image2 { get; set; }
        [StringLength(500)]
        public string? Image3 { get; set; }

        // Auditing
        public DateTime? CreationTime { get; set; }
        public int? Creation_User { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public int? LastModification_User { get; set; }
        public bool? IsDeleted { get; set; } = false;
        public DateTime? DeletionTime { get; set; }
        public int? Deletion_User { get; set; }

        [ForeignKey("TripId")]
        public virtual Trip Trip { get; set; } = null!;

        public virtual ICollection<ItineraryStop> ItineraryStops { get; set; } = new List<ItineraryStop>();
        public virtual ICollection<Stay> Stays { get; set; } = new List<Stay>();
    }
}
