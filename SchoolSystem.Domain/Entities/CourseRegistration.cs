namespace SchoolSystem.Domain.Entities;

public class CourseRegistration
{
    public int Id { get; set; }

    public int ParticipantId { get; set; }
    public Participant Participant { get; set; } = null!;

    public int CourseInstanceId { get; set; }
    public CourseInstance CourseInstance { get; set; } = null!;

    public DateTime RegistrationDate { get; set; }
}
