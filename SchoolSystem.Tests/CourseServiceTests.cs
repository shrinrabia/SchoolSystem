using Moq;
using Microsoft.Extensions.Caching.Memory;
using SchoolSystem.Application.DTOs;
using SchoolSystem.Application.Services;
using SchoolSystem.Domain.Entities;
using SchoolSystem.Domain.Interfaces;

namespace SchoolSystem.Tests;

public class CourseServiceTests
{
    private readonly Mock<IUnitOfWork> _uow;
    private readonly Mock<IRepository<Course>> _courseRepo;
    private readonly SchoolService _svc;

    public CourseServiceTests()
    {
        _uow = new Mock<IUnitOfWork>();
        _courseRepo = new Mock<IRepository<Course>>();
        _uow.Setup(u => u.Courses).Returns(_courseRepo.Object);
        _svc = new SchoolService(_uow.Object, new MemoryCache(new MemoryCacheOptions()));
    }

    [Fact]
    public async Task GetAllCourses_ReturnsList()
    {
        var courses = new List<Course>
        {
            new Course { Id = 1, Name = "Mathematics", Description = "Algebra & calculus", TeacherId = 10 },
            new Course { Id = 2, Name = "Physics", Description = "Mechanics", TeacherId = 10 }
        };
        _courseRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(courses);

        var result = (await _svc.GetAllCoursesAsync()).ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal("Mathematics", result[0].Name);
    }

    [Fact]
    public async Task GetCourseById_ReturnsNull_ForMissingCourse()
    {
        _courseRepo.Setup(r => r.GetByIdAsync(42)).ReturnsAsync((Course?)null);

        var dto = await _svc.GetCourseByIdAsync(42);

        Assert.Null(dto);
    }

    [Fact]
    public async Task GetCourseById_MapsDtoCorrectly()
    {
        var course = new Course
        {
            Id = 3,
            Name = "History",
            Description = "Modern European history",
            TeacherId = 8
        };
        _courseRepo.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(course);

        var dto = await _svc.GetCourseByIdAsync(3);

        Assert.NotNull(dto);
        Assert.Equal("History", dto!.Name);
        Assert.Equal("Modern European history", dto.Description);
        Assert.Equal(8, dto.TeacherId);
    }

    [Fact]
    public async Task CreateCourse_PersistsAndReturnsDto()
    {
        var input = new CreateCourseDto
        {
            Name = "Biology",
            Description = "Cell biology and genetics",
            TeacherId = 4
        };

        var result = await _svc.CreateCourseAsync(input);

        Assert.Equal("Biology", result.Name);
        Assert.Equal("Cell biology and genetics", result.Description);
        Assert.Equal(4, result.TeacherId);
        _courseRepo.Verify(r => r.AddAsync(It.Is<Course>(c => c.Name == "Biology")), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateCourse_ModifiesFields_WhenCourseExists()
    {
        var existing = new Course
        {
            Id = 5, Name = "Old Course", Description = "Old desc", TeacherId = 1
        };
        _courseRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(existing);

        await _svc.UpdateCourseAsync(5, new CreateCourseDto
        {
            Name = "Updated Course",
            Description = "New description",
            TeacherId = 2
        });

        Assert.Equal("Updated Course", existing.Name);
        Assert.Equal("New description", existing.Description);
        Assert.Equal(2, existing.TeacherId);
        _uow.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateCourse_NoOp_WhenCourseMissing()
    {
        _courseRepo.Setup(r => r.GetByIdAsync(77)).ReturnsAsync((Course?)null);

        await _svc.UpdateCourseAsync(77, new CreateCourseDto
        {
            Name = "Whatever", Description = "desc", TeacherId = 1
        });

        _courseRepo.Verify(r => r.Update(It.IsAny<Course>()), Times.Never);
    }

    [Fact]
    public async Task DeleteCourse_RemovesCourse_WhenItExists()
    {
        var course = new Course { Id = 6, Name = "To Delete", Description = "", TeacherId = 1 };
        _courseRepo.Setup(r => r.GetByIdAsync(6)).ReturnsAsync(course);

        await _svc.DeleteCourseAsync(6);

        _courseRepo.Verify(r => r.Delete(course), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteCourse_SkipsDeletion_WhenNotFound()
    {
        _courseRepo.Setup(r => r.GetByIdAsync(88)).ReturnsAsync((Course?)null);

        await _svc.DeleteCourseAsync(88);

        _courseRepo.Verify(r => r.Delete(It.IsAny<Course>()), Times.Never);
    }
}
