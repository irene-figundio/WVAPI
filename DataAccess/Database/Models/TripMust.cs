using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AI_Integration.DataAccess.Database.Models
{
    public class TripMust
    {
        public int Id { get; set; }
        public int TripId { get; set; }

        [Required]
        public string Text { get; set; } = null!;

        public int TypeId { get; set; } // 1: Inclusion, 2: Exclusion

        // Auditing
        public DateTime? CreationTime { get; set; }
        public int? Creation_User { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public int? LastModification_User { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? DeletionTime { get; set; }
        public int? Deletion_User { get; set; }

        [ForeignKey("TripId")]
        public virtual Trip Trip { get; set; } = null!;
    }
}
