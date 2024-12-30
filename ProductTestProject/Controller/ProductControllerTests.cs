using Application.Services.Product.Command.Create;
using Application.Services.Product.Command.Delete;
using Application.Services.Product.Command.Update;
using Application.Services.Product.Queries.GetDetail;
using Application.Services.Product.Queries.GetList;
using Domain.DTOs.Product;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductTask.Controllers;

namespace ProductTestProject.Controller
{
    public class ProductControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly ProductsController _controller;

        public ProductControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new ProductsController(_mediatorMock.Object);
        }

        [Fact]
        public async Task GetAllProducts_ReturnsOkResult_WithListOfProducts()
        {
            // Arrange
            var productList = new List<ProductDto>
        {
            new ProductDto { Id = 1, Name = "Product1" },
            new ProductDto { Id = 2, Name = "Product2" }
        };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetProductsListQuery>(), default))
                .ReturnsAsync(productList);

            // Act
            var result = await _controller.GetAllProducts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsAssignableFrom<List<ProductDto>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task GetProductById_ReturnsOkResult_WithProductDetail()
        {
            // Arrange
            var productId = 1L;
            var productDetail = new ProductDetailDto { Id = productId, Name = "Product1", Description = "Description of Product1" };

            // Setup the mock to return the product detail when GetProductDetailQuery is sent
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetProductDetailQuery>(), default))
                .ReturnsAsync(productDetail);  // Correct setup with ReturnsAsync

            // Act
            var result = await _controller.GetProductById(productId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<ProductDetailDto>(okResult.Value);
            Assert.Equal(productId, returnValue.Id);
        }

        [Fact]
        public async Task Create_ReturnsOkResult_WithProductId()
        {
            // Arrange
            var createCommand = new CreateProduct { Name = "New Product" };
            var productId = 1L;
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateProduct>(), default))
                .ReturnsAsync(productId);

            // Act
            var result = await _controller.Create(createCommand);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<long>(okResult.Value);
            Assert.Equal(productId, returnValue);
        }
        [Fact]
        public async Task Update_ReturnsNoContent()
        {
            // Arrange
            var updateCommand = new UpdateProduct { Id = 1L, Name = "Updated Product" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateProduct>(), default))
                .ReturnsAsync(Unit.Value); // Mocking no return value

            // Act
            var result = await _controller.Update(updateCommand);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
        [Fact]
        public async Task Delete_ReturnsNoContent()
        {
            // Arrange
            var productId = 1L;
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteProduct>(), default))
                .ReturnsAsync(Unit.Value);

            // Act
            var result = await _controller.Delete(productId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

    }
}
