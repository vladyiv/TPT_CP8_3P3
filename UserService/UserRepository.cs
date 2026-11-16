using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace PetstoreService
{
    public class UserRepository
    {
        const string BaseUrl = "https://petstore.swagger.io/v2";
        private readonly HttpClient _client;

        public UserRepository(HttpClient client)
        {
            _client = client;
        }
        public async Task<HttpResponseMessage> CreateUsersWithListAsync(List<User> users)
        {
            return await _client.PostAsJsonAsync($"{BaseUrl}/user/createWithList", users);
        }
        public async Task<HttpResponseMessage> GetByUsernameAsync(string username)
        {
            return await _client.GetAsync($"{BaseUrl}/user/{username}");
        }
        public async Task<HttpResponseMessage> PostAsync(User user)
        {
            return await _client.PostAsJsonAsync<User>($"{BaseUrl}/user", user);
        }
        public async Task<HttpResponseMessage> DeleteAsync(string username)
        {
            return await _client.DeleteAsync($"{BaseUrl}/user/{username}");
        }
        public async Task<HttpResponseMessage> PutUserAsync(string username, User user)
        {
            return await _client.PutAsJsonAsync<User>($"{BaseUrl}/user/{username}", user);
        }
        public async Task<HttpResponseMessage> LoginAsync(string username, string password)
        {
            return await _client.GetAsync($"{BaseUrl}/user/login?username={username}&password={password}");
        }
        public async Task<HttpResponseMessage> LogoutAsync()
        {
            return await _client.GetAsync($"{BaseUrl}/user/logout");
        }
        public async Task<HttpResponseMessage> PostLogoutAsync() // для негативной проверки logout (post вместо get)
        {
            return await _client.PostAsync($"{BaseUrl}/user/logout", null);
        }
    }
}

