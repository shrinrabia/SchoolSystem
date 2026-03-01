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
}
