using System.ComponentModel.DataAnnotations;

namespace VITBO.Models
{
    public class CreateEventRequest
    {
        [Required]
        public string Title { get; set; } = default!;

        public string Description { get; set; } = default!;

        [Required]
        public DateTime EventDate { get; set; } = DateTime.Today;

        public string? Location { get; set; }

        public decimal Price { get; set; } = 0;

        public bool IsOnline { get; set; } = false;

        public int LangID { get; set; } = 1;

        public int? CategoryId { get; set; }
    }
}
