using Microsoft.Extensions.Configuration;

namespace Indtec.Labz.Catalog.Infrastructure.Secrets;

public sealed class ConfigurationDatabaseCredentialsProvider(IConfiguration configuration) : IDatabaseCredentialsProvider
{
    public Task<DatabaseCredentials> GetAsync(CancellationToken cancellationToken)
    {
        var password = configuration["Database:Password"];
        if (string.IsNullOrWhiteSpace(password))
            throw new InvalidOperationException("Database password is not configured. Use .NET user-secrets or an environment variable.");

        return Task.FromResult(new DatabaseCredentials(password));
    }
}