using FakeItEasy;
using FluentAssertions;
using OrderApi.Application.DTOs;
using OrderApi.Application.Interfaces;
using OrderApi.Application.Services;
using OrderApi.Domain.Entities;
using System.Data;
using System.Linq.Expressions;
using System.Net.Http.Json;

namespace UnitTest.OrderApi.Services
{
    public class OrderServiceTest
    {
        private readonly IOrderService orderServiceInterface;
        private readonly IOrder orderInterface;
        public OrderServiceTest()
        {
            orderInterface = A.Fake<IOrder>();
            orderServiceInterface = A.Fake<IOrderService>();
        }

        // Create Fake Http Message Handler
        public class FakeHttpMessageHandler(HttpResponseMessage response) : HttpMessageHandler
        {
            private readonly HttpResponseMessage _response = response;

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => Task.FromResult(_response);

        }

        //Create Fake Http Client Using Fake Http Message Handler
        public static HttpClient CreateFakeHttpClient(object o)
        {
            var httpResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = JsonContent.Create(o)
            };
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(httpResponseMessage);
            var _httpClient = new HttpClient(fakeHttpMessageHandler)
            {
                BaseAddress = new Uri("http://localhost")
            };
            return _httpClient;
        }

        // Get Product
        [Fact]
        public async Task GetProduct_ValidProductId_ReturnProduct()
        {
            // Arrange
            var productId = 1;
            var productDTO = new ProductDTO(1, "Product 1", 13, 56.78m);
            var _httpClient = CreateFakeHttpClient(productDTO);

            // system Under Test - SUT
            // We only The httpClient to make calls
            // specify only httpClient and Null to the rest
            var _orderService = new OrderService(null!, _httpClient, null!);

            // Act
            var result = await _orderService.GetProduct(productId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(productId);
            result.Name.Should().Be("Product 1");
        }

        [Fact]
        public async Task GetProduct_InvalidProductId_ReturnPNull()
        {
            // Arrange
            var productId = 1; 
            var _httpClient = CreateFakeHttpClient(null!);

            // System Under Test - SUT
            var _orderService = new OrderService(null!, _httpClient, null!);

            // Act
            var result = await _orderService.GetProduct(productId);

            // Assert
            result.Should().BeNull();
        }

        // Get Client Orders By Id
        [Fact]
        public async Task GetOrdersByClients_OrderExist_ReturnOrderDatails()
        {
            // Arrange
            int clientId = 1;
            var orders = new List<Order>
            {
                new() { Id = 1, ProductId = 1, ClientId = clientId, PurchaseQuantity = 2, OrderedDate = DateTime.UtcNow },
                new() { Id = 1, ProductId = 2, ClientId = clientId, PurchaseQuantity = 1, OrderedDate = DateTime.UtcNow }
            };

            // mock the GetOrderBy method
            A.CallTo(() => orderInterface.GetOrdersAsync(A<Expression<Func<Order, bool>>>.Ignored)).Returns(orders);
            var _orderService = new OrderService(orderInterface!, null!, null!);
            // Act
            var result = await _orderService.GetOrderByClientId(clientId);

            //Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(orders.Count);
            result.Should().HaveCountGreaterThanOrEqualTo(2);

        }




    }
}
