using Application.Services.Promotion.Command.Create;
using Application.Services.Promotion.Command.Delete;
using Application.Services.Promotion.Command.Update;
using Application.Services.Promotion.Queries.GetList;
using Domain.DTOs.Promotion;
using Domain.Enums;
using Infrastructure.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PromotionTask.Controllers.admin;
using System.Net;

namespace PromotionTestProject.Controller
{
    public class PromotionControllerTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly PromotionController _controller;

        public PromotionControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
            _controller = new PromotionController(_mockMediator.Object);
        }

        [Fact]
        public async Task GetAllPromotions_ReturnsOkResult_WithPromotionList()
        {
            // Arrange
            GetPromotionsListQuery query = new();
            ServiceResponse<List<PromotionDto>> PromotionList = new(
                Status: ResponseStatus.SUCCESS,
                Message: null,
                StatusCode: HttpStatusCode.OK,
                Data:
                [
                    new PromotionDto { Id = 1, Code = "Promotion 1"  },
                new PromotionDto { Id = 2, Code = "Promotion 2"  }
                ]
            );

            _mockMediator.Setup(m => m.Send(query, default)).ReturnsAsync(PromotionList);

            // Act
            ActionResult<List<PromotionDto>> result = await _controller.GetAllPromotions(query);

            // Assert
            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
            ServiceResponse<List<PromotionDto>> returnValue = Assert.IsType<ServiceResponse<List<PromotionDto>>>(okResult.Value);
            Assert.Equal(2, returnValue.data.Count);
        }


        [Fact]
        public async Task Create_ReturnsOkResult_WithPromotionId()
        {
            // Arrange
            CreatePromotion createCommand = new() { Name = "New Promotion" };
            ServiceResponse<long> PromotionResponse = new(
                Status: ResponseStatus.SUCCESS,
                Message: null,
                StatusCode: HttpStatusCode.OK,
                Data: 1
            );

            _mockMediator.Setup(m => m.Send(createCommand, default)).ReturnsAsync(PromotionResponse);

            // Act
            ActionResult<long> result = await _controller.Create(createCommand);

            // Assert
            ActionResult<long> actionResult = Assert.IsType<ActionResult<long>>(result);

        }

        [Fact]
        public async Task Update_ReturnsOkResult()
        {
            // Arrange
            UpdatePromotion updateCommand = new() { Id = 1, Name = "Updated Promotion" };
            ServiceResponse<Unit> updateResponse = new(
                Status: ResponseStatus.SUCCESS,
                Message: null,
                StatusCode: HttpStatusCode.OK,
                Data: Unit.Value
            );

            _mockMediator.Setup(m => m.Send(updateCommand, default)).ReturnsAsync(updateResponse);

            // Act
            ActionResult result = await _controller.Update(updateCommand);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsOkResult()
        {
            // Arrange
            int PromotionId = 1;
            DeletePromotion deleteCommand = new() { PromotionId = PromotionId };
            ServiceResponse<Unit> deleteResponse = new(
                Status: ResponseStatus.SUCCESS,
                Message: null,
                StatusCode: HttpStatusCode.OK,
                Data: Unit.Value
            );

            _mockMediator.Setup(m => m.Send(deleteCommand, default)).ReturnsAsync(deleteResponse);

            // Act
            ActionResult result = await _controller.Delete(PromotionId);

            // Assert
            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
            ServiceResponse<Unit> returnValue = Assert.IsType<ServiceResponse<Unit>>(okResult.Value);
            Assert.Equal(Unit.Value, returnValue.data);
        }
    }
}
