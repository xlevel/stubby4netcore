using System;
using System.Collections.Generic;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using stubby4netcore.Configuration.Data;

namespace stubby4netcore.Configuration.Yaml
{
    public class YamlConfigurationProcessor: IConfigurationProcessor
    {
        private string _config;

        public YamlConfigurationProcessor(string config)
        {
            _config = config;
        }

        public IEnumerable<EndPoint> GetConfiguration()
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(new CamelCaseNamingConvention())
                .Build();
            
            var endpoint = deserializer.Deserialize<List<EndPoint>>(_config);
            return endpoint;
        }
    }
}