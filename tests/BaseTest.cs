using dotnetMVP.Models.DTO.Writeup;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace NTest
{

    public class BaseTest : AppBase
    {
        [Fact]
        public async Task EnsureDatabaseIsWorking()
        {
            // Test method to see if project is running and the test framework is working
            var c = CreateClient();

            var response = await c.GetAsync("/api/writeups/all");
            var wp = await response.Content.ReadFromJsonAsync<List<ShowWriteUpDto>>();

            wp.Should().NotBeNull();
        }

        [Fact]
        public async Task TestAuthorizeAttribute()
        {
            var c = CreateClient();

            // Test endppoint with [Authorize] attribute
            var response = await c.GetAsync("/api/platform/all");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        
    }
}
