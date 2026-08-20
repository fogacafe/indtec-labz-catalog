using Indtec.Labz.Catalog.Domain.Music;
using Indtec.Labz.Catalog.Domain.Setlists;
using Indtec.Labz.Catalog.Infrastructure.Persistence;

namespace Indtec.Labz.Catalog.IntegrationTests;

public sealed class CatalogPersistenceTests(PostgresFixture fixture) : IClassFixture<PostgresFixture>
{
    [Fact]
    public async Task Published_setlist_round_trips_through_postgres()
    {
        var musicRepository = new MusicRepository(fixture.ConnectionFactory);
        var setlistRepository = new SetlistRepository(fixture.ConnectionFactory);
        var music = Music.Create("Everlong", "Foo Fighters", TimeSpan.FromSeconds(250), 158, MusicalKey.D).Value!;
        var setlist = Setlist.Create("LABZ Live").Value!;

        await musicRepository.AddAsync(music, CancellationToken.None);
        await setlistRepository.AddAsync(setlist, CancellationToken.None);
        Assert.True(setlist.AddSong(music.Id, music.Duration, MusicalKey.E).IsSuccess);
        Assert.True(setlist.Publish(new DateTimeOffset(2026, 8, 20, 22, 0, 0, TimeSpan.Zero)).IsSuccess);
        await setlistRepository.SaveAsync(setlist, CancellationToken.None);

        var persisted = await setlistRepository.GetAsync(setlist.Id, CancellationToken.None);

        Assert.NotNull(persisted);
        Assert.Equal(SetlistStatus.Published, persisted.Status);
        var song = Assert.Single(persisted.Songs);
        Assert.Equal(music.Id, song.MusicId);
        Assert.Equal(MusicalKey.E, song.PerformanceKey);
        Assert.Equal(music.Duration, persisted.TotalDuration);
    }
}