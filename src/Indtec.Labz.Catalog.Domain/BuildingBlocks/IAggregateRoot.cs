namespace Indtec.Labz.Catalog.Domain.BuildingBlocks;

public interface IAggregateRoot<out TId> : IEntity<TId>;
