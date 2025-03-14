
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data;
using ProductApi.Infrastructure.Repositories;
using System.Linq.Expressions;

namespace UnitTest.ProductApi.Repositories
{
    public class ProductRepositoryTest
    {
        private readonly ProductDbContext productDbContext;
        private readonly ProductRepository productRepository;

        public ProductRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<ProductDbContext>()
                .UseInMemoryDatabase(databaseName: "ProductRepositoryTest")
                .Options;

            // Set up dependencies
            productDbContext = new ProductDbContext(options);
            productRepository = new ProductRepository(productDbContext);
        }

        // Create Product
        [Fact]
        public async Task CreateProduct_WhenProductExists_ReturnsResponseWithErrorMessage()
        {
            // Arrange
            var existingProduct = new Product { Name = "ExistingProduct" };
            productDbContext.Products.Add(existingProduct);
            await productDbContext.SaveChangesAsync();

            // Act
            var result = await productRepository.CreateAsync(existingProduct);

            // Assert
            result.Should().NotBeNull();
            result.Flag.Should().BeFalse();
            result.Message.Should().Be("ExistingProduct already added");
        }

        [Fact]
        public async Task CreateProduct_WhenProductDoesNotExist_AddProductReturnsResponseWithSuccessMessage()
        {
            // Arrange
            var product = new Product { Name = "New Product" };

            // Act
            var result = await productRepository.CreateAsync(product);

            // Assert
            result.Should().NotBeNull();
            result.Flag.Should().BeTrue();
            result.Message.Should().Be($"{product.Name} created successfully");
        }

        // Delete Product
        [Fact]
        public async Task DeleteAsync_WhenProductIsFound_ReturnSuccessResponse()
        {
            //Arrange
            var product = new Product() { Id = 1, Name = "Existing Product", Quantity = 10, Price = 100.70m };
            productDbContext.Products.Add(product);

            // Act 
            var result = await productRepository.DeleteAsync(product);

            // Assert
            result.Should().NotBeNull();
            result.Flag.Should().BeTrue();
            result.Message.Should().Be($"{product.Name} deleted successfully");
        }

        [Fact]
        public async Task DeleteAsync_WhenProductIsNotFound_ReturnNotFoundResponse()
        {
            // Arrange
            var product = new Product() { Id = 2, Name = "NonExistingProduct", Quantity = 10, Price = 100.70m };

            // Act 
            var result = await productRepository.DeleteAsync(product);

            // Assert
            result.Should().NotBeNull();
            result.Flag.Should().BeFalse();
            result.Message.Should().Be($"{product.Name} not found");
        }


        // Get Product by Id
        [Fact]
        public async Task FindByIdAsync_WhenProductIsFound_ReturnProduct()
        {
            // Arrange
            var product = new Product() { Id = 1, Name = "ExistingProduct", Quantity = 10, Price = 100.70m };
            productDbContext.Products.Add(product);
            await productDbContext.SaveChangesAsync();

            // Act
            var result = await productRepository.FindByIdAsync(product.Id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Name.Should().Be("ExistingProduct");

        }

        [Fact]
        public async Task FindByIdAsync_WhenProductIsNotFound_ReturnNull()
        {
            // Arrange

            // Act
            var result = await productRepository.FindByIdAsync(99);

            // Assert
            result.Should().BeNull();

        }


        // Get All Products
        [Fact]
        public async Task GetAllAsync_WhenProductsAreFound_ReturnProducts()
        {
            // Arrange
            var products = new List<Product>
                {
                new() { Id = 1, Name = "Product 1" },
                new() { Id = 2, Name = "Product 2" }
                };
            productDbContext.Products.AddRange(products);
            await productDbContext.SaveChangesAsync();


            // Act
            var result = await productRepository.GetAllAsync();

            //Assert
            result.Should().NotBeNull();
            result.Count().Should().Be(2);
            result.Should().Contain(p => p.Name == "Product 1");
            result.Should().Contain(p => p.Name == "Product 2");
        }
        [Fact]
        public async Task GetAllAsync_WhenNoProductsAreFound_ReturnNull()
        {
            // Arrange

            // Act
            var result = await productRepository.GetAllAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }
    
        
        // Get By Any Type( Int. String, Bool etc..)
        [Fact]
        public async Task GetByAsync_WhenProductIsFound_ReturnProduct()
        {
            // Arrange
            var product = new Product() { Id = 1, Name = "Product 1" };
            productDbContext.Products.Add(product);
            await productDbContext.SaveChangesAsync();
            Expression<Func<Product, bool>> predicate = p => p.Name == "Product 1";

            //Act 
            var result = await productRepository.GetByAsync(predicate);

            //assert
            result.Should().NotBeNull();
            result.Name.Should().Be( "Product 1");
        }
        
        [Fact]
        public async Task GetByAsync_WhenProductIsNotFound_ReturnNull()
        {
            // Arrange
            Expression<Func<Product, bool>> predicate = p => p.Name == "NonExistingProduct";

            // Act
            var result = await productRepository.GetByAsync(predicate);

            // Assert
            result.Should().BeNull();
        }


        // Update Product
        [Fact]
        public async Task UpdateAsync_WhenProductExists_ReturnSuccessResponse()
        {
            // Arrange
            var product = new Product() { Id = 1, Name = "Existing Product", Quantity = 10, Price = 100.70m };
            productDbContext.Products.Add(product);
            await productDbContext.SaveChangesAsync();

            product.Name = "Updated Product";
            product.Quantity = 20;
            product.Price = 150.50m;

            // Act
            var result = await productRepository.UpdateAsync(product);

            // Assert
            result.Should().NotBeNull();
            result.Flag.Should().BeTrue();
            result.Message.Should().Be($"{product.Name} updated successfully");
        }

        [Fact]
        public async Task UpdateAsync_WhenProductDoesNotExist_ReturnNotFoundResponse()
        {
            // Arrange
            var product = new Product() { Id = 99, Name = "NonExistingProduct", Quantity = 10, Price = 100.70m };

            // Act
            var result = await productRepository.UpdateAsync(product);

            // Assert
            result.Should().NotBeNull();
            result.Flag.Should().BeFalse();
            result.Message.Should().Be($"{product.Name} not found");
        }

     








    }
}
