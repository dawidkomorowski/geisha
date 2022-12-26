using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.Components
{
    // TODO Add documentation.
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

        public virtual void OnStart()
        {
        }

        public virtual void OnUpdate(GameTime gameTime)
        {
        }

        public virtual void OnFixedUpdate()
        {
        }

        public virtual void OnRemove()
        {
        }
    }
}