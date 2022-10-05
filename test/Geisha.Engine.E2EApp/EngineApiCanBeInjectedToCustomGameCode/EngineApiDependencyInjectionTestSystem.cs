using Geisha.Engine.Core;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.Systems;

namespace Geisha.Engine.E2EApp.EngineApiCanBeInjectedToCustomGameCode
{
    internal sealed class EngineApiDependencyInjectionTestSystem : ICustomSystem
    {
        public string Name => "EngineApiDependencyInjectionTestSystem";

        public void ProcessFixedUpdate()
        {
        }

        public void ProcessUpdate(GameTime gameTime)
        {
            E2ETest.Report("E7691D98-AF87-4268-9C39-43822A790377", "Engine API Injected Into System");
        }

        public void OnEntityCreated(Entity entity)
        {
        }

        public void OnEntityRemoved(Entity entity)
        {
        }

        public void OnEntityParentChanged(Entity entity, Entity? oldParent, Entity? newParent)
        {
        }

        public void OnComponentCreated(Component component)
        {
        }

        public void OnComponentRemoved(Component component)
        {
        }
    }
}