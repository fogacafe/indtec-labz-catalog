namespace Indtec.Labz.Catalog.Domain.BuildingBlocks;

public interface IEntity<out TId>
{
    TId Id { get; }
}