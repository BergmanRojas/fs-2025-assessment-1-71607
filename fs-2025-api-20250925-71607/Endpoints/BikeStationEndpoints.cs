using fs_2025_api_20250925_71607.Models;
using fs_2025_api_20250925_71607.Services;

namespace fs_2025_api_20250925_71607.Endpoints;

public static class BikeStationEndpoints
{
    public static void AddBikeStationEndpoints(this IEndpointRouteBuilder app)
    {
        // Grupo principal: /api/v1/stations
        var groupV1 = app.MapGroup("/api/v1/stations");

        // 1) Lista con filtro, búsqueda, orden y paginación
        groupV1.MapGet("", GetStations);

        // 2) Detalle por número de estación
        groupV1.MapGet("/{number:int}", GetStationByNumber);

        // 3) Resumen agregado
        groupV1.MapGet("/summary", GetSummary);

        // 4) Crear (en memoria, usando el servicio)
        groupV1.MapPost("", CreateStation);

        // 5) Actualizar (en memoria, usando el servicio)
        groupV1.MapPut("/{number:int}", UpdateStation);
    }

    // ---------- Endpoints ----------

    // GET /api/v1/stations?q=&status=&minBikes=&sort=&dir=&page=&pageSize=
    private static IResult GetStations(
        IBikeStationService service,
        string? q,
        string? status,
        int? minBikes,
        string? sort,
        string? dir,
        int page = 1,
        int pageSize = 10)
    {
        var items = service.GetStations(
            q,
            status,
            minBikes,
            sort,
            dir,
            page,
            pageSize,
            out var totalCount);

        var result = new
        {
            page,
            pageSize,
            totalCount,
            items
        };

        return Results.Ok(result);
    }

    // GET /api/v1/stations/{number}
    private static IResult GetStationByNumber(int number, IBikeStationService service)
    {
        var station = service.GetByNumber(number);

        return station is null
            ? Results.NotFound()
            : Results.Ok(station);
    }

    // GET /api/v1/stations/summary
    private static IResult GetSummary(IBikeStationService service)
    {
        var summary = service.GetSummary();
        return Results.Ok(summary);
    }

    // POST /api/v1/stations
    private static IResult CreateStation(Station station, IBikeStationService service)
    {
        if (station is null)
        {
            return Results.BadRequest("Station body is required.");
        }

        var created = service.Create(station);

        return Results.Created($"/api/v1/stations/{created.Number}", created);
    }

    // PUT /api/v1/stations/{number}
    private static IResult UpdateStation(int number, Station station, IBikeStationService service)
    {
        if (station is null)
        {
            return Results.BadRequest("Station body is required.");
        }

        if (number != station.Number)
        {
            return Results.BadRequest("URL number must match station.Number");
        }

        var updated = service.Update(number, station);

        if (updated is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(updated);
    }
}