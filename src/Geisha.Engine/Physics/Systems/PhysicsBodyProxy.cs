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
    private readonly PhysicsSystemState _physicsSystemState;
    private readonly PhysicsScene2D _physicsScene;
    private readonly RigidBody2D _body;

    private PhysicsBodyProxy(PhysicsSystemState physicsSystemState, in PhysicsScene2D physicsScene2D,
        Transform2DComponent transform, Collider2DComponent collider, KinematicRigidBody2DComponent? kinematicBodyComponent)
    {
        _physicsSystemState = physicsSystemState;
        _physicsScene = physicsScene2D;

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

        SynchronizeBody();
    }

    public static PhysicsBodyProxy CreateStatic(PhysicsSystemState physicsSystemState, in PhysicsScene2D physicsScene2D,
        Transform2DComponent transform, Collider2DComponent collider)
    {
        return new PhysicsBodyProxy(physicsSystemState, physicsScene2D, transform, collider, null);
    }

    public static PhysicsBodyProxy CreateKinematic(PhysicsSystemState physicsSystemState, in PhysicsScene2D physicsScene2D,
        Transform2DComponent transform, Collider2DComponent collider, KinematicRigidBody2DComponent? kinematicBodyComponent)
    {
        return new PhysicsBodyProxy(physicsSystemState, physicsScene2D, transform, collider, kinematicBodyComponent);
    }

    public Entity Entity => Transform.Entity;
    public Transform2DComponent Transform { get; }
    public Collider2DComponent Collider { get; }
    public KinematicRigidBody2DComponent? KinematicBodyComponent { get; }

    public RigidBodyId RigidBodyId => _body.Id;
    public RigidBody2D RigidBody => _body;

    public AxisAlignedRectangle BoundingRectangle => _body.BoundingRectangle;
    public int ContactCount => _body.ContactCount;

    public int GetContacts(Span<Contact2D> contacts)
    {
        var writeCount = Math.Min(_body.ContactCount, contacts.Length);

        Span<Contact> bodyContacts = stackalloc Contact[writeCount];
        var internalWriteCount = _body.GetContacts(bodyContacts);

        Debug.Assert(writeCount == internalWriteCount, "Unexpected number of internal contacts.");

        for (var i = 0; i < writeCount; i++)
        {
            var contact = bodyContacts[i];
            var contactManifold = contact.ContactManifold;

            var thisIsBody1 = _body.Id == contact.Body1Id;
            var otherBody = thisIsBody1 ? RigidBody2D.GetById(contact.Body2Id) : RigidBody2D.GetById(contact.Body1Id);
            var otherProxy = _physicsSystemState.GetProxyById(otherBody.Id);

            FixedList2<ContactPoint2D> contactPoints2D = default;
            for (var j = 0; j < contactManifold.ContactPoints.Count; j++)
            {
                var cp = contactManifold.ContactPoints[j];
                var thisLocalPosition = thisIsBody1 ? cp.LocalPosition1 : cp.LocalPosition2;
                var otherLocalPosition = thisIsBody1 ? cp.LocalPosition2 : cp.LocalPosition1;

                // Convert local positions to be oriented according to body rotations.
                thisLocalPosition = (Matrix3x3.CreateRotation(-_body.Rotation) * thisLocalPosition.Homogeneous).ToVector2();
                otherLocalPosition = (Matrix3x3.CreateRotation(-otherBody.Rotation) * otherLocalPosition.Homogeneous).ToVector2();

                contactPoints2D.Add(new ContactPoint2D(cp.WorldPosition, thisLocalPosition, otherLocalPosition));
            }

            var collisionNormal = thisIsBody1 ? contactManifold.CollisionNormal : -contactManifold.CollisionNormal;

            contacts[i] = new Contact2D(Collider, otherProxy.Collider, collisionNormal, contactManifold.PenetrationDepth, contactPoints2D.ToReadOnly());
        }

        return writeCount;
    }

    public bool ContainsPoint(in Vector2 point) => _body.ContainsPoint(point);
    public bool Overlaps(in AxisAlignedRectangle axisAlignedRectangle) => _body.Overlaps(axisAlignedRectangle);
    public bool Overlaps(in Circle circle) => _body.Overlaps(circle);
    public bool Overlaps(in Rectangle rectangle) => _body.Overlaps(rectangle);

    public void Dispose()
    {
        Collider.PhysicsBodyProxy = null;
        _physicsScene.DestroyBody(_body);
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
                var finalMatrix = Transform.ComputeWorldTransformMatrix();
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

        _body.EnableCollisionDetection = Collider.Enabled;
        _body.IsSensor = Collider.IsSensor;
        _body.CollisionLayer = Collider.CollisionLayer.Value;
        _body.CollisionMask = Collider.CollisionMask.Value;

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