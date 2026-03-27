namespace VITBO.Models
{
    public class ContentCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int LangID { get; set; }
    }
}
