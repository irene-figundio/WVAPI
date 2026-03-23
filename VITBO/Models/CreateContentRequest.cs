using System.ComponentModel.DataAnnotations;

namespace VITBO.Models
{
    public class CreateContentRequest
    {
        [Required]
        public string Title { get; set; } = default!;

        [Required]
        public string Text { get; set; } = default!;

        public DateTime PublishDate { get; set; } = DateTime.Now;

        [Required]
        public string ContentType { get; set; } = default!;

        public bool IsPublished { get; set; } = true;

        public int LangID { get; set; } = 1;
    }
}
