using Application.Services.Product.Command.Delete;
using Domain.Enums;
using Infrastructure.Common;
using Infrastructure.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ProductTestProject.Service.Promotion
{
    public class DeletePromotionHandlerTests
    {
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly Mock<IResourceHelper> _resourceHelperMock;
        private readonly DeleteProductHandler _handler;

        public DeletePromotionHandlerTests()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _resourceHelperMock = new Mock<IResourceHelper>();

            _handler = new DeleteProductHandler(
                _productRepositoryMock.Object,
                _httpContextAccessorMock.Object,
                _resourceHelperMock.Object
            );
        }

        [Fact]
        public async Task Handle_ReturnsSuccessResponse_WhenProductDeleted()
        {
            // Arrange
            var deleteProductCommand = new DeleteProduct { ProductId = 1 };
            var product = new Domain.Models.Products { Id = 1, Name = "Test Product" };

            // Mocking the HTTP context for an Admin user
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.Role, "Admin")
            }));
            // Mocking the repository behavior
            _productRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<long>())).ReturnsAsync(product);
            _productRepositoryMock.Setup(r => r.DeleteAsync(It.IsAny<Domain.Models.Products>())).Returns(Task.CompletedTask);

            // Mocking the ResourceHelper behavior
            _resourceHelperMock.Setup(r => r.Product(It.IsAny<string>())).Returns("Product deleted successfully");

            // Act
            var result = await _handler.Handle(deleteProductCommand, CancellationToken.None);

            // Assert
            var response = Assert.IsType<ServiceResponse<Unit>>(result);
            Assert.Equal(HttpStatusCode.OK, response.statusCode);  // Verify the status code is OK
            Assert.Equal(ResponseStatus.SUCCESS, response.status);  // Verify the response status is success
        }
        [Fact]
        public async Task Handle_ReturnsUnauthorizedResponse_WhenUserIsNotAdmin()
        {
            // Arrange
            var deleteProductCommand = new DeleteProduct { ProductId = 1 };

            // Mocking the HTTP context for a non-Admin user
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.Role, "User")  // Non-Admin role
            })); 
            // Act
            var result = await _handler.Handle(deleteProductCommand, CancellationToken.None);

            // Assert
            var response = Assert.IsType<ServiceResponse<Unit>>(result);
            Assert.Equal(HttpStatusCode.Unauthorized, response.statusCode);  // Verify the status code is Unauthorized
            Assert.Equal(ResponseStatus.NOT_ALLOWED, response.status);  // Verify the response status is NOT_ALLOWED
        }
        [Fact]
        public async Task Handle_ReturnsNotFoundResponse_WhenProductDoesNotExist()
        {
            // Arrange
            var deleteProductCommand = new DeleteProduct { ProductId = 1 };

            // Mocking the HTTP context for an Admin user
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.Role, "Admin")
            })); 
            // Mocking the repository to return null (Product not found)
            _productRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<long>())).ReturnsAsync((Domain.Models.Products)null);

            // Mocking the ResourceHelper behavior
            _resourceHelperMock.Setup(r => r.Product(It.IsAny<string>())).Returns("Product not found");

            // Act
            var result = await _handler.Handle(deleteProductCommand, CancellationToken.None);

            // Assert
            var response = Assert.IsType<ServiceResponse<Unit>>(result);
            Assert.Equal(HttpStatusCode.NotFound, response.statusCode);  // Verify the status code is Not Found
            Assert.Equal(ResponseStatus.NOT_FOUND, response.status);  // Verify the response status is NOT_FOUND
        }

        [Fact]
        public async Task Handle_ReturnsErrorResponse_WhenExceptionOccurs()
        {
            // Arrange
            var deleteProductCommand = new DeleteProduct { ProductId = 1 };

            // Mocking the HTTP context for an Admin user
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.Role, "Admin")
            })); 
            // Simulating an exception in the repository layer
            _productRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<long>())).ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _handler.Handle(deleteProductCommand, CancellationToken.None);

            // Assert
            var response = Assert.IsType<ServiceResponse<Unit>>(result);
            Assert.Equal(HttpStatusCode.InternalServerError, response.statusCode);  // Verify the status code is InternalServerError
            Assert.Equal(ResponseStatus.ERROR, response.status);  // Verify the response status is ERROR
        }

    }
}
