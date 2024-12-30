namespace ProductTestProject.Service.Product
{
    using Application.Services.Product.Command.Delete;
    using Domain.Models;
    using Infrastructure.Interfaces;
    using MediatR;
    using Moq;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;

    public class DeleteProductHandlerTests
    {
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly DeleteProductHandler _handler;

        public DeleteProductHandlerTests()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _handler = new DeleteProductHandler(_productRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_DeletesProduct()
        {
            // Arrange
            DeleteProduct deleteProductRequest = new()
            {
                ProductId = 1 // ProductId to delete
            };

            Products product = new()
            {
                Id = 1,
                Name = "Test Product",
                Price = 10.0m
            };

            // Mock the repository to return a product when GetByIdAsync is called
            _productRepositoryMock.Setup(r => r.GetByIdAsync(deleteProductRequest.ProductId))
                .ReturnsAsync(product);

            // Mock the DeleteAsync to not do anything but to confirm it was called
            _productRepositoryMock.Setup(r => r.DeleteAsync(It.IsAny<Products>()))
                .Returns(Task.CompletedTask);

            // Act
            Unit result = await _handler.Handle(deleteProductRequest, CancellationToken.None);

            // Assert
            Assert.Equal(Unit.Value, result); // Ensure that the return value is Unit.Value (indicating no result)
            _productRepositoryMock.Verify(r => r.GetByIdAsync(deleteProductRequest.ProductId), Times.Once); // Ensure GetByIdAsync was called once
            _productRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Products>()), Times.Once); // Ensure DeleteAsync was called once
        }

        [Fact]
        public async Task Handle_ProductNotFound_ThrowsException()
        {
            // Arrange
            DeleteProduct deleteProductRequest = new()
            {
                ProductId = 999 // Non-existent ProductId
            };

            // Mock the repository to return null when no product is found
            _productRepositoryMock.Setup(r => r.GetByIdAsync(deleteProductRequest.ProductId))
                .ReturnsAsync((Products)null); // Simulating product not found

            // Act & Assert
            Exception exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(deleteProductRequest, CancellationToken.None));
            Assert.Equal("Product not found", exception.Message); // You can customize the exception message based on your handler logic
        }
    }

}
