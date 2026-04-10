using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AI_Integration.DataAccess.Database.Models
{
    [Table("StorageMappings", Schema = "dbo")]
    public class StorageMapping
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string ParentType { get; set; } = null!;

        public int ParentId { get; set; }

        [Required]
        [StringLength(50)]
        public string StorageArea { get; set; } = null!;

        public int ProgressiveNumber { get; set; }

        [Required]
        [StringLength(255)]
        public string FolderName { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
