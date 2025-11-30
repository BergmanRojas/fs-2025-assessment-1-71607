using fs_2025_api_20250925_71607.Data;
using fs_2025_api_20250925_71607.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace fs_2025_api_20250925_71607.Services
{
    public class CosmosBikeStationService
    {
        private readonly Container _container;

        public CosmosBikeStationService(CosmosClient client, IOptions<CosmosSettings> options)
        {
            var settings = options.Value;
            _container = client.GetContainer(settings.DatabaseName, settings.ContainerName);
        }

        // 🔹 GET /api/v2/stations  (simple, sin filtros avanzados)
        public async Task<IReadOnlyList<Station>> GetStationsAsync()
        {
            var query = new QueryDefinition("SELECT * FROM c");
            var iterator = _container.GetItemQueryIterator<Station>(query);

            var results = new List<Station>();

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                results.AddRange(response);
            }

            return results;
        }

        // 🔹 GET /api/v2/stations/{number}
        public async Task<Station?> GetByNumberAsync(int number)
        {
            // suponiendo que el "number" es único y está guardado como campo
            var query = new QueryDefinition(
                "SELECT * FROM c WHERE c.number = @number"
            ).WithParameter("@number", number);

            var iterator = _container.GetItemQueryIterator<Station>(query);

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                var station = response.Resource.FirstOrDefault();
                if (station != null) return station;
            }

            return null;
        }
    }
}