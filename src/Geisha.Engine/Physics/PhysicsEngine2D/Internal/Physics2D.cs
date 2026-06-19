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

        public static void SetCircleCollider(RigidBodyId id, double radius)
        {
            ref var body = ref GetBodyData(id);
            // TODO: Implement actual logic.
        }

        public static void SetRectangleCollider(RigidBodyId id, in SizeD size)
        {
            ref var body = ref GetBodyData(id);
            // TODO: Implement actual logic.
        }

        public static void SetTileCollider(RigidBodyId id)
        {
            ref var body = ref GetBodyData(id);
            // TODO: Implement actual logic.
        }

        private static ref RigidBodyData GetBodyData(RigidBodyId id)
        {
            ref var scene = ref PhysicsSceneData.Get(id.PhysicsSceneId);
            return ref scene.GetBodyData(id);
        }
    }
}