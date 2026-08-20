using Indtec.Labz.Catalog.Domain.Music;

namespace Indtec.Labz.Catalog.UnitTests;

public sealed class MusicTests
{
    [Fact]
    public void Create_accumulates_independent_validation_errors()
    {
        var result = Music.Create("", "", TimeSpan.Zero, 0, MusicalKey.C);

        Assert.True(result.IsFailure);
        Assert.Equal(4, result.Errors.Count);
        Assert.Contains(MusicErrors.NameRequired, result.Errors);
        Assert.Contains(MusicErrors.ArtistRequired, result.Errors);
        Assert.Contains(MusicErrors.InvalidDuration, result.Errors);
        Assert.Contains(MusicErrors.InvalidBpm, result.Errors);
    }
}