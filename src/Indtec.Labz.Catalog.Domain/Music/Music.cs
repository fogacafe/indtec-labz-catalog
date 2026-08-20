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
        if (string.IsNullOrWhiteSpace(name)) return Result<Music>.Failure(MusicErrors.NameRequired);
        if (string.IsNullOrWhiteSpace(artist)) return Result<Music>.Failure(MusicErrors.ArtistRequired);
        if (duration <= TimeSpan.Zero) return Result<Music>.Failure(MusicErrors.InvalidDuration);
        if (bpm is <= 0) return Result<Music>.Failure(MusicErrors.InvalidBpm);

        return Result<Music>.Success(new Music(Guid.NewGuid(), name.Trim(), artist.Trim(), duration, bpm, originalKey));
    }
}