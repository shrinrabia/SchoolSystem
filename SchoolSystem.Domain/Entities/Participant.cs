namespace SchoolSystem.Domain.Entities;

public class Participant
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public ICollection<CourseRegistration> Registrations { get; set; } = new List<CourseRegistration>();
}
