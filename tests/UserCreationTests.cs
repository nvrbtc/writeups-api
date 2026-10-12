using dotnetMVP.Models.DTO.User;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace NTest
{
    public class UserCreationTests : AppBase
    {
        [Fact]
        public async Task invalid_creation_params()
        {
            var c = CreateClient();
            CreateUserDto invalidUsername = new()
            {
                Username = "1kk1",
                Password = "$t0ngPassword!1",
                ConfirmPassword = "$t0ngPassword!1",
            };

            var response = await c.PostAsJsonAsync<CreateUserDto>("/api/user/register",invalidUsername);

            response.StatusCode.Should().NotBe(HttpStatusCode.OK);
        }
    }
}
