using fs_2025_api_20250925_71607.Data;

namespace fs_2025_api_20250925_71607.Endpoints;

public static class CourseEndPoints
{
    public static IEndpointRouteBuilder AddCourseEndPoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/courses").WithTags("Courses");

        // Lista de cursos (preview)
        group.MapGet("/", (CourseData data) =>
            Results.Ok(data.Courses.Select(c => new
            {
                c.Id,
                c.CourseName,
                c.CourseType,
                c.PriceInUSD
            }))
        );

        // Detalle de un curso
        group.MapGet("/{id:int}", (int id, CourseData data) =>
        {
            var course = data.Courses.FirstOrDefault(c => c.Id == id);
            return course is null ? Results.NotFound() : Results.Ok(course);
        });

        return app;
    }
}
