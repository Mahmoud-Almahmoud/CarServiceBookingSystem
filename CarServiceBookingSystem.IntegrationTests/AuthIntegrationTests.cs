using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace CarServiceBookingSystem.IntegrationTests;

public class AuthIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthIntegrationTests(CustomWebApplicationFactory factory)
    {
        factory.SeedRolesAsync().GetAwaiter().GetResult();
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_Should_Return_Success()
    {
        var request = new
        {
            fullName = "Test User",
            email = $"test-{Guid.NewGuid()}@test.com",
            phoneNumber = "0500000000",
            password = "Test123!"
        };

        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadAsStringAsync();

        body.Should().Contain("accessToken");
        body.Should().Contain("refreshToken");
    }

    [Fact]
    public async Task Login_Should_Return_Success()
    {
        var email = $"login-{Guid.NewGuid()}@test.com";

        var registerRequest = new
        {
            fullName = "Login User",
            email,
            phoneNumber = "0500000000",
            password = "Test123!"
        };

        var registerResponse = await _client.PostAsJsonAsync("/api/v1/auth/register", registerRequest);
        registerResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var loginRequest = new
        {
            email,
            password = "Test123!"
        };

        var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        var body = await loginResponse.Content.ReadAsStringAsync();

        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK, because: body);

        body.Should().Contain("accessToken");
        body.Should().Contain("refreshToken");
    }

    [Fact]
    public async Task Protected_Profile_Should_Return_Success_With_Jwt()
    {
        var email = $"profile-{Guid.NewGuid()}@test.com";

        var registerRequest = new
        {
            fullName = "Profile User",
            email,
            phoneNumber = "0500000000",
            password = "Test123!"
        };

        var registerResponse = await _client.PostAsJsonAsync(
            "/api/v1/auth/register",
            registerRequest);

        registerResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var registerBody = await registerResponse.Content
            .ReadFromJsonAsync<AuthTestResponse>();

        registerBody.Should().NotBeNull();
        registerBody!.Data.AccessToken.Should().NotBeNullOrWhiteSpace();

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer",
                registerBody.Data.AccessToken);

        var profileResponse = await _client.GetAsync("/api/v1/profile/me");

        profileResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Sessions_Should_Return_Current_Session()
    {
        var email = $"session-{Guid.NewGuid()}@test.com";

        var registerRequest = new
        {
            fullName = "Session User",
            email,
            phoneNumber = "0500000000",
            password = "Test123!"
        };

        var registerResponse = await _client.PostAsJsonAsync(
            "/api/v1/auth/register",
            registerRequest);

        var registerBody = await registerResponse.Content
            .ReadFromJsonAsync<AuthTestResponse>();

        registerResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                registerBody!.Data.AccessToken);

        var sessionsResponse = await _client.GetAsync("/api/v1/auth/sessions");

        var body = await sessionsResponse.Content.ReadAsStringAsync();

        sessionsResponse.StatusCode.Should().Be(HttpStatusCode.OK, because: body);

        body.Should().Contain("\"isCurrentSession\":true");
    }

    [Fact]
    public async Task RevokeSession_Should_Revoke_Another_Session()
    {
        var email = $"session-revoke-{Guid.NewGuid()}@test.com";

        var registerRequest = new
        {
            fullName = "Session User",
            email,
            phoneNumber = "0500000000",
            password = "Test123!"
        };

        var registerResponse = await _client.PostAsJsonAsync(
            "/api/v1/auth/register",
            registerRequest);

        registerResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var firstLoginResponse = await _client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new
            {
                email,
                password = "Test123!"
            });

        var firstLoginBody = await firstLoginResponse.Content
            .ReadFromJsonAsync<AuthTestResponse>();

        var secondLoginResponse = await _client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new
            {
                email,
                password = "Test123!"
            });

        var secondLoginBody = await secondLoginResponse.Content
            .ReadFromJsonAsync<AuthTestResponse>();

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                secondLoginBody!.Data.AccessToken);

        var revokeResponse = await _client.DeleteAsync(
            $"/api/v1/auth/sessions/{firstLoginBody!.Data.SessionId}");

        var body = await revokeResponse.Content.ReadAsStringAsync();

        revokeResponse.StatusCode.Should().Be(HttpStatusCode.OK, because: body);
        body.Should().Contain("Session revoked successfully");
    }
    [Fact]
    public async Task RevokeSession_Should_Fail_When_Revoking_Current_Session()
    {
        var email = $"current-session-{Guid.NewGuid()}@test.com";

        var registerRequest = new
        {
            fullName = "Current Session User",
            email,
            phoneNumber = "0500000000",
            password = "Test123!"
        };

        var registerResponse = await _client.PostAsJsonAsync(
            "/api/v1/auth/register",
            registerRequest);

        registerResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var registerBody = await registerResponse.Content
            .ReadFromJsonAsync<AuthTestResponse>();

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                registerBody!.Data.AccessToken);

        var revokeResponse = await _client.DeleteAsync(
            $"/api/v1/auth/sessions/{registerBody.Data.SessionId}");

        var body = await revokeResponse.Content.ReadAsStringAsync();

        revokeResponse.StatusCode.Should().Be(HttpStatusCode.NotFound, because: body);
        body.Should().Contain("You cannot revoke the current session");
    }
    [Fact]
    public async Task LogoutAllDevices_Should_Revoke_All_User_Sessions()
    {
        var email = $"logout-all-{Guid.NewGuid()}@test.com";

        var registerRequest = new
        {
            fullName = "Logout All User",
            email,
            phoneNumber = "0500000000",
            password = "Test123!"
        };

        var registerResponse = await _client.PostAsJsonAsync(
            "/api/v1/auth/register",
            registerRequest);

        registerResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var loginResponse = await _client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new
            {
                email,
                password = "Test123!"
            });

        var loginBody = await loginResponse.Content
            .ReadFromJsonAsync<AuthTestResponse>();

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                loginBody!.Data.AccessToken);

        var logoutAllResponse = await _client.PostAsync(
            "/api/v1/auth/logout-all-devices",
            null);

        var body = await logoutAllResponse.Content.ReadAsStringAsync();

        logoutAllResponse.StatusCode.Should().Be(HttpStatusCode.OK, because: body);
        body.Should().Contain("Logged out from all devices");
    }

    [Fact]
    public async Task Login_Should_Lock_Account_After_Too_Many_Failed_Attempts()
    {
        var email = $"lockout-{Guid.NewGuid()}@test.com";

        var registerRequest = new
        {
            fullName = "Lockout User",
            email,
            phoneNumber = "0500000000",
            password = "Test123!"
        };

        var registerResponse = await _client.PostAsJsonAsync(
            "/api/v1/auth/register",
            registerRequest);

        registerResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        for (var i = 0; i < 5; i++)
        {
            await _client.PostAsJsonAsync(
                "/api/v1/auth/login",
                new
                {
                    email,
                    password = "Wrong123!"
                });
        }

        var lockedResponse = await _client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new
            {
                email,
                password = "Wrong123!"
            });

        var body = await lockedResponse.Content.ReadAsStringAsync();

        lockedResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized, because: body);
        body.Should().Contain("Account is temporarily locked");
    }
}


public class AuthTestResponse
{
    public bool Success { get; set; }
    public AuthTestData Data { get; set; } = new();
}

public class AuthTestData
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public int SessionId { get; set; }
}