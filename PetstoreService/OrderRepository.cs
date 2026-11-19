using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace PetstoreService
{
    public class OrderRepository
    {
        const string BaseUrl = "https://petstore.swagger.io/v2";
        private readonly HttpClient _client;

        public OrderRepository(HttpClient client)
        {
            _client = client;
        }
        public async Task<HttpResponseMessage> DeleteOrderAsync(long id)
        {
            return await _client.DeleteAsync($"{BaseUrl}/store/order/{id}");
        }
        public async Task<HttpResponseMessage> GetInventoryAsync()
        {
            return await _client.GetAsync($"{BaseUrl}/store/inventory");
        }
        public async Task<HttpResponseMessage> PostInventoryAsync() // для негативного теста
        {
            Order order = null;
            return await _client.PostAsJsonAsync($"{BaseUrl}/store/inventory", order);
        }
        public async Task<HttpResponseMessage> GetOrderByIdAsync(long id)
        {
            return await _client.GetAsync($"{BaseUrl}/store/order/{id}");
        }
        public async Task<HttpResponseMessage> CreateOrderAsync(Order order)
        {
            return await _client.PostAsJsonAsync($"{BaseUrl}/store/order", order);
        }
    }
}
