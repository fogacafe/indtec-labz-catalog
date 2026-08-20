using Indtec.Labz.Catalog.Domain.BuildingBlocks;

namespace Indtec.Labz.Catalog.Domain.Music;

public interface IMusicRepository : IRepository<Music, Guid>
{
    Task AddAsync(Music music, CancellationToken cancellationToken);
    Task<Music?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Music>> ListAsync(CancellationToken cancellationToken);
}