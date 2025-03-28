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
    private int _layout;

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
                        ActionName = "ChangeLayout",
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
            inputComponent.BindAction("ChangeLayout", ChangeLayout);
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
            case 0:
                Layout.RectangleColliders(Scene);
                break;
            case 1:
                Layout.CircleColliders(Scene);
                break;
            case 2:
                Layout.KinematicBodies(Scene);
                break;
            default:
                throw new InvalidOperationException("Unsupported Layout");
        }
    }

    private void ChangeLayout()
    {
        _layout = (_layout + 1) % 3;

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