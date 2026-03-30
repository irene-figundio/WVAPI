namespace AI_Integration.DataAccess.Database.Models
{
    public class APIToken
    {
        public bool? IsDeleted { get; set; } = false;
        public DateTime? DeletionDate { get; set; }
        public int ID { get; set; }
        public DateTime DataCreation { get; set; }
        public string Token { get; set; }
        public string Platform { get; set; }
        public DateTime DataExpiration { get; set; }
    }
}
