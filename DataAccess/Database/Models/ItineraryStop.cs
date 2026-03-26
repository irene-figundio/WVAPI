using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AI_Integration.DataAccess.Database.Models
{
    public class ItineraryStop
    {
        public int Id { get; set; }
        public int DayId { get; set; }

        public TimeSpan? Time { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        [Required]
        [StringLength(50)]
        public string Type { get; set; } = null!; // activity, meal, transfer, experience

        public int OrderIndex { get; set; }

        // Auditing
        public DateTime? CreationTime { get; set; }
        public int? Creation_User { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public int? LastModification_User { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? DeletionTime { get; set; }
        public int? Deletion_User { get; set; }

        [ForeignKey("DayId")]
        public virtual ItineraryDay ItineraryDay { get; set; } = null!;
    }
}
