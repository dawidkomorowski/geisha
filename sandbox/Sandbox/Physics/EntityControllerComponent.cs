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
    private string _movementType = "Free";
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
                    },
                    new ActionMapping
                    {
                        ActionName = "ChangeMovementType",
                        HardwareActions =
                        {
                            new HardwareAction
                            {
                                HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(Key.Backslash)
                            }
                        }
                    },
                    new ActionMapping
                    {
                        ActionName = "ResetPosition",
                        HardwareActions =
                        {
                            new HardwareAction
                            {
                                HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(Key.Backspace)
                            }
                        }
                    },
                    new ActionMapping
                    {
                        ActionName = "ToggleCollisionResponse",
                        HardwareActions =
                        {
                            new HardwareAction
                            {
                                HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(Key.EqualsSign)
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
            inputComponent.BindAction("ChangeMovementType", ChangeMovement);
            inputComponent.BindAction("ResetPosition", ResetPosition);
            inputComponent.BindAction("ToggleCollisionResponse", ToggleCollisionResponse);
        }

        _inputComponent = Entity.GetComponent<InputComponent>();
        _kinematicBody = Entity.GetComponent<KinematicRigidBody2DComponent>();

        UpdateInfoComponent();
    }

    public override void OnFixedUpdate()
    {
        if (_inputComponent.HardwareInput.KeyboardInput is { LeftCtrl: true, Up: true })
        {
            _linearVelocity += 5;
            UpdateInfoComponent();
        }

        if (_inputComponent.HardwareInput.KeyboardInput is { LeftCtrl: true, Down: true })
        {
            _linearVelocity -= 5;
            _linearVelocity = Math.Max(0, _linearVelocity);
            UpdateInfoComponent();
        }

        switch (_movementType)
        {
            case "Free":
                ProcessFreeMovement();
                break;
            case "Platform":
                ProcessPlatformMovement();
                break;
            default:
                throw new InvalidOperationException($"Unsupported movement type: {_movementType}");
        }
    }

    private void ProcessFreeMovement()
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

        _kinematicBody.LinearVelocity = linearVelocity.OfLength(_linearVelocity);
        _kinematicBody.AngularVelocity = angularVelocity;
    }

    private void ProcessPlatformMovement()
    {
        var linearVelocity = _kinematicBody.LinearVelocity.WithX(0);

        if (_inputComponent.HardwareInput.KeyboardInput.Right)
        {
            linearVelocity += Vector2.UnitX.OfLength(_linearVelocity);
        }

        if (_inputComponent.HardwareInput.KeyboardInput.Left)
        {
            linearVelocity += -Vector2.UnitX.OfLength(_linearVelocity);
        }

        const double gravity = 100;
        linearVelocity = linearVelocity.WithY(linearVelocity.Y - gravity);

        Collider2DComponent colliderComponent = _type switch
        {
            "Circle" => Entity.GetComponent<CircleColliderComponent>(),
            "Square" => Entity.GetComponent<RectangleColliderComponent>(),
            "Rectangle" => Entity.GetComponent<RectangleColliderComponent>(),
            _ => throw new InvalidOperationException($"Unsupported type: {_type}")
        };

        var canJump = false;

        // TODO This is a temporary solution to prevent entity from falling through the platform. It should be replaced with proper collision handling by Physics System.
        if (colliderComponent.IsColliding)
        {
            foreach (var contact in colliderComponent.Contacts)
            {
                if (contact.CollisionNormal.Y > 0 && linearVelocity.Y <= 0)
                {
                    linearVelocity = linearVelocity.WithY(0);
                    canJump = true;
                    break;
                }

                // It prevents entity from sticking to the bottom of the platform.
                if (contact.CollisionNormal.Y < 0 && linearVelocity.Y > 0)
                {
                    linearVelocity = linearVelocity.WithY(0);
                }
            }
        }

        if (canJump && _inputComponent.HardwareInput.KeyboardInput is { LeftCtrl: false, Up: true })
        {
            const double jumpForce = 2000;
            linearVelocity = linearVelocity.WithY(jumpForce);
        }

        _kinematicBody.LinearVelocity = linearVelocity;
        _kinematicBody.AngularVelocity = 0d;
    }

    private void ChangeMovement()
    {
        _movementType = _movementType switch
        {
            "Free" => "Platform",
            "Platform" => "Free",
            _ => throw new InvalidOperationException($"Unsupported movement type: {_movementType}")
        };

        UpdateInfoComponent();
    }

    private void ResetPosition()
    {
        Entity.GetComponent<Transform2DComponent>().Transform = Transform2D.Identity;
        _kinematicBody.LinearVelocity = Vector2.Zero;
        _kinematicBody.AngularVelocity = 0;
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

    private void ToggleCollisionResponse()
    {
        _kinematicBody.EnableCollisionResponse = !_kinematicBody.EnableCollisionResponse;
    }

    private void UpdateInfoComponent()
    {
        var infoComponent = Scene.RootEntities.Single(e => e.HasComponent<InfoComponent>()).GetComponent<InfoComponent>();
        infoComponent.OnLinearVelocity(_linearVelocity);
        infoComponent.OnMovementType(_movementType);
    }
}

public sealed class EntityControllerComponentFactory : ComponentFactory<EntityControllerComponent>
{
    protected override EntityControllerComponent CreateComponent(Entity entity) => new(entity);
}