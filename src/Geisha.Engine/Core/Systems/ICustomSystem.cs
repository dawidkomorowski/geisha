using Geisha.Engine.Core.GameLoop;
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
}