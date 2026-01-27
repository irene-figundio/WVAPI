using Microsoft.EntityFrameworkCore;

namespace AI_Integration.DataAccess
{
    public class VitinerarioDbContext
    {
        private readonly IConfiguration _configuration;

        public VitinerarioDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
        }
    }
}
