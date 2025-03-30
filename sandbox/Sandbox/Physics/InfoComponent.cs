using System.Linq;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Input;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Input.Mapping;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Components;

namespace Sandbox.Physics;

public sealed class InfoComponent : BehaviorComponent
{
    private bool _showInfo = true;
    private double _spawnSizeFactor;
    private double _linearVelocity;
    private string _movementType = string.Empty;

    public InfoComponent(Entity entity) : base(entity)
    {
    }

    public override void OnStart()
    {
        if (!Entity.HasComponent<Transform2DComponent>())
        {
            Entity.CreateComponent<Transform2DComponent>();
        }

        if (!Entity.HasComponent<InputComponent>())
        {
            var inputComponent = Entity.CreateComponent<InputComponent>();
            inputComponent.InputMapping = new InputMapping
            {
                ActionMappings =
                {
                    new ActionMapping
                    {
                        ActionName = "ToggleInfo",
                        HardwareActions =
                        {
                            new HardwareAction
                            {
                                HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(Key.Tab)
                            }
                        }
                    }
                }
            };

            inputComponent.BindAction("ToggleInfo", ToggleInfo);
        }

        if (!Entity.HasComponent<TextRendererComponent>())
        {
            var textRendererComponent = Entity.CreateComponent<TextRendererComponent>();
            textRendererComponent.SortingLayerName = "UI";
            textRendererComponent.Text = "Info";
            textRendererComponent.OrderInLayer = 1;
            textRendererComponent.TextAlignment = TextAlignment.Leading;
            textRendererComponent.ParagraphAlignment = ParagraphAlignment.Near;
        }

        if (!Entity.HasComponent<RectangleRendererComponent>())
        {
            var background = Entity.CreateChildEntity();
            background.CreateComponent<Transform2DComponent>();
            var rectangleRendererComponent = background.CreateComponent<RectangleRendererComponent>();
            rectangleRendererComponent.SortingLayerName = "UI";
            rectangleRendererComponent.Color = Color.FromArgb(255, 128, 128, 128);
            rectangleRendererComponent.Dimensions = new Vector2(200, 200);
            rectangleRendererComponent.FillInterior = true;
        }

        ToggleInfo();
    }

    public void OnSpawnSizeFactor(double spawnSizeFactor)
    {
        _spawnSizeFactor = spawnSizeFactor;
        ToggleInfo();
        ToggleInfo();
    }

    public void OnLinearVelocity(double linearVelocity)
    {
        _linearVelocity = linearVelocity;
        ToggleInfo();
        ToggleInfo();
    }

    public void OnMovementType(string movementType)
    {
        _movementType = movementType;
        ToggleInfo();
        ToggleInfo();
    }

    private void ToggleInfo()
    {
        _showInfo = !_showInfo;

        if (_showInfo)
        {
            SetInfo($@"TAB                 Hide info panel

LAYOUT MANAGEMENT
-----------------
1-4                 Load predefined layout
F1                  Spawn square
F2                  Spawn circle
F3                  Spawn wide rectangle
F4                  Spawn tall rectangle
RMB                 Delete entity
Scroll              Change spawn size factor

ENTITY MANAGEMENT
-----------------
BACKSPACE           Reset position
CTRL + UP/DOWN      Change linear velocity
F5                  Change to circle
F6                  Change to square
F7                  Change to rectangle
[/]                 Change size
\                   Change movement type

FREE CONTROLS
-------------
UP/DOWN/LEFT/RIGHT  Move entity
Z/X                 Rotate entity

PLATFORM CONTROLS
-----------------
LEFT/RIGHT          Move entity
UP                  Jump

SETTINGS
--------
SpawnSizeFactor     {_spawnSizeFactor:F1}
LinearVelocity      {_linearVelocity:F1}
MovementType        {_movementType}");
        }
        else
        {
            SetInfo("TAB - Show info panel");
        }
    }

    private void SetInfo(string info)
    {
        var textRendererComponent = Entity.GetComponent<TextRendererComponent>();
        textRendererComponent.MaxWidth = 1000;
        textRendererComponent.Text = info;
        ResizeBackgroundToText();
    }

    private void ResizeBackgroundToText()
    {
        var textRendererComponent = Entity.GetComponent<TextRendererComponent>();
        var textMetrics = textRendererComponent.TextMetrics;
        textRendererComponent.MaxWidth = textMetrics.Width;
        textRendererComponent.MaxHeight = textMetrics.Height;
        textRendererComponent.Pivot = new Vector2(textMetrics.Width / 2, textMetrics.Height / 2);
        var rectangleRendererComponent = Entity.Children[0].GetComponent<RectangleRendererComponent>();
        var margin = new Vector2(20, 20);
        rectangleRendererComponent.Dimensions = textRendererComponent.LayoutRectangle.Dimensions + margin;

        var transform2DComponent = Entity.GetComponent<Transform2DComponent>();
        var cameraComponent = Scene.RootEntities.Single(e => e.HasComponent<CameraComponent>()).GetComponent<CameraComponent>();
        transform2DComponent.Translation = cameraComponent.ViewRectangle / 2 - rectangleRendererComponent.Dimensions / 2;
    }
}

public sealed class InfoComponentFactory : ComponentFactory<InfoComponent>
{
    protected override InfoComponent CreateComponent(Entity entity) => new(entity);
}