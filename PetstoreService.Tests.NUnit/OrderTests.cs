using PetstoreService;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace PetstoreService
{
    [TestFixture]
    public class OrderTests
    {
        private HttpClient _httpClient;
        private OrderRepository _service;

        [SetUp]
        public void SetUp()
        {
            _httpClient = new HttpClient();
            _service = new OrderRepository(_httpClient);
        }

        [TearDown]
        public void TearDown()
        {
            _httpClient.Dispose();
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

        [Test]
        public async Task GetInventoryStore_Pos()
        {
            HttpResponseMessage response = await _service.GetInventoryAsync();
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task GetInventoryStore_Neg()
        {
            HttpResponseMessage response = await _service.PostInventoryAsync();
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.MethodNotAllowed));
        }

        [Test]
        public async Task PostOrder_Pos()
        {
            Order order = CreateTestOrder();
            HttpResponseMessage response = await _service.CreateOrderAsync(order);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            HttpResponseMessage getResponse = await _service.GetOrderByIdAsync(order.Id);
            Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            Order getOrder = await getResponse.Content.ReadFromJsonAsync<Order>();
            Assert.That(getOrder.Id, Is.EqualTo(order.Id));
        }

        [Test]
        public async Task PostOrder_Neg()
        {
            HttpResponseMessage response = await _service.CreateOrderAsync(null);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task GetOrderById_Pos()
        {
            Order order = CreateTestOrder();
            await _service.CreateOrderAsync(order);

            HttpResponseMessage response = await _service.GetOrderByIdAsync(order.Id);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            Order getOrder = await response.Content.ReadFromJsonAsync<Order>();
            Assert.That(getOrder?.Id, Is.EqualTo(order.Id));
        }

        [Test]
        public async Task GetOrderById_Neg()
        {
            Order order = CreateTestOrder();
            await _service.DeleteOrderAsync(order.Id);

            HttpResponseMessage response = await _service.GetOrderByIdAsync(order.Id);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task DeleteOrder_Pos()
        {
            Order order = CreateTestOrder();
            await _service.CreateOrderAsync(order);

            HttpResponseMessage response = await _service.DeleteOrderAsync(order.Id);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            HttpResponseMessage getResponse = await _service.GetOrderByIdAsync(order.Id);
            Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task DeleteOrder_Neg()
        {
            Order order = CreateTestOrder();
            await _service.DeleteOrderAsync(order.Id);

            HttpResponseMessage response = await _service.DeleteOrderAsync(order.Id);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }
    }
}