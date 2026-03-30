namespace VITBO.Models
{
    public class ContentDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Text { get; set; } = null!;
        public DateTime PublishDate { get; set; }
        public string? CoverImage { get; set; }
        public string ContentType { get; set; } = null!;
        public bool IsPublished { get; set; }
        public string? Subtitle { get; set; }
        public int? CategoryId { get; set; }
        public string? Preview { get; set; }
        public string? HeroImage { get; set; }
    }
}
