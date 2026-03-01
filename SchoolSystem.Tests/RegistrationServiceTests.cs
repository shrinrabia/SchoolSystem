using Moq;
using SchoolSystem.Application.DTOs;
using SchoolSystem.Application.Services;
using SchoolSystem.Domain.Entities;
using SchoolSystem.Domain.Interfaces;

namespace SchoolSystem.Tests;

public class RegistrationServiceTests
{
    private readonly Mock<IUnitOfWork> _uow;
    private readonly Mock<IRepository<CourseRegistration>> _regRepo;
    private readonly SchoolService _svc;

    public RegistrationServiceTests()
    {
        _uow = new Mock<IUnitOfWork>();
        _regRepo = new Mock<IRepository<CourseRegistration>>();
        _uow.Setup(u => u.CourseRegistrations).Returns(_regRepo.Object);
        _svc = new SchoolService(_uow.Object);
    }

    [Fact]
    public async Task GetAllRegistrations_ReturnsMappedDtos()
    {
        var regs = new List<CourseRegistration>
        {
            new CourseRegistration
            {
                Id = 1,
                ParticipantId = 10,
                CourseInstanceId = 20,
                RegistrationDate = new DateTime(2025, 8, 15)
            },
            new CourseRegistration
            {
                Id = 2,
                ParticipantId = 11,
                CourseInstanceId = 20,
                RegistrationDate = new DateTime(2025, 8, 16)
            }
        };
        _regRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(regs);

        var result = (await _svc.GetAllRegistrationsAsync()).ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal(10, result[0].ParticipantId);
        Assert.Equal(20, result[1].CourseInstanceId);
    }

    [Fact]
    public async Task GetRegistrationById_ReturnsExpectedDto()
    {
        var reg = new CourseRegistration
        {
            Id = 7,
            ParticipantId = 3,
            CourseInstanceId = 14,
            RegistrationDate = new DateTime(2025, 11, 1)
        };
        _regRepo.Setup(r => r.GetByIdAsync(7)).ReturnsAsync(reg);

        var dto = await _svc.GetRegistrationByIdAsync(7);

        Assert.NotNull(dto);
        Assert.Equal(3, dto!.ParticipantId);
        Assert.Equal(14, dto.CourseInstanceId);
        Assert.Equal(new DateTime(2025, 11, 1), dto.RegistrationDate);
    }

    [Fact]
    public async Task GetRegistrationById_ReturnsNull_IfNotFound()
    {
        _regRepo.Setup(r => r.GetByIdAsync(55))
            .ReturnsAsync((CourseRegistration?)null);

        Assert.Null(await _svc.GetRegistrationByIdAsync(55));
    }

    [Fact]
    public async Task CreateRegistration_SetsDateAutomatically()
    {
        var before = DateTime.UtcNow;

        var dto = new CreateCourseRegistrationDto
        {
            ParticipantId = 5,
            CourseInstanceId = 8
        };

        var result = await _svc.CreateRegistrationAsync(dto);

        // the service uses DateTime.UtcNow internally
        Assert.Equal(5, result.ParticipantId);
        Assert.Equal(8, result.CourseInstanceId);
        Assert.True(result.RegistrationDate >= before,
            "Registration date should be set to roughly the current UTC time");
        _regRepo.Verify(r => r.AddAsync(It.IsAny<CourseRegistration>()), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteRegistration_RemovesAndSaves_WhenFound()
    {
        var reg = new CourseRegistration
        {
            Id = 15, ParticipantId = 1, CourseInstanceId = 2,
            RegistrationDate = DateTime.Today
        };
        _regRepo.Setup(r => r.GetByIdAsync(15)).ReturnsAsync(reg);

        await _svc.DeleteRegistrationAsync(15);

        _regRepo.Verify(r => r.Delete(reg), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteRegistration_DoesNothing_WhenMissing()
    {
        _regRepo.Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((CourseRegistration?)null);

        await _svc.DeleteRegistrationAsync(999);

        _regRepo.Verify(r => r.Delete(It.IsAny<CourseRegistration>()), Times.Never);
        _uow.Verify(u => u.SaveChangesAsync(), Times.Never);
    }
}
