namespace VITBO.Models
{
    public class ContentImageDto
    {
        public int Id { get; set; }
        public int ContentId { get; set; }
        public string ImageUrl { get; set; } = null!;
        public string? Caption { get; set; }
        public int? Position { get; set; }
        public int LangID { get; set; }
    }
}
