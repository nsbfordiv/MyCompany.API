var builder = WebApplication.CreateBuilder(args);

// Option A: explicitly add console + debug
builder.Logging.ClearProviders();
builder.Logging.AddConsole(); // for terminal / dotnet run
builder.Logging.AddDebug();   // for Visual Studio Output window
builder.Logging.SetMinimumLevel(LogLevel.Information); // optional but nice

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};


app.MapGet("/weather", (ILogger<Program> logger) =>
{
    var temperature = Random.Shared.Next(-20, 55);
    var city = "Phoenix";

    // Structured logging
    logger.LogInformation(
        "Weather requested for {City} at {Timestamp} with temperature {TemperatureC}",
        city,
        DateTimeOffset.UtcNow,
        temperature
    );

    return Results.Ok(new { City = city, TemperatureC = temperature });
});

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapGet("/notes", () =>
{
    var notes = new[]
    {
        new { Id = 1, Text = "First note", CreatedAt = DateTime.UtcNow },
        new { Id = 2, Text = "Second note", CreatedAt = DateTime.UtcNow }
    };

    return Results.Ok(notes);
})
.WithName("GetNotes");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
