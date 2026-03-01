using System;

namespace SchoolSystem.Application.DTOs;

public class CourseRegistrationDto
{
    public int Id { get; set; }
    public int ParticipantId { get; set; }
    public string ParticipantName { get; set; } = string.Empty;
    public int CourseInstanceId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public DateTime RegistrationDate { get; set; }
}

public class CreateCourseRegistrationDto
{
    public int ParticipantId { get; set; }
    public int CourseInstanceId { get; set; }
}
