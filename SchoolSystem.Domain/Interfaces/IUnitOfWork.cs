using SchoolSystem.Domain.Entities;
using System.Threading.Tasks;

namespace SchoolSystem.Domain.Interfaces;

public interface IUnitOfWork
{
    IRepository<Course> Courses { get; }
    IRepository<Teacher> Teachers { get; }
    IRepository<CourseInstance> CourseInstances { get; }
    IRepository<Participant> Participants { get; }
    IRepository<CourseRegistration> CourseRegistrations { get; }
    
    Task<int> SaveChangesAsync();
    Task<List<CourseRegistrationCountDto>> GetRegistrationCountsAsync();
}

public class CourseRegistrationCountDto
{
    public string CourseName { get; set; } = string.Empty;
    public int RegistrationCount { get; set; }
}
