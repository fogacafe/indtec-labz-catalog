namespace Indtec.Labz.Catalog.Domain.BuildingBlocks;

public interface IRepository<TAggregate, TId>
    where TAggregate : IAggregateRoot<TId>;
