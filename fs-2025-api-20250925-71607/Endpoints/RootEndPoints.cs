namespace fs_2025_api_20250925_71607.Endpoints;

public static class RootEndPoints
{
    public static IEndpointRouteBuilder AddRootEndPoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/", () => Results.Ok(new
            {
                Service = "fs-2025-api",
                Status = "ok",
                Time = DateTime.UtcNow
            }))
            .WithTags("Root");

        return app;
    }
}