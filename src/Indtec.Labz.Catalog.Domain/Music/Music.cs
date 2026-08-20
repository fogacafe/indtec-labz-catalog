namespace Indtec.Labz.Catalog.Domain.Music;

public sealed class Music
{
    private Music(Guid id, string name, string artist, TimeSpan duration, int? bpm, MusicalKey originalKey)
    {
        Id = id;
        Name = name;
        Artist = artist;
        Duration = duration;
        Bpm = bpm;
        OriginalKey = originalKey;
    }

    public Guid Id { get; }
    public string Name { get; }
    public string Artist { get; }
    public TimeSpan Duration { get; }
    public int? Bpm { get; }
    public MusicalKey OriginalKey { get; }

    public static Music Create(string name, string artist, TimeSpan duration, int? bpm, MusicalKey originalKey)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Music name is required.");
        if (string.IsNullOrWhiteSpace(artist)) throw new DomainException("Artist is required.");
        if (duration <= TimeSpan.Zero) throw new DomainException("Duration must be greater than zero.");
        if (bpm is <= 0) throw new DomainException("BPM must be greater than zero when informed.");

        return new Music(Guid.NewGuid(), name.Trim(), artist.Trim(), duration, bpm, originalKey);
    }
}