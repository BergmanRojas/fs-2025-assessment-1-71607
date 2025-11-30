using fs_2025_api_20250925_71607.Data;
using fs_2025_api_20250925_71607.Services;
using fs_2025_api_20250925_71607.Background;
using Microsoft.Extensions.Options;
using Microsoft.Azure.Cosmos;

namespace fs_2025_api_20250925_71607.Startup
{
    public static class Dependencies
    {
        public static void AddDependencies(this WebApplicationBuilder builder)
        {
            // Lo que ya tenías
            builder.Services.AddTransient<CourseData>();
            builder.Services.AddSingleton<BookData>();

            // ✅ Servicio de estaciones de Dublin Bikes
            builder.Services.AddSingleton<IBikeStationService, BikeStationService>();
            
            // ✅ Cache en memoria
            builder.Services.AddMemoryCache();
            
            // ✅ Configuración Cosmos
            builder.Services.Configure<CosmosSettings>(
                builder.Configuration.GetSection("Cosmos"));

            // ✅ CosmosClient (usa connection string del appsettings)
            builder.Services.AddSingleton(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<CosmosSettings>>().Value;
                return new CosmosClient(settings.ConnectionString);
            });

            // ✅ Servicio específico para v2 (Cosmos)
            builder.Services.AddSingleton<CosmosBikeStationService>();

            // ✅ (si ya tenías background BikeDataUpdater, déjalo como está)
            builder.Services.AddHostedService<BikeDataUpdater>();
            
            
        }
    }
}