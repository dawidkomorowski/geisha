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
    public SpriteRendererComponent? ButonBImage { get; set; }
    public SpriteRendererComponent? ButonXImage { get; set; }
    public SpriteRendererComponent? ButonYImage { get; set; }

    public SpriteRendererComponent? DPadUpImage { get; set; }
    public SpriteRendererComponent? DPadDownImage { get; set; }
    public SpriteRendererComponent? DPadLeftImage { get; set; }
    public SpriteRendererComponent? DPadRightImage { get; set; }

    public SpriteRendererComponent? StartImage { get; set; }
    public SpriteRendererComponent? BackImage { get; set; }

    public SpriteRendererComponent? LeftStrickImage { get; set; }
    public Transform2DComponent? LeftStickTransform { get; set; }

    public SpriteRendererComponent? RightStickImage { get; set; }
    public Transform2DComponent? RightStickTransform { get; set; }

    public override void OnUpdate(in TimeStep timeStep)
    {
        if (ConnectionStateLabel is null) return;
        if (PacketNumberLabel is null) return;

        if (ButonAImage is null) return;
        if (ButonBImage is null) return;
        if (ButonXImage is null) return;
        if (ButonYImage is null) return;

        if (DPadUpImage is null) return;
        if (DPadDownImage is null) return;
        if (DPadLeftImage is null) return;
        if (DPadRightImage is null) return;

        if (StartImage is null) return;
        if (BackImage is null) return;

        if (LeftStrickImage is null) return;
        if (LeftStickTransform is null) return;

        if (RightStickImage is null) return;
        if (RightStickTransform is null) return;

        var controllerState = _inputBackend.GetControllerState(ControllerIndex.One);

        ConnectionStateLabel.Text = controllerState.IsConnected ? "Connected" : "Disconnected";
        PacketNumberLabel.Text = controllerState.PacketNumber.ToString();

        ButonAImage.Sprite = controllerState.ButtonA
            ? _assetStore.GetAsset<Sprite>(AssetId.Parse("ce9750b2-c9b3-4e4a-8bfa-4ff1f2ed5e65"))
            : _assetStore.GetAsset<Sprite>(AssetId.Parse("ea57a9e0-815e-4789-b48c-64b33e14c14f"));

        ButonBImage.Sprite = controllerState.ButtonB
            ? _assetStore.GetAsset<Sprite>(AssetId.Parse("3a9ff6f9-cecf-4f15-ad99-f6f557682f7a"))
            : _assetStore.GetAsset<Sprite>(AssetId.Parse("e1761492-b779-4a4f-871e-188356feed46"));

        ButonXImage.Sprite = controllerState.ButtonX
            ? _assetStore.GetAsset<Sprite>(AssetId.Parse("a6d8b6f3-f246-42fe-9c14-25159614d5fb"))
            : _assetStore.GetAsset<Sprite>(AssetId.Parse("c4949127-9b35-4b96-b80c-5bbbe7dd130b"));

        ButonYImage.Sprite = controllerState.ButtonY
            ? _assetStore.GetAsset<Sprite>(AssetId.Parse("1ae66fce-c0f9-474e-bddf-7718d24d219d"))
            : _assetStore.GetAsset<Sprite>(AssetId.Parse("ef1ad0db-accb-4cf7-9612-faa854a17f2f"));

        DPadUpImage.Visible = controllerState.ButtonDPadUp;
        DPadDownImage.Visible = controllerState.ButtonDPadDown;
        DPadLeftImage.Visible = controllerState.ButtonDPadLeft;
        DPadRightImage.Visible = controllerState.ButtonDPadRight;

        StartImage.Sprite = controllerState.ButtonStart
            ? _assetStore.GetAsset<Sprite>(AssetId.Parse("2581e31e-3aa7-449d-8c2f-9eda5619a4cc"))
            : _assetStore.GetAsset<Sprite>(AssetId.Parse("35a90976-0ffc-4bfd-86b4-be05dcf13f1c"));

        BackImage.Sprite = controllerState.ButtonBack
            ? _assetStore.GetAsset<Sprite>(AssetId.Parse("448abad2-aec2-4c23-977c-7db72df81394"))
            : _assetStore.GetAsset<Sprite>(AssetId.Parse("78b192ed-8121-4b89-9d20-9c3708e1a9b1"));

        LeftStrickImage.Sprite = controllerState.ButtonLeftThumb
            ? _assetStore.GetAsset<Sprite>(AssetId.Parse("f785cc71-7078-44da-b5df-0d59927e60b7"))
            : _assetStore.GetAsset<Sprite>(AssetId.Parse("6cbfa428-2af1-4333-bbb6-95f301b18d5d"));
        LeftStickTransform.Translation = controllerState.LeftThumbPosition * 25;

        RightStickImage.Sprite = controllerState.ButtonRightThumb
            ? _assetStore.GetAsset<Sprite>(AssetId.Parse("8e9efdfd-201d-40fe-a47a-a4c0f1b9bac4"))
            : _assetStore.GetAsset<Sprite>(AssetId.Parse("05dd403a-ab7a-41dd-98ea-e60f7fc8673b"));
        RightStickTransform.Translation = controllerState.RightThumbPosition * 25;
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