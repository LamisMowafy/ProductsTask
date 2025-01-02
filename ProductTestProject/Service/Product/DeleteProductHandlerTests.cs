using Moq;
using Xunit;
using Microsoft.AspNetCore.Http;
using MediatR;
using System.Threading.Tasks;
using System.Threading;
using System.Security.Claims;
using System.Net;
using Domain.Models;
using Infrastructure.Interfaces;
using Application.Services.Product.Command.Delete;
using Resource;
using System;

namespace Application.Tests
{
    public class DeleteProductHandlerTests
    {
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly Mock<IResourceHelper> _mockResourceHelper;
        private readonly DeleteProductHandler _handler;

        public DeleteProductHandlerTests()
        {
            _mockProductRepository = new Mock<IProductRepository>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockResourceHelper = new Mock<IResourceHelper>();

            _handler = new DeleteProductHandler(
                _mockProductRepository.Object,
                _mockHttpContextAccessor.Object,
                _mockResourceHelper.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnUnauthorized_WhenUserIsNotAdmin()
        {
            // Arrange
            var deleteProductRequest = new DeleteProduct { ProductId = 1 };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "testuser")
            }));

            _mockHttpContextAccessor.Setup(x => x.HttpContext.User).Returns(claimsPrincipal);

            // Act
            var result = await _handler.Handle(deleteProductRequest, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, result.statusCode);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var deleteProductRequest = new DeleteProduct { ProductId = 1 };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim(ClaimTypes.Role, "Admin")
            }));

            _mockHttpContextAccessor.Setup(x => x.HttpContext.User).Returns(claimsPrincipal);

            // Simulate that the product does not exist in the repository
            _mockProductRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<long>())).ReturnsAsync((Products)null);
            _mockResourceHelper.Setup(x => x.Product("ERRORMESSAGE_PRODUCT_NOT_EXIST")).Returns("Product not found");

            // Act
            var result = await _handler.Handle(deleteProductRequest, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, result.statusCode);
            Assert.Equal("Product not found", result.message);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenProductIsDeleted()
        {
            // Arrange
            var deleteProductRequest = new DeleteProduct { ProductId = 1 };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim(ClaimTypes.Role, "Admin")
            }));

            _mockHttpContextAccessor.Setup(x => x.HttpContext.User).Returns(claimsPrincipal);
            var product = new Products { Id = 1 };
            _mockProductRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<long>())).ReturnsAsync(product);
            _mockProductRepository.Setup(repo => repo.DeleteAsync(It.IsAny<Products>())).Returns(Task.CompletedTask);
            _mockResourceHelper.Setup(x => x.Product("SUCCESSMESSAGE_PRODUCT_DELETED")).Returns("Product deleted successfully");

            // Act
            var result = await _handler.Handle(deleteProductRequest, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.statusCode);
            Assert.Equal("Product deleted successfully", result.message);
        }

        [Fact]
        public async Task Handle_ShouldReturnInternalError_WhenExceptionOccurs()
        {
            // Arrange
            var deleteProductRequest = new DeleteProduct { ProductId = 1 };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim(ClaimTypes.Role, "Admin")
            }));

            _mockHttpContextAccessor.Setup(x => x.HttpContext.User).Returns(claimsPrincipal);
            _mockProductRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<long>())).Throws(new Exception("Internal error"));
            _mockResourceHelper.Setup(x => x.Shared("ERROREMESSAGE_INTERNAL")).Returns("Internal server error");

            // Act
            var result = await _handler.Handle(deleteProductRequest, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, result.statusCode);
            Assert.Equal("Internal server error", result.message);
        }
    }
}
