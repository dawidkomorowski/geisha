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
    private RigidBody2D? _body;

    private PhysicsBodyProxy(Transform2DComponent transform, Collider2DComponent collider, KinematicRigidBody2DComponent? kinematicBodyComponent)
    {
        Transform = transform;
        Collider = collider;
        KinematicBodyComponent = kinematicBodyComponent;
    }

    public static PhysicsBodyProxy CreateStatic(Transform2DComponent transform, Collider2DComponent collider)
    {
        return new PhysicsBodyProxy(transform, collider, null);
    }

    public static PhysicsBodyProxy CreateKinematic(Transform2DComponent transform, Collider2DComponent collider,
        KinematicRigidBody2DComponent? kinematicBodyComponent)
    {
        return new PhysicsBodyProxy(transform, collider, kinematicBodyComponent);
    }

    public Entity Entity => Transform.Entity;
    public Transform2DComponent Transform { get; }
    public Collider2DComponent Collider { get; }
    public KinematicRigidBody2DComponent? KinematicBodyComponent { get; }

    public void CreateInternalBody(PhysicsScene2D physicsScene2D)
    {
        Debug.Assert(_body == null, "_body == null");

        var bodyType = KinematicBodyComponent is null ? BodyType.Static : BodyType.Kinematic;

        _body = Collider switch
        {
            CircleColliderComponent circleColliderComponent
                => physicsScene2D.CreateBody(bodyType, new Circle(circleColliderComponent.Radius)),
            RectangleColliderComponent rectangleColliderComponent
                => physicsScene2D.CreateBody(bodyType, new AxisAlignedRectangle(rectangleColliderComponent.Dimensions)),
            _
                => throw new InvalidOperationException($"Unsupported collider component type: {Collider.GetType()}.")
        };

        _body.CustomData = this;

        SynchronizeBody();
    }

    public void Dispose()
    {
        Debug.Assert(_body != null, "_body != null");

        Collider.ClearContacts();
        _body.Scene.RemoveBody(_body);
    }

    public void SynchronizeBody()
    {
        Debug.Assert(_body != null, nameof(_body) + " != null");

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
            // TODO How to support hierarchy of static colliders? Is it enough? It does not support scale.
            var finalMatrix = TransformHierarchy.Calculate2DTransformationMatrix(Entity);
            var finalTransform = finalMatrix.ToTransform();
            _body.Position = finalTransform.Translation;
            _body.Rotation = finalTransform.Rotation;
        }

        switch (Collider)
        {
            case CircleColliderComponent circleColliderComponent:
                _body.SetCollider(new Circle(circleColliderComponent.Radius));
                break;
            case RectangleColliderComponent rectangleColliderComponent:
                _body.SetCollider(new AxisAlignedRectangle(rectangleColliderComponent.Dimensions));
                break;
            default:
                throw new InvalidOperationException($"Unsupported collider component type: {Collider.GetType()}.");
        }
    }

    public void SynchronizeComponents()
    {
        Debug.Assert(_body != null, nameof(_body) + " != null");

        Collider.ClearContacts();

        for (var i = 0; i < _body.Contacts.Count; i++)
        {
            var contact = _body.Contacts[i];
            var thisIsBody1 = _body == contact.Body1;
            var otherBody = thisIsBody1 ? contact.Body2 : contact.Body1;
            Debug.Assert(otherBody.CustomData != null, "otherBody.CustomData != null");
            var otherProxy = (PhysicsBodyProxy)otherBody.CustomData;

            FixedList2<ContactPoint2D> contactPoints2D = default;
            for (int j = 0; j < contact.ContactPoints.Count; j++)
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

            var contact2D = new Contact2D(Collider, otherProxy.Collider, collisionNormal, contact.PenetrationDepth, contactPoints2D.ToReadOnly());
            Collider.AddContact(contact2D);
        }

        if (KinematicBodyComponent is not null)
        {
            Transform.Translation = _body.Position;
            Transform.Rotation = _body.Rotation;
            KinematicBodyComponent.LinearVelocity = _body.LinearVelocity;
            KinematicBodyComponent.AngularVelocity = _body.AngularVelocity;
        }
    }
}