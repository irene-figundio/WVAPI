using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AI_Integration.DataAccess.Database.Models
{
    public class EventNeed
    {
        [Key]
        public int Id { get; set; }

        public int EventId { get; set; }

        public string Description { get; set; } = null!; // HTML content

        [ForeignKey("EventId")]
        public virtual Event? Event { get; set; }

        public bool? IsDeleted { get; set; } = false;
        public DateTime? DeletionDate { get; set; }
    }
}
