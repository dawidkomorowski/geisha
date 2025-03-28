using System;
using System.Linq;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Input;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Input.Mapping;
using Geisha.Engine.Physics.Components;
using Geisha.Engine.Rendering.Components;

namespace Sandbox.Physics;

public sealed class LayoutControllerComponent : BehaviorComponent
{
    private int _layout = 1;

    public LayoutControllerComponent(Entity entity) : base(entity)
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
                    },
                    new ActionMapping
                    {
                        ActionName = "DeleteEntity",
                        HardwareActions =
                        {
                            new HardwareAction
                            {
                                HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(Key.Delete)
                            },
                            new HardwareAction
                            {
                                HardwareInputVariant = HardwareInputVariant.CreateMouseVariant(HardwareInputVariant.MouseVariant.RightButton)
                            }
                        }
                    },
                    new ActionMapping
                    {
                        ActionName = "SpawnSquare",
                        HardwareActions =
                        {
                            new HardwareAction
                            {
                                HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(Key.F1)
                            }
                        }
                    },
                    new ActionMapping
                    {
                        ActionName = "SpawnCircle",
                        HardwareActions =
                        {
                            new HardwareAction
                            {
                                HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(Key.F2)
                            }
                        }
                    }
                }
            };
            inputComponent.BindAction("SetLayout1", () => SetLayout(1));
            inputComponent.BindAction("SetLayout2", () => SetLayout(2));
            inputComponent.BindAction("SetLayout3", () => SetLayout(3));
            inputComponent.BindAction("DeleteEntity", DeleteEntity);
            inputComponent.BindAction("SpawnSquare", () => SpawnRectangleStaticBody(100, 100));
            inputComponent.BindAction("SpawnCircle", () => SpawnCircleStaticBody(50));
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
                throw new InvalidOperationException($"Unsupported layout: {_layout}");
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

    private void DeleteEntity()
    {
        var mousePosition = GetMousePosition();

        // TODO Create issue for adding API for hit-testing colliders.
        // TODO Create issue for adding API for world queries for physics.
        // TODO Is removal of all dynamic physics entities needed? It does not work right now.
        foreach (var entity in Scene.RootEntities.Where(e => e.HasComponent<DynamicPhysicsEntityComponent>()))
        {
            if (entity.HasComponent<RectangleColliderComponent>())
            {
                var rectangleColliderComponent = entity.GetComponent<RectangleColliderComponent>();
                var transform2DComponent = entity.GetComponent<Transform2DComponent>();
                var rectangle = new Rectangle(rectangleColliderComponent.Dimensions).Transform(transform2DComponent.ToMatrix());

                if (rectangle.Contains(mousePosition))
                {
                    entity.RemoveAfterFixedTimeStep();
                }
            }

            if (entity.HasComponent<CircleColliderComponent>())
            {
                var circleColliderComponent = entity.GetComponent<CircleColliderComponent>();
                var transform2DComponent = entity.GetComponent<Transform2DComponent>();
                var circle = new Circle(circleColliderComponent.Radius).Transform(transform2DComponent.ToMatrix());

                if (circle.Contains(mousePosition))
                {
                    entity.RemoveAfterFixedTimeStep();
                }
            }
        }
    }

    private void SpawnRectangleStaticBody(double w, double h)
    {
        var mousePosition = GetMousePosition();
        PhysicsEntityFactory.CreateRectangleStaticBody(Scene, mousePosition.X, mousePosition.Y, w, h);
    }

    private void SpawnCircleStaticBody(double r)
    {
        var mousePosition = GetMousePosition();
        PhysicsEntityFactory.CreateCircleStaticBody(Scene, mousePosition.X, mousePosition.Y, r);
    }

    private Vector2 GetMousePosition()
    {
        var inputComponent = Entity.GetComponent<InputComponent>();
        var cameraComponent = Scene.RootEntities.Single(e => e.HasComponent<CameraComponent>()).GetComponent<CameraComponent>();

        var mousePosition = inputComponent.HardwareInput.MouseInput.Position;
        return cameraComponent.ScreenPointToWorld2DPoint(mousePosition);
    }
}

public sealed class LayoutControllerComponentFactory : ComponentFactory<LayoutControllerComponent>
{
    protected override LayoutControllerComponent CreateComponent(Entity entity) => new(entity);
}