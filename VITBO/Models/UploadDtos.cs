namespace VITBO.Models
{
    public class HeroImageDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }

    public class UploadResponseDto
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? FileUrl { get; set; }
    }
}
