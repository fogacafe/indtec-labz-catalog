using Dapper;
using Indtec.Labz.Catalog.Infrastructure.Persistence;
using Indtec.Labz.Catalog.Infrastructure.Secrets;
using Npgsql;
using Testcontainers.PostgreSql;

namespace Indtec.Labz.Catalog.IntegrationTests;

public sealed class PostgresFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:17-alpine")
        .WithDatabase("indtec_catalog")
        .WithUsername("indtec")
        .WithPassword("integration-test-only")
        .Build();

    public DbConnectionFactory ConnectionFactory { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        await using var connection = new NpgsqlConnection(_container.GetConnectionString());
        var schema = await File.ReadAllTextAsync(FindSchemaPath());
        await connection.ExecuteAsync(schema);

        var builder = new NpgsqlConnectionStringBuilder(_container.GetConnectionString());
        var options = new DatabaseOptions { Host = builder.Host!, Port = builder.Port, Name = builder.Database!, Username = builder.Username! };
        ConnectionFactory = new DbConnectionFactory(options, new TestCredentialsProvider(builder.Password!));
    }

    public Task DisposeAsync() => _container.DisposeAsync().AsTask();

    private static string FindSchemaPath()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, "database", "001_init.sql");
            if (File.Exists(candidate)) return candidate;
            directory = directory.Parent;
        }
        throw new FileNotFoundException("Could not locate database/001_init.sql.");
    }

    private sealed class TestCredentialsProvider(string password) : IDatabaseCredentialsProvider
    {
        public Task<DatabaseCredentials> GetAsync(CancellationToken cancellationToken) => Task.FromResult(new DatabaseCredentials(password));
    }
}