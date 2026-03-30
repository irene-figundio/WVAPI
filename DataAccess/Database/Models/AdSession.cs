namespace AI_Integration.DataAccess.Database.Models
{
    public class AdSession
    {
        public int Id { get; set; }

        public int ID_Campaing { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // Auditing
        public DateTime? CreationTime { get; set; }
        public int? Creation_User { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public int? LastModification_User { get; set; }     // <-- corretto a int?
        public bool? IsDeleted { get; set; } = false;
        public DateTime? DeletionTime { get; set; }
        public int? Deletion_User { get; set; }

        // Navigation
        public virtual AdCampaign? Campaign { get; set; }
        public virtual List<AIVideo> Videos { get; set; } = new();
    }
}
