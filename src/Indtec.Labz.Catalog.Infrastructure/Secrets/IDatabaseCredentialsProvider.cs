namespace Indtec.Labz.Catalog.Infrastructure.Secrets;

public interface IDatabaseCredentialsProvider
{
    Task<DatabaseCredentials> GetAsync(CancellationToken cancellationToken);
}