using System;
using System.Globalization;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering.Components;

namespace Sandbox.GamepadController;

internal sealed class ControllerStateComponent : BehaviorComponent
{
    public ControllerStateComponent(Entity entity) : base(entity)
    {
    }

    public TextRendererComponent? ConnectionStateLabel { get; set; }
    public TextRendererComponent? PacketNumberLabel { get; set; }

    public override void OnUpdate(in TimeStep timeStep)
    {
        if (ConnectionStateLabel is null) return;
        if (PacketNumberLabel is null) return;

        PacketNumberLabel.Text = DateTime.Now.ToString(CultureInfo.InvariantCulture);
    }
}

internal sealed class ControllerStateComponentFactory : ComponentFactory<ControllerStateComponent>
{
    protected override ControllerStateComponent CreateComponent(Entity entity) => new(entity);
}