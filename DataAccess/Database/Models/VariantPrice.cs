using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AI_Integration.DataAccess.Database.Models
{
    public class VariantPrice
    {
        [Key]
        public int Id { get; set; }

        public int EventId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [ForeignKey("EventId")]
        public virtual Event? Event { get; set; }

        public bool? IsDeleted { get; set; } = false;
        public DateTime? DeletionDate { get; set; }
    }
}
