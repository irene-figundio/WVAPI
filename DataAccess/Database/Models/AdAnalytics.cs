namespace AI_Integration.DataAccess.Database.Models
{
    public class AdAnalytics
    {
        public bool? IsDeleted { get; set; }
        public DateTime? DeletionDate { get; set; }
        public int Id { get; set; }
        public int SessionId { get; set; }
        public int VideoId { get; set; }
        public int? NumViews { get; set; }
        public int? NumClick { get; set; }
        public string Platform { get; set; }

        public DateTime SendDate { get; set; }
    }
}
