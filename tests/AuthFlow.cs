using dotnetMVP.Models.DTO.User;
using FluentAssertions;
using System.Net.Http.Json;
using System.Net;

namespace NTest
{
    public class AuthFlow : AppBase
    {
        // +1 bug
        [Fact]
        public async Task registration_returns_jwt_tokens()
        {
            var c = CreateClient();
            CreateUserDto newUser = new CreateUserDto()
            {
                Username = "TestUser1",
                Password = "TestPassword1!", // Change dto to Identity password requirements. Good test, reveals that the pass requirements are not met/enforced by current dto [DataAnnotations] attributes
                ConfirmPassword = "TestPassword1!" // With "TestPassword1" returns 401 in {loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);} and 200 in {userCreated.StatusCode.Should().Be(HttpStatusCode.OK);}
            };

            var userCreated = await c.PostAsJsonAsync("/api/user/register", newUser);
            LoginAttemptDto loginAttempt = new LoginAttemptDto()
            {
                Username = newUser.Username,
                Password = newUser.Password
            };
            var loginResponse = await c.PostAsJsonAsync("/api/auth/login", loginAttempt);
            var tokens = await loginResponse.Content.ReadFromJsonAsync<JwtTokens>();

            userCreated.StatusCode.Should().Be(HttpStatusCode.OK); // user creation should be successful
            loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            tokens.AccessToken.Should().NotBeNullOrEmpty();
            tokens.RefreshToken.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task weak_password_fails()
        {
            var c = CreateClient();
            CreateUserDto weakUser = new()
            {
                Username = "weakuser",
                Password = "password123",
                ConfirmPassword = "password123"
            };

            var response = await c.PostAsJsonAsync("api/user/register", weakUser);

            response.StatusCode.Should().NotBe(HttpStatusCode.OK);
        }

        // Investigate about TimeProvider
        [Fact]
        public async Task refresh_works_properly()
        {
            
        }
    }
}
