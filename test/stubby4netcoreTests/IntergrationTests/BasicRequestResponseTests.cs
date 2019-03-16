using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using stubby4netcore;
using stubby4netcore.Configuration;
using stubby4netcore.Configuration.Data;
using Xunit;

namespace stubby4netcoreTests.IntergrationTests
{
    public class BasicRequestResponseTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public BasicRequestResponseTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/", HttpStatusCode.OK)]
        [InlineData("/not-modified", HttpStatusCode.NotModified)]
        [InlineData("/bad-request", HttpStatusCode.BadRequest)]
        [InlineData("/internal-server-error", HttpStatusCode.InternalServerError)]
        public async void Get_WhenEndpointCalled_TheCorrectStatusCodeIsReturned(string url, HttpStatusCode statusCode)
        {
            void ConfigureTestServices(IServiceCollection services) =>
                services.AddSingleton<IConfigurationProcessorFactory, TestConfigurationProcessorFactory>();

            var client = _factory.WithWebHostBuilder(builder =>
                    builder.ConfigureTestServices(ConfigureTestServices))
                .CreateClient();

            var response = await client.GetAsync(url);

            Assert.Equal(statusCode, response.StatusCode);
        }

        //TODO: Extend to Theory to conver all content headers
        [Theory]
        [InlineData("/header/content-type/json", "application/json")]
        [InlineData("/header/content-type/text", "text/plain")]
        public async void Get_WhenEndpointCalledAndContentTypeHeaderSpecified_TheCorrectHeaderValueIsReturned(string url, string contentType)
        {
            void ConfigureTestServices(IServiceCollection services) =>
                services.AddSingleton<IConfigurationProcessorFactory, TestConfigurationProcessorFactory>();

            var client = _factory.WithWebHostBuilder(builder =>
                    builder.ConfigureTestServices(ConfigureTestServices))
                .CreateClient();

            var response = await client.GetAsync(url);

            Assert.True(response.Content.Headers.Contains("Content-Type"));
            Assert.Equal(contentType, response.Content.Headers.GetValues("Content-Type").First());
        }

        //TODO: Add a Theory to cover all non content response headers.
    }

    public class TestConfigurationProcessorFactory : IConfigurationProcessorFactory
    {
        public IConfigurationProcessor getProcessor(string configFilePath)
        {
            return new TestConfigurationProcessor();
        }
    }

    public class TestConfigurationProcessor : IConfigurationProcessor
    {

        public IEnumerable<stubby4netcore.Configuration.Data.EndPoint> GetConfiguration()
        {
            var config = new List<stubby4netcore.Configuration.Data.EndPoint>();
            
            config.Add(CreateEndPoint("/", CreateDefaultHeaders()));
            config.Add(CreateEndPoint("/not-modified", CreateDefaultHeaders(), 304));
            config.Add(CreateEndPoint("/bad-request", CreateDefaultHeaders(), 400));
            config.Add(CreateEndPoint("/internal-server-error", CreateDefaultHeaders(), 500));
            
            config.Add(CreateEndPoint("/header/content-type/json", CreateDefaultHeaders()));
            var headers = CreateDefaultHeaders();
            headers["Content-Type"] = "text/plain";
            config.Add(CreateEndPoint("/header/content-type/text", headers));

            return config;
        }

        private stubby4netcore.Configuration.Data.EndPoint CreateEndPoint(string url, IDictionary<string, string> headers, int status = 200) {
            return new stubby4netcore.Configuration.Data.EndPoint 
            {
                Request = new Request
                {
                    Url = url
                },
                Response = new Response
                {
                    Status = status,
                    Headers = headers
                }
            };
        }

        private IDictionary<string, string> CreateDefaultHeaders() {
            return new Dictionary<string, string> {{"Content-Type", "application/json"}};
        }
    }
}
