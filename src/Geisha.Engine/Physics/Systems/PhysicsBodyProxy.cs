using System;
using System.Diagnostics;
using Geisha.Engine.Core.Collections;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Physics.Components;
using Geisha.Engine.Physics.PhysicsEngine2D;

namespace Geisha.Engine.Physics.Systems;

internal sealed class PhysicsBodyProxy : IDisposable
{
    private readonly RigidBody2D _body;

    private PhysicsBodyProxy(PhysicsScene2D physicsScene2D, Transform2DComponent transform, Collider2DComponent collider,
        KinematicRigidBody2DComponent? kinematicBodyComponent)
    {
        Transform = transform;
        Collider = collider;
        KinematicBodyComponent = kinematicBodyComponent;

        Collider.PhysicsBodyProxy = this;

        var bodyType = KinematicBodyComponent is null ? BodyType.Static : BodyType.Kinematic;

        _body = Collider switch
        {
            CircleColliderComponent circleColliderComponent => physicsScene2D.CreateBody(bodyType, circleColliderComponent.Radius),
            RectangleColliderComponent rectangleColliderComponent => physicsScene2D.CreateBody(bodyType, rectangleColliderComponent.Dimensions.ToSizeD()),
            TileColliderComponent => physicsScene2D.CreateTileBody(),
            _ => throw new InvalidOperationException($"Unsupported collider component type: {Collider.GetType()}.")
        };

        _body.Proxy = this;

        SynchronizeBody();
    }

    public static PhysicsBodyProxy CreateStatic(PhysicsScene2D physicsScene2D, Transform2DComponent transform, Collider2DComponent collider)
    {
        return new PhysicsBodyProxy(physicsScene2D, transform, collider, null);
    }

    public static PhysicsBodyProxy CreateKinematic(PhysicsScene2D physicsScene2D, Transform2DComponent transform, Collider2DComponent collider,
        KinematicRigidBody2DComponent? kinematicBodyComponent)
    {
        return new PhysicsBodyProxy(physicsScene2D, transform, collider, kinematicBodyComponent);
    }

    public Entity Entity => Transform.Entity;
    public Transform2DComponent Transform { get; }
    public Collider2DComponent Collider { get; }
    public KinematicRigidBody2DComponent? KinematicBodyComponent { get; }

    public bool IsColliding => _body.Contacts.Count > 0;

    public Contact2D[] GetContacts()
    {
        if (_body.Contacts.Count == 0)
        {
            return Array.Empty<Contact2D>();
        }

        var contacts = new Contact2D[_body.Contacts.Count];

        for (var i = 0; i < _body.Contacts.Count; i++)
        {
            var contact = _body.Contacts[i];
            var thisIsBody1 = _body == contact.Body1;
            var otherBody = thisIsBody1 ? contact.Body2 : contact.Body1;
            Debug.Assert(otherBody.Proxy != null, "otherBody.Proxy != null");

            FixedList2<ContactPoint2D> contactPoints2D = default;
            for (var j = 0; j < contact.ContactPoints.Count; j++)
            {
                var cp = contact.ContactPoints[j];
                var thisLocalPosition = thisIsBody1 ? cp.LocalPosition1 : cp.LocalPosition2;
                var otherLocalPosition = thisIsBody1 ? cp.LocalPosition2 : cp.LocalPosition1;

                // Convert local positions to be oriented according to body rotations.
                thisLocalPosition = (Matrix3x3.CreateRotation(-_body.Rotation) * thisLocalPosition.Homogeneous).ToVector2();
                otherLocalPosition = (Matrix3x3.CreateRotation(-otherBody.Rotation) * otherLocalPosition.Homogeneous).ToVector2();

                contactPoints2D.Add(new ContactPoint2D(cp.WorldPosition, thisLocalPosition, otherLocalPosition));
            }

            var collisionNormal = thisIsBody1 ? contact.CollisionNormal : -contact.CollisionNormal;

            contacts[i] = new Contact2D(Collider, otherBody.Proxy.Collider, collisionNormal, contact.PenetrationDepth, contactPoints2D.ToReadOnly());
        }

        return contacts;
    }

    public void Dispose()
    {
        Collider.PhysicsBodyProxy = null;
        _body.Scene.RemoveBody(_body);
    }

    internal void SynchronizeBody()
    {
        if (KinematicBodyComponent is not null)
        {
            _body.Position = Transform.Translation;
            _body.Rotation = Transform.Rotation;
            _body.LinearVelocity = KinematicBodyComponent.LinearVelocity;
            _body.AngularVelocity = KinematicBodyComponent.AngularVelocity;
            _body.EnableCollisionResponse = KinematicBodyComponent.EnableCollisionResponse;
        }
        else
        {
            if (Entity.IsRoot)
            {
                _body.Position = Transform.Translation;
                _body.Rotation = Transform.Rotation;
            }
            else
            {
                var finalMatrix = TransformHierarchy.Calculate2DTransformationMatrix(Entity);
                var finalTransform = finalMatrix.ToTransform();
                _body.Position = finalTransform.Translation;
                _body.Rotation = finalTransform.Rotation;
            }

            if (_body.ColliderType is ColliderType.Tile)
            {
                Transform.Translation = _body.Position;
                Transform.Rotation = _body.Rotation;
                Transform.Scale = Vector2.One; // Tile collider does not support scaling.
            }
        }

        switch (Collider)
        {
            case CircleColliderComponent circleColliderComponent:
                _body.SetCircleCollider(circleColliderComponent.Radius);
                break;
            case RectangleColliderComponent rectangleColliderComponent:
                _body.SetRectangleCollider(rectangleColliderComponent.Dimensions.ToSizeD());
                break;
            case TileColliderComponent:
                _body.SetTileCollider();
                break;
            default:
                throw new InvalidOperationException($"Unsupported collider component type: {Collider.GetType()}.");
        }
    }

    internal void SynchronizeComponents()
    {
        if (KinematicBodyComponent is null) return;

        Transform.Translation = _body.Position;
        Transform.Rotation = _body.Rotation;
        KinematicBodyComponent.LinearVelocity = _body.LinearVelocity;
        KinematicBodyComponent.AngularVelocity = _body.AngularVelocity;
    }
}