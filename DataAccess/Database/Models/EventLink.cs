using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AI_Integration.DataAccess.Database.Models
{
    public class EventLink
    {
        public bool? IsDeleted { get; set; } = false;
        public DateTime? DeletionDate { get; set; }
        public int Id { get; set; }
        public int EventId { get; set; }
        [Required]
        [StringLength(500)]
        public string LinkUrl { get; set; } = null!;
        [StringLength(255)]
        public string? Description { get; set; }

        public int LangID { get; set; }
        [ForeignKey("LangID")]
        public virtual Language? Language { get; set; }

        [NotMapped]
        public virtual Event? Event { get; set; } = null!;
    }
}