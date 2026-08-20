using Indtec.Labz.Catalog.Domain.Music;
using Indtec.Labz.Catalog.Domain.Setlists;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
var app = builder.Build();

if (app.Environment.IsDevelopment()) app.MapOpenApi();

// LABZ / 001 starts with an intentionally small HTTP surface.
// Persistence-backed handlers are introduced incrementally as the application use cases evolve.
app.MapPost("/songs", (CreateMusicRequest request) =>
{
    var music = Music.Create(request.Name, request.Artist, TimeSpan.FromSeconds(request.DurationSeconds), request.Bpm, request.OriginalKey);
    return Results.Created($"/songs/{music.Id}", music);
});

app.MapPost("/setlists", (CreateSetlistRequest request) =>
{
    var setlist = Setlist.Create(request.Name);
    return Results.Created($"/setlists/{setlist.Id}", new { setlist.Id, setlist.Name, setlist.Status });
});

app.MapGet("/health", () => Results.Ok(new { status = "nominal", lab = "INDTEC LABZ / 001" }));
app.Run();

public sealed record CreateMusicRequest(string Name, string Artist, int DurationSeconds, int? Bpm, MusicalKey OriginalKey);
public sealed record CreateSetlistRequest(string Name);