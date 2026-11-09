using System;
using System.Globalization;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Input.Backend;
using Geisha.Engine.Rendering.Components;

namespace Sandbox.GamepadController;

internal sealed class ControllerStateComponent : BehaviorComponent
{
    private readonly IInputBackend _inputBackend;

    public ControllerStateComponent(Entity entity, IInputBackend inputBackend) : base(entity)
    {
        _inputBackend = inputBackend;
    }

    public TextRendererComponent? ConnectionStateLabel { get; set; }
    public TextRendererComponent? PacketNumberLabel { get; set; }

    public override void OnUpdate(in TimeStep timeStep)
    {
        if (ConnectionStateLabel is null) return;
        if (PacketNumberLabel is null) return;

        var controllerState = _inputBackend.GetControllerState(ControllerIndex.One);

        ConnectionStateLabel.Text = controllerState.IsConnected ? "Connected" : "Disconnected";
        PacketNumberLabel.Text = controllerState.PacketNumber.ToString();
    }
}

internal sealed class ControllerStateComponentFactory : ComponentFactory<ControllerStateComponent>
{
    private readonly IInputBackend _inputBackend;

    public ControllerStateComponentFactory(IInputBackend inputBackend)
    {
        _inputBackend = inputBackend;
    }

    protected override ControllerStateComponent CreateComponent(Entity entity) => new(entity, _inputBackend);
}