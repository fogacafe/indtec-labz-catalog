using Indtec.Labz.Catalog.Domain.Results;
using Indtec.Labz.Catalog.Domain.Setlists;

namespace Indtec.Labz.Catalog.Application.Setlists;

public sealed class PublishSetlist(ISetlistRepository repository, TimeProvider clock)
{
    public async Task<Result<SetlistPublished>> ExecuteAsync(Guid id, CancellationToken cancellationToken)
    {
        var setlist = await repository.GetAsync(id, cancellationToken);
        if (setlist is null) return Result<SetlistPublished>.Failure(SetlistErrors.NotFound);

        var published = setlist.Publish(clock.GetUtcNow());
        if (published.IsFailure) return published;

        await repository.SaveAsync(setlist, cancellationToken);
        return published;
    }
}