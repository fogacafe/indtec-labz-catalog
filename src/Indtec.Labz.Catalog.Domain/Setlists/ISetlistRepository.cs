using Indtec.Labz.Catalog.Domain.BuildingBlocks;

namespace Indtec.Labz.Catalog.Domain.Setlists;

public interface ISetlistRepository : IRepository<Setlist, Guid>
{
    Task AddAsync(Setlist setlist, CancellationToken cancellationToken);
    Task<Setlist?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task SaveAsync(Setlist setlist, CancellationToken cancellationToken);
}