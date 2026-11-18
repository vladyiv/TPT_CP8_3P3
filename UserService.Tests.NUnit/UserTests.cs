using System.Net;
namespace PetstoreService
{
    [TestFixture]
    public class UserTests
    {
        private HttpClient _httpClient;
        private UserRepository _service;
               
        [SetUp]
        public void Setup()
        {
            _httpClient = new HttpClient();
            _service = new UserRepository(_httpClient);
        }
        [TearDown]
        public async Task TearDown()
        {
            _httpClient.Dispose();
        }            

        private static User CreateTestUser()
        {
            string username = $"user_{Random.Shared.Next(0, 999999)}";
            return new User()
            {
                Id = Random.Shared.Next(0, 999999),
                Username = "vlada",
                FirstName = "Vlada",
                LastName = "I.",
                Email = $"{username}@example.com",
                Password = "12345",
                Phone = "-",
                UserStatus = 1
            };
        }

        [Test]
        public async Task PostListUser_Pos()
        {
            List<User> usersList = new List<User> { CreateTestUser() };
            HttpResponseMessage response = await _service.CreateUsersWithListAsync(usersList);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task PostListUser_Neg()
        {
            HttpResponseMessage response = await _service.CreateUsersWithListAsync(null);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.MethodNotAllowed));
        }

        [Test]
        public async Task GetByUsernameUser_Pos()
        {
            User user = CreateTestUser();
            await _service.CreateUserAsync(user);
            HttpResponseMessage response = await _service.GetByUsernameAsync(user.Username);
            Assert.That(response, Is.Not.Null);
        }

        [Test]
        public async Task GetByUsernameUser_Neg()
        {
            string username = CreateTestUser().Username;
            await _service.DeleteAsync(username);
            HttpResponseMessage response = await _service.GetByUsernameAsync(username);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task PutUser_Pos()
        {
            User user = CreateTestUser();
            string username = user.Username;
            user.FirstName = "Not Vlada";
            HttpResponseMessage response = await _service.PutUserAsync(username, user);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task PutUser_Neg()
        {
            User user = CreateTestUser();
            string username = user.Username;
            HttpResponseMessage response = await _service.PutUserAsync(username, null);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.MethodNotAllowed));
        }

        [Test]
        public async Task DeleteUser_Pos()
        {
            User user = CreateTestUser();
            await _service.CreateUserAsync(user);
            string username = user.Username;
            HttpResponseMessage response = await _service.DeleteAsync(username);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task DeleteUser_Neg()
        {
            string username = ".";
            HttpResponseMessage response = await _service.DeleteAsync(username);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.MethodNotAllowed));
        }

        [Test]
        public async Task GetLoginUser_Pos()
        {
            User user = CreateTestUser();
            await _service.CreateUserAsync(user);
            HttpResponseMessage response = await _service.LoginAsync(user.Username, user.Password);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task GetLoginUser_Neg()
        {
            User user = CreateTestUser();
            await _service.CreateUserAsync(user);
            HttpResponseMessage response = await _service.LoginAsync(user.Username, "wrong_password");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task GetLogoutUser_Pos()
        {
            HttpResponseMessage response = await _service.LogoutAsync();
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task GetLogoutUser_Neg()
        {
            HttpResponseMessage response = await _service.PostLogoutAsync();
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.MethodNotAllowed));
        }

        [Test]
        public async Task PostArrayUser_Pos()
        {
            List<User> usersList = new List<User> { CreateTestUser() };
            HttpResponseMessage response = await _service.CreateUsersWithArrayAsync(usersList);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task PostArrayUser_Neg()
        {
            HttpResponseMessage response = await _service.CreateUsersWithArrayAsync(null);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.MethodNotAllowed));
        }

        [Test]
        public async Task PostUser_Pos()
        {
            User user = CreateTestUser();
            HttpResponseMessage response = await _service.CreateUserAsync(user);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task PostUser_Neg()
        {
            HttpResponseMessage response = await _service.CreateUsersWithListAsync(null);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.MethodNotAllowed));
        }
    }
}