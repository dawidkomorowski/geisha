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

        public static void SetSubsteps(PhysicsSceneId id, int substeps)
        {
            ref var scene = ref PhysicsSceneData.Get(id);
            scene.SimulationParameters.Substeps = substeps;
        }

        public static int GetSubsteps(PhysicsSceneId id)
        {
            throw new NotImplementedException();
        }

        public static void SetVelocityIterations(PhysicsSceneId id, int velocityIterations)
        {
            ref var scene = ref PhysicsSceneData.Get(id);
            scene.SimulationParameters.VelocityIterations = velocityIterations;
        }

        public static int GetVelocityIterations(PhysicsSceneId id)
        {
            throw new NotImplementedException();
        }

        public static void SetPositionIterations(PhysicsSceneId id, int positionIterations)
        {
            ref var scene = ref PhysicsSceneData.Get(id);
            scene.SimulationParameters.PositionIterations = positionIterations;
        }

        public static int GetPositionIterations(PhysicsSceneId id)
        {
            throw new NotImplementedException();
        }

        public static void SetPenetrationTolerance(PhysicsSceneId id, double penetrationTolerance)
        {
            ref var scene = ref PhysicsSceneData.Get(id);
            scene.SimulationParameters.PenetrationTolerance = penetrationTolerance;
        }

        public static double GetPenetrationTolerance(PhysicsSceneId id)
        {
            throw new NotImplementedException();
        }

        public static RigidBodyId CreateBody(PhysicsSceneId id, BodyType bodyType, double circleColliderRadius)
        {
            ref var scene = ref PhysicsSceneData.Get(id);
            return scene.CreateBody();
        }

        public static RigidBodyId CreateBody(PhysicsSceneId id, BodyType bodyType, in SizeD rectangleColliderSize)
        {
            ref var scene = ref PhysicsSceneData.Get(id);
            return scene.CreateBody();
        }

        public static RigidBodyId CreateTileBody(PhysicsSceneId id)
        {
            ref var scene = ref PhysicsSceneData.Get(id);
            return scene.CreateBody();
        }
    }

    public static class Body
    {
        public static void SetPosition(RigidBodyId id, in Vector2 position)
        {
            ref var body = ref GetBodyData(id);
            body.Position = position;
        }

        public static void SetRotation(RigidBodyId id, double rotation)
        {
            ref var body = ref GetBodyData(id);
            body.Rotation = rotation;
        }

        public static void SetEnableCollisionDetection(RigidBodyId id, bool enableCollisionDetection)
        {
            ref var body = ref GetBodyData(id);
            body.EnableCollisionDetection = enableCollisionDetection;
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

        private static ref RigidBodyData GetBodyData(RigidBodyId id)
        {
            ref var scene = ref PhysicsSceneData.Get(id.PhysicsSceneId);
            return ref scene.GetBodyData(id);
        }
    }
}