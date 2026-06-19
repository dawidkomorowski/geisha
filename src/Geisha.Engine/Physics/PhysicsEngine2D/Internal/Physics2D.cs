using Geisha.Engine.Core.Math;
using System;

namespace Geisha.Engine.Physics.PhysicsEngine2D.Internal;

internal static class Physics2D
{
    public static class Scene
    {
        public static PhysicsSceneId Create()
        {
            return PhysicsSceneContext.Create();
        }

        public static void SetSubsteps(PhysicsSceneId id, int substeps)
        {
            ref var context = ref PhysicsSceneContext.Get(id);
            context.SimulationParameters.Substeps = substeps;
        }

        public static int GetSubsteps(PhysicsSceneId id)
        {
            throw new NotImplementedException();
        }

        public static void SetVelocityIterations(PhysicsSceneId id, int velocityIterations)
        {
            ref var context = ref PhysicsSceneContext.Get(id);
            context.SimulationParameters.VelocityIterations = velocityIterations;
        }

        public static int GetVelocityIterations(PhysicsSceneId id)
        {
            throw new NotImplementedException();
        }

        public static void SetPositionIterations(PhysicsSceneId id, int positionIterations)
        {
            ref var context = ref PhysicsSceneContext.Get(id);
            context.SimulationParameters.PositionIterations = positionIterations;
        }

        public static int GetPositionIterations(PhysicsSceneId id)
        {
            throw new NotImplementedException();
        }

        public static void SetPenetrationTolerance(PhysicsSceneId id, double penetrationTolerance)
        {
            ref var context = ref PhysicsSceneContext.Get(id);
            context.SimulationParameters.PenetrationTolerance = penetrationTolerance;
        }

        public static double GetPenetrationTolerance(PhysicsSceneId id)
        {
            throw new NotImplementedException();
        }

        public static RigidBodyId CreateBody(PhysicsSceneId id, BodyType bodyType, double circleColliderRadius)
        {
            ref var context = ref PhysicsSceneContext.Get(id);
            return context.CreateBody();
        }

        public static RigidBodyId CreateBody(PhysicsSceneId id, BodyType bodyType, in SizeD rectangleColliderSize)
        {
            ref var context = ref PhysicsSceneContext.Get(id);
            return context.CreateBody();
        }

        public static RigidBodyId CreateTileBody(PhysicsSceneId id)
        {
            ref var context = ref PhysicsSceneContext.Get(id);
            return context.CreateBody();
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

        private static ref RigidBodyData GetBodyData(RigidBodyId id)
        {
            ref var context = ref PhysicsSceneContext.Get(id.PhysicsSceneId);
            return ref context.GetBodyData(id);
        }
    }
}