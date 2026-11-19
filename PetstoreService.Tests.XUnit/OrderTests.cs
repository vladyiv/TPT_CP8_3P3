using PetstoreService;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace PetstoreService
{
    public class OrderTests
    {
        private HttpClient _httpClient;
        private OrderRepository _service;
        public OrderTests()
        {
            _httpClient = new HttpClient();
            _service = new OrderRepository(_httpClient);
        }

        private static Order CreateTestOrder()
        {
            return new Order()
            {
                Id = Random.Shared.Next(0, int.MaxValue),
                PetId = Random.Shared.Next(0, int.MaxValue),
                Quantity = 1,
                ShipDate = DateTime.Now,
                Status = "placed",
                Complete = false
            };
        }

        [Fact]
        public async Task GetInventoryStore_Pos()
        {
            HttpResponseMessage response = await _service.GetInventoryAsync();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetInventoryStore_Neg()
        {
            HttpResponseMessage response = await _service.PostInventoryAsync();
            Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
        }

        [Fact]
        public async Task PostOrder_Pos()
        {
            Order order = CreateTestOrder();
            HttpResponseMessage response = await _service.CreateOrderAsync(order);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            HttpResponseMessage getResponse = await _service.GetOrderByIdAsync(order.Id);
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            Order getOrder = await getResponse.Content.ReadFromJsonAsync<Order>();
            Assert.Equal(order.Id, getOrder?.Id);
        }

        [Fact]
        public async Task PostOrder_Neg()
        {
            HttpResponseMessage response = await _service.CreateOrderAsync(null);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetOrderById_Pos()
        {
            Order order = CreateTestOrder();
            await _service.CreateOrderAsync(order);

            HttpResponseMessage response = await _service.GetOrderByIdAsync(order.Id);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            Order? getOrder = await response.Content.ReadFromJsonAsync<Order>();
            Assert.Equal(order.Id, getOrder?.Id);
        }

        [Fact]
        public async Task GetOrderById_Neg()
        {
            Order order = CreateTestOrder();
            await _service.DeleteOrderAsync(order.Id);

            HttpResponseMessage response = await _service.GetOrderByIdAsync(order.Id);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteOrder_Pos()
        {
            Order order = CreateTestOrder();
            await _service.CreateOrderAsync(order);

            HttpResponseMessage response = await _service.DeleteOrderAsync(order.Id);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            HttpResponseMessage getResponse = await _service.GetOrderByIdAsync(order.Id);
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public async Task DeleteOrder_Neg()
        {
            Order order = CreateTestOrder();
            await _service.DeleteOrderAsync(order.Id);

            HttpResponseMessage response = await _service.DeleteOrderAsync(order.Id);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
