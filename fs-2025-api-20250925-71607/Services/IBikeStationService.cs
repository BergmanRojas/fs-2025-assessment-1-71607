using fs_2025_api_20250925_71607.Models;

namespace fs_2025_api_20250925_71607.Services
{
    public interface IBikeStationService
    {
        // Lista con filtros + búsqueda + orden + paginación
        IEnumerable<Station> GetStations(
            string? q,
            string? status,
            int? minBikes,
            string? sort,
            string? dir,
            int page,
            int pageSize,
            out int totalCount);

        Station? GetByNumber(int number);
        BikeSummary GetSummary();

        Station Create(Station station);
        Station? Update(int number, Station station);
        
        void InvalidateCache();
    }

    // DTO de resultado paginado
    public record PagedResult<T>(
        IReadOnlyList<T> Items,
        int TotalCount,
        int Page,
        int PageSize);
}