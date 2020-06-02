namespace Geisha.Engine
{
    /// <summary>
    ///     Specifies interface of game that can be run by Geisha Engine. Implement this interface to provide custom game
    ///     functionality.
    /// </summary>
    public interface IGame
    {
        /// <summary>
        ///     Title of game window.
        /// </summary>
        string WindowTitle { get; }

        /// <summary>
        ///     Registers game components in components registry.
        /// </summary>
        /// <param name="componentsRegistry"><see cref="IComponentsRegistry" /> object that provides components registration API.</param>
        /// <remarks>
        ///     Implement this method to register components, systems and other services the game provides for an engine to
        ///     use.
        /// </remarks>
        void RegisterComponents(IComponentsRegistry componentsRegistry);
    }
}