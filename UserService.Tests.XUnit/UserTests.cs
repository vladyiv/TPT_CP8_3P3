using Microsoft.VisualBasic.FileIO;
using System.Net;
using Xunit.Abstractions;
using PetstoreService;

namespace UserService.Tests.XUnit;

public class UserTests
{
    private readonly HttpClient _httpClient;
    private readonly UserRepository _service;

    public UserTests()
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 PetstoreTests/1.0");
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        _httpClient.DefaultRequestHeaders.Add("api_key", "special-key");

        _service = new UserRepository(_httpClient);
    }

    private static User CreateTestUser() => new User()
    {
        Id = 1,
        Username = "vlada",
        FirstName = "Vlada",
        LastName = "I.",
        Email = "ivanovavs23@st.ithub.ru",
        Password = "12345",
        Phone = "-",
        UserStatus = 1
    };

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
        Assert.Equal(HttpStatusCode.UnsupportedMediaType, response.StatusCode);
    }
    [Fact]
    public async Task GetByUsernameUser_Pos()
    {
        User user = CreateTestUser();
        await _service.PostAsync(user);
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
        await _service.PostAsync(user);
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
        await _service.PostAsync(user);
        HttpResponseMessage response = await _service.LoginAsync(user.Username, user.Password);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetLoginUser_Neg()
    {
        User user = CreateTestUser();
        await _service.PostAsync(user);
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
        HttpResponseMessage response = await _service.CreateUsersWithListAsync(usersList);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    [Fact]
    public async Task PostArrayUser_Neg()
    {
        HttpResponseMessage response = await _service.CreateUsersWithListAsync(null);
        Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
    }
    [Fact]
    public async Task PostUser_Pos()
    {
        List<User> usersList = new List<User> { CreateTestUser() };
        HttpResponseMessage response = await _service.CreateUsersWithListAsync(usersList);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    [Fact]
    public async Task PostUser_Neg()
    {
        HttpResponseMessage response = await _service.CreateUsersWithListAsync(null);
        Assert.Equal(HttpStatusCode.UnsupportedMediaType, response.StatusCode);
    }
}


