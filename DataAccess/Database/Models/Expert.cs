using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AI_Integration.DataAccess.Database.Models
{
    public class Expert
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; } = null!;

        public string? Bio { get; set; }

        [StringLength(500)]
        public string? PhotoUrl { get; set; }

        [StringLength(255)]
        public string? Email { get; set; }

        public DateTime? CreatedAt { get; set; }

        public int LangID { get; set; }
        [ForeignKey("LangID")]
        public virtual Language? Language { get; set; }

        public virtual ICollection<EventExpert> EventExperts { get; set; } = new List<EventExpert>();
        public virtual ICollection<ContentExpert> ContentExperts { get; set; } = new List<ContentExpert>();
    }
}
