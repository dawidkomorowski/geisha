using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.Components
{
    public abstract class Behavior : IComponent
    {
        public Entity Entity { get; set; }

        public virtual void OnUpdate(double deltaTime)
        {
        }

        public virtual void OnFixedUpdate()
        {
        }
    }
}