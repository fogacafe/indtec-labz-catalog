using Amazon.SecretsManager;
using Indtec.Labz.Catalog.Api;
using Indtec.Labz.Catalog.Application.Setlists;
using Indtec.Labz.Catalog.Domain.Music;
using Indtec.Labz.Catalog.Domain.Setlists;
using Indtec.Labz.Catalog.Infrastructure.Persistence;
using Indtec.Labz.Catalog.Infrastructure.Secrets;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton(TimeProvider.System);

var database = builder.Configuration.GetSection(DatabaseOptions.SectionName).Get<DatabaseOptions>()
    ?? throw new InvalidOperationException("Database configuration is missing.");
builder.Services.AddSingleton(database);

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSingleton<IDatabaseCredentialsProvider, ConfigurationDatabaseCredentialsProvider>();
}
else
{
    builder.Services.AddSingleton<IAmazonSecretsManager, AmazonSecretsManagerClient>();
    builder.Services.AddSingleton<IDatabaseCredentialsProvider>(sp =>
        new AwsSecretsManagerDatabaseCredentialsProvider(
            sp.GetRequiredService<IAmazonSecretsManager>(),
            database.SecretId ?? throw new InvalidOperationException("Database:SecretId is required outside Development.")));
}

builder.Services.AddSingleton<DbConnectionFactory>();
builder.Services.AddScoped<IMusicRepository, MusicRepository>();
builder.Services.AddScoped<ISetlistRepository, SetlistRepository>();
builder.Services.AddScoped<PublishSetlist>();

var app = builder.Build();

app.MapPost("/songs", async (CreateMusicRequest request, IMusicRepository repository, CancellationToken ct) =>
{
    var result = Music.Create(request.Name, request.Artist, TimeSpan.FromSeconds(request.DurationSeconds), request.Bpm, request.OriginalKey);
    if (result.IsFailure) return result.ToProblem();
    await repository.AddAsync(result.Value!, ct);
    return Results.Created($"/songs/{result.Value!.Id}", result.Value);
});

app.MapGet("/songs", async (IMusicRepository repository, CancellationToken ct) => Results.Ok(await repository.ListAsync(ct)));

app.MapPost("/setlists", async (CreateSetlistRequest request, ISetlistRepository repository, CancellationToken ct) =>
{
    var result = Setlist.Create(request.Name);
    if (result.IsFailure) return result.ToProblem();
    await repository.AddAsync(result.Value!, ct);
    return Results.Created($"/setlists/{result.Value!.Id}", result.Value);
});

app.MapPost("/setlists/{id:guid}/songs", async (Guid id, AddSetlistSongRequest request, ISetlistRepository setlists, IMusicRepository music, CancellationToken ct) =>
{
    var setlist = await setlists.GetAsync(id, ct); if (setlist is null) return Results.NotFound();
    var catalogMusic = await music.GetAsync(request.MusicId, ct); if (catalogMusic is null) return Results.NotFound();
    var result = setlist.AddSong(catalogMusic.Id, catalogMusic.Duration, request.PerformanceKey);
    if (result.IsFailure) return result.ToProblem(StatusCodes.Status409Conflict);
    await setlists.SaveAsync(setlist, ct);
    return Results.NoContent();
});

app.MapPost("/setlists/{id:guid}/publish", async (Guid id, PublishSetlist useCase, CancellationToken ct) =>
{
    var result = await useCase.ExecuteAsync(id, ct);
    return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblem(StatusCodes.Status409Conflict);
});

app.MapGet("/setlists/{id:guid}", async (Guid id, ISetlistRepository repository, CancellationToken ct) =>
{
    var setlist = await repository.GetAsync(id, ct);
    return setlist is null ? Results.NotFound() : Results.Ok(new { setlist.Id, setlist.Name, setlist.Status, setlist.TotalDuration, setlist.Songs });
});

app.MapGet("/health", () => Results.Ok(new { status = "nominal", lab = "INDTEC LABZ / 001" }));
app.Run();

public partial class Program;
public sealed record CreateMusicRequest(string Name, string Artist, int DurationSeconds, int? Bpm, MusicalKey OriginalKey);
public sealed record CreateSetlistRequest(string Name);
public sealed record AddSetlistSongRequest(Guid MusicId, MusicalKey PerformanceKey);