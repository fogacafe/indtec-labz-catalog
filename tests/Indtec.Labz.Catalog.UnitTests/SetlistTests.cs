using Indtec.Labz.Catalog.Domain.Music;
using Indtec.Labz.Catalog.Domain.Setlists;

namespace Indtec.Labz.Catalog.UnitTests;

public sealed class SetlistTests
{
    [Fact]
    public void Publish_requires_at_least_one_music()
    {
        var setlist = Setlist.Create("Friday night").Value!;
        var result = setlist.Publish(DateTimeOffset.UtcNow);
        Assert.True(result.IsFailure);
        Assert.Contains(SetlistErrors.EmptySetlist, result.Errors);
    }

    [Fact]
    public void Same_music_cannot_be_added_twice()
    {
        var setlist = Setlist.Create("Friday night").Value!;
        var musicId = Guid.NewGuid();
        Assert.True(setlist.AddSong(musicId, TimeSpan.FromMinutes(4), MusicalKey.E).IsSuccess);
        var result = setlist.AddSong(musicId, TimeSpan.FromMinutes(4), MusicalKey.D);
        Assert.True(result.IsFailure);
        Assert.Contains(SetlistErrors.DuplicateMusic, result.Errors);
    }

    [Fact]
    public void Published_setlist_is_immutable()
    {
        var setlist = Setlist.Create("Friday night").Value!;
        Assert.True(setlist.AddSong(Guid.NewGuid(), TimeSpan.FromMinutes(4), MusicalKey.E).IsSuccess);
        Assert.True(setlist.Publish(DateTimeOffset.UtcNow).IsSuccess);
        var result = setlist.AddSong(Guid.NewGuid(), TimeSpan.FromMinutes(3), MusicalKey.A);
        Assert.True(result.IsFailure);
        Assert.Contains(SetlistErrors.PublishedIsImmutable, result.Errors);
    }
}