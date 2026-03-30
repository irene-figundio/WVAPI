namespace VITBO.Models
{
    public class EventDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime EventDate { get; set; }
        public DateTime? StartDate { get; set; }
        public TimeSpan? StartTime { get; set; }
        public DateTime? EndDate { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string? CoverImage { get; set; }
        public DateTime? BookingEndDate { get; set; }
        public string? Location { get; set; }
        public string? Organizer { get; set; }
        public string? ContactInfo { get; set; }
        public decimal Price { get; set; }
        public bool IsOnline { get; set; }
        public int LangID { get; set; }
        public string? Subtitle { get; set; }
        public int? CategoryId { get; set; }
        public string? Coordinates { get; set; }
        public string? HeroImage { get; set; }
    }
}
