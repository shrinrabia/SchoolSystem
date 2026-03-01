using System.Threading.Tasks;
using SchoolSystem.Domain.Entities;
using SchoolSystem.Domain.Interfaces;

namespace SchoolSystem.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    
    public IRepository<Course> Courses { get; private set; }
    public IRepository<Teacher> Teachers { get; private set; }
    public IRepository<CourseInstance> CourseInstances { get; private set; }
    public IRepository<Participant> Participants { get; private set; }
    public IRepository<CourseRegistration> CourseRegistrations { get; private set; }

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Courses = new Repository<Course>(_context);
        Teachers = new Repository<Teacher>(_context);
        CourseInstances = new Repository<CourseInstance>(_context);
        Participants = new Repository<Participant>(_context);
        CourseRegistrations = new Repository<CourseRegistration>(_context);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
