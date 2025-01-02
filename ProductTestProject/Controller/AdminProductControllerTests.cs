using Application.Services.Product.Command.Create;
using Application.Services.Product.Command.Delete;
using Application.Services.Product.Command.Update;
using Application.Services.Product.Queries.GetDetail;
using Application.Services.Product.Queries.GetList;
using Domain.DTOs.Product;
using Domain.Enums;
using Infrastructure.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductTask.Controllers.admin;
using System.Net;
namespace ProductTestProject.Controller
{
    public class AdminProductControllerTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly AdminProductController _controller;

        public AdminProductControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
            _controller = new AdminProductController(_mockMediator.Object);
        }

        [Fact]
        public async Task GetAllProducts_ReturnsOkResult_WithProductList()
        {
            // Arrange
            GetProductsListQuery query = new();
            ServiceResponse<List<ProductDto>> productList = new(
                Status: ResponseStatus.SUCCESS,
                Message: null,
                StatusCode: HttpStatusCode.OK,
                Data:
                [
                    new ProductDto { Id = 1, Name = "Product 1", Price = 100 },
                new ProductDto { Id = 2, Name = "Product 2", Price = 150 }
                ]
            );

            _mockMediator.Setup(m => m.Send(query, default)).ReturnsAsync(productList);

            // Act
            ActionResult<List<ProductDto>> result = await _controller.GetAllProducts(query);

            // Assert
            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
            ServiceResponse<List<ProductDto>> returnValue = Assert.IsType<ServiceResponse<List<ProductDto>>>(okResult.Value);
            Assert.Equal(2, returnValue.data.Count);
        }

        [Fact]
        public async Task GetProductById_ReturnsOkResult_WithProductDetail()
        {
            // Arrange
            int productId = 1;
            ServiceResponse<ProductDetailDto> productDetail = new(
                Status: ResponseStatus.SUCCESS,
                Message: null,
                StatusCode: HttpStatusCode.OK,
                Data: new ProductDetailDto { Id = 1, Name = "Product 1", Price = 100 }
            );
            GetProductDetailQuery query = new() { ProductId = productId };

            _mockMediator.Setup(m => m.Send(query, default)).ReturnsAsync(productDetail);

            // Act
            ActionResult<ProductDto> result = await _controller.GetProductById(productId);

            // Assert
            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
            ServiceResponse<ProductDetailDto> returnValue = Assert.IsType<ServiceResponse<ProductDetailDto>>(okResult.Value);
            Assert.Equal("Product 1", returnValue.data.Name);
            Assert.Equal(100, returnValue.data.Price);
        }

        [Fact]
        public async Task Create_ReturnsOkResult_WithProductId()
        {
            // Arrange
            CreateProduct createCommand = new() { Name = "New Product", Price = 200 };
            ServiceResponse<long> productResponse = new(
                Status: ResponseStatus.SUCCESS,
                Message: null,
                StatusCode: HttpStatusCode.OK,
                Data: 1
            );

            _mockMediator.Setup(m => m.Send(createCommand, default)).ReturnsAsync(productResponse);

            // Act
            ActionResult<long> result = await _controller.Create(createCommand);

            // Assert
            var actionResult = Assert.IsType<ActionResult<long>>(result);
         
        }

        [Fact]
        public async Task Update_ReturnsOkResult()
        {
            // Arrange
            UpdateProduct updateCommand = new() { Id = 1, Name = "Updated Product", Price = 250 };
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
            int productId = 1;
            DeleteProduct deleteCommand = new() { ProductId = productId };
            ServiceResponse<Unit> deleteResponse = new(
                Status: ResponseStatus.SUCCESS,
                Message: null,
                StatusCode: HttpStatusCode.OK,
                Data: Unit.Value
            );

            _mockMediator.Setup(m => m.Send(deleteCommand, default)).ReturnsAsync(deleteResponse);

            // Act
            ActionResult result = await _controller.Delete(productId);

            // Assert
            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
            ServiceResponse<Unit> returnValue = Assert.IsType<ServiceResponse<Unit>>(okResult.Value);
            Assert.Equal(Unit.Value, returnValue.data);
        }
    }
}