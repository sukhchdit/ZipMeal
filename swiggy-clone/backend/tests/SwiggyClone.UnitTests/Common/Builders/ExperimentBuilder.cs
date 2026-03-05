using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;

namespace SwiggyClone.UnitTests.Common.Builders;

public sealed class ExperimentBuilder
{
    private Guid _id = TestConstants.ExperimentId;
    private string _key = "test_experiment";
    private string _name = "Test Experiment";
    private ExperimentStatus _status = ExperimentStatus.Draft;
    private Guid _createdByUserId = TestConstants.AdminId;

    public ExperimentBuilder WithId(Guid id) { _id = id; return this; }
    public ExperimentBuilder WithKey(string key) { _key = key; return this; }
    public ExperimentBuilder WithName(string name) { _name = name; return this; }
    public ExperimentBuilder WithStatus(ExperimentStatus status) { _status = status; return this; }
    public ExperimentBuilder WithCreatedByUserId(Guid userId) { _createdByUserId = userId; return this; }

    public Experiment Build() => new()
    {
        Id = _id,
        Key = _key,
        Name = _name,
        Status = _status,
        CreatedByUserId = _createdByUserId,
        CreatedAt = DateTimeOffset.UtcNow,
        UpdatedAt = DateTimeOffset.UtcNow,
    };
}
