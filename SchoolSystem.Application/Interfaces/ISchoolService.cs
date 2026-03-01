using System.Collections.Generic;
using System.Threading.Tasks;
using SchoolSystem.Application.DTOs;

namespace SchoolSystem.Application.Interfaces;

public interface ISchoolService
{
    // Teachers
    Task<IEnumerable<TeacherDto>> GetAllTeachersAsync();
    Task<TeacherDto?> GetTeacherByIdAsync(int id);
    Task<TeacherDto> CreateTeacherAsync(CreateTeacherDto dto);
    Task UpdateTeacherAsync(int id, CreateTeacherDto dto);
    Task DeleteTeacherAsync(int id);

    // Courses
    Task<IEnumerable<CourseDto>> GetAllCoursesAsync();
    Task<CourseDto?> GetCourseByIdAsync(int id);
    Task<CourseDto> CreateCourseAsync(CreateCourseDto dto);
    Task UpdateCourseAsync(int id, CreateCourseDto dto);
    Task DeleteCourseAsync(int id);

    // Course Instances
    Task<IEnumerable<CourseInstanceDto>> GetAllCourseInstancesAsync();
    Task<CourseInstanceDto?> GetCourseInstanceByIdAsync(int id);
    Task<CourseInstanceDto> CreateCourseInstanceAsync(CreateCourseInstanceDto dto);
    Task UpdateCourseInstanceAsync(int id, CreateCourseInstanceDto dto);
    Task DeleteCourseInstanceAsync(int id);

    // Participants
    Task<IEnumerable<ParticipantDto>> GetAllParticipantsAsync();
    Task<ParticipantDto?> GetParticipantByIdAsync(int id);
    Task<ParticipantDto> CreateParticipantAsync(CreateParticipantDto dto);
    Task UpdateParticipantAsync(int id, CreateParticipantDto dto);
    Task DeleteParticipantAsync(int id);

    // Registrations
    Task<IEnumerable<CourseRegistrationDto>> GetAllRegistrationsAsync();
    Task<CourseRegistrationDto?> GetRegistrationByIdAsync(int id);
    Task<CourseRegistrationDto> CreateRegistrationAsync(CreateCourseRegistrationDto dto);
    Task DeleteRegistrationAsync(int id);
}
