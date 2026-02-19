using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SkillExtraction.Infrastructure.Persistence;

/// <summary>
/// Design-time factory for ApplicationDbContext to support EF Core migrations.
/// </summary>
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        
        // Use SQLite with a default connection string for migrations
        optionsBuilder.UseSqlite("Data Source=skillextraction.db");

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
