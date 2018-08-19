using System.Collections.Generic;
using stubby4netcore.Configuration.Data;

namespace stubby4netcore.Configuration
{
    public interface IConfigurationProcessor
    {
        IEnumerable<EndPoint> GetConfiguration();
    }
}