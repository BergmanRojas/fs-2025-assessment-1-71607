using System.Text.Json;
using fs_2025_api_20250925_71607.Models;
using fs_2025_api_20250925_71607.Background;
using Microsoft.Extensions.Caching.Memory;

namespace fs_2025_api_20250925_71607.Services
{
    public class BikeStationService : IBikeStationService
    {
        private readonly List<Station> _stations;
        private readonly IMemoryCache _cache;

        // Versión simple para invalidar cache cuando cambian los datos
        private int _cacheVersion = 0;

        public BikeStationService(IWebHostEnvironment env, IMemoryCache cache)
        {
            _cache = cache;

            var path = Path.Combine(env.ContentRootPath, "Data", "dublinbike.json");
            var json = File.ReadAllText(path);

            _stations = JsonSerializer.Deserialize<List<Station>>(json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<Station>();
        }

        // 🔹 Filtros / búsqueda / orden / paginación + CACHE 5 MIN
        public IEnumerable<Station> GetStations(
            string? q,
            string? status,
            int? minBikes,
            string? sort,
            string? dir,
            int page,
            int pageSize,
            out int totalCount)
        {
            // ✅ clave de cache única por combinación de parámetros + versión
            var cacheKey = $"stations:v{_cacheVersion}:{q}:{status}:{minBikes}:{sort}:{dir}:{page}:{pageSize}";

            if (_cache.TryGetValue(cacheKey, out (List<Station> Items, int Count) cached))
            {
                totalCount = cached.Count;
                return cached.Items;
            }

            IEnumerable<Station> query = _stations;

            // Búsqueda por nombre / dirección
            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(s =>
                    (!string.IsNullOrEmpty(s.Name) &&
                     s.Name.Contains(q, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(s.Address) &&
                     s.Address.Contains(q, StringComparison.OrdinalIgnoreCase)));
            }

            // Filtro por status (OPEN / CLOSED)
            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(s =>
                    string.Equals(s.Status, status, StringComparison.OrdinalIgnoreCase));
            }

            // Filtro por mínimo de bicicletas disponibles
            if (minBikes.HasValue && minBikes.Value > 0)
            {
                query = query.Where(s => s.AvailableBikes >= minBikes.Value);
            }

            // Helper local para ocupación
            static double Occupancy(Station s) =>
                s.BikeStands <= 0 ? 0.0 : (double)s.AvailableBikes / s.BikeStands;

            // Ordenación
            var sortKey = sort?.ToLowerInvariant();
            var dirDesc = string.Equals(dir, "desc", StringComparison.OrdinalIgnoreCase);

            query = (sortKey, dirDesc) switch
            {
                ("name", false)  => query.OrderBy(s => s.Name),
                ("name", true)   => query.OrderByDescending(s => s.Name),

                ("bikes", false) => query.OrderBy(s => s.AvailableBikes),
                ("bikes", true)  => query.OrderByDescending(s => s.AvailableBikes),

                ("occupancy", false) => query.OrderBy(Occupancy),
                ("occupancy", true)  => query.OrderByDescending(Occupancy),

                // default: por número ASC / DESC
                (_, false) => query.OrderBy(s => s.Number),
                (_, true)  => query.OrderByDescending(s => s.Number)
            };

            // Normalizar paginación
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            totalCount = query.Count();

            var items = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // ✅ Guardar en cache 5 minutos
            _cache.Set(cacheKey, (items, totalCount), TimeSpan.FromMinutes(5));

            return items;
        }

        public Station? GetByNumber(int number) =>
            _stations.SingleOrDefault(s => s.Number == number);

        // 🔹 Summary también cacheado 5 minutos
        public BikeSummary GetSummary()
        {
            const string summaryKey = "stations:summary";

            if (_cache.TryGetValue(summaryKey, out BikeSummary cachedSummary))
            {
                return cachedSummary;
            }

            var totalStations        = _stations.Count;
            var totalBikeStands      = _stations.Sum(s => s.BikeStands);
            var totalAvailableBikes  = _stations.Sum(s => s.AvailableBikes);
            var totalAvailableStands = _stations.Sum(s => s.AvailableStands);

            var openStations = _stations.Count(s =>
                string.Equals(s.Status, "OPEN", StringComparison.OrdinalIgnoreCase));

            var closedStations = totalStations - openStations;

            // Evitar división por cero
            var withStands = _stations.Where(s => s.BikeStands > 0).ToList();
            double avgOccupancy = withStands.Count == 0
                ? 0
                : withStands.Average(s => (double)s.AvailableBikes / s.BikeStands);

            var summary = new BikeSummary
            {
                TotalStations        = totalStations,
                TotalBikeStands      = totalBikeStands,
                TotalAvailableBikes  = totalAvailableBikes,
                TotalAvailableStands = totalAvailableStands,
                OpenStations         = openStations,
                ClosedStations       = closedStations,
                AverageOccupancy     = avgOccupancy
            };

            _cache.Set(summaryKey, summary, TimeSpan.FromMinutes(5));

            return summary;
        }

        public Station Create(Station station)
        {
            if (station == null) throw new ArgumentNullException(nameof(station));

            if (station.Number == 0)
            {
                station.Number = _stations.Any() ? _stations.Max(s => s.Number) + 1 : 1;
            }

            // ✅ marcar fecha de actualización al crear
            station.LastUpdateEpochMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            _stations.Add(station);

            // invalidar cache
            _cacheVersion++;
            _cache.Remove("stations:summary");

            return station;
        }

        public Station? Update(int number, Station station)
        {
            var existing = _stations.SingleOrDefault(s => s.Number == number);
            if (existing == null) return null;

            existing.Name            = station.Name;
            existing.Address         = station.Address;
            existing.Status          = station.Status;
            existing.BikeStands      = station.BikeStands;
            existing.AvailableBikes  = station.AvailableBikes;
            existing.AvailableStands = station.AvailableStands;

            // 👇 ahora con GeoPosition (ya lo tienes)
            if (station.Position != null)
            {
                existing.Position.Lat = station.Position.Lat;
                existing.Position.Lng = station.Position.Lng;
            }

            // ✅ actualizar marca de tiempo
            existing.LastUpdateEpochMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            // invalidar cache
            _cacheVersion++;
            _cache.Remove("stations:summary");

            return existing;
        }
        public void InvalidateCache()
        {
            _cacheVersion++;
            _cache.Remove("stations:summary");
        }
      public BikeStationService(List<Station> stations, IMemoryCache cache)
          {
              _stations = stations ?? throw new ArgumentNullException(nameof(stations));
              _cache = cache;
          }  
        
    }
    
}