using fs_2025_api_20250925_71607.Data;

namespace fs_2025_api_20250925_71607.Startup
{
    public static class Dependencies
    {
        public static void AddDependencies(this WebApplicationBuilder builder)
        {
            // lo que ya tenías (CourseData, etc.)
            builder.Services.AddTransient<CourseData>();

            // nuevo
            builder.Services.AddSingleton<BookData>();
        }
    }
}