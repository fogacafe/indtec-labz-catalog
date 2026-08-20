using Indtec.Labz.Catalog.Domain.Music;

namespace Indtec.Labz.Catalog.Domain.Setlists;

public sealed class Setlist
{
    private readonly List<SetlistSong> _songs = [];

    private Setlist(Guid id, string name)
    {
        Id = id;
        Name = name;
        Status = SetlistStatus.Draft;
    }

    public Guid Id { get; }
    public string Name { get; }
    public SetlistStatus Status { get; private set; }
    public IReadOnlyCollection<SetlistSong> Songs => _songs.AsReadOnly();
    public TimeSpan TotalDuration => TimeSpan.FromTicks(_songs.Sum(x => x.Duration.Ticks));

    public static Setlist Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Setlist name is required.");
        return new Setlist(Guid.NewGuid(), name.Trim());
    }

    public void AddSong(Guid musicId, TimeSpan duration, MusicalKey performanceKey)
    {
        EnsureDraft();
        if (_songs.Any(x => x.MusicId == musicId)) throw new DomainException("A music can appear only once in a setlist.");
        if (duration <= TimeSpan.Zero) throw new DomainException("Music duration must be greater than zero.");

        _songs.Add(new SetlistSong(musicId, _songs.Count + 1, duration, performanceKey));
    }

    public void RemoveSong(Guid musicId)
    {
        EnsureDraft();
        var song = _songs.SingleOrDefault(x => x.MusicId == musicId) ?? throw new DomainException("Music is not part of this setlist.");
        _songs.Remove(song);
        for (var index = 0; index < _songs.Count; index++) _songs[index].MoveTo(index + 1);
    }

    public SetlistPublished Publish(DateTimeOffset occurredAt)
    {
        EnsureDraft();
        if (_songs.Count == 0) throw new DomainException("A setlist must contain at least one music before publication.");
        Status = SetlistStatus.Published;
        return new SetlistPublished(Id, occurredAt);
    }

    private void EnsureDraft()
    {
        if (Status != SetlistStatus.Draft) throw new DomainException("Published setlists cannot be changed.");
    }
}