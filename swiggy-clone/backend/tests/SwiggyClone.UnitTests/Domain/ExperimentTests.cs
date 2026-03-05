using FluentAssertions;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;

namespace SwiggyClone.UnitTests.Domain;

public sealed class ExperimentTests
{
    [Fact]
    public void NewExperiment_Key_DefaultsToEmpty()
    {
        var experiment = new Experiment();

        experiment.Key.Should().BeEmpty();
    }

    [Fact]
    public void NewExperiment_Name_DefaultsToEmpty()
    {
        var experiment = new Experiment();

        experiment.Name.Should().BeEmpty();
    }

    [Fact]
    public void NewExperiment_Status_DefaultsToDraft()
    {
        var experiment = new Experiment();

        experiment.Status.Should().Be(ExperimentStatus.Draft);
    }

    [Fact]
    public void NewExperiment_Collections_AreInitializedEmpty()
    {
        var experiment = new Experiment();

        experiment.Variants.Should().NotBeNull().And.BeEmpty();
        experiment.Assignments.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void NewExperiment_NullableProperties_AreNull()
    {
        var experiment = new Experiment();

        experiment.Description.Should().BeNull();
        experiment.TargetAudience.Should().BeNull();
        experiment.StartDate.Should().BeNull();
        experiment.EndDate.Should().BeNull();
        experiment.GoalDescription.Should().BeNull();
    }

    [Fact]
    public void Experiment_SetProperties_RetainsValues()
    {
        var createdByUserId = Guid.NewGuid();
        var startDate = DateTimeOffset.UtcNow;
        var endDate = startDate.AddDays(14);

        var experiment = new Experiment
        {
            Key = "homepage_banner_v2",
            Name = "Homepage Banner Test v2",
            Description = "Testing new banner design",
            Status = ExperimentStatus.Active,
            TargetAudience = "all_users",
            StartDate = startDate,
            EndDate = endDate,
            GoalDescription = "Increase click-through rate by 10%",
            CreatedByUserId = createdByUserId
        };

        experiment.Key.Should().Be("homepage_banner_v2");
        experiment.Name.Should().Be("Homepage Banner Test v2");
        experiment.Description.Should().Be("Testing new banner design");
        experiment.Status.Should().Be(ExperimentStatus.Active);
        experiment.TargetAudience.Should().Be("all_users");
        experiment.StartDate.Should().Be(startDate);
        experiment.EndDate.Should().Be(endDate);
        experiment.GoalDescription.Should().Be("Increase click-through rate by 10%");
        experiment.CreatedByUserId.Should().Be(createdByUserId);
    }

    [Fact]
    public void Experiment_VariantsCollection_CanAddVariants()
    {
        var experiment = new Experiment { Id = Guid.NewGuid() };
        var controlVariant = new ExperimentVariant
        {
            ExperimentId = experiment.Id,
            Key = "control",
            Name = "Control",
            AllocationPercent = 50,
            IsControl = true
        };
        var treatmentVariant = new ExperimentVariant
        {
            ExperimentId = experiment.Id,
            Key = "treatment",
            Name = "New Banner",
            AllocationPercent = 50,
            IsControl = false,
            ConfigJson = "{\"banner_url\": \"https://example.com/new-banner.png\"}"
        };

        experiment.Variants.Add(controlVariant);
        experiment.Variants.Add(treatmentVariant);

        experiment.Variants.Should().HaveCount(2);
        experiment.Variants.Should().ContainSingle(v => v.IsControl);
        experiment.Variants.Should().ContainSingle(v => !v.IsControl);
    }

    [Fact]
    public void Experiment_AllStatusValues_CanBeSet()
    {
        var experiment = new Experiment();

        experiment.Status = ExperimentStatus.Draft;
        experiment.Status.Should().Be(ExperimentStatus.Draft);

        experiment.Status = ExperimentStatus.Active;
        experiment.Status.Should().Be(ExperimentStatus.Active);

        experiment.Status = ExperimentStatus.Paused;
        experiment.Status.Should().Be(ExperimentStatus.Paused);

        experiment.Status = ExperimentStatus.Completed;
        experiment.Status.Should().Be(ExperimentStatus.Completed);

        experiment.Status = ExperimentStatus.Archived;
        experiment.Status.Should().Be(ExperimentStatus.Archived);
    }

    [Fact]
    public void Experiment_InheritsBaseEntity_HasSoftDeleteSupport()
    {
        var experiment = new Experiment();

        experiment.IsDeleted.Should().BeFalse();
        experiment.DeletedAt.Should().BeNull();
        experiment.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void Experiment_SoftDelete_SetsIsDeletedAndDeletedAt()
    {
        var experiment = new Experiment();

        experiment.SoftDelete();

        experiment.IsDeleted.Should().BeTrue();
        experiment.DeletedAt.Should().NotBeNull();
    }

    [Fact]
    public void Experiment_AssignmentsCollection_CanAddAssignments()
    {
        var experiment = new Experiment { Id = Guid.NewGuid() };
        var assignment = new UserVariantAssignment
        {
            ExperimentId = experiment.Id,
            UserId = Guid.NewGuid()
        };

        experiment.Assignments.Add(assignment);

        experiment.Assignments.Should().HaveCount(1);
        experiment.Assignments.Should().Contain(assignment);
    }

    [Fact]
    public void Experiment_WithNoEndDate_EndDateIsNull()
    {
        var experiment = new Experiment
        {
            Status = ExperimentStatus.Active,
            StartDate = DateTimeOffset.UtcNow,
            EndDate = null
        };

        experiment.EndDate.Should().BeNull();
    }
}
