using System.Text.Json;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;

namespace Indtec.Labz.Catalog.Infrastructure.Secrets;

public sealed class AwsSecretsManagerDatabaseCredentialsProvider(
    IAmazonSecretsManager secretsManager,
    string secretId) : IDatabaseCredentialsProvider
{
    public async Task<DatabaseCredentials> GetAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(secretId))
            throw new InvalidOperationException("Database SecretId is not configured.");

        var response = await secretsManager.GetSecretValueAsync(new GetSecretValueRequest { SecretId = secretId }, cancellationToken);
        if (string.IsNullOrWhiteSpace(response.SecretString))
            throw new InvalidOperationException("Database secret does not contain a SecretString.");

        var payload = JsonSerializer.Deserialize<SecretPayload>(response.SecretString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        if (string.IsNullOrWhiteSpace(payload?.Password))
            throw new InvalidOperationException("Database secret must contain a 'password' property.");

        return new DatabaseCredentials(payload.Password);
    }

    private sealed record SecretPayload(string Password);
}