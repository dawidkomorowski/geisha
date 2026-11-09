using Geisha.Engine.Core;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Input.Backend;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Components;

namespace Sandbox.GamepadController;

internal sealed class ControllerStateComponent : BehaviorComponent
{
    private readonly IInputBackend _inputBackend;
    private readonly IAssetStore _assetStore;

    public ControllerStateComponent(Entity entity, IInputBackend inputBackend, IAssetStore assetStore) : base(entity)
    {
        _inputBackend = inputBackend;
        _assetStore = assetStore;
    }

    public TextRendererComponent? ConnectionStateLabel { get; set; }
    public TextRendererComponent? PacketNumberLabel { get; set; }

    public SpriteRendererComponent? ButonAImage { get; set; }

    public override void OnUpdate(in TimeStep timeStep)
    {
        if (ConnectionStateLabel is null) return;
        if (PacketNumberLabel is null) return;

        if (ButonAImage is null) return;

        var controllerState = _inputBackend.GetControllerState(ControllerIndex.One);

        ConnectionStateLabel.Text = controllerState.IsConnected ? "Connected" : "Disconnected";
        PacketNumberLabel.Text = controllerState.PacketNumber.ToString();

        ButonAImage.Sprite = (controllerState.Buttons & ControllerButtons.A) != ControllerButtons.None
            ? _assetStore.GetAsset<Sprite>(AssetId.Parse("ce9750b2-c9b3-4e4a-8bfa-4ff1f2ed5e65"))
            : _assetStore.GetAsset<Sprite>(AssetId.Parse("ea57a9e0-815e-4789-b48c-64b33e14c14f"));
    }
}

internal sealed class ControllerStateComponentFactory : ComponentFactory<ControllerStateComponent>
{
    private readonly IInputBackend _inputBackend;
    private readonly IAssetStore _assetStore;

    public ControllerStateComponentFactory(IInputBackend inputBackend, IAssetStore assetStore)
    {
        _inputBackend = inputBackend;
        _assetStore = assetStore;
    }

    protected override ControllerStateComponent CreateComponent(Entity entity) => new(entity, _inputBackend, _assetStore);
}