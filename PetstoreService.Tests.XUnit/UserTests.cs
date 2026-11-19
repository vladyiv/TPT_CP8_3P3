using System.Net;
using System.Net.Http.Json;

namespace PetstoreService
{
    public class UserTests
    {
        private readonly HttpClient _httpClient;
        private readonly UserRepository _service;

        public UserTests()
        {
            _httpClient = new HttpClient();
            _service = new UserRepository(_httpClient);
        }

        private static User CreateTestUser()
        {
            string username = $"user_{Random.Shared.Next(1, int.MaxValue)}";
            return new User()
            {
                Id = Random.Shared.Next(1, int.MaxValue),
                Username = "vlada",
                FirstName = "Vlada",
                LastName = "I.",
                Email = $"{username}@example.com",
                Password = "12345",
                Phone = "-",
                UserStatus = 1
            };
        }

        [Fact]
        public async Task PostListUser_Pos()
        {
            List<User> usersList = new List<User> { CreateTestUser() };
            HttpResponseMessage response = await _service.CreateUsersWithListAsync(usersList);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        [Fact]
        public async Task PostListUser_Neg()
        {
            HttpResponseMessage response = await _service.CreateUsersWithListAsync(null);
            Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
        }
        [Fact]
        public async Task GetByUsernameUser_Pos()
        {
            User user = CreateTestUser();
            await _service.CreateUserAsync(user);
            HttpResponseMessage response = await _service.GetByUsernameAsync(user.Username);
            Assert.NotNull(response);
        }
        [Fact]
        public async Task GetByUsernameUser_Neg()
        {
            User user = CreateTestUser();
            string username = CreateTestUser().Username;
            await _service.DeleteAsync(username);
            HttpResponseMessage response = await _service.GetByUsernameAsync(username);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task PutUser_Pos()
        {
            User user = CreateTestUser();
            string username = user.Username;
            user.FirstName = "Not Vlada";
            HttpResponseMessage response = await _service.PutUserAsync(username, user);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            HttpResponseMessage getResponse = await _service.GetByUsernameAsync(username);
            User getUser = await getResponse.Content.ReadFromJsonAsync<User>();
            Assert.Equal(user.FirstName, getUser.FirstName);
        }

        [Fact]
        public async Task PutUser_Neg()
        {
            User user = CreateTestUser();
            string username = user.Username;
            HttpResponseMessage response = await _service.PutUserAsync(username, null);
            Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
        }

        [Fact]
        public async Task DeleteUser_Pos()
        {
            User user = CreateTestUser();
            await _service.CreateUserAsync(user);
            string username = user.Username;
            HttpResponseMessage response = await _service.DeleteAsync(username);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        [Fact]
        public async Task DeleteUser_Neg()
        {
            string username = ".";
            HttpResponseMessage response = await _service.DeleteAsync(username);
            Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
        }
        [Fact]
        public async Task GetLoginUser_Pos()
        {
            User user = CreateTestUser();
            await _service.CreateUserAsync(user);
            HttpResponseMessage response = await _service.LoginAsync(user.Username, user.Password);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetLoginUser_Neg()
        {
            User user = CreateTestUser();
            await _service.CreateUserAsync(user);
            HttpResponseMessage response = await _service.LoginAsync(user.Username, "wrong_password");
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        [Fact]
        public async Task GetLogoutUser_Pos()
        {
            HttpResponseMessage response = await _service.LogoutAsync();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetLogoutUser_Neg()
        {
            HttpResponseMessage response = await _service.PostLogoutAsync();
            Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
        }
        [Fact]
        public async Task PostArrayUser_Pos()
        {
            List<User> usersList = new List<User> { CreateTestUser() };
            HttpResponseMessage response = await _service.CreateUsersWithArrayAsync(usersList);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        [Fact]
        public async Task PostArrayUser_Neg()
        {
            HttpResponseMessage response = await _service.CreateUsersWithArrayAsync(null);
            Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
        }
        [Fact]
        public async Task PostUser_Pos()
        {
            User user = CreateTestUser();
            string username = user.Username;
            HttpResponseMessage response = await _service.CreateUserAsync(user);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            HttpResponseMessage getResponse = await _service.GetByUsernameAsync(username);
            User getUser = await getResponse.Content.ReadFromJsonAsync<User>();
            Assert.Equal(user.FirstName, getUser.FirstName);
        }
        [Fact]
        public async Task PostUser_Neg()
        {
            HttpResponseMessage response = await _service.CreateUserAsync(null);
            Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
        }
    }
}


