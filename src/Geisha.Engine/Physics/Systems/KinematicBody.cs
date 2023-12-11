using System;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Physics.Components;

namespace Geisha.Engine.Physics.Systems;

internal sealed class KinematicBody : IDisposable
{
    public KinematicBody(Transform2DComponent transform, Collider2DComponent collider)
    {
        Transform = transform;
        Collider = collider;

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
    public Matrix3x3 FinalTransform { get; private set; }

    public bool IsRectangleCollider { get; }
    public Rectangle TransformedRectangle { get; private set; }

    public bool IsCircleCollider { get; }
    public Circle TransformedCircle { get; private set; }

    public void UpdateFinalTransform()
    {
        FinalTransform = TransformHierarchy.Calculate2DTransformationMatrix(Entity);
        if (IsCircleCollider)
        {
            TransformedCircle = new Circle(((CircleColliderComponent)Collider).Radius).Transform(FinalTransform);
        }
        else if (IsRectangleCollider)
        {
            TransformedRectangle = new Rectangle(((RectangleColliderComponent)Collider).Dimensions).Transform(FinalTransform);
        }
    }

    public void Dispose()
    {
        Collider.ClearCollidingEntities();
    }
}