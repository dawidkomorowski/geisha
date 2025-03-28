using System;
using System.Linq;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Input;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Input.Mapping;
using Geisha.Engine.Physics.Components;

namespace Sandbox.Physics;

public sealed class EntityControllerComponent : BehaviorComponent
{
    private const double AngularVelocity = Math.PI / 4;
    private double _linearVelocity = 400;
    private double _sizeFactor = 1d;
    private string _type = "Square";
    private InputComponent _inputComponent = null!;
    private KinematicRigidBody2DComponent _kinematicBody = null!;

    public EntityControllerComponent(Entity entity) : base(entity)
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
                        ActionName = "ChangeToCircle",
                        HardwareActions =
                        {
                            new HardwareAction
                            {
                                HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(Key.F5)
                            }
                        }
                    },
                    new ActionMapping
                    {
                        ActionName = "ChangeToSquare",
                        HardwareActions =
                        {
                            new HardwareAction
                            {
                                HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(Key.F6)
                            }
                        }
                    },
                    new ActionMapping
                    {
                        ActionName = "ChangeToRectangle",
                        HardwareActions =
                        {
                            new HardwareAction
                            {
                                HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(Key.F7)
                            }
                        }
                    },
                    new ActionMapping
                    {
                        ActionName = "SizeIncrease",
                        HardwareActions =
                        {
                            new HardwareAction
                            {
                                HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(Key.CloseBrackets)
                            }
                        }
                    },
                    new ActionMapping
                    {
                        ActionName = "SizeDecrease",
                        HardwareActions =
                        {
                            new HardwareAction
                            {
                                HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(Key.OpenBrackets)
                            }
                        }
                    }
                }
            };

            inputComponent.BindAction("ChangeToCircle", () => SetCollider("Circle"));
            inputComponent.BindAction("ChangeToSquare", () => SetCollider("Square"));
            inputComponent.BindAction("ChangeToRectangle", () => SetCollider("Rectangle"));
            inputComponent.BindAction("SizeIncrease", () => UpdateSize(0.1));
            inputComponent.BindAction("SizeDecrease", () => UpdateSize(-0.1));
        }

        _inputComponent = Entity.GetComponent<InputComponent>();
        _kinematicBody = Entity.GetComponent<KinematicRigidBody2DComponent>();

        UpdateInfoComponent();
    }

    public override void OnFixedUpdate()
    {
        var linearVelocity = Vector2.Zero;
        var angularVelocity = 0d;

        if (_inputComponent.HardwareInput.KeyboardInput is { LeftCtrl: false, Up: true })
        {
            linearVelocity += Vector2.UnitY;
        }

        if (_inputComponent.HardwareInput.KeyboardInput is { LeftCtrl: false, Down: true })
        {
            linearVelocity += -Vector2.UnitY;
        }

        if (_inputComponent.HardwareInput.KeyboardInput.Right)
        {
            linearVelocity += Vector2.UnitX;
        }

        if (_inputComponent.HardwareInput.KeyboardInput.Left)
        {
            linearVelocity += -Vector2.UnitX;
        }

        if (_inputComponent.HardwareInput.KeyboardInput.X)
        {
            angularVelocity -= AngularVelocity;
        }

        if (_inputComponent.HardwareInput.KeyboardInput.Z)
        {
            angularVelocity += AngularVelocity;
        }

        if (_inputComponent.HardwareInput.KeyboardInput is { LeftCtrl: true, Up: true })
        {
            _linearVelocity += 5;
            UpdateInfoComponent();
        }

        if (_inputComponent.HardwareInput.KeyboardInput is { LeftCtrl: true, Down: true })
        {
            _linearVelocity -= 5;
            UpdateInfoComponent();
        }

        _kinematicBody.LinearVelocity = linearVelocity.OfLength(_linearVelocity);
        _kinematicBody.AngularVelocity = angularVelocity;
    }

    private void SetCollider(string type)
    {
        _type = type;

        foreach (var component in Entity.Components.ToArray())
        {
            if (component is Collider2DComponent)
            {
                Entity.RemoveComponent(component);
            }
        }

        switch (type)
        {
            case "Circle":
            {
                var circleColliderComponent = Entity.CreateComponent<CircleColliderComponent>();
                circleColliderComponent.Radius = 50 * _sizeFactor;
                break;
            }
            case "Square":
            {
                var rectangleColliderComponent = Entity.CreateComponent<RectangleColliderComponent>();
                rectangleColliderComponent.Dimensions = new Vector2(100 * _sizeFactor, 100 * _sizeFactor);
                break;
            }
            case "Rectangle":
            {
                var rectangleColliderComponent = Entity.CreateComponent<RectangleColliderComponent>();
                rectangleColliderComponent.Dimensions = new Vector2(200 * _sizeFactor, 100 * _sizeFactor);
                break;
            }
            default:
                throw new InvalidOperationException($"Unsupported type: {type}");
        }
    }

    private void UpdateSize(double delta)
    {
        _sizeFactor = Math.Max(0.1, Math.Min(2.0, _sizeFactor + delta));
        SetCollider(_type);
    }

    private void UpdateInfoComponent()
    {
        var infoComponent = Scene.RootEntities.Single(e => e.HasComponent<InfoComponent>()).GetComponent<InfoComponent>();
        infoComponent.OnLinearVelocity(_linearVelocity);
    }
}

public sealed class EntityControllerComponentFactory : ComponentFactory<EntityControllerComponent>
{
    protected override EntityControllerComponent CreateComponent(Entity entity) => new(entity);
}