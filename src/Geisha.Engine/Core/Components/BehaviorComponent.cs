using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.Components
{
    // TODO Add documentation.
    public abstract class BehaviorComponent : IComponent
    {
        internal bool Started { get; set; } = false;

        public Entity Entity { get; internal set; }

        public virtual void OnStart()
        {
        }

        public virtual void OnUpdate(GameTime gameTime)
        {
        }

        public virtual void OnFixedUpdate()
        {
        }
    }
}