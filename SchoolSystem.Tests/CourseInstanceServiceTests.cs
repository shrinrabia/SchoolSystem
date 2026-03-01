using Moq;
using SchoolSystem.Application.DTOs;
using SchoolSystem.Application.Services;
using SchoolSystem.Domain.Entities;
using SchoolSystem.Domain.Interfaces;

namespace SchoolSystem.Tests;

public class CourseInstanceServiceTests
{
    private readonly Mock<IUnitOfWork> _uow;
    private readonly Mock<IRepository<CourseInstance>> _instanceRepo;
    private readonly SchoolService _svc;

    public CourseInstanceServiceTests()
    {
        _uow = new Mock<IUnitOfWork>();
        _instanceRepo = new Mock<IRepository<CourseInstance>>();
        _uow.Setup(u => u.CourseInstances).Returns(_instanceRepo.Object);
        _svc = new SchoolService(_uow.Object);
    }

    [Fact]
    public async Task GetAllInstances_ReturnsExpectedCount()
    {
        var instances = new List<CourseInstance>
        {
            new CourseInstance
            {
                Id = 1,
                StartDate = new DateTime(2025, 9, 1),
                EndDate = new DateTime(2025, 12, 20),
                CourseId = 5
            },
            new CourseInstance
            {
                Id = 2,
                StartDate = new DateTime(2026, 2, 1),
                EndDate = new DateTime(2026, 6, 15),
                CourseId = 5
            }
        };
        _instanceRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(instances);

        var result = (await _svc.GetAllCourseInstancesAsync()).ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal(5, result[0].CourseId);
    }

    [Fact]
    public async Task GetInstanceById_ReturnsDto_WithCorrectDates()
    {
        var start = new DateTime(2025, 3, 15);
        var end = new DateTime(2025, 7, 10);
        var instance = new CourseInstance
        {
            Id = 8, StartDate = start, EndDate = end, CourseId = 12
        };
        _instanceRepo.Setup(r => r.GetByIdAsync(8)).ReturnsAsync(instance);

        var dto = await _svc.GetCourseInstanceByIdAsync(8);

        Assert.NotNull(dto);
        Assert.Equal(start, dto!.StartDate);
        Assert.Equal(end, dto.EndDate);
        Assert.Equal(12, dto.CourseId);
    }

    [Fact]
    public async Task GetInstanceById_ReturnsNull_ForUnknownId()
    {
        _instanceRepo.Setup(r => r.GetByIdAsync(333))
            .ReturnsAsync((CourseInstance?)null);

        Assert.Null(await _svc.GetCourseInstanceByIdAsync(333));
    }

    [Fact]
    public async Task CreateInstance_PersistsEntity()
    {
        var dto = new CreateCourseInstanceDto
        {
            StartDate = new DateTime(2026, 1, 10),
            EndDate = new DateTime(2026, 5, 30),
            CourseId = 3
        };

        var result = await _svc.CreateCourseInstanceAsync(dto);

        Assert.Equal(new DateTime(2026, 1, 10), result.StartDate);
        Assert.Equal(3, result.CourseId);
        _instanceRepo.Verify(r => r.AddAsync(It.IsAny<CourseInstance>()), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateInstance_ChangesFields_WhenFound()
    {
        var existing = new CourseInstance
        {
            Id = 4,
            StartDate = new DateTime(2025, 1, 1),
            EndDate = new DateTime(2025, 6, 1),
            CourseId = 2
        };
        _instanceRepo.Setup(r => r.GetByIdAsync(4)).ReturnsAsync(existing);

        var newStart = new DateTime(2025, 2, 1);
        var newEnd = new DateTime(2025, 7, 1);
        await _svc.UpdateCourseInstanceAsync(4, new CreateCourseInstanceDto
        {
            StartDate = newStart, EndDate = newEnd, CourseId = 9
        });

        Assert.Equal(newStart, existing.StartDate);
        Assert.Equal(newEnd, existing.EndDate);
        Assert.Equal(9, existing.CourseId);
        _uow.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateInstance_NoOp_WhenNotFound()
    {
        _instanceRepo.Setup(r => r.GetByIdAsync(500))
            .ReturnsAsync((CourseInstance?)null);

        await _svc.UpdateCourseInstanceAsync(500, new CreateCourseInstanceDto
        {
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddMonths(3),
            CourseId = 1
        });

        _instanceRepo.Verify(r => r.Update(It.IsAny<CourseInstance>()), Times.Never);
    }

    [Fact]
    public async Task DeleteInstance_Works_WhenExists()
    {
        var instance = new CourseInstance
        {
            Id = 11,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(90),
            CourseId = 7
        };
        _instanceRepo.Setup(r => r.GetByIdAsync(11)).ReturnsAsync(instance);

        await _svc.DeleteCourseInstanceAsync(11);

        _instanceRepo.Verify(r => r.Delete(instance), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteInstance_Skips_WhenNotFound()
    {
        _instanceRepo.Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((CourseInstance?)null);

        await _svc.DeleteCourseInstanceAsync(999);

        _instanceRepo.Verify(r => r.Delete(It.IsAny<CourseInstance>()), Times.Never);
    }
}
