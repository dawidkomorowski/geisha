﻿using Geisha.Engine.Core.Math;

namespace Geisha.Engine.Physics.PhysicsEngine2D;

internal static class PositionConstraint
{
    public static SeparationInfo GetSeparationInfo(in Contact contact)
    {
        if (contact.Body1.IsCircleCollider && contact.Body2.IsCircleCollider)
        {
            contact.Body1.TransformedCircleCollider.Overlaps(contact.Body2.TransformedCircleCollider, out var separationInfo);
            return separationInfo;
        }

        if (contact.Body1.IsCircleCollider && contact.Body2.IsRectangleCollider)
        {
            contact.Body1.TransformedCircleCollider.Overlaps(contact.Body2.TransformedRectangleCollider, out var separationInfo);
            return separationInfo;
        }

        if (contact.Body1.IsRectangleCollider && contact.Body2.IsCircleCollider)
        {
            contact.Body1.TransformedRectangleCollider.Overlaps(contact.Body2.TransformedCircleCollider, out var separationInfo);
            return separationInfo;
        }

        if (contact.Body1.IsRectangleCollider && contact.Body2.IsRectangleCollider)
        {
            contact.Body1.TransformedRectangleCollider.Overlaps(contact.Body2.TransformedRectangleCollider, out var separationInfo);
            return separationInfo;
        }

        return new SeparationInfo(Vector2.Zero, 0);
    }
}