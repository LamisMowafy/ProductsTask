using Application.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

public class JwtAuthenticationTests : IClassFixture<WebApplicationFactory<Program>> // For .NET 6+, replace Program with Startup if required
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public JwtAuthenticationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    // Helper method to generate a valid JWT token (mock for testing purposes)
    private string GenerateJwtToken()
    {
        var loginRequest = new LoginRequest
        {
            Username = "admin",
            Password = "password"
        };

        HttpResponseMessage loginResponse = _client.PostAsJsonAsync("api/account/login", loginRequest).Result;
        loginResponse.EnsureSuccessStatusCode();

        string responseContent = loginResponse.Content.ReadAsStringAsync().Result;
        dynamic? tokenObject = JsonConvert.DeserializeObject<dynamic>(responseContent);
        return tokenObject?.Token;
    }

    [Fact]
    public async Task Should_Return_Unauthorized_When_No_Token_Provided()
    {
        HttpResponseMessage response = await _client.GetAsync("/api/products"); // Protected endpoint
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Should_Return_Forbidden_When_Invalid_Token_Provided()
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "invalid_token");

        HttpResponseMessage response = await _client.GetAsync("/api/products"); // Protected endpoint
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode); // May vary depending on the setup
    }

    [Fact]
    public async Task Should_Return_Ok_When_Valid_Token_Provided()
    {
        string token = GenerateJwtToken(); // Generate a valid token for the test
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        HttpResponseMessage response = await _client.GetAsync("/api/products"); // Protected endpoint
        response.EnsureSuccessStatusCode(); // Will throw an exception if the status code is not successful

        // Optionally, you can assert response content or model
        string responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("product", responseContent.ToLower()); // Assuming the response contains product data
    }

    [Fact]
    public async Task Should_Return_BadRequest_When_Invalid_Login()
    {
        LoginRequestDto loginRequest = new()
        {
            Username = "invalid_user",
            Password = "invalid_password"
        };

        HttpResponseMessage loginResponse = await _client.PostAsJsonAsync("api/account/login", loginRequest);
        Assert.Equal(HttpStatusCode.Unauthorized, loginResponse.StatusCode);
    }
}
