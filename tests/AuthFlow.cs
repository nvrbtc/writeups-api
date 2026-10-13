using dotnetMVP.Models.DTO.User;
using FluentAssertions;
using System.Net.Http.Json;
using System.Net;
using System.Text;

namespace NTest
{
    public class AuthFlow : AppBase
    {
        // +1 bug
        [Fact]
        public async Task registration_returns_jwt_tokens_and_200()
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

        /* Tests other part of app, paste in other test class
         *  [Fact]
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
         * 
         * 
         * 
         */

        // Investigate about TimeProvider
        [Fact]
        public async Task jwt_auth_returns_401_wihout_signature()
        {
            //TODO:
            var c = CreateClient();
            //Create new user to test
            CreateUserDto newUser = new()
            {
                Username = "TestUser2", //i need more flexible approach to create unique users for tests
                Password = "TestP@ssword2!",
                ConfirmPassword = "TestP@ssword2!"
            };
            var userCreated = await c.PostAsJsonAsync("/api/user/register", newUser);
            userCreated.StatusCode.Should().Be(HttpStatusCode.OK);

            //Login to gain jwt tokens
            var userLogin = await c.PostAsJsonAsync<LoginAttemptDto>("/api/auth/login", new()
            {
                Password = newUser.Password,
                Username = newUser.Username
            });

            userLogin.StatusCode.Should().Be(HttpStatusCode.OK);

            JwtTokens responseTokens = await userLogin.Content.ReadFromJsonAsync<JwtTokens>();


            var securedEndpoint = new HttpRequestMessage(HttpMethod.Get, "/api/challenges/all");
            securedEndpoint.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", responseTokens.AccessToken);

            var originalTokensResponse = await c.SendAsync(securedEndpoint);
            originalTokensResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            //Jwt structure {header}.{payload}.{signature}
            //Remove the {signature} part from token


            var jwtParts = responseTokens.AccessToken.Split('.');
            string jwtWithoutSign = $"{jwtParts[0]}.{jwtParts[1]}.";

            JwtTokens modifiedTokens = new()
            {
                AccessToken = jwtWithoutSign,
                RefreshToken = responseTokens.RefreshToken
            };
            //Request the secured endpoint with Bearer http header

            securedEndpoint = new HttpRequestMessage(HttpMethod.Get, "/api/challenges/all");
            securedEndpoint.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", modifiedTokens.AccessToken);
            var securedEndpointResponse = await c.SendAsync(securedEndpoint);

            securedEndpointResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
