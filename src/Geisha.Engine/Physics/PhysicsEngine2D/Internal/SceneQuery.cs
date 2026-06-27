using Geisha.Engine.Core.Math;

namespace Geisha.Engine.Physics.PhysicsEngine2D.Internal;

internal static class SceneQuery
{
    public static void QueryPoint<TQueryHandler>(in PhysicsSceneData scene, in Vector2 point, ref TQueryHandler handler)
        where TQueryHandler : struct, IRigidBodyIdQueryHandler
    {
        foreach (ref var body in scene.GetBodiesSpan())
        {
            if (body.ContainsPoint(point))
            {
                if (!handler.Handle(body.Id))
                {
                    return;
                }
            }
        }
    }

    public static void QueryBounds<TQueryHandler>(in PhysicsSceneData scene, in AxisAlignedRectangle axisAlignedRectangle, ref TQueryHandler handler)
        where TQueryHandler : struct, IRigidBodyIdQueryHandler
    {
        foreach (ref var body in scene.GetBodiesSpan())
        {
            if (body.BoundingRectangle.Overlaps(axisAlignedRectangle))
            {
                if (!handler.Handle(body.Id))
                {
                    return;
                }
            }
        }
    }

    public static void QueryOverlap<TQueryHandler>(in PhysicsSceneData scene, in AxisAlignedRectangle axisAlignedRectangle, ref TQueryHandler handler)
        where TQueryHandler : struct, IRigidBodyIdQueryHandler
    {
        foreach (ref var body in scene.GetBodiesSpan())
        {
            if (body.Overlaps(axisAlignedRectangle))
            {
                if (!handler.Handle(body.Id))
                {
                    return;
                }
            }
        }
    }

    public static void QueryOverlap<TQueryHandler>(in PhysicsSceneData scene, in Circle circle, ref TQueryHandler handler)
        where TQueryHandler : struct, IRigidBodyIdQueryHandler
    {
        foreach (ref var body in scene.GetBodiesSpan())
        {
            if (body.Overlaps(circle))
            {
                if (!handler.Handle(body.Id))
                {
                    return;
                }
            }
        }
    }

    public static void QueryOverlap<TQueryHandler>(in PhysicsSceneData scene, in Rectangle rectangle, ref TQueryHandler handler)
        where TQueryHandler : struct, IRigidBodyIdQueryHandler
    {
        foreach (ref var body in scene.GetBodiesSpan())
        {
            if (body.Overlaps(rectangle))
            {
                if (!handler.Handle(body.Id))
                {
                    return;
                }
            }
        }
    }
}