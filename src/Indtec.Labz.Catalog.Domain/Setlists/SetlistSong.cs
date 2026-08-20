using Indtec.Labz.Catalog.Domain.Music;

namespace Indtec.Labz.Catalog.Domain.Setlists;

public sealed class SetlistSong
{
    internal SetlistSong(Guid musicId, int position, TimeSpan duration, MusicalKey performanceKey)
    {
        MusicId = musicId; Position = position; Duration = duration; PerformanceKey = performanceKey;
    }

    public Guid MusicId { get; }
    public int Position { get; private set; }
    public TimeSpan Duration { get; }
    public MusicalKey PerformanceKey { get; }

    public static SetlistSong Rehydrate(Guid musicId, int position, TimeSpan duration, MusicalKey performanceKey)
        => new(musicId, position, duration, performanceKey);

    internal void MoveTo(int position) => Position = position;
}