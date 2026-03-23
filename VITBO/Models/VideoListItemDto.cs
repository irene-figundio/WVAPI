namespace VITBO.Models
{
    public class VideoListItemDto
    {
        public int Id { get; set; }
        public int ID_Session { get; set; }
        public string? Title { get; set; }
        public string? Url_Video { get; set; }
        public string PublicUrl { get; set; } = string.Empty;
        public bool? IsLandscape { get; set; }
        public int? Play_Priority { get; set; }
        public DateTime DataCreation { get; set; }
    }
}
