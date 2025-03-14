using eCommerce.SharedLibrary.Responses;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Presentation.Controllers;
namespace UnitTest.ProductApi.Controllers
{
    public class ProductsControllerTest
    {
        private readonly IProduct productInterface;
        private readonly ProductsController productsController;

        public ProductsControllerTest()
        {
            // Set up dependancies
            productInterface = A.Fake<IProduct>();

            // Set up System Under Test -SUT
            productsController = new ProductsController(productInterface);
        }

        // Get All Products
        [Fact]
        public async Task GetProducts_WhenProductExists_ReturnsOkResponseWithProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new(){ Id = 1, Name = "Product 1", Quantity =10 , Price = 100.70m },
                new(){ Id = 2, Name = "Product 2",Quantity =110 , Price = 1004.70m }
            };

            // set up fake response for GetAllAsync method
            A.CallTo(() => productInterface.GetAllAsync()).Returns(products);

            // Act
            var result = await productsController.GetProducts();

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

            var returnedProducts = okResult.Value as IEnumerable<ProductDTO>;
            returnedProducts.Should().NotBeNull();
            returnedProducts.Should().HaveCount(2);
            returnedProducts!.First().Id.Should().Be(1);
            returnedProducts!.Last().Id.Should().Be(2);

        }

        [Fact]
        public async Task GetProducts_WhenNoProductExists_ReturnsNotFoundResponse()
        {
            // Arrange
            var products = new List<Product>();

            // set up fake response for GetAllAsync method
            A.CallTo(() => productInterface.GetAllAsync()).Returns(products);

            // Act
            var result = await productsController.GetProducts();

            // Assert
            var notFoundResult = result.Result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult!.StatusCode.Should().Be(StatusCodes.Status404NotFound);

            var message = notFoundResult.Value as string;
            message.Should().Be("No products detected in the database");
        }


        // Create Product
        [Fact]
        public async Task CreateProduct_WhenModelStateIsInValid_ReturnsBadResponse()
        {
            // Arrange
            var productDTO = new ProductDTO(1, "Product 1", 10, 100.70m);
            productsController.ModelState.AddModelError("Name", "Required");

            // Act
            var result = await productsController.CreateProduct(productDTO);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task CreateProduct_WhenPCreateIsSuccessful_ReturnsOkResponse()
        {
            // Arrange
            var productDTO = new ProductDTO(1, "Product 1", 10, 100.70m);
            var response = new Response(true, "Created");

            // Act
            A.CallTo(() => productInterface.CreateAsync(A<Product>.Ignored)).Returns(response);
            var result = await productsController.CreateProduct(productDTO);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult!.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

            var returnedResponse = okResult.Value as Response;
            returnedResponse!.Message.Should().Be("Created");
            returnedResponse!.Flag.Should().BeTrue();


        }

        [Fact]
        public async Task CreateProduct_WhenCreateFails_ReturnsBadRequestResponse()
        {
            // Arrange
            var productDTO = new ProductDTO(1, "Product 1", 10, 100.70m);
            var response = new Response(false, "Failed");

            // Act
            A.CallTo(() => productInterface.CreateAsync(A<Product>.Ignored)).Returns(response);
            var result = await productsController.CreateProduct(productDTO);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

            var returnedResponse = badRequestResult.Value as Response;
            returnedResponse!.Message.Should().Be("Failed");
            returnedResponse!.Flag.Should().BeFalse();
        }


        // Update product
        [Fact]
        public async Task UpdateProduct_WhenModelStateIsInValid_ReturnsBadResponse()
        {
            // Arrange
            var productDTO = new ProductDTO(1, "Product 1", 10, 100.70m);
            productsController.ModelState.AddModelError("Name", "Required");
            // Act
            var result = await productsController.UpdateProduct(productDTO);
            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task UpdateProduct_WhenUpdateIsSuccessful_ReturnsOkResponse()
        {
            // Arrange
            var productDTO = new ProductDTO(1, "Product 1", 10, 100.70m);
            var response = new Response(true, "Updated");
            // Act
            A.CallTo(() => productInterface.UpdateAsync(A<Product>.Ignored)).Returns(response);
            var result = await productsController.UpdateProduct(productDTO);
            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult!.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);
            var returnedResponse = okResult.Value as Response;
            returnedResponse!.Message.Should().Be("Updated");
            returnedResponse!.Flag.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateProduct_WhenUpdateFails_ReturnsBadRequestResponse()
        {
            // Arrange
            var productDTO = new ProductDTO(1, "Product 1", 10, 100.70m);
            var response = new Response(false, "Failed");
            // Act
            A.CallTo(() => productInterface.UpdateAsync(A<Product>.Ignored)).Returns(response);
            var result = await productsController.UpdateProduct(productDTO);
            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            var returnedResponse = badRequestResult.Value as Response;
            returnedResponse!.Message.Should().Be("Failed");
            returnedResponse!.Flag.Should().BeFalse();
        }


        // Delete Product
        [Fact]
        public async Task DeleteProduct_WhenDeleteIsSuccessful_ReturnsOkResponse()
        {
            // Arrange
            var productDtO = new ProductDTO(1, "Product 1", 10, 100.70m);
            var response = new Response(true, "Deleted");

            // Act
            A.CallTo(() => productInterface.DeleteAsync(A<Product>.Ignored)).Returns(response);
            var result = await productsController.DeleteProduct(productDtO);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult!.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);
            var returnedResponse = okResult.Value as Response;
            returnedResponse!.Message.Should().Be("Deleted");
            returnedResponse!.Flag.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteProduct_WhenDeleteFails_ReturnsBadRequestResponse()
        {
            // Arrange
            var productDtO = new ProductDTO(1, "Product 1", 10, 100.70m);
            var response = new Response(false, "Delete Failed");

            // Act
            A.CallTo(() => productInterface.DeleteAsync(A<Product>.Ignored)).Returns(response);
            var result = await productsController.DeleteProduct(productDtO);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

            var returnedResponse = badRequestResult.Value as Response;
            returnedResponse!.Message.Should().Be("Delete Failed");
            returnedResponse!.Flag.Should().BeFalse();
        }


        }
    }
