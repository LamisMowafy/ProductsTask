using Application.Services.Promotion.Command.Update;
using AutoMapper;
using Domain.Enums;
using Domain.Models;
using Infrastructure.Common;
using Infrastructure.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Resource;
using System.Net;
using System.Security.Claims;

namespace ProductTestProject.Service.Promotion
{
    public class UpdatePromotionHandlerTests
    {
        private readonly Mock<IRepository<Promotions>> _promotionRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly Mock<IResourceHelper> _resourceHelperMock;
        private readonly UpdatePromotionHandler _handler;

        public UpdatePromotionHandlerTests()
        {
            _promotionRepositoryMock = new Mock<IRepository<Promotions>>();
            _mapperMock = new Mock<IMapper>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _resourceHelperMock = new Mock<IResourceHelper>();

            _handler = new UpdatePromotionHandler(
                _promotionRepositoryMock.Object,
                _mapperMock.Object,
                _httpContextAccessorMock.Object,
                _resourceHelperMock.Object);
        }
        [Fact]
        public async Task Handle_ReturnsSuccessResponse_WhenPromotionUpdated()
        {
            // Arrange
            UpdatePromotion updatePromotionCommand = new() { Id = 1, Name = "Winter Sale Updated", Description = "Updated 50% off" };
            Promotions promotion = new() { Id = 1,  Description = "50% off" };

            ServiceResponse<Unit> serviceResponse = new(Unit.Value, "Promotion updated successfully", HttpStatusCode.OK, ResponseStatus.SUCCESS);

            // Mocking the HTTP context
            ClaimsPrincipal claimsPrincipal = new(new ClaimsIdentity(new Claim[]
            {
        new(ClaimTypes.Role, "Admin")
            })); 

            // Mocking the AutoMapper behavior
            _mapperMock.Setup(m => m.Map<Promotions>(updatePromotionCommand)).Returns(promotion);

            // Mocking the repository behavior for the update
            _promotionRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Promotions>())).Returns(Task.CompletedTask);

            // Mocking the Resource Helper behavior
            _resourceHelperMock.Setup(r => r.Product(It.IsAny<string>())).Returns("Promotion updated successfully");

            // Act
            ServiceResponse<Unit> result = await _handler.Handle(updatePromotionCommand, CancellationToken.None);

            // Assert
            ServiceResponse<Unit> response = Assert.IsType<ServiceResponse<Unit>>(result);
            Assert.Equal(HttpStatusCode.OK, response.statusCode);  // Verify the status code is OK
            Assert.Equal(ResponseStatus.SUCCESS, response.status);  // Verify the response status is success
        }
        [Fact]
        public async Task Handle_ReturnsUnauthorizedResponse_WhenUserIsNotAdmin()
        {
            // Arrange
            var updatePromotionCommand = new UpdatePromotion { Id = 1, Name = "Summer Sale", Description = "50% off" };

            // Mocking the HTTP context for a non-Admin user
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.Role, "User")  // Non-Admin role
            })); 
            // Act
            var result = await _handler.Handle(updatePromotionCommand, CancellationToken.None);

            // Assert
            var response = Assert.IsType<ServiceResponse<Unit>>(result);
            Assert.Equal(HttpStatusCode.Unauthorized, response.statusCode);  // Verify the status code is Unauthorized
            Assert.Equal(ResponseStatus.NOT_ALLOWED, response.status);  // Verify the response status is NOT_ALLOWED
        }
        [Fact]
        public async Task Handle_ReturnsFailureResponse_WhenValidationFails()
        {
            // Arrange
            var updatePromotionCommand = new UpdatePromotion { Id = 1, Name = "", Description = "Invalid promotion with no name" };

            // Mocking the HTTP context
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.Role, "Admin")
            })); 
            // Mocking the AutoMapper behavior
            var promotion = new Promotions();
            _mapperMock.Setup(m => m.Map<Promotions>(updatePromotionCommand)).Returns(promotion);

            // Mocking validation failure (returning some validation errors)
            var validationResult = new FluentValidation.Results.ValidationResult();
            validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure("Name", "Name is required"));
            var validator = new UpdateValidator(); 

            // Act
            var result = await _handler.Handle(updatePromotionCommand, CancellationToken.None);

            // Assert
            var response = Assert.IsType<ServiceResponse<Unit>>(result);
            Assert.Equal(HttpStatusCode.Forbidden, response.statusCode);  // Verify the status code is Forbidden
            Assert.Equal(ResponseStatus.FAILURE, response.status);  // Verify the response status is failure
        }
        [Fact]
        public async Task Handle_ReturnsErrorResponse_WhenExceptionOccurs()
        {
            // Arrange
            var updatePromotionCommand = new UpdatePromotion { Id = 1, Name = "Black Friday Sale", Description = "Huge discounts!" };

            // Mocking the HTTP context
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.Role, "Admin")
            })); 
            // Simulating an exception in the repository layer
            _promotionRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Promotions>())).ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _handler.Handle(updatePromotionCommand, CancellationToken.None);

            // Assert
            var response = Assert.IsType<ServiceResponse<Unit>>(result);
            Assert.Equal(HttpStatusCode.InternalServerError, response.statusCode);  // Verify the status code is InternalServerError
            Assert.Equal(ResponseStatus.ERROR, response.status);  // Verify the response status is ERROR
        }

    }

}
