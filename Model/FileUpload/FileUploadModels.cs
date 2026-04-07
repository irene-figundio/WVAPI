using Microsoft.AspNetCore.Http;

namespace AI_Integration.Model.FileUpload
{
    public enum ParentType
    {
        Events,
        Content,
        HeroImage
    }

    public enum UploadType
    {
        EventGalleryImage,
        EventStageImage,
        EventProgramPdf,
        EventCoverImage,
        ContentGalleryImage,
        ContentCoverImage,
        HeroImage
    }

    public class FileUploadRequest
    {
        public ParentType ParentType { get; set; }
        public int ParentId { get; set; }
        public UploadType UploadType { get; set; }
        public IFormFile File { get; set; } = null!;
        public string? Caption { get; set; }
        public int? LangId { get; set; }
        public int? TripId { get; set; }
        public int? DayNumber { get; set; }
    }

    public class FileUploadResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? FileUrl { get; set; }
        public string? PhysicalPath { get; set; }
    }
}
