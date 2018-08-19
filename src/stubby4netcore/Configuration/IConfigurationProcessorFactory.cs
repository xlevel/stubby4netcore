
namespace stubby4netcore.Configuration
{
    public interface IConfigurationProcessorFactory
    {
        IConfigurationProcessor getProcessor(string configFilePath);
    }
}