namespace AI_Integration.DataAccess.Database.Models
{
    public class UserStatus
    {
        public int Id { get; set; }                // 0 = NotActive, 1 = Active
        public string Name { get; set; } = null!;  // Nome leggibile (es. Active, NotActive)
        public string ResourceKey { get; set; } = null!;

        // Navigation
        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}
