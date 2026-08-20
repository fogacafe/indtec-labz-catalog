using Indtec.Labz.Catalog.Domain.BuildingBlocks;
using Indtec.Labz.Catalog.Domain.Music;
using Indtec.Labz.Catalog.Domain.Results;

namespace Indtec.Labz.Catalog.Domain.Setlists;

public sealed class Setlist : IAggregateRoot<Guid>
{
    private readonly List<SetlistSong> _songs = [];
    private Setlist(Guid id, string name, SetlistStatus status) { Id = id; Name = name; Status = status; }

    public Guid Id { get; }
    public string Name { get; }
    public SetlistStatus Status { get; private set; }
    public IReadOnlyCollection<SetlistSong> Songs => _songs.AsReadOnly();
    public TimeSpan TotalDuration => TimeSpan.FromTicks(_songs.Sum(x => x.Duration.Ticks));

    public static Result<Setlist> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return Result<Setlist>.Failure(SetlistErrors.NameRequired);
        return Result<Setlist>.Success(new Setlist(Guid.NewGuid(), name.Trim(), SetlistStatus.Draft));
    }

    public static Setlist Rehydrate(Guid id, string name, SetlistStatus status, IEnumerable<SetlistSong> songs)
    {
        var setlist = new Setlist(id, name, status);
        setlist._songs.AddRange(songs.OrderBy(x => x.Position));
        return setlist;
    }

    public Result AddSong(Guid musicId, TimeSpan duration, MusicalKey performanceKey)
    {
        var editable = EnsureDraft(); if (editable.IsFailure) return editable;
        if (_songs.Any(x => x.MusicId == musicId)) return Result.Failure(SetlistErrors.DuplicateMusic);
        if (duration <= TimeSpan.Zero) return Result.Failure(SetlistErrors.InvalidDuration);
        _songs.Add(new SetlistSong(musicId, _songs.Count + 1, duration, performanceKey));
        return Result.Success();
    }

    public Result RemoveSong(Guid musicId)
    {
        var editable = EnsureDraft(); if (editable.IsFailure) return editable;
        var song = _songs.SingleOrDefault(x => x.MusicId == musicId);
        if (song is null) return Result.Failure(SetlistErrors.MusicNotFound);
        _songs.Remove(song);
        for (var index = 0; index < _songs.Count; index++) _songs[index].MoveTo(index + 1);
        return Result.Success();
    }

    public Result<SetlistPublished> Publish(DateTimeOffset occurredAt)
    {
        var editable = EnsureDraft();
        if (editable.IsFailure) return Result<SetlistPublished>.Failure(editable.Errors);
        if (_songs.Count == 0) return Result<SetlistPublished>.Failure(SetlistErrors.EmptySetlist);
        Status = SetlistStatus.Published;
        return Result<SetlistPublished>.Success(new SetlistPublished(Id, occurredAt));
    }

    private Result EnsureDraft() => Status == SetlistStatus.Draft ? Result.Success() : Result.Failure(SetlistErrors.PublishedIsImmutable);
}