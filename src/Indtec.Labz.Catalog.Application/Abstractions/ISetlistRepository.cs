using Indtec.Labz.Catalog.Domain.Setlists;

namespace Indtec.Labz.Catalog.Application.Abstractions;

public interface ISetlistRepository
{
    Task AddAsync(Setlist setlist, CancellationToken cancellationToken);
    Task<Setlist?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task SaveAsync(Setlist setlist, CancellationToken cancellationToken);
}