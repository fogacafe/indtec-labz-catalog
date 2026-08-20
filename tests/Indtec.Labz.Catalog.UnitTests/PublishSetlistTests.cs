using Indtec.Labz.Catalog.Application.Setlists;
using Indtec.Labz.Catalog.Domain.Music;
using Indtec.Labz.Catalog.Domain.Setlists;

namespace Indtec.Labz.Catalog.UnitTests;

public sealed class PublishSetlistTests
{
    [Fact]
    public async Task Uses_injected_time_provider_for_publication_timestamp()
    {
        var setlist = Setlist.Create("Friday night").Value!;
        setlist.AddSong(Guid.NewGuid(), TimeSpan.FromMinutes(4), MusicalKey.E);
        var repository = new InMemorySetlistRepository(setlist);
        var expected = new DateTimeOffset(2026, 8, 20, 13, 0, 0, TimeSpan.Zero);
        var useCase = new PublishSetlist(repository, new FixedTimeProvider(expected));

        var result = await useCase.ExecuteAsync(setlist.Id, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(expected, result.Value!.OccurredAt);
    }

    private sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => now;
    }

    private sealed class InMemorySetlistRepository(Setlist setlist) : ISetlistRepository
    {
        public Task AddAsync(Setlist value, CancellationToken cancellationToken) => Task.CompletedTask;
        public Task<Setlist?> GetAsync(Guid id, CancellationToken cancellationToken) => Task.FromResult(id == setlist.Id ? setlist : null);
        public Task SaveAsync(Setlist value, CancellationToken cancellationToken) => Task.CompletedTask;
    }
}