using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.Systems
{
    /// <summary>
    ///     <see cref="ICustomSystem" /> interface defines API for implementing custom game systems that are run by the engine
    ///     in the game loop.
    /// </summary>
    /// <remarks>
    ///     <p>
    ///         To implement custom system create class implementing <see cref="ICustomSystem" /> interface and register it in
    ///         extension as single instance of <see cref="ICustomSystem" />.
    ///     </p>
    ///     <p>
    ///         Custom systems are executed in between main engine systems. First engine runs input and behavior systems, then
    ///         custom systems are run, finally rest of main engine systems are run (physics, rendering, etc.). Exact execution
    ///         order depends on which processing method is used. Fixed time step processing is done before variable time step
    ///         processing.
    ///     </p>
    /// </remarks>
    public interface ICustomSystem
    {
        /// <summary>
        ///     Name of custom system. It is used to uniquely identify custom system in configuration.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Implement system processing logic in this method. This method is executed at constant rate of fixed updates per
        ///     second. This method may be executed zero, one or multiple times per frame depending on how long time elapsed since
        ///     previous frame. This method is suitable for implementing logic that requires determinism, stability and strong
        ///     frame rate independence.
        /// </summary>
        /// <param name="scene"><see cref="Scene" /> that is the currently loaded and processed.</param>
        void ProcessFixedUpdate(Scene scene);

        /// <summary>
        ///     Implement system processing logic in this method. This method is executed at variable rate of updates per second as
        ///     it is directly tied to frame rate. It is executed exactly once per frame.
        /// </summary>
        /// <param name="scene"><see cref="Scene" /> that is the currently loaded and processed.</param>
        /// <param name="gameTime">Game time elapsed since previous frame.</param>
        void ProcessUpdate(Scene scene, GameTime gameTime);
    }
}