using fs_2025_api_20250925_71607.Data;
using fs_2025_api_20250925_71607.Services;
using fs_2025_api_20250925_71607.Background;

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
            
            builder.Services.AddMemoryCache();
            
            builder.Services.AddHostedService<BikeDataUpdater>();
        }
    }
}