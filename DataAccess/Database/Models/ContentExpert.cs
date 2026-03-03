using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AI_Integration.DataAccess.Database.Models
{
    public class ContentExpert
    {
        public int Id { get; set; }

        public int ContentId { get; set; }
        [ForeignKey("ContentId")]
        public virtual Content? Content { get; set; }

        public int ExpertId { get; set; }
        [ForeignKey("ExpertId")]
        public virtual Expert? Expert { get; set; }
    }
}
