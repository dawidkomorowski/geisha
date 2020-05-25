using Autofac;

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
        ///     Registers game components in Autofac container.
        /// </summary>
        /// <param name="containerBuilder">Autofac container builder that provides components registration API.</param>
        /// <remarks>Implement this method to register components and services the game provides in Autofac container.</remarks>
        // TODO Is it good idea to present to the game code the Autofac container builder the engine uses. Maybe more limited but dedicated API for registering custom game components for engine to use should be introduced?
        void Register(ContainerBuilder containerBuilder);
    }
}