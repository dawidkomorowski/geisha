using System;
using System.Linq;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Input;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Input.Mapping;

namespace Sandbox.Physics;

public sealed class LayoutComponent : BehaviorComponent
{
    private int _layout = 1;

    public LayoutComponent(Entity entity) : base(entity)
    {
    }

    public override void OnStart()
    {
        if (!Entity.HasComponent<InputComponent>())
        {
            var inputComponent = Entity.CreateComponent<InputComponent>();
            inputComponent.InputMapping = new InputMapping
            {
                ActionMappings =
                {
                    new ActionMapping
                    {
                        ActionName = "SetLayout1",
                        HardwareActions =
                        {
                            new HardwareAction
                            {
                                HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(Key.D1)
                            }
                        }
                    },
                    new ActionMapping
                    {
                        ActionName = "SetLayout2",
                        HardwareActions =
                        {
                            new HardwareAction
                            {
                                HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(Key.D2)
                            }
                        }
                    },
                    new ActionMapping
                    {
                        ActionName = "SetLayout3",
                        HardwareActions =
                        {
                            new HardwareAction
                            {
                                HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(Key.D3)
                            }
                        }
                    }
                }
            };
            inputComponent.BindAction("SetLayout1", () => SetLayout(1));
            inputComponent.BindAction("SetLayout2", () => SetLayout(2));
            inputComponent.BindAction("SetLayout3", () => SetLayout(3));
        }
    }

    public override void OnFixedUpdate()
    {
        if (Scene.RootEntities.Any(e => e.HasComponent<DynamicPhysicsEntityComponent>()))
        {
            return;
        }

        switch (_layout)
        {
            case 1:
                Layout.RectangleColliders(Scene);
                break;
            case 2:
                Layout.CircleColliders(Scene);
                break;
            case 3:
                Layout.KinematicBodies(Scene);
                break;
            default:
                throw new InvalidOperationException($"Unsupported Layout: {_layout}");
        }
    }

    private void SetLayout(int layout)
    {
        _layout = layout;

        foreach (var entity in Scene.RootEntities.Where(e => e.HasComponent<DynamicPhysicsEntityComponent>()))
        {
            entity.RemoveAfterFixedTimeStep();
        }
    }
}

public sealed class LayoutComponentFactory : ComponentFactory<LayoutComponent>
{
    protected override LayoutComponent CreateComponent(Entity entity) => new(entity);
}