using Moq;
using AutoMapper;
using Xunit;
using Microsoft.AspNetCore.Http;
using MediatR;
using System.Threading.Tasks;
using System.Threading;
using System.Security.Claims;
using System.Linq;
using System.Net;
using Domain.Models;
using Infrastructure.Interfaces;
using Application.Services.Product.Command.Update;
using FluentValidation;
using System;
using Resource;

namespace Application.Tests
{
    public class UpdateProductHandlerTests
    {
        private readonly Mock<IRepository<Products>> _mockProductRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly Mock<IResourceHelper> _mockResourceHelper;
        private readonly UpdateProductHandler _handler;

        public UpdateProductHandlerTests()
        {
            _mockProductRepository = new Mock<IRepository<Products>>();
            _mockMapper = new Mock<IMapper>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockResourceHelper = new Mock<IResourceHelper>();

            _handler = new UpdateProductHandler(
                _mockProductRepository.Object,
                _mockMapper.Object,
                _mockHttpContextAccessor.Object,
                _mockResourceHelper.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnUnauthorized_WhenUserIsNotAdmin()
        {
            // Arrange
            var updateProductRequest = new UpdateProduct { Id = 1, /* Populate with necessary data */ };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "testuser")
            }));

            _mockHttpContextAccessor.Setup(x => x.HttpContext.User).Returns(claimsPrincipal);

            // Act
            var result = await _handler.Handle(updateProductRequest, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, result.statusCode);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenValidationFails()
        {
            // Arrange
            var updateProductRequest = new UpdateProduct { Id = 1, /* Populate with invalid data */ };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim(ClaimTypes.Role, "Admin")
            }));

            _mockHttpContextAccessor.Setup(x => x.HttpContext.User).Returns(claimsPrincipal);

            // Mock the validation failure
            var validator = new UpdateValidator();
            var validationResult = new FluentValidation.Results.ValidationResult(new[]
            {
                new FluentValidation.Results.ValidationFailure("Field", "Invalid value")
            });
            _mockResourceHelper.Setup(x => x.Shared("NOT_VALID")).Returns("Invalid data");

            // Act
            var result = await _handler.Handle(updateProductRequest, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, result.statusCode);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var updateProductRequest = new UpdateProduct { Id = 1,Name="test",Description="desc" /* Populate with valid data */ };
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
            var result = await _handler.Handle(updateProductRequest, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, result.statusCode);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenProductIsUpdated()
        {
            // Arrange
            var updateProductRequest = new UpdateProduct { Id = 1, Name = "test", Description = "desc" /* Populate with valid data */ };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim(ClaimTypes.Role, "Admin")
            }));

            _mockHttpContextAccessor.Setup(x => x.HttpContext.User).Returns(claimsPrincipal);
            var product = new Products { Id = 1, /* Set existing product properties */ };
            _mockProductRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<long>())).ReturnsAsync(product);
            _mockProductRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Products>())).Returns(Task.CompletedTask);
            _mockResourceHelper.Setup(x => x.Product("SUCCESSMESSAGE_PRODUCT_UPDATED")).Returns("Product updated successfully");

            // Act
            var result = await _handler.Handle(updateProductRequest, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.statusCode);
        }

        [Fact]
        public async Task Handle_ShouldReturnInternalError_WhenExceptionOccurs()
        {
            // Arrange
            var updateProductRequest = new UpdateProduct { Id = 1, Name = "test", Description = "desc" /* Populate with necessary data */ };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim(ClaimTypes.Role, "Admin")
            }));

            _mockHttpContextAccessor.Setup(x => x.HttpContext.User).Returns(claimsPrincipal);
            _mockProductRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<long>())).Throws(new Exception("Internal error"));
            _mockResourceHelper.Setup(x => x.Shared("ERROREMESSAGE_INTERNAL")).Returns("Internal server error");

            // Act
            var result = await _handler.Handle(updateProductRequest, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, result.statusCode);
        }
    }
}
