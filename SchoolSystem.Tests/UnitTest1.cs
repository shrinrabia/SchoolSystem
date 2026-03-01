using Moq;
using SchoolSystem.Application.DTOs;
using SchoolSystem.Application.Services;
using SchoolSystem.Domain.Entities;
using SchoolSystem.Domain.Interfaces;
using System.Threading.Tasks;

namespace SchoolSystem.Tests;

public class SchoolTests
{
    [Fact]
    public async Task CreateTeacherAsync_ShouldAddTeacher_WhenValidDataProvided()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockRepo = new Mock<IRepository<Teacher>>();

        mockUnitOfWork.Setup(u => u.Teachers).Returns(mockRepo.Object);

        var service = new SchoolService(mockUnitOfWork.Object);
        var createDto = new CreateTeacherDto { Name = "Anna", Email = "anna@school.com" };

        // Act
        var result = await service.CreateTeacherAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Anna", result.Name);
        Assert.Equal("anna@school.com", result.Email);
        
        mockRepo.Verify(r => r.AddAsync(It.IsAny<Teacher>()), Times.Once);
        mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}
