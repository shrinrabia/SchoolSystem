namespace SchoolSystem.Domain.Entities;

public class CourseInstance
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;

    public ICollection<CourseRegistration> Registrations { get; set; } = new List<CourseRegistration>();
}
