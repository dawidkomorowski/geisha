using System;
using System.Collections.Generic;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Physics.Components;

namespace Geisha.Engine.Physics.Systems;

internal sealed class KinematicBody : IDisposable
{
    public KinematicBody(Transform2DComponent transform, Collider2DComponent collider, KinematicRigidBody2DComponent kinematicBodyComponent)
    {
        Transform = transform;
        Collider = collider;
        KinematicBodyComponent = kinematicBodyComponent;

        switch (collider)
        {
            case RectangleColliderComponent:
                IsRectangleCollider = true;
                break;
            case CircleColliderComponent:
                IsCircleCollider = true;
                break;
            default:
                throw new ArgumentException($"Unknown collider component type: {collider.GetType()}.", nameof(collider));
        }
    }

    public Entity Entity => Transform.Entity;
    public Transform2DComponent Transform { get; }
    public Collider2DComponent Collider { get; }
    public KinematicRigidBody2DComponent KinematicBodyComponent { get; }
    public Matrix3x3 FinalTransform { get; private set; }

    public Vector2 Position { get; set; }
    public double Rotation { get; set; }
    public Vector2 LinearVelocity { get; set; }
    public double AngularVelocity { get; set; }

    public bool IsRectangleCollider { get; }
    public Rectangle TransformedRectangle { get; private set; }

    public bool IsCircleCollider { get; }
    public Circle TransformedCircle { get; private set; }

    public AxisAlignedRectangle BoundingRectangle { get; private set; }

    public List<Contact> Contacts { get; } = new List<Contact>();

    public void InitializeKinematicData()
    {
        Position = Transform.Translation;
        Rotation = Transform.Rotation;
        LinearVelocity = KinematicBodyComponent.LinearVelocity;
        AngularVelocity = KinematicBodyComponent.AngularVelocity;
    }

    public void UpdateTransform()
    {
        Transform.Translation = Position;
        Transform.Rotation = Rotation;

        FinalTransform = Transform.ToMatrix();

        if (IsCircleCollider)
        {
            TransformedCircle = new Circle(((CircleColliderComponent)Collider).Radius).Transform(FinalTransform);
            BoundingRectangle = TransformedCircle.GetBoundingRectangle();
        }
        else if (IsRectangleCollider)
        {
            TransformedRectangle = new Rectangle(((RectangleColliderComponent)Collider).Dimensions).Transform(FinalTransform);
            BoundingRectangle = TransformedRectangle.GetBoundingRectangle();
        }
    }

    public void Dispose()
    {
        Collider.ClearCollidingEntities();
    }
}