using Microsoft.EntityFrameworkCore;

namespace DevelopmentProposalScrapper.Infrastructure;

public class ApplicationDbContext : DbContext
{
   public DbSet<Domain.Entities.DevelopmentApplication> DevelopmentApplications => Set<Domain.Entities.DevelopmentApplication>();
   
   protected override void OnModelCreating(ModelBuilder modelBuilder)
   {
       modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
   }
}
