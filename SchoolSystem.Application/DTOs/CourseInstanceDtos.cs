using System;

namespace SchoolSystem.Application.DTOs;

public class CourseInstanceDto
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
}

public class CreateCourseInstanceDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int CourseId { get; set; }
}
