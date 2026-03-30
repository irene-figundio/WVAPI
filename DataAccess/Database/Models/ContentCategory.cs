using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AI_Integration.DataAccess.Database.Models
{
    public class ContentCategory
    {
        public bool? IsDeleted { get; set; } = false;
        public DateTime? DeletionDate { get; set; }
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public int LangID { get; set; }

        [ForeignKey("LangID")]
        public virtual Language? Language { get; set; }
    }
}
