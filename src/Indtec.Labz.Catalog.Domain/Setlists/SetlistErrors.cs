using Indtec.Labz.Catalog.Domain.Results;

namespace Indtec.Labz.Catalog.Domain.Setlists;

public static class SetlistErrors
{
    public static readonly Error NameRequired = new("setlist.name_required", "Setlist name is required.");
    public static readonly Error PublishedIsImmutable = new("setlist.published_immutable", "Published setlists cannot be changed.");
    public static readonly Error DuplicateMusic = new("setlist.duplicate_music", "A music can appear only once in a setlist.");
    public static readonly Error InvalidDuration = new("setlist.invalid_duration", "Music duration must be greater than zero.");
    public static readonly Error MusicNotFound = new("setlist.music_not_found", "Music is not part of this setlist.");
    public static readonly Error EmptySetlist = new("setlist.empty", "A setlist must contain at least one music before publication.");
    public static readonly Error NotFound = new("setlist.not_found", "Setlist was not found.");
}