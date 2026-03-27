namespace VITBO.Models
{
    public class ContentLinkDto
    {
        public int Id { get; set; }
        public int ContentId { get; set; }
        public string LinkUrl { get; set; } = null!;
        public string? Description { get; set; }
        public int LangID { get; set; }
    }
}
