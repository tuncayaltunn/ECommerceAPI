using ETicaretAPI.Application.Abstractions.Hubs;
using ETicaretAPI.Application.Features.Commands.Product.CreateProduct;
using ETicaretAPI.Application.Repositories.IProductRepositories;
using ETicaretAPI.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace ETicaretAPI.Application.Tests.Features.Products;

public class CreateProductCommandHandlerTests
{
    private readonly Mock<IProductWriteRepository> _productWriteRepositoryMock;
    private readonly Mock<IProductHubService> _productHubServiceMock;
    private readonly CreateProductCommandHandler _sut;

    public CreateProductCommandHandlerTests()
    {
        _productWriteRepositoryMock = new Mock<IProductWriteRepository>();
        _productHubServiceMock = new Mock<IProductHubService>();

        _productWriteRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Product>()))
            .ReturnsAsync(true);

        _productWriteRepositoryMock
            .Setup(r => r.SaveAsync())
            .ReturnsAsync(1);

        _sut = new CreateProductCommandHandler(
            _productWriteRepositoryMock.Object,
            _productHubServiceMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidRequest_PersistsProductAndReturnsResponse()
    {
        // Arrange
        var request = new CreateProductCommandRequest
        {
            Name = "Test Product",
            Stock = 10,
            Price = 99.90f
        };

        // Act
        var response = await _sut.Handle(request, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();

        _productWriteRepositoryMock.Verify(
            r => r.AddAsync(It.Is<Product>(p =>
                p.Name == request.Name &&
                p.Stock == request.Stock &&
                p.Price == request.Price)),
            Times.Once);

        _productWriteRepositoryMock.Verify(r => r.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_WithValidRequest_BroadcastsProductAddedNotification()
    {
        // Arrange
        var request = new CreateProductCommandRequest
        {
            Name = "Notebook",
            Stock = 5,
            Price = 1500f
        };

        // Act
        await _sut.Handle(request, CancellationToken.None);

        // Assert
        _productHubServiceMock.Verify(
            h => h.ProductAddedMessageAsync(It.Is<string>(msg => msg.Contains("Notebook"))),
            Times.Once);
    }

    [Fact]
    public async Task Handle_AlwaysSavesAfterAdd_NotInReverseOrder()
    {
        // Arrange
        var sequence = new MockSequence();
        var orderedRepo = new Mock<IProductWriteRepository>(MockBehavior.Strict);

        orderedRepo.InSequence(sequence)
            .Setup(r => r.AddAsync(It.IsAny<Product>()))
            .ReturnsAsync(true);

        orderedRepo.InSequence(sequence)
            .Setup(r => r.SaveAsync())
            .ReturnsAsync(1);

        var hub = new Mock<IProductHubService>();
        hub.Setup(h => h.ProductAddedMessageAsync(It.IsAny<string>()))
           .Returns(Task.CompletedTask);

        var sut = new CreateProductCommandHandler(orderedRepo.Object, hub.Object);

        var request = new CreateProductCommandRequest
        {
            Name = "Order Sensitive",
            Stock = 1,
            Price = 1f
        };

        // Act
        var act = async () => await sut.Handle(request, CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
    }
}
