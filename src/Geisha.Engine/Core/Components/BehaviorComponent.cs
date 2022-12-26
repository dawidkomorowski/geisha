using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.Systems;

namespace Geisha.Engine.Core.Components
{
    /// <summary>
    ///     Base class for components with custom behavior. Derive from this class to implement components providing custom
    ///     behavior.
    /// </summary>
    /// <remarks>
    ///     <p>
    ///         Implementing custom components derived from <see cref="BehaviorComponent" /> is the simplest way to implement
    ///         game logic. The game logic is solely contained in component together with relevant data.
    ///     </p>
    ///     <p>
    ///         For more advanced scenarios when certain game mechanics may involve multiple components and complex state it
    ///         may be worth to use <see cref="ICustomSystem" /> API with custom components derived directly from
    ///         <see cref="Component" />.
    ///     </p>
    /// </remarks>
    public abstract class BehaviorComponent : Component
    {
        /// <summary>
        ///     Initializes new instance of <see cref="BehaviorComponent" /> class which is attached to specified entity.
        /// </summary>
        /// <param name="entity">Entity to which new component is attached.</param>
        protected BehaviorComponent(Entity entity) : base(entity)
        {
        }

        internal bool Started { get; set; } = false;

        /// <summary>
        ///     Override this method to provide custom behavior on start of the component lifetime.
        /// </summary>
        /// <remarks>
        ///     This method is executed exactly once when the <see cref="BehaviorSystem" /> encounters this
        ///     <see cref="BehaviorComponent" /> instance for the first time during the game loop update.
        /// </remarks>
        public virtual void OnStart()
        {
        }

        /// <summary>
        ///     Override this method to provide custom behavior on each update of the game loop for this component.
        /// </summary>
        /// <param name="gameTime"><see cref="GameTime" /> this game loop update is run with.</param>
        public virtual void OnUpdate(GameTime gameTime)
        {
        }

        /// <summary>
        ///     Override this method to provide custom behavior on each fixed update of the game loop for this component.
        /// </summary>
        public virtual void OnFixedUpdate()
        {
        }

        /// <summary>
        ///     Override this method to provide custom behavior on end of the component lifetime just before it is removed.
        /// </summary>
        /// <remarks>
        ///     This method is executed exactly once for the component that was just removed from entity, when the
        ///     <see cref="BehaviorSystem" /> encounters this <see cref="BehaviorComponent" /> instance in the game loop update to
        ///     perform internal cleanup.
        /// </remarks>
        public virtual void OnRemove()
        {
        }
    }
}