using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.Net.Http;
using Xunit;
using Aiursoft.Developer;
using System.Threading.Tasks;
using System.Net;

namespace Aiursoft.Developer.Test.IntegratedTest
{
    public class TestHome
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public TestHome()
        {
            _server = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        [Fact]
        public async Task HomeRedirect()
        {
            var response = await _client.GetAsync("/");
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        }
    }
}
