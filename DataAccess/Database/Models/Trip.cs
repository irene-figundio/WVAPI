using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AI_Integration.DataAccess.Database.Models
{
    public class Trip
    {
        public int Id { get; set; }
        public int EventId { get; set; }

        [Required]
        [StringLength(255)]
        public string DepartureCity { get; set; } = null!;

        [Required]
        [StringLength(255)]
        public string DepartureCountry { get; set; } = null!;

        [Required]
        [StringLength(255)]
        public string ArrivalCity { get; set; } = null!;

        [StringLength(255)]
        public string? ArrivalCountry { get; set; }

        public int DurationDays { get; set; }
        public int DurationNights { get; set; }
        public int MaxGuests { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = null!; // done, cancelled, booking, started, in_progress

        // Auditing
        public DateTime? CreationTime { get; set; }
        public int? Creation_User { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public int? LastModification_User { get; set; }
        public bool? IsDeleted { get; set; } = false;
        public DateTime? DeletionTime { get; set; }
        public int? Deletion_User { get; set; }

        [ForeignKey("EventId")]
        public virtual Event Event { get; set; } = null!;

        public virtual ICollection<Stay> Stays { get; set; } = new List<Stay>();
        public virtual ICollection<ItineraryDay> ItineraryDays { get; set; } = new List<ItineraryDay>();
        public virtual ICollection<TripMust> TripMusts { get; set; } = new List<TripMust>();
    }
}
