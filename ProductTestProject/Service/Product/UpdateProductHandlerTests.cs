using Application.Services.Product.Command.Update;
using AutoMapper;
using Domain.Models;
using Infrastructure.Interfaces;
using MediatR;
using Moq;

public class UpdateProductHandlerTests
{
    private readonly Mock<IRepository<Products>> _productRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UpdateProductHandler _handler;

    public UpdateProductHandlerTests()
    {
        _productRepositoryMock = new Mock<IRepository<Products>>();
        _mapperMock = new Mock<IMapper>();
        _handler = new UpdateProductHandler(_productRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_UpdatesProduct()
    {
        // Arrange
        UpdateProduct updateProductRequest = new()
        {
            Id = 1,
            Name = "Updated Product Name",
            Price = 20.0m
        };

        Products product = new()
        {
            Id = 1,
            Name = "Old Product Name",
            Price = 10.0m
        };

        // Mock the IMapper to return the mapped Products object
        _mapperMock.Setup(m => m.Map<Products>(It.IsAny<UpdateProduct>())).Returns(product);

        // Mock the repository to simulate updating the product
        _productRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Products>()))
            .Returns(Task.CompletedTask);

        // Act
        Unit result = await _handler.Handle(updateProductRequest, CancellationToken.None);

        // Assert
        Assert.Equal(Unit.Value, result); // Ensure the return value is Unit.Value
        _productRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Products>()), Times.Once); // Ensure UpdateAsync was called once
    }

    [Fact]
    public async Task Handle_ProductNotFound_ThrowsException()
    {
        // Arrange
        UpdateProduct updateProductRequest = new()
        {
            Id = 999, // Non-existent ProductId
            Name = "Updated Product",
            Price = 25.0m
        };

        // Mock the IMapper to map to a product (even though it's non-existent)
        Products product = new()
        {
            Id = 999,
            Name = updateProductRequest.Name,
            Price = updateProductRequest.Price
        };

        // Simulate no matching product by having the repository return null or not updating anything
        _productRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Products>()))
            .ThrowsAsync(new Exception("Product not found"));

        // Act & Assert
        Exception exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(updateProductRequest, CancellationToken.None));
        Assert.Equal("Product not found", exception.Message); // You can customize the exception message based on your handler logic
    }
}
