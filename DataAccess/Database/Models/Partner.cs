using System;
using System.ComponentModel.DataAnnotations;

namespace AI_Integration.DataAccess.Database.Models
{
    public class Partner
    {
        public bool? IsDeleted { get; set; }
        public DateTime? DeletionDate { get; set; }
        public int Id { get; set; }

        public string? Description { get; set; }

        [StringLength(500)]
        public string? LinkUrl { get; set; }

        [StringLength(500)]
        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; }
    }
}
