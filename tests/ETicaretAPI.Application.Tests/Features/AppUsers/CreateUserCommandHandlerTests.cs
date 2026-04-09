using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Application.DTOs.User;
using ETicaretAPI.Application.Features.Commands.AppUser.CreateUser;
using FluentAssertions;
using Moq;
using Xunit;

namespace ETicaretAPI.Application.Tests.Features.AppUsers;

public class CreateUserCommandHandlerTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly CreateUserCommandHandler _sut;

    public CreateUserCommandHandlerTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _sut = new CreateUserCommandHandler(_userServiceMock.Object);
    }

    [Fact]
    public async Task Handle_WhenUserServiceSucceeds_MapsAllFieldsAndReturnsSuccessResponse()
    {
        // Arrange
        var request = new CreateUserCommandRequest
        {
            Username = "johndoe",
            NameSurname = "John Doe",
            Email = "john@example.com",
            Password = "Strong!Pass123",
            PasswordConfirm = "Strong!Pass123"
        };

        _userServiceMock
            .Setup(s => s.CreateAsync(It.IsAny<CreateUser>()))
            .ReturnsAsync(new CreateUserResponse
            {
                Succeeded = true,
                Message = "Kullanıcı başarıyla oluşturulmuştur"
            });

        // Act
        var response = await _sut.Handle(request, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Succeeded.Should().BeTrue();
        response.Message.Should().Be("Kullanıcı başarıyla oluşturulmuştur");

        _userServiceMock.Verify(
            s => s.CreateAsync(It.Is<CreateUser>(u =>
                u.Username == request.Username &&
                u.Email == request.Email &&
                u.NameSurname == request.NameSurname &&
                u.Password == request.Password &&
                u.PasswordConfirm == request.PasswordConfirm)),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenUserServiceFails_PropagatesFailureResponse()
    {
        // Arrange
        var request = new CreateUserCommandRequest
        {
            Username = "duplicate",
            Email = "duplicate@example.com",
            NameSurname = "Dup User",
            Password = "Strong!Pass123",
            PasswordConfirm = "Strong!Pass123"
        };

        _userServiceMock
            .Setup(s => s.CreateAsync(It.IsAny<CreateUser>()))
            .ReturnsAsync(new CreateUserResponse
            {
                Succeeded = false,
                Message = "DuplicateUserName - Kullanıcı zaten var"
            });

        // Act
        var response = await _sut.Handle(request, CancellationToken.None);

        // Assert
        response.Succeeded.Should().BeFalse();
        response.Message.Should().Contain("DuplicateUserName");
    }
}
