using Dapper;
using Indtec.Labz.Catalog.Domain.Music;

namespace Indtec.Labz.Catalog.Infrastructure.Persistence;

public sealed class MusicRepository(DbConnectionFactory connectionFactory) : IMusicRepository
{
    public async Task AddAsync(Music music, CancellationToken cancellationToken)
    {
        const string sql = """
            insert into music (id, name, artist, duration_seconds, bpm, original_key)
            values (@Id, @Name, @Artist, @DurationSeconds, @Bpm, @OriginalKey)
            """;
        await using var connection = await connectionFactory.CreateAsync(cancellationToken);
        await connection.ExecuteAsync(new CommandDefinition(sql, new { music.Id, music.Name, music.Artist, DurationSeconds = (int)music.Duration.TotalSeconds, music.Bpm, OriginalKey = music.OriginalKey.ToString() }, cancellationToken: cancellationToken));
    }

    public async Task<Music?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        const string sql = "select id, name, artist, duration_seconds, bpm, original_key from music where id = @id";
        await using var connection = await connectionFactory.CreateAsync(cancellationToken);
        var row = await connection.QuerySingleOrDefaultAsync<MusicRow>(new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken));
        return row is null ? null : Music.Rehydrate(row.Id, row.Name, row.Artist, TimeSpan.FromSeconds(row.DurationSeconds), row.Bpm, Enum.Parse<MusicalKey>(row.OriginalKey));
    }

    public async Task<IReadOnlyCollection<Music>> ListAsync(CancellationToken cancellationToken)
    {
        const string sql = "select id, name, artist, duration_seconds, bpm, original_key from music order by artist, name";
        await using var connection = await connectionFactory.CreateAsync(cancellationToken);
        var rows = await connection.QueryAsync<MusicRow>(new CommandDefinition(sql, cancellationToken: cancellationToken));
        return rows.Select(x => Music.Rehydrate(x.Id, x.Name, x.Artist, TimeSpan.FromSeconds(x.DurationSeconds), x.Bpm, Enum.Parse<MusicalKey>(x.OriginalKey))).ToArray();
    }

    private sealed record MusicRow(Guid Id, string Name, string Artist, int DurationSeconds, int? Bpm, string OriginalKey);
}