using System.ComponentModel.DataAnnotations;

namespace VITBO.Models
{
    public class CreateEventRequest
    {
        [Required]
        public string Title { get; set; } = default!;
        [Required]
        public string Description { get; set; } = default!;

        [Required]
        public DateTime EventDate { get; set; } = DateTime.Today;

        public DateTime? EndDate { get; set; }

        [Required]
        public DateTime BookingEndDate { get; set; } = DateTime.Today;

        [Required]
        public string? Location { get; set; }
        [Required]
        public string? CoverImage { get; set; }
        public string? HeroImage { get; set; }

       public string Organizer { get; set; } = "Vitinerario®";

        public decimal Price { get; set; } = 0;

        public bool IsOnline { get; set; } = false;

        public int LangID { get; set; } = 1;

        public int? CategoryId { get; set; } = 1;

        public bool HasVariantPrice { get; set; } = true;
        public bool HasNeeds { get; set; } = true;
        public string? ProgramPdf { get; set; }

        public Microsoft.AspNetCore.Http.IFormFile? CoverImageFile { get; set; }
        public Microsoft.AspNetCore.Http.IFormFile? ProgramPdfFile { get; set; }
    }
}
