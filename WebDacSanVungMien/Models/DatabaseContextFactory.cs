using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace WebDacSanVungMien.Models
{
    public class DatabaseContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
    {
        public DatabaseContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();

            optionsBuilder.UseSqlServer("Server=.;Database=DacSanVungMien;User Id=sa;Password=123;TrustServerCertificate=True;");

            return new DatabaseContext(optionsBuilder.Options);
        }
    }
}
