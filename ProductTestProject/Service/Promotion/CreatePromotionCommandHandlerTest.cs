using Application.Services.Promotion.Command.Create;
using AutoMapper;
using Domain.Enums;
using Domain.Models;
using Infrastructure.Common;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Moq;
using Resource;
using System.Net;
using System.Security.Claims;

namespace ProductTestProject.Service.Promotion
{
    public class CreatePromotionCommandHandlerTest
    {
        private readonly Mock<IPromotionRepository> _promotionRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly Mock<IResourceHelper> _resourceHelperMock;
        private readonly CreatePromotionHandler _handler;

        public CreatePromotionCommandHandlerTest()
        {
            _promotionRepositoryMock = new Mock<IPromotionRepository>();
            _mapperMock = new Mock<IMapper>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _resourceHelperMock = new Mock<IResourceHelper>();

            _handler = new CreatePromotionHandler(
                _promotionRepositoryMock.Object,
                _mapperMock.Object,
                _httpContextAccessorMock.Object,
                _resourceHelperMock.Object);
        }
        [Fact]
        public async Task Handle_ReturnsSuccessResponse_WhenPromotionCreated()
        {
            // Arrange
            CreatePromotion createPromotionCommand = new() { Name = "Summer Sale", Description = "50% off" };
            Promotions promotion = new() { Id = 123, Description = "50% off" };

            ServiceResponse<long> serviceResponse = new(promotion.Id, "Promotion created successfully", HttpStatusCode.OK, ResponseStatus.SUCCESS);

            // Mocking the HTTP context
            ClaimsPrincipal claimsPrincipal = new(new ClaimsIdentity(new Claim[]
            {
        new(ClaimTypes.Role, "Admin")
            }));

            // Mocking the AutoMapper behavior
            _mapperMock.Setup(m => m.Map<Promotions>(createPromotionCommand)).Returns(promotion);

            // Mocking the repository behavior
            _promotionRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Promotions>())).ReturnsAsync(promotion);

            // Mocking the Resource Helper behavior
            _resourceHelperMock.Setup(r => r.Promotion(It.IsAny<string>())).Returns("Promotion created successfully");

            // Act
            ServiceResponse<long> result = await _handler.Handle(createPromotionCommand, CancellationToken.None);

            // Assert
            ServiceResponse<long> response = Assert.IsType<ServiceResponse<long>>(result);
            Assert.Equal(123, response.data);  // Verify the ID of the created promotion
            Assert.Equal(HttpStatusCode.OK, response.statusCode);  // Verify the status code is OK
            Assert.Equal(ResponseStatus.SUCCESS, response.status);  // Verify the response status is success
        }
        [Fact]
        public async Task Handle_ReturnsUnauthorizedResponse_WhenUserIsNotAdmin()
        {
            // Arrange
            var createPromotionCommand = new CreatePromotion { Name = "Winter Sale", Description = "30% off" };

            // Mocking the HTTP context for a non-Admin user
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.Role, "User")  // Non-Admin role
            }));
           
            // Act
            var result = await _handler.Handle(createPromotionCommand, CancellationToken.None);

            // Assert
            var response = Assert.IsType<ServiceResponse<long>>(result);
            Assert.Equal(0, response.data);  // The ID should be 0 in case of failure
            Assert.Equal(HttpStatusCode.Unauthorized, response.statusCode);  // Verify the status code is Unauthorized
            Assert.Equal(ResponseStatus.NOT_ALLOWED, response.status);  // Verify the response status is NOT_ALLOWED
        }
        [Fact]
        public async Task Handle_ReturnsFailureResponse_WhenValidationFails()
        {
            // Arrange
            var createPromotionCommand = new CreatePromotion { Name = "", Description = "Invalid promotion with no name" };

            // Mocking the HTTP context
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.Role, "Admin")
            })); 
            // Mocking the AutoMapper behavior
            var promotion = new Promotions();
            _mapperMock.Setup(m => m.Map<Promotions>(createPromotionCommand)).Returns(promotion);

            // Mocking validation failure (returning some validation errors)
            var validationResult = new FluentValidation.Results.ValidationResult();
            validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure("Name", "Name is required"));
            // Act
            var result = await _handler.Handle(createPromotionCommand, CancellationToken.None);

            // Assert
            var response = Assert.IsType<ServiceResponse<long>>(result);
            Assert.Equal(0, response.data);  // The ID should be 0 in case of failure
            Assert.Equal(HttpStatusCode.Forbidden, response.statusCode);  // Verify the status code is Forbidden
            Assert.Equal(ResponseStatus.FAILURE, response.status);  // Verify the response status is failure
        }
        [Fact]
        public async Task Handle_ReturnsErrorResponse_WhenExceptionOccurs()
        {
            // Arrange
            var createPromotionCommand = new CreatePromotion { Name = "Black Friday Sale", Description = "Huge discounts!" };

            // Mocking the HTTP context
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.Role, "Admin")
            })); 
            // Simulating an exception in the repository layer
            _promotionRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Promotions>())).ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _handler.Handle(createPromotionCommand, CancellationToken.None);

            // Assert
            var response = Assert.IsType<ServiceResponse<long>>(result);
            Assert.Equal(0, response.data);  // The ID should be 0 in case of error
            Assert.Equal(HttpStatusCode.InternalServerError, response.statusCode);  // Verify the status code is InternalServerError
            Assert.Equal(ResponseStatus.ERROR, response.status);  // Verify the response status is ERROR
        }

    }
}
