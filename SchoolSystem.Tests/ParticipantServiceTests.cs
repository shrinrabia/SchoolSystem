using Moq;
using SchoolSystem.Application.DTOs;
using SchoolSystem.Application.Services;
using SchoolSystem.Domain.Entities;
using SchoolSystem.Domain.Interfaces;

namespace SchoolSystem.Tests;

public class ParticipantServiceTests
{
    private readonly Mock<IUnitOfWork> _uow;
    private readonly Mock<IRepository<Participant>> _participantRepo;
    private readonly SchoolService _svc;

    public ParticipantServiceTests()
    {
        _uow = new Mock<IUnitOfWork>();
        _participantRepo = new Mock<IRepository<Participant>>();
        _uow.Setup(u => u.Participants).Returns(_participantRepo.Object);
        _svc = new SchoolService(_uow.Object);
    }

    [Fact]
    public async Task GetAllParticipants_ReturnsEmpty_WhenNoneExist()
    {
        _participantRepo.Setup(r => r.GetAllAsync())
            .ReturnsAsync(Enumerable.Empty<Participant>());

        var result = await _svc.GetAllParticipantsAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllParticipants_MapsAllFields()
    {
        var participants = new List<Participant>
        {
            new Participant { Id = 1, Name = "Fatima", Email = "fatima@edu.nl" },
            new Participant { Id = 2, Name = "Jan", Email = "jan@edu.nl" },
            new Participant { Id = 3, Name = "Mei", Email = "mei@edu.nl" }
        };
        _participantRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(participants);

        var result = (await _svc.GetAllParticipantsAsync()).ToList();

        Assert.Equal(3, result.Count);
        Assert.Equal("fatima@edu.nl", result[0].Email);
        Assert.Equal("Jan", result[1].Name);
    }

    [Fact]
    public async Task GetParticipantById_ReturnsCorrectDto()
    {
        var p = new Participant { Id = 10, Name = "Ravi", Email = "ravi@school.com" };
        _participantRepo.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(p);

        var dto = await _svc.GetParticipantByIdAsync(10);

        Assert.NotNull(dto);
        Assert.Equal("Ravi", dto!.Name);
    }

    [Fact]
    public async Task GetParticipantById_ReturnsNull_WhenMissing()
    {
        _participantRepo.Setup(r => r.GetByIdAsync(200))
            .ReturnsAsync((Participant?)null);

        Assert.Null(await _svc.GetParticipantByIdAsync(200));
    }

    [Fact]
    public async Task CreateParticipant_StoresAndReturnsDto()
    {
        var dto = new CreateParticipantDto { Name = "Sarah", Email = "sarah@school.com" };

        var result = await _svc.CreateParticipantAsync(dto);

        Assert.Equal("Sarah", result.Name);
        Assert.Equal("sarah@school.com", result.Email);
        _participantRepo.Verify(r => r.AddAsync(It.Is<Participant>(
            p => p.Name == "Sarah" && p.Email == "sarah@school.com")), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateParticipant_UpdatesWhenFound()
    {
        var existing = new Participant { Id = 4, Name = "Old", Email = "old@test.com" };
        _participantRepo.Setup(r => r.GetByIdAsync(4)).ReturnsAsync(existing);

        await _svc.UpdateParticipantAsync(4, new CreateParticipantDto
        {
            Name = "Updated",
            Email = "updated@test.com"
        });

        Assert.Equal("Updated", existing.Name);
        Assert.Equal("updated@test.com", existing.Email);
        _uow.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateParticipant_DoesNothing_WhenNotFound()
    {
        _participantRepo.Setup(r => r.GetByIdAsync(123))
            .ReturnsAsync((Participant?)null);

        await _svc.UpdateParticipantAsync(123, new CreateParticipantDto
        {
            Name = "Ghost",
            Email = "ghost@void.com"
        });

        _participantRepo.Verify(r => r.Update(It.IsAny<Participant>()), Times.Never);
    }

    [Fact]
    public async Task DeleteParticipant_DeletesAndSaves()
    {
        var p = new Participant { Id = 9, Name = "ToRemove", Email = "rm@mail.com" };
        _participantRepo.Setup(r => r.GetByIdAsync(9)).ReturnsAsync(p);

        await _svc.DeleteParticipantAsync(9);

        _participantRepo.Verify(r => r.Delete(p), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteParticipant_NoSave_WhenMissing()
    {
        _participantRepo.Setup(r => r.GetByIdAsync(404))
            .ReturnsAsync((Participant?)null);

        await _svc.DeleteParticipantAsync(404);

        _uow.Verify(u => u.SaveChangesAsync(), Times.Never);
    }
}
