namespace AI_Integration.DataAccess.Database.Models
{
    public class WineAI
    {
        public bool? IsDeleted { get; set; }
        public DateTime? DeletionDate { get; set; }
        public int ID { get; set; }
        public DateTime CreationDate { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public string Dispositivo { get; set; }

    }
}
