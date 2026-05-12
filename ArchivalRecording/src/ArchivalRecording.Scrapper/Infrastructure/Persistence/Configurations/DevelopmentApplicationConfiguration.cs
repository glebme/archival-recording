using DevelopmentProposalScrapper.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevelopmentProposalScrapper.Infrastructure.Persistence.Configurations;

public class DevelopmentApplicationConfiguration : IEntityTypeConfiguration<DevelopmentApplication>
{
    public void Configure(EntityTypeBuilder<DevelopmentApplication> builder)
    {
        builder.HasKey(da => da.PlanningPortalApplicationNumber);

        builder.Property(da => da.PlanningPortalApplicationNumber)
            .IsRequired()
            .HasMaxLength(50); // Assuming a reasonable max length

        builder.Property(da => da.DateLastUpdated);

        builder.Property(da => da.DeterminationDate)
            .IsRequired();

        builder.Property(da => da.ApplicationStatus);

        builder.Property(da => da.ApplicationType);

        builder.OwnsOne(da => da.Council, council =>
        {
            council.Property(c => c.CouncilName)
                .HasMaxLength(200);
        });

        builder.Property(da => da.CouncilApplicationNumber)
            .IsRequired();

        builder.OwnsMany(da => da.ProposedDevelopmentTypes, devType =>
        {
            devType.Property(dt => dt.DevelopmentType)
                .HasMaxLength(200);
        });

        builder.OwnsMany(da => da.Addresses, location =>
        {
            location.Property(l => l.FullAddress)
                .HasMaxLength(500);
            location.Property(l => l.X)
                .HasMaxLength(50);
            location.Property(l => l.Y)
                .HasMaxLength(50);
            location.Property(l => l.StreetNumber1)
                .HasMaxLength(50);
            location.Property(l => l.StreetNumber2)
                .HasMaxLength(50);
            location.Property(l => l.StreetNumber3)
                .HasMaxLength(50);
            location.Property(l => l.StreetName)
                .HasMaxLength(200);
            location.Property(l => l.StreetType)
                .HasMaxLength(50);
            location.Property(l => l.Suburb)
                .HasMaxLength(100);
            location.Property(l => l.Postcode)
                .HasMaxLength(10);
            location.Property(l => l.State)
                .HasMaxLength(50);
            location.OwnsMany(l => l.Lot, lot =>
            {
                lot.Property(lo => lo.LotNo)
                    .HasMaxLength(50);
                lot.Property(lo => lo.PlanLabel)
                    .HasMaxLength(50);
            });
        });
    }
}
