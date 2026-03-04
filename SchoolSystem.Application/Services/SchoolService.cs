using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using SchoolSystem.Application.DTOs;
using SchoolSystem.Application.Interfaces;
using SchoolSystem.Domain.Entities;
using SchoolSystem.Domain.Interfaces;

namespace SchoolSystem.Application.Services;

public class SchoolService : ISchoolService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMemoryCache _cache;

    public SchoolService(IUnitOfWork unitOfWork, IMemoryCache cache)
    {
        _unitOfWork = unitOfWork;
        _cache = cache;
    }

    public async Task<IEnumerable<TeacherDto>> GetAllTeachersAsync()
    {
        var teachers = await _unitOfWork.Teachers.GetAllAsync();
        return teachers.Select(t => new TeacherDto { Id = t.Id, Name = t.Name, Email = t.Email });
    }

    public async Task<TeacherDto?> GetTeacherByIdAsync(int id)
    {
        var t = await _unitOfWork.Teachers.GetByIdAsync(id);
        return t == null ? null : new TeacherDto { Id = t.Id, Name = t.Name, Email = t.Email };
    }

    public async Task<TeacherDto> CreateTeacherAsync(CreateTeacherDto dto)
    {
        var teacher = new Teacher { Name = dto.Name, Email = dto.Email };
        await _unitOfWork.Teachers.AddAsync(teacher);
        await _unitOfWork.SaveChangesAsync();
        return new TeacherDto { Id = teacher.Id, Name = teacher.Name, Email = teacher.Email };
    }

    public async Task UpdateTeacherAsync(int id, CreateTeacherDto dto)
    {
        var teacher = await _unitOfWork.Teachers.GetByIdAsync(id);
        if (teacher != null)
        {
            teacher.Name = dto.Name;
            teacher.Email = dto.Email;
            _unitOfWork.Teachers.Update(teacher);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task DeleteTeacherAsync(int id)
    {
        var teacher = await _unitOfWork.Teachers.GetByIdAsync(id);
        if (teacher != null)
        {
            _unitOfWork.Teachers.Delete(teacher);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<CourseDto>> GetAllCoursesAsync()
    {
        if (_cache.TryGetValue("all_courses", out IEnumerable<CourseDto>? cached) && cached != null)
            return cached;

        var courses = await _unitOfWork.Courses.GetAllAsync();
        var result = courses.Select(c => new CourseDto 
        { 
            Id = c.Id, 
            Name = c.Name, 
            Description = c.Description, 
            TeacherId = c.TeacherId 
        }).ToList();

        _cache.Set("all_courses", (IEnumerable<CourseDto>)result, TimeSpan.FromMinutes(5));
        return result;
    }

    public async Task<CourseDto?> GetCourseByIdAsync(int id)
    {
        var c = await _unitOfWork.Courses.GetByIdAsync(id);
        return c == null ? null : new CourseDto 
        { 
            Id = c.Id, 
            Name = c.Name, 
            Description = c.Description, 
            TeacherId = c.TeacherId 
        };
    }

    public async Task<CourseDto> CreateCourseAsync(CreateCourseDto dto)
    {
        var course = new Course 
        { 
            Name = dto.Name, 
            Description = dto.Description, 
            TeacherId = dto.TeacherId 
        };
        await _unitOfWork.Courses.AddAsync(course);
        await _unitOfWork.SaveChangesAsync();
        _cache.Remove("all_courses");
        return new CourseDto 
        { 
            Id = course.Id, 
            Name = course.Name, 
            Description = course.Description, 
            TeacherId = course.TeacherId 
        };
    }

    public async Task UpdateCourseAsync(int id, CreateCourseDto dto)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(id);
        if (course != null)
        {
            course.Name = dto.Name;
            course.Description = dto.Description;
            course.TeacherId = dto.TeacherId;
            _unitOfWork.Courses.Update(course);
            await _unitOfWork.SaveChangesAsync();
            _cache.Remove("all_courses");
        }
    }

    public async Task DeleteCourseAsync(int id)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(id);
        if (course != null)
        {
            _unitOfWork.Courses.Delete(course);
            await _unitOfWork.SaveChangesAsync();
            _cache.Remove("all_courses");
        }
    }

    public async Task<IEnumerable<CourseInstanceDto>> GetAllCourseInstancesAsync()
    {
        var instances = await _unitOfWork.CourseInstances.GetAllAsync();
        return instances.Select(i => new CourseInstanceDto 
        { 
            Id = i.Id, 
            StartDate = i.StartDate, 
            EndDate = i.EndDate, 
            CourseId = i.CourseId 
        });
    }

    public async Task<CourseInstanceDto?> GetCourseInstanceByIdAsync(int id)
    {
        var i = await _unitOfWork.CourseInstances.GetByIdAsync(id);
        return i == null ? null : new CourseInstanceDto 
        { 
            Id = i.Id, 
            StartDate = i.StartDate, 
            EndDate = i.EndDate, 
            CourseId = i.CourseId 
        };
    }

    public async Task<CourseInstanceDto> CreateCourseInstanceAsync(CreateCourseInstanceDto dto)
    {
        var instance = new CourseInstance 
        { 
            StartDate = dto.StartDate, 
            EndDate = dto.EndDate, 
            CourseId = dto.CourseId 
        };
        await _unitOfWork.CourseInstances.AddAsync(instance);
        await _unitOfWork.SaveChangesAsync();
        return new CourseInstanceDto 
        { 
            Id = instance.Id, 
            StartDate = instance.StartDate, 
            EndDate = instance.EndDate, 
            CourseId = instance.CourseId 
        };
    }

    public async Task UpdateCourseInstanceAsync(int id, CreateCourseInstanceDto dto)
    {
        var instance = await _unitOfWork.CourseInstances.GetByIdAsync(id);
        if (instance != null)
        {
            instance.StartDate = dto.StartDate;
            instance.EndDate = dto.EndDate;
            instance.CourseId = dto.CourseId;
            _unitOfWork.CourseInstances.Update(instance);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task DeleteCourseInstanceAsync(int id)
    {
        var instance = await _unitOfWork.CourseInstances.GetByIdAsync(id);
        if (instance != null)
        {
            _unitOfWork.CourseInstances.Delete(instance);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<ParticipantDto>> GetAllParticipantsAsync()
    {
        var parts = await _unitOfWork.Participants.GetAllAsync();
        return parts.Select(p => new ParticipantDto { Id = p.Id, Name = p.Name, Email = p.Email });
    }

    public async Task<ParticipantDto?> GetParticipantByIdAsync(int id)
    {
        var p = await _unitOfWork.Participants.GetByIdAsync(id);
        return p == null ? null : new ParticipantDto { Id = p.Id, Name = p.Name, Email = p.Email };
    }

    public async Task<ParticipantDto> CreateParticipantAsync(CreateParticipantDto dto)
    {
        var part = new Participant { Name = dto.Name, Email = dto.Email };
        await _unitOfWork.Participants.AddAsync(part);
        await _unitOfWork.SaveChangesAsync();
        return new ParticipantDto { Id = part.Id, Name = part.Name, Email = part.Email };
    }

    public async Task UpdateParticipantAsync(int id, CreateParticipantDto dto)
    {
        var p = await _unitOfWork.Participants.GetByIdAsync(id);
        if (p != null)
        {
            p.Name = dto.Name;
            p.Email = dto.Email;
            _unitOfWork.Participants.Update(p);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task DeleteParticipantAsync(int id)
    {
        var p = await _unitOfWork.Participants.GetByIdAsync(id);
        if (p != null)
        {
            _unitOfWork.Participants.Delete(p);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<CourseRegistrationDto>> GetAllRegistrationsAsync()
    {
        var regs = await _unitOfWork.CourseRegistrations.GetAllAsync();
        return regs.Select(r => new CourseRegistrationDto 
        { 
            Id = r.Id, 
            ParticipantId = r.ParticipantId, 
            CourseInstanceId = r.CourseInstanceId, 
            RegistrationDate = r.RegistrationDate 
        });
    }

    public async Task<CourseRegistrationDto?> GetRegistrationByIdAsync(int id)
    {
        var r = await _unitOfWork.CourseRegistrations.GetByIdAsync(id);
        return r == null ? null : new CourseRegistrationDto 
        { 
            Id = r.Id, 
            ParticipantId = r.ParticipantId, 
            CourseInstanceId = r.CourseInstanceId, 
            RegistrationDate = r.RegistrationDate 
        };
    }

    public async Task<CourseRegistrationDto> CreateRegistrationAsync(CreateCourseRegistrationDto dto)
    {
        var reg = new CourseRegistration 
        { 
            ParticipantId = dto.ParticipantId, 
            CourseInstanceId = dto.CourseInstanceId, 
            RegistrationDate = System.DateTime.UtcNow 
        };
        await _unitOfWork.CourseRegistrations.AddAsync(reg);
        await _unitOfWork.SaveChangesAsync();
        return new CourseRegistrationDto 
        { 
            Id = reg.Id, 
            ParticipantId = reg.ParticipantId, 
            CourseInstanceId = reg.CourseInstanceId, 
            RegistrationDate = reg.RegistrationDate 
        };
    }

    public async Task DeleteRegistrationAsync(int id)
    {
        var r = await _unitOfWork.CourseRegistrations.GetByIdAsync(id);
        if (r != null)
        {
            _unitOfWork.CourseRegistrations.Delete(r);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<List<Domain.Interfaces.CourseRegistrationCountDto>> GetRegistrationCountsAsync()
    {
        return await _unitOfWork.GetRegistrationCountsAsync();
    }

    public async Task<List<CourseRegistrationDto>> BatchRegisterAsync(List<CreateCourseRegistrationDto> dtos)
    {
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var results = new List<CourseRegistrationDto>();
            foreach (var dto in dtos)
            {
                var reg = new CourseRegistration
                {
                    ParticipantId = dto.ParticipantId,
                    CourseInstanceId = dto.CourseInstanceId,
                    RegistrationDate = DateTime.UtcNow
                };
                await _unitOfWork.CourseRegistrations.AddAsync(reg);
                await _unitOfWork.SaveChangesAsync();
                results.Add(new CourseRegistrationDto
                {
                    Id = reg.Id,
                    ParticipantId = reg.ParticipantId,
                    CourseInstanceId = reg.CourseInstanceId,
                    RegistrationDate = reg.RegistrationDate
                });
            }
            await _unitOfWork.CommitTransactionAsync();
            return results;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
