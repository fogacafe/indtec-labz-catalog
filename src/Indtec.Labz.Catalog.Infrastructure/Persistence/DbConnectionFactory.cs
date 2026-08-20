using System.Data.Common;
using Npgsql;

namespace Indtec.Labz.Catalog.Infrastructure.Persistence;

public sealed class DbConnectionFactory(string connectionString)
{
    public DbConnection Create() => new NpgsqlConnection(connectionString);
}