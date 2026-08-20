using Indtec.Labz.Catalog.Domain.BuildingBlocks;
using Indtec.Labz.Catalog.Domain.Results;

namespace Indtec.Labz.Catalog.Domain.Music;

public sealed class Music : IAggregateRoot<Guid>
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

    public static Result<Music> Create(string name, string artist, TimeSpan duration, int? bpm, MusicalKey originalKey)
    {
        var errors = new List<Error>();
        if (string.IsNullOrWhiteSpace(name)) errors.Add(MusicErrors.NameRequired);
        if (string.IsNullOrWhiteSpace(artist)) errors.Add(MusicErrors.ArtistRequired);
        if (duration <= TimeSpan.Zero) errors.Add(MusicErrors.InvalidDuration);
        if (bpm is <= 0) errors.Add(MusicErrors.InvalidBpm);
        if (errors.Count > 0) return Result<Music>.Failure(errors);

        return Result<Music>.Success(new Music(Guid.NewGuid(), name.Trim(), artist.Trim(), duration, bpm, originalKey));
    }

    public static Music Rehydrate(Guid id, string name, string artist, TimeSpan duration, int? bpm, MusicalKey originalKey)
        => new(id, name, artist, duration, bpm, originalKey);
}