namespace VITBO.Models
{
    public class PhotoGalleryDto
    {
        public int Id { get; set; }
        public int GalleryId { get; set; }
        public string ImageUrl { get; set; } = null!;
        public string? Caption { get; set; }
        public int LangID { get; set; }
    }
}
