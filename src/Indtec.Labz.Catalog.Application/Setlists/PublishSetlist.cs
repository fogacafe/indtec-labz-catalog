using Indtec.Labz.Catalog.Application.Abstractions;
using Indtec.Labz.Catalog.Domain;
using Indtec.Labz.Catalog.Domain.Setlists;

namespace Indtec.Labz.Catalog.Application.Setlists;

public sealed class PublishSetlist(ISetlistRepository repository, TimeProvider clock)
{
    public async Task<SetlistPublished> ExecuteAsync(Guid id, CancellationToken cancellationToken)
    {
        var setlist = await repository.GetAsync(id, cancellationToken)
            ?? throw new DomainException("Setlist was not found.");

        var published = setlist.Publish(clock.GetUtcNow());
        await repository.SaveAsync(setlist, cancellationToken);
        return published;
    }
}