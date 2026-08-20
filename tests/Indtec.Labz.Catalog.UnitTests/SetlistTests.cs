using Indtec.Labz.Catalog.Domain;
using Indtec.Labz.Catalog.Domain.Music;
using Indtec.Labz.Catalog.Domain.Setlists;

namespace Indtec.Labz.Catalog.UnitTests;

public sealed class SetlistTests
{
    [Fact]
    public void Publish_requires_at_least_one_music()
    {
        var setlist = Setlist.Create("Friday night");
        Assert.Throws<DomainException>(() => setlist.Publish(DateTimeOffset.UtcNow));
    }

    [Fact]
    public void Same_music_cannot_be_added_twice()
    {
        var setlist = Setlist.Create("Friday night");
        var musicId = Guid.NewGuid();
        setlist.AddSong(musicId, TimeSpan.FromMinutes(4), MusicalKey.E);
        Assert.Throws<DomainException>(() => setlist.AddSong(musicId, TimeSpan.FromMinutes(4), MusicalKey.D));
    }

    [Fact]
    public void Published_setlist_is_immutable()
    {
        var setlist = Setlist.Create("Friday night");
        setlist.AddSong(Guid.NewGuid(), TimeSpan.FromMinutes(4), MusicalKey.E);
        setlist.Publish(DateTimeOffset.UtcNow);
        Assert.Throws<DomainException>(() => setlist.AddSong(Guid.NewGuid(), TimeSpan.FromMinutes(3), MusicalKey.A));
    }
}