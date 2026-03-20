using DevelopmentProposalScrapper.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevelopmentProposalScrapper.Infrastructure.Persistence.Configurations;

public class DevelopmentApplicationConfiguration : IEntityTypeConfiguration<DevelopmentApplication>
{
    public void Configure(EntityTypeBuilder<DevelopmentApplication> builder)
    {
        throw new NotImplementedException();
    }
}
