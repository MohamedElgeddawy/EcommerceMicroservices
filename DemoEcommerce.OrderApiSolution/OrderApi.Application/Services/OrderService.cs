﻿using OrderApi.Application.DTOs;
using OrderApi.Application.DTOs.Conversions;
using OrderApi.Application.Interfaces;
using Polly;
using Polly.Registry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Application.Services
{
    public class OrderService(IOrder  orderInterface,HttpClient httpClient,
        ResiliencePipelineProvider<string> resiliencePipeline) : IOrderService
    {
        //Get Product
        public async Task<ProductDTO> GetProduct(int productId)
        {
            // Call Product Api using HttpClient 
            // Redirect this call t the API Gateway since product API is not response to outsiders.
            var getProduct = await httpClient.GetAsync($"/api/Products/{productId}");
            if (!getProduct.IsSuccessStatusCode)
                return null!;
            var product = await getProduct.Content.ReadFromJsonAsync<ProductDTO>();
            return product!;
        }

        // Get User
        public async Task<AppUserDTO> GetUser(int userId)
        {
            // Call User Api using HttpClient 
            // Redirect this call t the API Gateway since user API is not response to outsiders.
            var getUser = await httpClient.GetAsync($"/api/products/{userId}");
            //var getUser = await httpClient.GetAsync($"http://localhost:5000/api/Authentication/{userId}");

            if (!getUser.IsSuccessStatusCode)
                return null!;
            var product = await getUser.Content.ReadFromJsonAsync<AppUserDTO>();
            return product!;
        }
        public async Task<OrderDetailsDTO> GetOrderDetails(int orderId)
        {
            // Prepare Order
            var order = await orderInterface.FindByIdAsync(orderId);
            if (order is null || order!.Id <= 0)
            
                    return null!;

                // Get Retry pipeline
             var retryPipeline = resiliencePipeline.GetPipeline("my-retry-pipeline");

            // Prepare Product
            var productDTO = await retryPipeline.ExecuteAsync(async token => await GetProduct(order.ProductId));

            // Prepare User
            var appUserDTO = await retryPipeline.ExecuteAsync(async token => await GetUser(order.ClientId));
            if (appUserDTO == null)
                throw new Exception("User not found");

            // Populate Order Details
            return new OrderDetailsDTO(
                order.Id,
                productDTO.Id,
                appUserDTO.Id,
                appUserDTO.Name,
                appUserDTO.Email,
                appUserDTO.Address,
                appUserDTO.TelephoneNumber,
                productDTO.Name,
                order.PurchaseQuantity,
                productDTO.Price,
                order.PurchaseQuantity * productDTO.Price,
                order.OrderedDate
            );
        }

        // Get Orders By Client Id
        public async Task<IEnumerable<OrderDTO>> GetOrderByClientId(int clientId)
        {
            // Get all Client's orders  
            var orders = await orderInterface.GetOrdersAsync(o => o.ClientId == clientId);
            if (!orders.Any())
            {
                return null!;
            }

            // Convet from entity to DTO
            var (_, _orders) = OrderConversion.FromEntity(null, orders);
            return _orders!;
        }


    }
}
