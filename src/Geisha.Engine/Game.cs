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

        /// <summary>
        ///     Configures audio subsystem. Override this method to programatically configure audio subsystem.
        /// </summary>
        /// <param name="configuration">Initial configuration of audio subsystem.</param>
        /// <returns>Instance of <see cref="AudioConfiguration" /> being used to configure engine audio subsystem.</returns>
        public virtual AudioConfiguration ConfigureAudio(AudioConfiguration configuration)
        {
            return configuration;
        }

        /// <summary>
        ///     Configures core systems and components. Override this method to programatically configure core systems and
        ///     components.
        /// </summary>
        /// <param name="configuration">Initial configuration of core systems and components.</param>
        /// <returns>Instance of <see cref="CoreConfiguration" /> being used to configure engine core systems and components.</returns>
        public virtual CoreConfiguration ConfigureCore(CoreConfiguration configuration)
        {
            return configuration;
        }

        /// <summary>
        ///     Configures physics subsystem. Override this method to programatically configure physics subsystem.
        /// </summary>
        /// <param name="configuration">Initial configuration of physics subsystem.</param>
        /// <returns>Instance of <see cref="PhysicsConfiguration" /> being used to configure engine physics subsystem.</returns>
        public virtual PhysicsConfiguration ConfigurePhysics(PhysicsConfiguration configuration)
        {
            return configuration;
        }

        /// <summary>
        ///     Configures rendering subsystem. Override this method to programatically configure rendering subsystem.
        /// </summary>
        /// <param name="configuration">Initial configuration of rendering subsystem.</param>
        /// <returns>Instance of <see cref="RenderingConfiguration" /> being used to configure engine rendering subsystem.</returns>
        public virtual RenderingConfiguration ConfigureRendering(RenderingConfiguration configuration)
        {
            return configuration;
        }
    }
}