using Donatas.Core.Configuration;
using System.Configuration;

namespace Donatas.Core.Configuration
{
    public static class CoreStorageAccount
    {
        public static string GetStorageAccount(CoreEnvironment coreEnvironment)
        {
            return coreEnvironment switch
            {
                CoreEnvironment.Local => "UseDevelopmentStorage=true",
                CoreEnvironment.Dev => "DefaultEndpointsProtocol=https;AccountName={AccountName};AccountKey={AccountKey};EndpointSuffix=core.windows.net",
                CoreEnvironment.Test => "DefaultEndpointsProtocol=https;AccountName={AccountName};AccountKey={AccountKey};EndpointSuffix=core.windows.net",
                CoreEnvironment.Prod => "DefaultEndpointsProtocol=https;AccountName={AccountName};AccountKey={AccountKey};EndpointSuffix=core.windows.net",
                _ => throw new ConfigurationErrorsException($"Core Environment [{coreEnvironment}] is unknown")
            };
        }
    }
}
