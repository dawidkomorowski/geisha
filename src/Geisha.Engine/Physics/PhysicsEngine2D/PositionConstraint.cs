using Geisha.Engine.Core.Math;

namespace Geisha.Engine.Physics.PhysicsEngine2D;

internal static class PositionConstraint
{
    public static MinimumTranslationVector GetMinimumTranslationVector(in Contact contact)
    {
        if (contact.Body1.IsCircleCollider && contact.Body2.IsCircleCollider)
        {
            contact.Body1.TransformedCircleCollider.Overlaps(contact.Body2.TransformedCircleCollider, out var mtv);
            return mtv;
        }

        if (contact.Body1.IsCircleCollider && contact.Body2.IsRectangleCollider)
        {
            contact.Body1.TransformedCircleCollider.Overlaps(contact.Body2.TransformedRectangleCollider, out var mtv);
            return mtv;
        }

        if (contact.Body1.IsRectangleCollider && contact.Body2.IsCircleCollider)
        {
            contact.Body1.TransformedRectangleCollider.Overlaps(contact.Body2.TransformedCircleCollider, out var mtv);
            return mtv;
        }

        if (contact.Body1.IsRectangleCollider && contact.Body2.IsRectangleCollider)
        {
            contact.Body1.TransformedRectangleCollider.Overlaps(contact.Body2.TransformedRectangleCollider, out var mtv);
            return mtv;
        }

        return default;
    }
}