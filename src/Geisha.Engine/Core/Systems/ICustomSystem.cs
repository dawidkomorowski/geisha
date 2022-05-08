using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.Systems
{
    /// <summary>
    ///     <see cref="ICustomSystem" /> interface defines API for implementing custom game systems that are run by the engine
    ///     in the game loop.
    /// </summary>
    /// <remarks>
    ///     <p>
    ///         <see cref="ICustomSystem" /> interface is a shorthand for implementing both <see cref="ICustomGameLoopStep" />
    ///         and <see cref="ISceneObserver" /> as it is the most typical approach to implement custom system.
    ///         <see cref="ICustomGameLoopStep" /> defines API for periodic updates in game loop while
    ///         <see cref="ISceneObserver" /> defines API for watching scene structure.
    ///     </p>
    ///     <p>
    ///         To implement custom system create class implementing <see cref="ICustomSystem" /> interface and register it in
    ///         <see cref="IGame.RegisterComponents" /> using <see cref="IComponentsRegistry.RegisterSystem{TCustomSystem}" />.
    ///         Implement <see cref="ISceneObserver" /> API to track entities and components relevant to the system. Implement
    ///         <see cref="ICustomGameLoopStep" /> API to execute custom processing of entities and components relevant to the
    ///         system.
    ///     </p>
    ///     <p>
    ///         Custom systems are executed in between main engine systems. First engine runs input and behavior systems, then
    ///         custom systems are run, finally rest of main engine systems are run (physics, rendering, etc.). Exact execution
    ///         order depends on which processing method is used. Fixed time step processing is done before variable time step
    ///         processing.
    ///     </p>
    /// </remarks>
    public interface ICustomSystem : ICustomGameLoopStep, ISceneObserver
    {
    }

    /// <summary>
    ///     <see cref="ICustomGameLoopStep" /> interface defines API for implementing custom game loop steps that are run by
    ///     the engine in the game loop.
    /// </summary>
    /// <remarks>
    ///     <p>
    ///         To implement custom game loop step create class implementing <see cref="ICustomGameLoopStep" /> interface and
    ///         register it in <see cref="IGame.RegisterComponents" />.
    ///     </p>
    ///     <p>
    ///         Custom game loop steps are executed in between main engine systems. First the engine runs input and behavior
    ///         systems, then custom game loop steps are run, finally rest of main engine systems are run (physics, rendering,
    ///         etc.). Exact execution order depends on which processing method is used. Fixed time step processing is done
    ///         before variable time step processing.
    ///     </p>
    /// </remarks>
    public interface ICustomGameLoopStep
    {
        /// <summary>
        ///     Name of custom game loop step. It is used to uniquely identify custom game loop step in configuration.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Implement game loop step processing logic in this method. This method is executed at constant rate of fixed updates
        ///     per second. This method may be executed zero, one or multiple times per frame depending on how long time elapsed
        ///     since previous frame. This method is suitable for implementing logic that requires determinism, stability and
        ///     strong frame rate independence.
        /// </summary>
        void ProcessFixedUpdate();

        /// <summary>
        ///     Implement game loop step processing logic in this method. This method is executed at variable rate of updates per
        ///     second as it is directly tied to frame rate. It is executed exactly once per frame.
        /// </summary>
        /// <param name="gameTime">Game time elapsed since previous frame.</param>
        void ProcessUpdate(GameTime gameTime);
    }
}