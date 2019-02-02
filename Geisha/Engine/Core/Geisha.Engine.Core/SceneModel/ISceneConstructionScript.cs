namespace Geisha.Engine.Core.SceneModel
{
    /// <summary>
    ///     Defines interface of a construction script that is run for matching <see cref="Scene" /> before it is being
    ///     processed by systems.
    /// </summary>
    /// <remarks>
    ///     Provide and register custom implementations of this interface for custom processing of <see cref="Scene" /> loaded
    ///     from file before it gets into systems processing pipeline. It can be used i.e. for setting up correct initial state
    ///     of <see cref="Scene" /> like from the saved game.
    /// </remarks>
    public interface ISceneConstructionScript
    {
        /// <summary>
        ///     Name of the construction script. It is the matching key between script and <see cref="Scene" /> and therefore must
        ///     be unique across all the scripts.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Defines script steps to perform on specified <see cref="Scene" />.
        /// </summary>
        /// <param name="scene"><see cref="Scene" /> to be processed by script.</param>
        void Execute(Scene scene);
    }
}