namespace Geisha.Engine;

/// <summary>
///     Base class for game that can be run by Geisha Engine. Subclass this class to provide custom game functionality.
/// </summary>
public abstract class Game
{
    /// <summary>
    ///     Title of game window.
    /// </summary>
    public virtual string WindowTitle => string.Empty;

    /// <summary>
    ///     Registers game components in components registry.
    /// </summary>
    /// <param name="componentsRegistry"><see cref="IComponentsRegistry" /> object that provides components registration API.</param>
    /// <remarks>
    ///     Override this method to register components, systems and other services the game provides for an engine to use.
    /// </remarks>
    public virtual void RegisterComponents(IComponentsRegistry componentsRegistry)
    {
    }

    /// <summary>
    ///     Configures the engine before running the game. Override this method to programatically configure the engine.
    /// </summary>
    /// <param name="configuration">Initial configuration of the engine.</param>
    /// <returns>Instance of <see cref="Configuration" /> that is used to configure the engine.</returns>
    /// <remarks>
    ///     This method is supplied with initial engine configuration that is either default configuration or configuration
    ///     loaded from file. You can use <c>with</c> keyword to easily overwrite subset of configuration options.
    /// </remarks>
    public virtual Configuration Configure(Configuration configuration)
    {
        return configuration;
    }
}