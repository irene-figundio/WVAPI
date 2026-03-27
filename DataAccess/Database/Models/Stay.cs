using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AI_Integration.DataAccess.Database.Models
{
    public class Stay
    {
        public int Id { get; set; }
        public int TripId { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        [StringLength(500)]
        public string? Image { get; set; }

        [StringLength(500)]
        public string? Location { get; set; }

        public int OrderIndex { get; set; }

        public int? ItineraryDayId { get; set; }

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

        [ForeignKey("ItineraryDayId")]
        public virtual ItineraryDay? ItineraryDay { get; set; }
    }
}
