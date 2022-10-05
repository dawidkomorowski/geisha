using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.E2EApp.EngineApiCanBeInjectedToCustomGameCode
{
    internal sealed class EngineApiDependencyInjectionTestComponent : BehaviorComponent
    {
        public EngineApiDependencyInjectionTestComponent(Entity entity) : base(entity)
        {
        }

        public override void OnStart()
        {
            E2ETest.Report("484E1AFA-EEFE-4E3A-9D8E-A304847C8C16", "Engine API Injected Into Component");
        }
    }

    internal sealed class EngineApiDependencyInjectionTestComponentFactory : ComponentFactory<EngineApiDependencyInjectionTestComponent>
    {
        protected override EngineApiDependencyInjectionTestComponent CreateComponent(Entity entity) => new(entity);
    }
}