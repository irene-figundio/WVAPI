namespace AI_Integration.DataAccess.Database.Models
{
    public class UploadedFile
    {
        public bool? IsDeleted { get; set; } = false;
        public DateTime? DeletionDate { get; set; }
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
        public long FileSize { get; set; }
        public string OpenAIFileId { get; set; }
        public string VectorStoreId { get; set; }
        public DateTime UploadDate { get; set; }
    }

}
