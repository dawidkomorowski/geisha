using Geisha.Engine.Audio;
using Geisha.Engine.Core;
using Geisha.Engine.Physics;
using Geisha.Engine.Rendering;

namespace Geisha.Engine
{
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

        public virtual AudioConfiguration ConfigureAudio(AudioConfiguration configuration)
        {
            return configuration;
        }

        public virtual CoreConfiguration ConfigureCore(CoreConfiguration configuration)
        {
            return configuration;
        }

        public virtual PhysicsConfiguration ConfigurePhysics(PhysicsConfiguration configuration)
        {
            return configuration;
        }

        public virtual RenderingConfiguration ConfigureRendering(RenderingConfiguration configuration)
        {
            return configuration;
        }
    }
}