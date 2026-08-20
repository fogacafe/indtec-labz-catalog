using System.Data.Common;
using Indtec.Labz.Catalog.Infrastructure.Secrets;
using Npgsql;

namespace Indtec.Labz.Catalog.Infrastructure.Persistence;

public sealed class DbConnectionFactory(DatabaseOptions options, IDatabaseCredentialsProvider credentialsProvider)
{
    public async Task<DbConnection> CreateAsync(CancellationToken cancellationToken = default)
    {
        var credentials = await credentialsProvider.GetAsync(cancellationToken);
        var connectionString = new NpgsqlConnectionStringBuilder
        {
            Host = options.Host,
            Port = options.Port,
            Database = options.Name,
            Username = options.Username,
            Password = credentials.Password
        }.ConnectionString;

        return new NpgsqlConnection(connectionString);
    }
}