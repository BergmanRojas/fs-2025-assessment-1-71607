namespace fs_2025_api_20250925_71607.Models;

public class Course
{
    public int Id { get; set; }
    public bool IsPreorder { get; set; }
    public string CourseUrl { get; set; } = string.Empty;
    public string CourseType { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public int CourseLessonCount { get; set; }
    public decimal CourseLengthInHours { get; set; }
    public string ShortDescription { get; set; } = string.Empty;
    public string CourseImage { get; set; } = string.Empty;
    public decimal PriceInUSD { get; set; }
    public string? CoursePreviewLink { get; set; }
}