using DevelopmentProposalScrapper.Domain.Entities;
using DevelopmentProposalScrapper.Infrastructure.External.Models.OnlineDA;
using ComponentModelDescriptionAttribute = System.ComponentModel.DescriptionAttribute;
using DomainDevelopmentApplication = DevelopmentProposalScrapper.Domain.Entities.DevelopmentApplication;

namespace DevelopmentProposalScrapperTests;

[TestFixture]
public class ApplicationTypeTests
{
    [Test]
    public void ApplicationType_DevelopmentApplication_HasCorrectDescription()
    {
        // Arrange
        var applicationType = ApplicationType.DevelopmentApplication;
        var fieldInfo = typeof(ApplicationType).GetField(applicationType.ToString());
        var descriptionAttribute = (ComponentModelDescriptionAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(ComponentModelDescriptionAttribute));

        // Assert
        Assert.That(descriptionAttribute?.Description, Is.EqualTo("Development Application"));
    }

    [Test]
    public void ApplicationType_ModificationApplication_HasCorrectDescription()
    {
        // Arrange
        var applicationType = ApplicationType.ModificationApplication;
        var fieldInfo = typeof(ApplicationType).GetField(applicationType.ToString());
        var descriptionAttribute = (ComponentModelDescriptionAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(ComponentModelDescriptionAttribute));

        // Assert
        Assert.That(descriptionAttribute?.Description, Is.EqualTo("Modification Application"));
    }

    [Test]
    public void ApplicationType_ReviewOfDetermination_HasCorrectDescription()
    {
        // Arrange
        var applicationType = ApplicationType.ReviewOfDetermination;
        var fieldInfo = typeof(ApplicationType).GetField(applicationType.ToString());
        var descriptionAttribute = (ComponentModelDescriptionAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(ComponentModelDescriptionAttribute));

        // Assert
        Assert.That(descriptionAttribute?.Description, Is.EqualTo("Review Of Determination"));
    }
}

[TestFixture]
public class ApplicationStatusTests
{

    [Test]
    public void ApplicationStatus_AdditionalInformationRequested_HasCorrectDescription()
    {
        // Arrange
        var applicationStatus = ApplicationStatus.AdditionalInformationRequested;
        var fieldInfo = typeof(ApplicationStatus).GetField(applicationStatus.ToString());
        var descriptionAttribute = (ComponentModelDescriptionAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(ComponentModelDescriptionAttribute));

        // Assert
        Assert.That(descriptionAttribute?.Description, Is.EqualTo("Additional Information Requested"));
    }

    [Test]
    public void ApplicationStatus_PendingLodgement_HasCorrectDescription()
    {
        // Arrange
        var applicationStatus = ApplicationStatus.PendingLodgement;
        var fieldInfo = typeof(ApplicationStatus).GetField(applicationStatus.ToString());
        var descriptionAttribute = (ComponentModelDescriptionAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(ComponentModelDescriptionAttribute));

        // Assert
        Assert.That(descriptionAttribute?.Description, Is.EqualTo("Pending Lodgement"));
    }


    [Test]
    public void ApplicationStatus_PendingCourtAppeal_HasCorrectDescription()
    {
        // Arrange
        var applicationStatus = ApplicationStatus.PendingCourtAppeal;
        var fieldInfo = typeof(ApplicationStatus).GetField(applicationStatus.ToString());
        var descriptionAttribute = (ComponentModelDescriptionAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(ComponentModelDescriptionAttribute));

        // Assert
        Assert.That(descriptionAttribute?.Description, Is.EqualTo("Pending Court Appeal"));
    }
}

[TestFixture]
public class DevelopmentApplicationEntityTests
{
    [Test]
    public void DevelopmentApplication_CanBeCreated_WithRequiredFields()
    {
        // Arrange & Act
        var app = new DomainDevelopmentApplication
        {
            PlanningPortalApplicationNumber = "APP-001",
            DateLastUpdated = DateTime.UtcNow,
            DeterminationDate = DateOnly.FromDateTime(DateTime.UtcNow),
            ApplicationStatus = ApplicationStatus.Determined,
            ApplicationType = ApplicationType.DevelopmentApplication,
            Council = new CouncilInfo { CouncilName = "Council of the City of Sydney" },
            ProposedDevelopmentTypes = new[] { new ProposedDevelopmentType { DevelopmentType = "Residential" } },
            Addresses = new[] { new Address { FullAddress = "123 Main St" } }
        };

        // Assert
        Assert.That(app.PlanningPortalApplicationNumber, Is.EqualTo("APP-001"));
        Assert.That(app.ApplicationStatus, Is.EqualTo(ApplicationStatus.Determined));
        Assert.That(app.ApplicationType, Is.EqualTo(ApplicationType.DevelopmentApplication));
    }

    [Test]
    public void DevelopmentApplication_IsImmutable_Record()
    {
        // Arrange
        var app1 = new DomainDevelopmentApplication
        {
            PlanningPortalApplicationNumber = "APP-001",
            DateLastUpdated = DateTime.UtcNow,
            DeterminationDate = DateOnly.FromDateTime(DateTime.UtcNow)
        };

        var app2 = new DomainDevelopmentApplication
        {
            PlanningPortalApplicationNumber = "APP-001",
            DateLastUpdated = app1.DateLastUpdated,
            DeterminationDate = app1.DeterminationDate
        };

        // Assert - Records should be equal based on values
        Assert.That(app1, Is.EqualTo(app2));
    }

    [Test]
    public void DevelopmentApplication_CanHaveNullDates()
    {
        // Arrange & Act
        var app = new DomainDevelopmentApplication
        {
            PlanningPortalApplicationNumber = "APP-001",
            DateLastUpdated = null,
            DeterminationDate = DateOnly.FromDateTime(DateTime.UtcNow)
        };

        // Assert
        Assert.That(app.DateLastUpdated, Is.Null);
    }

    [Test]
    public void DevelopmentApplication_CanHaveNullStatus()
    {
        // Arrange & Act
        var app = new DomainDevelopmentApplication
        {
            PlanningPortalApplicationNumber = "APP-001",
            DateLastUpdated = DateTime.UtcNow,
            DeterminationDate = DateOnly.FromDateTime(DateTime.UtcNow),
            ApplicationStatus = null
        };

        // Assert
        Assert.That(app.ApplicationStatus, Is.Null);
    }

    [Test]
    public void DevelopmentApplication_CanHaveNullType()
    {
        // Arrange & Act
        var app = new DomainDevelopmentApplication
        {
            PlanningPortalApplicationNumber = "APP-001",
            DateLastUpdated = DateTime.UtcNow,
            DeterminationDate = DateOnly.FromDateTime(DateTime.UtcNow),
            ApplicationType = null
        };

        // Assert
        Assert.That(app.ApplicationType, Is.Null);
    }

    [Test]
    public void DevelopmentApplication_CanHaveNullCouncil()
    {
        // Arrange & Act
        var app = new DomainDevelopmentApplication
        {
            PlanningPortalApplicationNumber = "APP-001",
            DateLastUpdated = DateTime.UtcNow,
            DeterminationDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Council = null
        };

        // Assert
        Assert.That(app.Council, Is.Null);
    }

    [Test]
    public void DevelopmentApplication_CanHaveNullAddresses()
    {
        // Arrange & Act
        var app = new DomainDevelopmentApplication
        {
            PlanningPortalApplicationNumber = "APP-001",
            DateLastUpdated = DateTime.UtcNow,
            DeterminationDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Addresses = null
        };

        // Assert
        Assert.That(app.Addresses, Is.Null);
    }

    [Test]
    public void DevelopmentApplication_CanHaveNullDevelopmentTypes()
    {
        // Arrange & Act
        var app = new DomainDevelopmentApplication
        {
            PlanningPortalApplicationNumber = "APP-001",
            DateLastUpdated = DateTime.UtcNow,
            DeterminationDate = DateOnly.FromDateTime(DateTime.UtcNow),
            ProposedDevelopmentTypes = null
        };

        // Assert
        Assert.That(app.ProposedDevelopmentTypes, Is.Null);
    }

    [Test]
    public void DevelopmentApplication_CanHaveMultipleAddresses()
    {
        // Arrange & Act
        var app = new DomainDevelopmentApplication
        {
            PlanningPortalApplicationNumber = "APP-001",
            DateLastUpdated = DateTime.UtcNow,
            DeterminationDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Addresses = new[]
            {
                new Address { FullAddress = "123 Main St" },
                new Address { FullAddress = "456 King St" },
                new Address { FullAddress = "789 Park Ave" }
            }
        };

        // Assert
        Assert.That(app.Addresses?.Count(), Is.EqualTo(3));
    }

    [Test]
    public void DevelopmentApplication_CanHaveMultipleDevelopmentTypes()
    {
        // Arrange & Act
        var app = new DomainDevelopmentApplication
        {
            PlanningPortalApplicationNumber = "APP-001",
            DateLastUpdated = DateTime.UtcNow,
            DeterminationDate = DateOnly.FromDateTime(DateTime.UtcNow),
            ProposedDevelopmentTypes = new[]
            {
                new ProposedDevelopmentType { DevelopmentType = "Residential" },
                new ProposedDevelopmentType { DevelopmentType = "Commercial" },
                new ProposedDevelopmentType { DevelopmentType = "Infrastructure" }
            }
        };

        // Assert
        Assert.That(app.ProposedDevelopmentTypes?.Count(), Is.EqualTo(3));
    }
}
















