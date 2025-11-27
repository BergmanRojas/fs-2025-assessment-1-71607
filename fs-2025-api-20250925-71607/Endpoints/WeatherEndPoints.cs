using fs_2025_api_20250925_71607.Models;

namespace fs_2025_api_20250925_71607.Endpoints;

public static class WeatherEndPoints
{
    internal static string[] summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild",
        "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    // Igual que el profe: WebApplication, no IEndpointRouteBuilder
    public static WebApplication AddWeatherEndPoints(this WebApplication app)
    {
        app.MapGet("/weatherforecast", () =>
            {
                var forecast = Enumerable.Range(1, 5).Select(index =>
                    new WeatherForecast(
                        DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                        Random.Shared.Next(-20, 55),
                        summaries[Random.Shared.Next(summaries.Length)]
                    )
                ).ToArray();

                return forecast;
            })
            .WithName("GetWeatherForecast")
            .WithOpenApi(); // opcional pero coincide con la captura

        return app;
    }
}