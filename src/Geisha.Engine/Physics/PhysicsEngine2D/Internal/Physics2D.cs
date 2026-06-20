using Geisha.Engine.Core.Math;
using System;

namespace Geisha.Engine.Physics.PhysicsEngine2D.Internal;

internal static class Physics2D
{
    public static class Scene
    {
        public static PhysicsSceneId Create()
        {
            return PhysicsSceneData.Create();
        }

        public static int GetSubsteps(PhysicsSceneId id)
        {
            throw new NotImplementedException();
        }

        public static void SetSubsteps(PhysicsSceneId id, int substeps)
        {
            ref var scene = ref PhysicsSceneData.Get(id);
            scene.SimulationParameters.Substeps = substeps;
        }

        public static int GetVelocityIterations(PhysicsSceneId id)
        {
            throw new NotImplementedException();
        }

        public static void SetVelocityIterations(PhysicsSceneId id, int velocityIterations)
        {
            ref var scene = ref PhysicsSceneData.Get(id);
            scene.SimulationParameters.VelocityIterations = velocityIterations;
        }

        public static int GetPositionIterations(PhysicsSceneId id)
        {
            throw new NotImplementedException();
        }

        public static void SetPositionIterations(PhysicsSceneId id, int positionIterations)
        {
            ref var scene = ref PhysicsSceneData.Get(id);
            scene.SimulationParameters.PositionIterations = positionIterations;
        }

        public static double GetPenetrationTolerance(PhysicsSceneId id)
        {
            throw new NotImplementedException();
        }

        public static void SetPenetrationTolerance(PhysicsSceneId id, double penetrationTolerance)
        {
            ref var scene = ref PhysicsSceneData.Get(id);
            scene.SimulationParameters.PenetrationTolerance = penetrationTolerance;
        }

        public static RigidBodyId CreateBody(PhysicsSceneId id, BodyType bodyType, double circleColliderRadius)
        {
            ref var scene = ref PhysicsSceneData.Get(id);
            return scene.CreateBody(bodyType);
        }

        public static RigidBodyId CreateBody(PhysicsSceneId id, BodyType bodyType, in SizeD rectangleColliderSize)
        {
            ref var scene = ref PhysicsSceneData.Get(id);
            return scene.CreateBody(bodyType);
        }

        public static RigidBodyId CreateTileBody(PhysicsSceneId id)
        {
            ref var scene = ref PhysicsSceneData.Get(id);
            return scene.CreateBody(BodyType.Static);
        }

        public static void Simulate(PhysicsSceneId id, TimeSpan timeStep)
        {
            ref var scene = ref PhysicsSceneData.Get(id);

            var substeps = scene.SimulationParameters.Substeps;

            var deltaTimeSeconds = timeStep.TotalSeconds / substeps;

            for (var substep = 0; substep < substeps; substep++)
            {
                KinematicIntegration.IntegrateKinematicMotion(ref scene, deltaTimeSeconds);
            }
        }

        //public static void QueryPoint<TQueryHandler>(PhysicsSceneId id, in Vector2 point, ref TQueryHandler handler)
        //    where TQueryHandler : struct, IRigidBodyQueryHandler
        //{
        //    ref var scene = ref PhysicsSceneData.Get(id);

        //    foreach (ref var body in scene.BodiesSpan)
        //    {
        //        if (body.ContainsPoint(point))
        //        {
        //            if (!handler.Handle(body))
        //            {
        //                return;
        //            }
        //        }
        //    }
        //}

        //public static void QueryBounds<TQueryHandler>(PhysicsSceneId id, in AxisAlignedRectangle axisAlignedRectangle, ref TQueryHandler handler)
        //    where TQueryHandler : struct, IRigidBodyQueryHandler
        //{
        //    ref var scene = ref PhysicsSceneData.Get(id);

        //    foreach (ref var body in scene.BodiesSpan)
        //    {
        //        if (body.BoundingRectangle.Overlaps(axisAlignedRectangle))
        //        {
        //            if (!handler.Handle(body))
        //            {
        //                return;
        //            }
        //        }
        //    }
        //}

        //public static void QueryOverlap<TQueryHandler>(PhysicsSceneId id, in AxisAlignedRectangle axisAlignedRectangle, ref TQueryHandler handler)
        //    where TQueryHandler : struct, IRigidBodyQueryHandler
        //{
        //    ref var scene = ref PhysicsSceneData.Get(id);

        //    foreach (ref var body in scene.BodiesSpan)
        //    {
        //        if (body.Overlaps(axisAlignedRectangle))
        //        {
        //            if (!handler.Handle(body))
        //            {
        //                return;
        //            }
        //        }
        //    }
        //}

        //public static void QueryOverlap<TQueryHandler>(PhysicsSceneId id, in Circle circle, ref TQueryHandler handler)
        //    where TQueryHandler : struct, IRigidBodyQueryHandler
        //{
        //    ref var scene = ref PhysicsSceneData.Get(id);

        //    foreach (ref var body in scene.BodiesSpan)
        //    {
        //        if (body.Overlaps(circle))
        //        {
        //            if (!handler.Handle(body))
        //            {
        //                return;
        //            }
        //        }
        //    }
        //}

        //public static void QueryOverlap<TQueryHandler>(PhysicsSceneId id, in Rectangle rectangle, ref TQueryHandler handler)
        //    where TQueryHandler : struct, IRigidBodyQueryHandler
        //{
        //    ref var scene = ref PhysicsSceneData.Get(id);

        //    foreach (ref var body in scene.BodiesSpan)
        //    {
        //        if (body.Overlaps(rectangle))
        //        {
        //            if (!handler.Handle(body))
        //            {
        //                return;
        //            }
        //        }
        //    }
        //}
    }

    public static class Body
    {
        public static Vector2 GetPosition(RigidBodyId id)
        {
            ref var body = ref GetBodyData(id);
            return body.Position;
        }

        public static void SetPosition(RigidBodyId id, in Vector2 position)
        {
            ref var body = ref GetBodyData(id);
            body.Position = position;
        }

        public static double GetRotation(RigidBodyId id)
        {
            ref var body = ref GetBodyData(id);
            return body.Rotation;
        }

        public static void SetRotation(RigidBodyId id, double rotation)
        {
            ref var body = ref GetBodyData(id);
            body.Rotation = rotation;
        }

        public static Vector2 GetLinearVelocity(RigidBodyId id)
        {
            ref var body = ref GetBodyData(id);
            return body.LinearVelocity;
        }

        public static void SetLinearVelocity(RigidBodyId id, in Vector2 linearVelocity)
        {
            ref var body = ref GetBodyData(id);
            body.LinearVelocity = linearVelocity;
        }

        public static double GetAngularVelocity(RigidBodyId id)
        {
            ref var body = ref GetBodyData(id);
            return body.AngularVelocity;
        }

        public static void SetAngularVelocity(RigidBodyId id, double angularVelocity)
        {
            ref var body = ref GetBodyData(id);
            body.AngularVelocity = angularVelocity;
        }

        public static bool GetEnableCollisionDetection(RigidBodyId id)
        {
            ref var body = ref GetBodyData(id);
            return body.EnableCollisionDetection;
        }

        public static void SetEnableCollisionDetection(RigidBodyId id, bool enableCollisionDetection)
        {
            ref var body = ref GetBodyData(id);
            body.EnableCollisionDetection = enableCollisionDetection;
        }

        public static bool GetEnableCollisionResponse(RigidBodyId id)
        {
            ref var body = ref GetBodyData(id);
            return body.EnableCollisionResponse;
        }

        public static void SetEnableCollisionResponse(RigidBodyId id, bool enableCollisionResponse)
        {
            ref var body = ref GetBodyData(id);
            body.EnableCollisionResponse = enableCollisionResponse;
        }

        public static void SetIsSensor(RigidBodyId id, bool isSensor)
        {
            ref var body = ref GetBodyData(id);
            body.IsSensor = isSensor;
        }

        public static void SetCollisionLayer(RigidBodyId id, uint collisionLayer)
        {
            ref var body = ref GetBodyData(id);
            body.CollisionLayer = collisionLayer;
        }

        public static void SetCollisionMask(RigidBodyId id, uint collisionMask)
        {
            ref var body = ref GetBodyData(id);
            body.CollisionMask = collisionMask;
        }

        public static AxisAlignedRectangle GetBoundingRectangle(RigidBodyId id)
        {
            ref var body = ref GetBodyData(id);
            return body.BoundingRectangle;
        }

        public static void SetCircleCollider(RigidBodyId id, double radius)
        {
            ref var body = ref GetBodyData(id);

            // TODO: Implement actual logic.

            if (body.ColliderType is ColliderType.Tile && body.EnableCollisionDetection)
            {
                //Scene.TileMap.RemoveTile(this);
            }

            body.ColliderType = ColliderType.Circle;
            body.CircleColliderRadius = radius;
            body.RectangleColliderSize = default;
            body.RecomputeCollider();
        }

        public static void SetRectangleCollider(RigidBodyId id, in SizeD size)
        {
            ref var body = ref GetBodyData(id);
            // TODO: Implement actual logic.

            if (body.ColliderType is ColliderType.Tile && body.EnableCollisionDetection)
            {
                //Scene.TileMap.RemoveTile(this);
            }

            body.ColliderType = ColliderType.Rectangle;
            body.CircleColliderRadius = 0;
            body.RectangleColliderSize = size;
            body.RecomputeCollider();
        }

        public static void SetTileCollider(RigidBodyId id)
        {
            ref var body = ref GetBodyData(id);
            // TODO: Implement actual logic.

            if (body.Type is BodyType.Kinematic)
            {
                throw new InvalidOperationException("Kinematic body cannot be Tile collider.");
            }

            if (body.ColliderType is not ColliderType.Tile)
            {
                body.ColliderType = ColliderType.Tile;
                //body.Position = Scene.TileMap.AlignPosition(_position);

                if (body.EnableCollisionDetection)
                {
                    //Scene.TileMap.CreateTile(this);
                }
            }

            body.CircleColliderRadius = 0;
            //body.RectangleColliderSize = Scene.TileSize;
            body.RecomputeCollider();
        }

        public static bool ContainsPoint(RigidBodyId id, in Vector2 point)
        {
            ref var body = ref GetBodyData(id);
            return body.ContainsPoint(point);
        }

        public static bool Overlaps(RigidBodyId id, in AxisAlignedRectangle axisAlignedRectangle)
        {
            ref var body = ref GetBodyData(id);
            return body.Overlaps(axisAlignedRectangle);
        }

        public static bool Overlaps(RigidBodyId id, in Circle circle)
        {
            ref var body = ref GetBodyData(id);
            return body.Overlaps(circle);
        }

        public static bool Overlaps(RigidBodyId id, in Rectangle rectangle)
        {
            ref var body = ref GetBodyData(id);
            return body.Overlaps(rectangle);
        }

        private static ref RigidBodyData GetBodyData(RigidBodyId id)
        {
            ref var scene = ref PhysicsSceneData.Get(id.PhysicsSceneId);
            return ref scene.GetBodyData(id);
        }
    }
}