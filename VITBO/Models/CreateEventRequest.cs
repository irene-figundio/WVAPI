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

        public DateTime? StartDate { get; set; }
        public TimeSpan? StartTime { get; set; }
        public DateTime? EndDate { get; set; }
        public TimeSpan? EndTime { get; set; }

        [Required]
        public DateTime BookingEndDate { get; set; } = DateTime.Today;

        [Required]
        public string? Location { get; set; }

        public string? CoverImage { get; set; }
        public string? Subtitle { get; set; }
        public string? HeroImage { get; set; }
        public string? Coordinates { get; set; }

       public string Organizer { get; set; } = "Vitinerario®";
       public string? ContactInfo { get; set; }

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
