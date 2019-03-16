using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using stubby4netcore.Configuration.Yaml;
using System.Net.Http;

namespace stubby4netcoreTests.UnitTests.Configuration.Yaml
{
    public class YamlConfigurationProcessorTests
    {
        private const string BasicConfig = @"
            - request:
                url: /
                method: GET
                post: name=Bob&email=bob@example.com
                file: homePageRequest.xml
              response:
                status: 200
                headers:
                    Content-Type: application/json
                    Access-Control-Allow-Origin: ""*""
                    server: stubbedServer/4.2
            ";

        private const string JsonConfig = @"
            - request:
                url: /
                method: GET
                post: name=Bob&email=bob@example.com
                file: homePageRequest.xml
              response:
                status: 200
                headers:
                    Content-Type: application/json
                body: >
                    {""name"":""Bob""}
            "; 

        [Fact]
        public void GetConfiguration_WhenSuppliedAValidYamlConfiguration_ReturnsACollectionOfEndPoints()
        {
            var config = BasicConfig;

            var processor = new YamlConfigurationProcessor(config);

            var result = processor.GetConfiguration();

            Assert.NotNull(result);
        }

        [Fact]
        public void GetConfiguration_WhenSuppliedAValidYamlConfiguration_ReturnsTheCorrectNumberOfEndPoints()
        {
            var config = BasicConfig;

            var processor = new YamlConfigurationProcessor(config);

            var result = processor.GetConfiguration();

            Assert.Single(result);
        }

        [Fact]
        public void GetConfiguration_WhenSuppliedAValidYamlConfiguration_TheEndPointContainsARequestNode()
        {
            var config = BasicConfig;

            var processor = new YamlConfigurationProcessor(config);

            var result = processor.GetConfiguration().First();

            Assert.NotNull(result.Request);
        }

        [Fact]
        public void GetConfiguration_WhenSuppliedAValidYamlConfiguration_TheRequestUrlIsCorrect()
        {
            var config = BasicConfig;

            var processor = new YamlConfigurationProcessor(config);

            var result = processor.GetConfiguration().First();

            Assert.Equal("/", result.Request.Url);
        }

        [Fact]
        public void GetConfiguration_WhenSuppliedAValidYamlConfiguration_TheRequestMethodIsCorrect()
        {
            var config = BasicConfig;

            var processor = new YamlConfigurationProcessor(config);

            var result = processor.GetConfiguration().First();

            Assert.Equal("GET", result.Request.Method);
        }

        [Fact]
        public void GetConfiguration_WhenSuppliedAValidYamlConfiguration_TheRequestPostDataIsCorrect()
        {
            var config = BasicConfig;

            var processor = new YamlConfigurationProcessor(config);

            var result = processor.GetConfiguration().First();

            Assert.Equal("name=Bob&email=bob@example.com", result.Request.Post);
        }

        [Fact]
        public void GetConfiguration_WhenSuppliedAValidYamlConfiguration_TheRequestFilePathIsCorrect()
        {
            var config = BasicConfig;

            var processor = new YamlConfigurationProcessor(config);

            var result = processor.GetConfiguration().First();

            Assert.Equal("homePageRequest.xml", result.Request.File);
        }

        [Fact]
        public void GetConfiguration_WhenSuppliedAValidYamlConfiguration_TheEndPointContainsAResponseNode()
        {
            var config = BasicConfig;

            var processor = new YamlConfigurationProcessor(config);

            var result = processor.GetConfiguration().First();

            Assert.NotNull(result.Response);
        }

        [Fact]
        public void GetConfiguration_WhenSuppliedAValidYamlConfiguration_TheResponseStatusIsCorrect()
        {
            var config = BasicConfig;

            var processor = new YamlConfigurationProcessor(config);

            var result = processor.GetConfiguration().First();

            Assert.Equal(200, result.Response.Status);
        }

        [Theory]
        [InlineData("Content-Type", "application/json")]
        [InlineData("Access-Control-Allow-Origin", "*")]
        [InlineData("server", "stubbedServer/4.2")]
        public void GetConfiguration_WhenSuppliedAValidYamlConfiguration_TheResponseHeadersAreCorrect(string name, string value)
        {
            var config = BasicConfig;

            var processor = new YamlConfigurationProcessor(config);

            var result = processor.GetConfiguration().First();

            Assert.True(result.Response.Headers.ContainsKey(name));
            Assert.Equal(value, result.Response.Headers[name]);
        }

        public void GetConfiguration_WhenSuppliedAConfigurationWthAJsonBody_TheResponseBodyIsCorrect() {
            var config = JsonConfig;

            var processor = new YamlConfigurationProcessor(config);

            var result = processor.GetConfiguration().First();

            Assert.Equal(@"{""name"":""Bob""}", result.Response.Body);
        }
    }
}
