namespace AI_Integration.DataAccess.Database.Models
{
    public class AdCampaign
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public decimal? Budget { get; set; }

        // Auditing
        public DateTime? CreationTime { get; set; }
        public int? Creation_User { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public int? LastModification_User { get; set; }   
        public bool? IsDeleted { get; set; }
        public DateTime? DeletionTime { get; set; }
        public int? Deletion_User { get; set; }

        // Navigation
        public virtual List<AdSession> Sessions { get; set; } = new();
    }
}
