
namespace SchoolSystem.Domain.Entities;
public class Course
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int TeacherId { get; set; }
    public Teacher Teacher { get; set; } = null!;

    public ICollection<CourseInstance> CourseInstances { get; set; } = new List<CourseInstance>();
}

