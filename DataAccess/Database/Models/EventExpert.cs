using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AI_Integration.DataAccess.Database.Models
{
    public class EventExpert
    {
        public bool? IsDeleted { get; set; } = false;
        public DateTime? DeletionDate { get; set; }
        public int Id { get; set; }

        public int EventId { get; set; }
        [ForeignKey("EventId")]
        public virtual Event? Event { get; set; }

        public int ExpertId { get; set; }
        [ForeignKey("ExpertId")]
        public virtual Expert? Expert { get; set; }
    }
}
