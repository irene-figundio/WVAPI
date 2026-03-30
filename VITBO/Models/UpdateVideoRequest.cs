namespace VITBO.Models
{
    public class UpdateVideoRequest
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Url_Video { get; set; }
        public bool IsLandscape { get; set; } = false;
        public int? Play_Priority { get; set; }
        public int? ID_Session { get; set; }
        public bool? IsDeleted { get; set; } = false; 
    }
}
