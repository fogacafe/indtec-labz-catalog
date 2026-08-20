using Dapper;
using Indtec.Labz.Catalog.Domain.Music;
using Indtec.Labz.Catalog.Domain.Setlists;

namespace Indtec.Labz.Catalog.Infrastructure.Persistence;

public sealed class SetlistRepository(DbConnectionFactory connectionFactory) : ISetlistRepository
{
    public async Task AddAsync(Setlist setlist, CancellationToken cancellationToken)
    {
        const string sql = "insert into setlist (id, name, status) values (@Id, @Name, @Status)";
        await using var connection = connectionFactory.Create();
        await connection.ExecuteAsync(new CommandDefinition(sql, new { setlist.Id, setlist.Name, Status = setlist.Status.ToString() }, cancellationToken: cancellationToken));
    }

    public async Task<Setlist?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        const string headerSql = "select id, name, status from setlist where id = @id";
        const string songsSql = """
            select ss.music_id, ss.position, m.duration_seconds, ss.performance_key
            from setlist_song ss join music m on m.id = ss.music_id
            where ss.setlist_id = @id order by ss.position
            """;
        await using var connection = connectionFactory.Create();
        var header = await connection.QuerySingleOrDefaultAsync<SetlistRow>(new CommandDefinition(headerSql, new { id }, cancellationToken: cancellationToken));
        if (header is null) return null;
        var rows = await connection.QueryAsync<SetlistSongRow>(new CommandDefinition(songsSql, new { id }, cancellationToken: cancellationToken));
        var songs = rows.Select(x => SetlistSong.Rehydrate(x.MusicId, x.Position, TimeSpan.FromSeconds(x.DurationSeconds), Enum.Parse<MusicalKey>(x.PerformanceKey)));
        return Setlist.Rehydrate(header.Id, header.Name, Enum.Parse<SetlistStatus>(header.Status), songs);
    }

    public async Task SaveAsync(Setlist setlist, CancellationToken cancellationToken)
    {
        await using var connection = connectionFactory.Create();
        await connection.OpenAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);
        await connection.ExecuteAsync(new CommandDefinition("update setlist set name=@Name, status=@Status where id=@Id", new { setlist.Id, setlist.Name, Status=setlist.Status.ToString() }, transaction, cancellationToken: cancellationToken));
        await connection.ExecuteAsync(new CommandDefinition("delete from setlist_song where setlist_id=@Id", new { setlist.Id }, transaction, cancellationToken: cancellationToken));
        const string insert = "insert into setlist_song (setlist_id, music_id, position, performance_key) values (@SetlistId,@MusicId,@Position,@PerformanceKey)";
        foreach (var song in setlist.Songs)
            await connection.ExecuteAsync(new CommandDefinition(insert, new { SetlistId=setlist.Id, song.MusicId, song.Position, PerformanceKey=song.PerformanceKey.ToString() }, transaction, cancellationToken: cancellationToken));
        await transaction.CommitAsync(cancellationToken);
    }

    private sealed record SetlistRow(Guid Id, string Name, string Status);
    private sealed record SetlistSongRow(Guid MusicId, int Position, int DurationSeconds, string PerformanceKey);
}