using Indtec.Labz.Catalog.Domain.Music;

namespace Indtec.Labz.Catalog.Application.Abstractions;

public interface IMusicRepository
{
    Task AddAsync(Music music, CancellationToken cancellationToken);
    Task<Music?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Music>> ListAsync(CancellationToken cancellationToken);
}