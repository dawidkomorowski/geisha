using System;

namespace Geisha.Engine.Core.Configuration
{
    public interface IDefaultConfigurationFactory
    {
        Type ConfigurationType { get; }
        IConfiguration CreateDefault();
    }
}