using Microsoft.EntityFrameworkCore;

namespace DevelopmentProposalScrapper.Infrastructure;

public class ApplicationDbContext : DbContext
{
    private readonly IConfiguration _configuration;

    public ApplicationDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_configuration.GetConnectionString("PostgresConnectionString"));
    }
    
   protected override void OnModelCreating(ModelBuilder modelBuilder)
   {
       modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
   }
   
   public DbSet<Domain.Entities.DevelopmentApplication> DevelopmentApplications => Set<Domain.Entities.DevelopmentApplication>();
   
}
