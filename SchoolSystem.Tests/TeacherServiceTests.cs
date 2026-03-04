using Moq;
using Microsoft.Extensions.Caching.Memory;
using SchoolSystem.Application.DTOs;
using SchoolSystem.Application.Services;
using SchoolSystem.Domain.Entities;
using SchoolSystem.Domain.Interfaces;

namespace SchoolSystem.Tests;

public class TeacherServiceTests
{
    private readonly Mock<IUnitOfWork> _uow;
    private readonly Mock<IRepository<Teacher>> _teacherRepo;
    private readonly SchoolService _svc;

    public TeacherServiceTests()
    {
        _uow = new Mock<IUnitOfWork>();
        _teacherRepo = new Mock<IRepository<Teacher>>();
        _uow.Setup(u => u.Teachers).Returns(_teacherRepo.Object);
        _svc = new SchoolService(_uow.Object, new MemoryCache(new MemoryCacheOptions()));
    }

    [Fact]
    public async Task GetAllTeachers_ReturnsEmpty_WhenNoTeachersExist()
    {
        _teacherRepo.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<Teacher>());

        var result = await _svc.GetAllTeachersAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllTeachers_ReturnsMappedDtos()
    {
        var teachers = new List<Teacher>
        {
            new Teacher { Id = 1, Name = "Erik", Email = "erik@school.nl" },
            new Teacher { Id = 2, Name = "Sophie", Email = "sophie@school.nl" }
        };
        _teacherRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(teachers);

        var result = (await _svc.GetAllTeachersAsync()).ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal("Erik", result[0].Name);
        Assert.Equal("sophie@school.nl", result[1].Email);
    }

    [Fact]
    public async Task GetTeacherById_ReturnsNull_WhenNotFound()
    {
        _teacherRepo.Setup(r => r.GetByIdAsync(99))
            .ReturnsAsync((Teacher?)null);

        var result = await _svc.GetTeacherByIdAsync(99);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetTeacherById_ReturnsDto_WhenFound()
    {
        var teacher = new Teacher { Id = 5, Name = "Lars", Email = "lars@edu.com" };
        _teacherRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(teacher);

        var result = await _svc.GetTeacherByIdAsync(5);

        Assert.NotNull(result);
        Assert.Equal(5, result!.Id);
        Assert.Equal("Lars", result.Name);
    }

    [Fact]
    public async Task UpdateTeacher_CallsUpdateAndSave_WhenTeacherExists()
    {
        var existing = new Teacher { Id = 3, Name = "Old Name", Email = "old@mail.com" };
        _teacherRepo.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(existing);

        var dto = new CreateTeacherDto { Name = "New Name", Email = "new@mail.com" };
        await _svc.UpdateTeacherAsync(3, dto);

        Assert.Equal("New Name", existing.Name);
        Assert.Equal("new@mail.com", existing.Email);
        _teacherRepo.Verify(r => r.Update(existing), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateTeacher_DoesNothing_WhenTeacherMissing()
    {
        _teacherRepo.Setup(r => r.GetByIdAsync(404)).ReturnsAsync((Teacher?)null);

        await _svc.UpdateTeacherAsync(404, new CreateTeacherDto { Name = "X", Email = "x@y.com" });

        _teacherRepo.Verify(r => r.Update(It.IsAny<Teacher>()), Times.Never);
        _uow.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteTeacher_RemovesAndSaves_WhenFound()
    {
        var teacher = new Teacher { Id = 7, Name = "Deleted", Email = "del@mail.com" };
        _teacherRepo.Setup(r => r.GetByIdAsync(7)).ReturnsAsync(teacher);

        await _svc.DeleteTeacherAsync(7);

        _teacherRepo.Verify(r => r.Delete(teacher), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteTeacher_SkipsSave_WhenNotFound()
    {
        _teacherRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Teacher?)null);

        await _svc.DeleteTeacherAsync(999);

        _teacherRepo.Verify(r => r.Delete(It.IsAny<Teacher>()), Times.Never);
        _uow.Verify(u => u.SaveChangesAsync(), Times.Never);
    }
}
