using System;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.E2EApp
{
    internal sealed class EngineApiDependencyInjectionTestComponent : BehaviorComponent
    {
        public EngineApiDependencyInjectionTestComponent(Entity entity) : base(entity)
        {
        }

        public override void OnUpdate(GameTime gameTime)
        {
            Console.WriteLine("484E1AFA-EEFE-4E3A-9D8E-A304847C8C16 EngineApiDependencyInjectionTestComponent");
        }
    }

    internal sealed class EngineApiDependencyInjectionTestComponentFactory : ComponentFactory<EngineApiDependencyInjectionTestComponent>
    {
        protected override EngineApiDependencyInjectionTestComponent CreateComponent(Entity entity) => new(entity);
    }
}