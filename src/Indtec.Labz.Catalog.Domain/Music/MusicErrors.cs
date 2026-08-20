using Indtec.Labz.Catalog.Domain.Results;

namespace Indtec.Labz.Catalog.Domain.Music;

public static class MusicErrors
{
    public static readonly Error NameRequired = new("music.name_required", "Music name is required.");
    public static readonly Error ArtistRequired = new("music.artist_required", "Artist is required.");
    public static readonly Error InvalidDuration = new("music.invalid_duration", "Duration must be greater than zero.");
    public static readonly Error InvalidBpm = new("music.invalid_bpm", "BPM must be greater than zero when informed.");
}