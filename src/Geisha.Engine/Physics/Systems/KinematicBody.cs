using System.Collections.Generic;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Physics.Components;
using Geisha.Engine.Physics.PhysicsEngine2D;

namespace Geisha.Engine.Physics.Systems;

internal sealed class KinematicBody
{
    public Transform2DComponent Transform { get; }
    public Collider2DComponent Collider { get; }
    public Matrix3x3 FinalTransform { get; private set; }

    public Vector2 Position { get; set; }
    public double Rotation { get; set; }

    public bool IsRectangleCollider { get; }
    public Rectangle TransformedRectangle { get; private set; }

    public bool IsCircleCollider { get; }
    public Circle TransformedCircle { get; private set; }

    public AxisAlignedRectangle BoundingRectangle { get; private set; }

    public List<Contact> Contacts { get; } = new List<Contact>();

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
}