using CSM_Database_Core.Entities.Abstractions.Interfaces;
using CSM_Database_Core.Validation.Abstractions.Bases;

namespace CSM_Database_Testing.Abstractions.Bases;

public record TestingEntityBase<TEntity>
    where TEntity : IEntity {

    public (string, (ValidatorBase, int)[])[] Expectations { get; init; } = [];
    public TEntity Mock { get; init; } = default!;

    public string Name { get; set; }

    public TestingEntityBase(string Name) {
        this.Name = Name;
    }

    public TestingEntityBase(string Name, TEntity Mock, (string, (ValidatorBase, int)[])[] Expectations) {
        this.Name = Name;
        this.Mock = Mock;
        this.Expectations = Expectations;
    }
}
