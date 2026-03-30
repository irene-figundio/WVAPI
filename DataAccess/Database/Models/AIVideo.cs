using Microsoft.AspNetCore.Http;

namespace AI_Integration.DataAccess.Database.Models
{
    public class AIVideo
    {
        public int Id { get; set; }

        public string? Dir_Path { get; set; }
        public string? Title { get; set; }
        public string? Url_Video { get; set; }
        public int? Play_Priority { get; set; }
        public bool? IsLandscape { get; set; }

        public int ID_Session { get; set; }

        public DateTime DataCreation { get; set; }
        public int? Creation_User { get; set; }

        public DateTime? DataUpdate { get; set; }
        public int? LastModification_User { get; set; }     // <-- corretto a int?
        public bool? IsDeleted { get; set; } = false;
        public DateTime? DeletionTime { get; set; }
        public int? Deletion_User { get; set; }

        // Navigation
        public virtual AdSession? Session { get; set; }
    }
}
