using Geisha.Engine.Core;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Input;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Input.Mapping;
using Geisha.Engine.Rendering.Components;

namespace Sandbox.Common;

public sealed class CommonEntityFactory
{
    private readonly IEngineManager _engineManager;

    public CommonEntityFactory(IEngineManager engineManager)
    {
        _engineManager = engineManager;
    }

    public Entity CreateCamera(Scene scene)
    {
        var camera = scene.CreateEntity();
        camera.CreateComponent<Transform2DComponent>();

        var cameraComponent = camera.CreateComponent<CameraComponent>();
        cameraComponent.ViewRectangle = new Vector2(1600, 900);
        cameraComponent.AspectRatioBehavior = AspectRatioBehavior.Underscan;

        return camera;
    }

    public Entity CreateBasicControls(Scene scene)
    {
        var entity = scene.CreateEntity();
        var inputComponent = entity.CreateComponent<InputComponent>();

        inputComponent.InputMapping = new InputMapping
        {
            ActionMappings =
            {
                new ActionMapping
                {
                    ActionName = "Exit",
                    HardwareActions =
                    {
                        new HardwareAction
                        {
                            HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(Key.Escape)
                        }
                    }
                }
            }
        };

        inputComponent.BindAction("Exit", _engineManager.ScheduleEngineShutdown);

        return entity;
    }
}