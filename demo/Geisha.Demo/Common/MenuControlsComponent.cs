using System.Diagnostics;
using System.Linq;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Input;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Input.Mapping;
using Geisha.Engine.Rendering.Components;

namespace Geisha.Demo.Common
{
    internal sealed class MenuControlsComponent : BehaviorComponent
    {
        private readonly IEngineManager _engineManager;
        private readonly ScreenManager _screenManager;

        public MenuControlsComponent(Entity entity, IEngineManager engineManager, ScreenManager screenManager) : base(entity)
        {
            _engineManager = engineManager;
            _screenManager = screenManager;
        }

        public override void OnStart()
        {
            var inputComponent = Entity.GetComponent<InputComponent>();
            inputComponent.InputMapping = InputMapping.CreateBuilder()
                .MapAction("Exit", Key.Escape)
                .MapAction("Next", Key.Enter)
                .MapAction("Previous", Key.Backspace)
                .MapAction("GoToUrl", Key.F1)
                .Build();

            inputComponent.BindAction("Exit", _engineManager.ScheduleEngineShutdown);
            inputComponent.BindAction("Next", _screenManager.Next);
            inputComponent.BindAction("Previous", _screenManager.Previous);
            inputComponent.BindAction("GoToUrl", () =>
            {
                var link = Scene.AllEntities.Single(e => e.Name == "url");
                var url = link.GetComponent<TextRendererComponent>().Text;
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            });
        }
    }

    internal sealed class MenuControlsComponentFactory : ComponentFactory<MenuControlsComponent>
    {
        private readonly IEngineManager _engineManager;
        private readonly ScreenManager _screenManager;

        public MenuControlsComponentFactory(IEngineManager engineManager, ScreenManager screenManager)
        {
            _engineManager = engineManager;
            _screenManager = screenManager;
        }

        protected override MenuControlsComponent CreateComponent(Entity entity) => new(entity, _engineManager, _screenManager);
    }
}
