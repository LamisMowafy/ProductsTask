using Application.Services.Product.Command.Create;
using AutoMapper;
using Domain.Models;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Moq;
using Resource;
using System.Net;
using System.Security.Claims;

namespace ProductTestProject.Controller
{
    public class CreateProductHandlerTests
    {
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly Mock<IResourceHelper> _mockResourceHelper;
        private readonly CreateProductHandler _handler;

        public CreateProductHandlerTests()
        {
            _mockProductRepository = new Mock<IProductRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockResourceHelper = new Mock<IResourceHelper>();

            _handler = new CreateProductHandler(
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
            CreateProduct createProductRequest = new() { /* Populate with necessary data */ };
            ClaimsPrincipal claimsPrincipal = new(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "testuser")
            }));

            _mockHttpContextAccessor.Setup(x => x.HttpContext.User).Returns(claimsPrincipal);

            // Act
            Infrastructure.Common.ServiceResponse<long> result = await _handler.Handle(createProductRequest, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, result.statusCode);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenValidationFails()
        {
            // Arrange
            CreateProduct createProductRequest = new() { /* Populate with invalid data */ };
            ClaimsPrincipal claimsPrincipal = new(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim(ClaimTypes.Role, "Admin")
            }));

            _mockHttpContextAccessor.Setup(x => x.HttpContext.User).Returns(claimsPrincipal);

            // Mock the validation failure
            CreateValidator validator = new();
            FluentValidation.Results.ValidationResult validationResult = new(new[]
            {
                new FluentValidation.Results.ValidationFailure("Field", "Invalid value")
            });
            _mockMapper.Setup(m => m.Map<Products>(It.IsAny<CreateProduct>())).Returns(new Products());
            _mockResourceHelper.Setup(x => x.Shared("NOT_VALID")).Returns("Invalid data");

            // Act
            Infrastructure.Common.ServiceResponse<long> result = await _handler.Handle(createProductRequest, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, result.statusCode);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenProductIsCreated()
        {
            // Arrange
            CreateProduct createProductRequest = new() { Name = "test", Description = "desc" };
            ClaimsPrincipal claimsPrincipal = new(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim(ClaimTypes.Role, "Admin")
            }));

            _mockHttpContextAccessor.Setup(x => x.HttpContext.User).Returns(claimsPrincipal);
            Products product = new() { Id = 1 };
            _mockMapper.Setup(m => m.Map<Products>(It.IsAny<CreateProduct>())).Returns(product);
            _mockProductRepository.Setup(repo => repo.AddAsync(It.IsAny<Products>())).ReturnsAsync(product);
            _mockResourceHelper.Setup(x => x.Product("SUCCESSMESSAGE_PRODUCT_CREATED")).Returns("Product created successfully");

            // Act
            Infrastructure.Common.ServiceResponse<long> result = await _handler.Handle(createProductRequest, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.statusCode);
            Assert.Equal("Product created successfully", result.message);
            Assert.Equal(1, result.data);
        }

        [Fact]
        public async Task Handle_ShouldReturnInternalError_WhenExceptionOccurs()
        {
            // Arrange
            CreateProduct createProductRequest = new() { /* Populate with necessary data */ };
            ClaimsPrincipal claimsPrincipal = new(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim(ClaimTypes.Role, "Admin")
            }));

            _mockHttpContextAccessor.Setup(x => x.HttpContext.User).Returns(claimsPrincipal);
            _mockMapper.Setup(m => m.Map<Products>(It.IsAny<CreateProduct>())).Throws(new System.Exception("Internal error"));
            _mockResourceHelper.Setup(x => x.Shared("ERROREMESSAGE_INTERNAL")).Returns("Internal server error");

            // Act
            Infrastructure.Common.ServiceResponse<long> result = await _handler.Handle(createProductRequest, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, result.statusCode);
            Assert.Equal("Internal server error", result.message);
        }
    }
}
