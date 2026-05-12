using Microsoft.EntityFrameworkCore;

namespace DevelopmentProposalScrapper.Infrastructure;

public class ApplicationDbContext(IConfiguration configuration) : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseNpgsql(configuration.GetConnectionString("PostgresConnectionString"));
    }
    
   protected override void OnModelCreating(ModelBuilder modelBuilder)
   {
       modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
   }
   
   public DbSet<Domain.Entities.DevelopmentApplication> DevelopmentApplications => Set<Domain.Entities.DevelopmentApplication>();
   
}
