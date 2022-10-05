using Geisha.Engine.Core;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.E2EApp
{
    internal sealed class ExitTestAppComponent : BehaviorComponent
    {
        private readonly IEngineManager _engineManager;
        private int _frameCounter = 0;

        public ExitTestAppComponent(Entity entity, IEngineManager engineManager) : base(entity)
        {
            _engineManager = engineManager;
        }

        public int ExitOnFrame { get; set; } = 1;

        public override void OnUpdate(GameTime gameTime)
        {
            _frameCounter++;

            if (_frameCounter == ExitOnFrame)
            {
                _engineManager.ScheduleEngineShutdown();
            }
        }
    }

    internal sealed class ExitTestAppComponentFactory : ComponentFactory<ExitTestAppComponent>
    {
        private readonly IEngineManager _engineManager;

        public ExitTestAppComponentFactory(IEngineManager engineManager)
        {
            _engineManager = engineManager;
        }

        protected override ExitTestAppComponent CreateComponent(Entity entity) => new(entity, _engineManager);
    }
}