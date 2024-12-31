using Application.Services.Product.Command.Create;
using AutoMapper;
using Domain.Models;
using FluentValidation.Results;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Moq;

public class CreateProductHandlerTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateProductHandler _handler;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessor;
    public CreateProductHandlerTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _mapperMock = new Mock<IMapper>();
        _httpContextAccessor = new Mock<IHttpContextAccessor>();
        _handler = new CreateProductHandler(_productRepositoryMock.Object, _mapperMock.Object, _httpContextAccessor.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsProductId()
    {
        // Arrange
        CreateProduct createProductRequest = new()
        {
            // Initialize your CreateProduct request here, for example:
            Name = "Test Product",
            Price = 10.0m
        };
        Products product = new()
        {
            Id = 1,
            Name = createProductRequest.Name,
            Price = createProductRequest.Price
        };

        // Mock the IMapper to return the mapped Products object
        _mapperMock.Setup(m => m.Map<Products>(It.IsAny<CreateProduct>())).Returns(product);

        // Mock validation to succeed
        Mock<CreateValidator> validatorMock = new();
        validatorMock.Setup(v => v.ValidateAsync(createProductRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        // Mock the IProductRepository to return the product with an Id after adding it
        _productRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Products>()))
            .ReturnsAsync(product);

        // Act
        long result = await _handler.Handle(createProductRequest, CancellationToken.None);

        // Assert
        Assert.Equal(1, result); // Ensure the returned ID matches the mock data
        _productRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Products>()), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidRequest_ThrowsException()
    {
        // Arrange
        CreateProduct createProductRequest = new()
        {
            // Initialize invalid request, for example:
            Name = "", // Invalid name
            Price = -10.0m // Invalid price
        };

        // Mock validation to return errors
        ValidationResult validationResult = new(new[] { new ValidationFailure("Name", "Name is required.") });
        Mock<CreateValidator> validatorMock = new();
        validatorMock.Setup(v => v.ValidateAsync(createProductRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        // Act & Assert
        Exception exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(createProductRequest, CancellationToken.None));
        Assert.Equal("Product is not valid", exception.Message);
    }
}
