using System.ComponentModel.Composition;

namespace Geisha.Engine.Core.Configuration
{
    [Export(typeof(IConfigurationManager))]
    public class ConfigurationManager : IConfigurationManager
    {
        public double FixedDeltaTime => 0.016;
    }
}