using System;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Input;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Input.Mapping;

namespace Sandbox.Physics;

public sealed class LayoutComponent : BehaviorComponent
{
    private Entity[] _layoutEntities = Array.Empty<Entity>();
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
        if (_layoutEntities.Length == 0)
        {
            _layoutEntities = _layout switch
            {
                0 => Layout.RectangleColliders(Entity.Scene),
                1 => Layout.CircleColliders(Entity.Scene),
                2 => Layout.KinematicBodies(Entity.Scene),
                _ => _layoutEntities
            };
        }
        else
        {
            if (_layoutEntities[0].IsRemoved)
            {
                _layoutEntities = Array.Empty<Entity>();
            }
        }
    }

    private void ChangeLayout()
    {
        _layout = (_layout + 1) % 3;

        foreach (var entity in _layoutEntities)
        {
            entity.RemoveAfterFixedTimeStep();
        }
    }
}

public sealed class LayoutComponentFactory : ComponentFactory<LayoutComponent>
{
    protected override LayoutComponent CreateComponent(Entity entity) => new(entity);
}