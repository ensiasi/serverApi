var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Root endpoint — confirms the app is alive and shows the machine it's running on.
app.MapGet("/", () =>
    $"Hello from TestApi running on {Environment.MachineName} at {DateTime.UtcNow:O} (UTC)");

// Health check — used later by reverse proxies, load balancers, Kubernetes probes, monitoring.
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timeUtc = DateTime.UtcNow }));

// A tiny in-memory data endpoint, so there's something with real JSON output to test.
var notes = new List<string> { "first note", "second note" };

app.MapGet("/notes", () => Results.Ok(notes));

app.MapPost("/notes", (NoteInput input) =>
{
    if (string.IsNullOrWhiteSpace(input.Text))
        return Results.BadRequest(new { error = "text is required" });

    notes.Add(input.Text);
    return Results.Created($"/notes/{notes.Count - 1}", input.Text);
});

app.Run();

// Record type for the POST body (C# top-level + record, .NET 10 style).
record NoteInput(string Text);
