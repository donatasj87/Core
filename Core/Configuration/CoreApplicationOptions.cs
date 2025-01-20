using Donatas.Core.Configuration;

namespace Donatas.Core.Configuration
{
    public class CoreApplicationOptions
    {
        public string Name { get; set; } = string.Empty;
        public CoreEnvironment Environment { get; set; } = CoreEnvironment.Local;
    }
}
