using Microsoft.Extensions.Hosting;
using fs_2025_api_20250925_71607.Services;

namespace fs_2025_api_20250925_71607.Background
{
    public class BikeDataUpdater : BackgroundService
    {
        private readonly IBikeStationService _service;
        private readonly Random _rand = new();

        public BikeDataUpdater(IBikeStationService service)
        {
            _service = service;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                UpdateAllStations();

                // Espera entre 10 y 20 segundos
                int delay = _rand.Next(10, 21);
                await Task.Delay(TimeSpan.FromSeconds(delay), stoppingToken);
            }
        }

        private void UpdateAllStations()
        {
            var all = _service.GetStations(
                q: null,
                status: null,
                minBikes: null,
                sort: null,
                dir: null,
                page: 1,
                pageSize: int.MaxValue,
                out var _ // no importa el count
            ).ToList();

            foreach (var s in all)
            {
                int stands = _rand.Next(10, 41); // 10–40 stands totales
                int bikes = _rand.Next(0, stands + 1);

                s.BikeStands = stands;
                s.AvailableBikes = bikes;
                s.AvailableStands = stands - bikes;

                s.LastUpdateEpochMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            }

            // invalida cache
            _service.InvalidateCache();
        }
    }
}