namespace Geisha.Engine.Core.Configuration
{
    public interface IConfigurationManager
    {
        TConfiguration GetConfiguration<TConfiguration>() where TConfiguration : class;
        EngineConfiguration GetEngineConfiguration();
    }
}