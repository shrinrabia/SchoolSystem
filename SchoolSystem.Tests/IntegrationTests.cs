using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SchoolSystem.Application.DTOs;
using SchoolSystem.Application.Services;
using SchoolSystem.Domain.Entities;
using SchoolSystem.Infrastructure.Data;

namespace SchoolSystem.Tests;

public class IntegrationTests
{
    private AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateTeacher_ThenGetAll_ReturnsCreatedTeacher()
    {
        var context = CreateContext();
        var uow = new UnitOfWork(context);
        var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new SchoolService(uow, cache);

        await service.CreateTeacherAsync(new CreateTeacherDto { Name = "Test Teacher", Email = "test@test.com" });

        var teachers = (await service.GetAllTeachersAsync()).ToList();
        Assert.Single(teachers);
        Assert.Equal("Test Teacher", teachers[0].Name);
    }

    [Fact]
    public async Task CreateCourse_AndDelete_CourseIsRemoved()
    {
        var context = CreateContext();
        var uow = new UnitOfWork(context);
        var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new SchoolService(uow, cache);

        var teacher = await service.CreateTeacherAsync(new CreateTeacherDto { Name = "Lars", Email = "lars@mail.com" });

        var course = await service.CreateCourseAsync(new CreateCourseDto
        {
            Name = "C# Basics",
            Description = "Intro course",
            TeacherId = teacher.Id
        });

        await service.DeleteCourseAsync(course.Id);

        var courses = (await service.GetAllCoursesAsync()).ToList();
        Assert.Empty(courses);
    }

    [Fact]
    public async Task CreateRegistration_LinksParticipantToCourseInstance()
    {
        var context = CreateContext();
        var uow = new UnitOfWork(context);
        var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new SchoolService(uow, cache);

        var teacher = await service.CreateTeacherAsync(new CreateTeacherDto { Name = "Anna", Email = "anna@edu.com" });
        var course = await service.CreateCourseAsync(new CreateCourseDto
        {
            Name = "Databases",
            Description = "SQL and EF Core",
            TeacherId = teacher.Id
        });
        var instance = await service.CreateCourseInstanceAsync(new CreateCourseInstanceDto
        {
            CourseId = course.Id,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddMonths(3)
        });
        var participant = await service.CreateParticipantAsync(new CreateParticipantDto
        {
            Name = "Erik",
            Email = "erik@student.com"
        });

        var reg = await service.CreateRegistrationAsync(new CreateCourseRegistrationDto
        {
            ParticipantId = participant.Id,
            CourseInstanceId = instance.Id
        });

        Assert.Equal(participant.Id, reg.ParticipantId);
        Assert.Equal(instance.Id, reg.CourseInstanceId);

        var allRegs = (await service.GetAllRegistrationsAsync()).ToList();
        Assert.Single(allRegs);
    }
}
