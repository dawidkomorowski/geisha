using System;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Input;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Input.Mapping;
using Geisha.Engine.Rendering.Components;
using Geisha.Engine.Rendering.Systems;
using Geisha.Engine.Windowing;

namespace Sandbox.Common;

public sealed class CommonEntityFactory
{
    private readonly IEngineManager _engineManager;
    private readonly IWindowingSystem _windowingSystem;
    private readonly IRenderingSystem _renderingSystem;

    public CommonEntityFactory(IEngineManager engineManager, IWindowingSystem windowingSystem, IRenderingSystem renderingSystem)
    {
        _engineManager = engineManager;
        _windowingSystem = windowingSystem;
        _renderingSystem = renderingSystem;
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

        inputComponent.InputMapping = InputMapping.CreateBuilder()
            .MapAction("Exit", Key.Escape)
            .MapAction("ToggleFullscreen", Key.F)
            .MapAction("ToggleCursorVisible", Key.C)
            .MapAction("ToggleVSync", Key.V)
            .Build();

        inputComponent.BindAction("Exit", _engineManager.ScheduleEngineShutdown);
        inputComponent.BindAction("ToggleFullscreen", () =>
        {
            _windowingSystem.DisplayMode = _windowingSystem.DisplayMode switch
            {
                DisplayMode.Windowed => DisplayMode.Fullscreen,
                DisplayMode.Fullscreen => DisplayMode.Windowed,
                _ => throw new InvalidOperationException("Unsupported display mode.")
            };
        });
        inputComponent.BindAction("ToggleCursorVisible", () => { _windowingSystem.CursorVisible = !_windowingSystem.CursorVisible; });
        inputComponent.BindAction("ToggleVSync", () => { _renderingSystem.VSyncEnabled = !_renderingSystem.VSyncEnabled; });

        return entity;
    }
}