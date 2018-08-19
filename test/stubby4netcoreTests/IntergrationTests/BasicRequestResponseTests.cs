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
        [Fact]
        public async void Get_WhenEndpointCalledAndContentTypeHeaderSpecified_TheCorrectHeaderValueIsReturned()
        {
            void ConfigureTestServices(IServiceCollection services) =>
                services.AddSingleton<IConfigurationProcessorFactory, TestConfigurationProcessorFactory>();

            var client = _factory.WithWebHostBuilder(builder =>
                    builder.ConfigureTestServices(ConfigureTestServices))
                .CreateClient();

            var response = await client.GetAsync("/header/content-type");

            Assert.True(response.Content.Headers.Contains("Content-Type"));
            Assert.Equal("application/json", response.Content.Headers.GetValues("Content-Type").First());
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
        public IEnumerable<stubby4netcore.Configuration.Data.EndPoint> GetConfiguration() => new[] {
            new stubby4netcore.Configuration.Data.EndPoint 
            {
                Request = new Request
                {
                    Url = "/"
                },
                Response = new Response
                {
                    Status = 200
                }
            },
            new stubby4netcore.Configuration.Data.EndPoint 
            {
                Request = new Request
                {
                    Url = "/not-modified"
                },
                Response = new Response
                {
                    Status = 304
                }
            },
            new stubby4netcore.Configuration.Data.EndPoint 
            {
                Request = new Request
                {
                    Url = "/bad-request"
                },
                Response = new Response
                {
                    Status = 400
                }
            },
            new stubby4netcore.Configuration.Data.EndPoint 
            {
                Request = new Request
                {
                    Url = "/internal-server-error"
                },
                Response = new Response
                {
                    Status = 500
                }
            },
            new stubby4netcore.Configuration.Data.EndPoint 
            {
                Request = new Request
                {
                    Url = "/header/content-type"
                },
                Response = new Response
                {
                    Status = 200,
                    Headers = new Dictionary<string, string> {{"Content-Type", "application/json"}}
                }
            },
            new stubby4netcore.Configuration.Data.EndPoint 
            {
                Request = new Request
                {
                    Url = "/header/content-type"
                },
                Response = new Response
                {
                    Status = 200,
                    Headers = new Dictionary<string, string> {{"Access-Control-Allow-Origin", "*"}}
                }
            }
        };

    }
}
