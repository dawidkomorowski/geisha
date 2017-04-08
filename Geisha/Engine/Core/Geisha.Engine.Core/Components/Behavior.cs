using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.Components
{
    public abstract class Behavior : IComponent
    {
        internal bool Started { get; set; } = false;

        public Entity Entity { get; internal set; }

        public virtual void OnStart()
        {
        }

        public virtual void OnUpdate(double deltaTime)
        {
        }

        public virtual void OnFixedUpdate()
        {
        }
    }
}