using System.Text.Json;
using fs_2025_api_20250925_71607.Models;

namespace fs_2025_api_20250925_71607.Data;

public class CourseData
{
    public List<Course> Courses { get; private set; } = new();

    public CourseData()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        // Igual al profe: lee desde la carpeta Data de la build (/bin/Debug/.../Data)
        var filePath = Path.Combine(AppContext.BaseDirectory, "Data", "coursedata.json");

        if (File.Exists(filePath))
        {
            var json = File.ReadAllText(filePath);
            Courses = JsonSerializer.Deserialize<List<Course>>(json, options) ?? new();
        }
    }
}