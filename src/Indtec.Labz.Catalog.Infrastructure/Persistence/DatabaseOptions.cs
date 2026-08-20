namespace Indtec.Labz.Catalog.Infrastructure.Persistence;

public sealed class DatabaseOptions
{
    public const string SectionName = "Database";

    public string Host { get; init; } = "localhost";
    public int Port { get; init; } = 5432;
    public string Name { get; init; } = "indtec_catalog";
    public string Username { get; init; } = "indtec";
    public string? Password { get; init; }
    public string? SecretId { get; init; }
}