using System;
using System.Runtime.InteropServices;
using Geisha.Engine.Core.Math;

namespace Geisha.Engine.Physics.PhysicsEngine2D.Internal;

internal static class Physics2D
{
    public static class Scene
    {
        public static PhysicsSceneId Create(in PhysicsScene2DDefinition sceneDefinition)
        {
            return PhysicsSceneData.Create(sceneDefinition);
        }

        public static void Destroy(PhysicsSceneId id)
        {
            PhysicsSceneData.Destroy(id);
        }

        public static bool IsValid(PhysicsSceneId id) => PhysicsSceneData.IsValid(id);

        public static int GetSubsteps(PhysicsSceneId id)
        {
            ref var scene = ref PhysicsSceneData.Get(id);
            return scene.SimulationParameters.Substeps;
        }

        public static void SetSubsteps(PhysicsSceneId id, int value)
        {
            ref var scene = ref PhysicsSceneData.Get(id);
            scene.SimulationParameters.Substeps = value;
        }

        public static int GetVelocityIterations(PhysicsSceneId id)
        {
            ref var scene = ref PhysicsSceneData.Get(id);
            return scene.SimulationParameters.VelocityIterations;
        }

        public static void SetVelocityIterations(PhysicsSceneId id, int value)
        {
            ref var scene = ref PhysicsSceneData.Get(id);
            scene.SimulationParameters.VelocityIterations = value;
        }

        public static int GetPositionIterations(PhysicsSceneId id)
        {
            ref var scene = ref PhysicsSceneData.Get(id);
            return scene.SimulationParameters.PositionIterations;
        }

        public static void SetPositionIterations(PhysicsSceneId id, int value)
        {
            ref var scene = ref PhysicsSceneData.Get(id);
            scene.SimulationParameters.PositionIterations = value;
        }

        public static double GetPenetrationTolerance(PhysicsSceneId id)
        {
            ref var scene = ref PhysicsSceneData.Get(id);
            return scene.SimulationParameters.PenetrationTolerance;
        }

        public static void SetPenetrationTolerance(PhysicsSceneId id, double value)
        {
            ref var scene = ref PhysicsSceneData.Get(id);
            scene.SimulationParameters.PenetrationTolerance = value;
        }

        public static SizeD GetTileSize(PhysicsSceneId id)
        {
            ref var scene = ref PhysicsSceneData.Get(id);
            return scene.TileSize;
        }

        public static int GetBodyCount(PhysicsSceneId id)
        {
            ref var scene = ref PhysicsSceneData.Get(id);
            var bodiesSpan = scene.GetBodiesSpan();
            return bodiesSpan.Length;
        }

        public static RigidBodyId GetBodyByRawIndex(PhysicsSceneId id, int rawIndex)
        {
            ref var scene = ref PhysicsSceneData.Get(id);
            var bodiesSpan = scene.GetBodiesSpan();
            return bodiesSpan[rawIndex].Id;
        }

        public static RigidBodyId CreateBody(PhysicsSceneId id, BodyType bodyType, double circleColliderRadius)
        {
            ref var scene = ref PhysicsSceneData.Get(id);
            ref var body = ref scene.CreateBody(bodyType);

            Body.SetCircleCollider(body.Id, circleColliderRadius);

            return body.Id;
        }

        public static RigidBodyId CreateBody(PhysicsSceneId id, BodyType bodyType, in SizeD rectangleColliderSize)
        {
            ref var scene = ref PhysicsSceneData.Get(id);
            ref var body = ref scene.CreateBody(bodyType);

            Body.SetRectangleCollider(body.Id, rectangleColliderSize);

            return body.Id;
        }

        public static RigidBodyId CreateTileBody(PhysicsSceneId id)
        {
            ref var scene = ref PhysicsSceneData.Get(id);
            ref var body = ref scene.CreateBody(BodyType.Static);

            Body.SetTileCollider(body.Id);

            return body.Id;
        }

        public static void DestroyBody(RigidBodyId id)
        {
            ref var scene = ref PhysicsSceneData.Get(id.PhysicsSceneId);
            scene.DestroyBody(id);
        }

        public static void Simulate(PhysicsSceneId id, TimeSpan timeStep)
        {
            ref var scene = ref PhysicsSceneData.Get(id);
            SimulationPipeline.Step(ref scene, timeStep);
        }

        public static void QueryPoint<TQueryHandler>(PhysicsSceneId id, in Vector2 point, ref TQueryHandler handler)
            where TQueryHandler : struct, IRigidBodyIdQueryHandler
        {
            ref var scene = ref PhysicsSceneData.Get(id);
            SceneQuery.QueryPoint(in scene, in point, ref handler);
        }

        public static void QueryBounds<TQueryHandler>(PhysicsSceneId id, in AABB2D aabb, ref TQueryHandler handler)
            where TQueryHandler : struct, IRigidBodyIdQueryHandler
        {
            ref var scene = ref PhysicsSceneData.Get(id);
            SceneQuery.QueryBounds(in scene, in aabb, ref handler);
        }

        public static void QueryOverlap<TQueryHandler>(PhysicsSceneId id, in AABB2D aabb, ref TQueryHandler handler)
            where TQueryHandler : struct, IRigidBodyIdQueryHandler
        {
            ref var scene = ref PhysicsSceneData.Get(id);
            SceneQuery.QueryOverlap(in scene, in aabb, ref handler);
        }

        public static void QueryOverlap<TQueryHandler>(PhysicsSceneId id, in Circle circle, ref TQueryHandler handler)
            where TQueryHandler : struct, IRigidBodyIdQueryHandler
        {
            ref var scene = ref PhysicsSceneData.Get(id);
            SceneQuery.QueryOverlap(in scene, in circle, ref handler);
        }

        public static void QueryOverlap<TQueryHandler>(PhysicsSceneId id, in Rectangle rectangle, ref TQueryHandler handler)
            where TQueryHandler : struct, IRigidBodyIdQueryHandler
        {
            ref var scene = ref PhysicsSceneData.Get(id);
            SceneQuery.QueryOverlap(in scene, in rectangle, ref handler);
        }

        public static ReadOnlySpan<SensorOverlapEvent> GetSensorOverlapEvents(PhysicsSceneId id)
        {
            ref var scene = ref PhysicsSceneData.Get(id);
            return CollectionsMarshal.AsSpan(scene.SensorOverlapEvents);
        }
    }

    public static class Body
    {
        public static bool IsValid(RigidBodyId id)
        {
            if (!PhysicsSceneData.IsValid(id.PhysicsSceneId))
            {
                return false;
            }

            ref var scene = ref PhysicsSceneData.Get(id.PhysicsSceneId);
            return scene.IsValidBodyId(id);
        }

        public static BodyType GetType(RigidBodyId id)
        {
            ref var body = ref GetBodyData(id);
            return body.Type;
        }

        public static ColliderType GetColliderType(RigidBodyId id)
        {
            ref var body = ref GetBodyData(id);
            return body.ColliderType;
        }

        public static CollisionNormalFilter GetCollisionNormalFilter(RigidBodyId id)
        {
            ref var body = ref GetBodyData(id);
            return body.CollisionNormalFilter;
        }

        public static Vector2 GetPosition(RigidBodyId id)
        {
            ref var body = ref GetBodyData(id);
            return body.Position;
        }

        public static void SetPosition(RigidBodyId id, in Vector2 value)
        {
            ref var scene = ref PhysicsSceneData.Get(id.PhysicsSceneId);
            ref var body = ref scene.GetBodyData(id);

            if (body.Position == value) return;

            if (body.ColliderType is ColliderType.Tile)
            {
                body.Position = body.EnableCollisionDetection
                    ? scene.TileMap.UpdateTile(ref body, body.Position, value)
                    : scene.TileMap.AlignPosition(value);
            }
            else
            {
                body.Position = value;
            }

            body.RecomputeCollider();
        }

        public static double GetRotation(RigidBodyId id)
        {
            ref var body = ref GetBodyData(id);
            return body.Rotation;
        }

        public static void SetRotation(RigidBodyId id, double value)
        {
            ref var body = ref GetBodyData(id);

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (body.Rotation == value) return;

            body.Rotation = body.ColliderType is ColliderType.Tile ? 0 : value;
            body.RecomputeCollider();
        }

        public static Vector2 GetLinearVelocity(RigidBodyId id)
        {
            ref var body = ref GetBodyData(id);
            return body.LinearVelocity;
        }

        public static void SetLinearVelocity(RigidBodyId id, in Vector2 value)
        {
            ref var body = ref GetBodyData(id);

            if (body.Type == BodyType.Static)
            {
                return;
            }

            body.LinearVelocity = value;
        }

        public static double GetAngularVelocity(RigidBodyId id)
        {
            ref var body = ref GetBodyData(id);
            return body.AngularVelocity;
        }

        public static void SetAngularVelocity(RigidBodyId id, double value)
        {
            ref var body = ref GetBodyData(id);

            if (body.Type == BodyType.Static)
            {
                return;
            }

            body.AngularVelocity = value;
        }

        public static bool GetEnableCollisionDetection(RigidBodyId id)
        {
            ref var body = ref GetBodyData(id);
            return body.EnableCollisionDetection;
        }

        public static void SetEnableCollisionDetection(RigidBodyId id, bool value)
        {
            ref var scene = ref PhysicsSceneData.Get(id.PhysicsSceneId);
            ref var body = ref scene.GetBodyData(id);

            if (body.EnableCollisionDetection != value && body.ColliderType is ColliderType.Tile)
            {
                if (value)
                {
                    scene.TileMap.CreateTile(ref body);
                }
                else
                {
                    scene.TileMap.RemoveTile(ref body);
                }
            }

            body.EnableCollisionDetection = value;
        }

        public static bool GetEnableCollisionResponse(RigidBodyId id)
        {
            ref var body = ref GetBodyData(id);
            return body.EnableCollisionResponse;
        }

        public static void SetEnableCollisionResponse(RigidBodyId id, bool value)
        {
            ref var body = ref GetBodyData(id);
            body.EnableCollisionResponse = value;
        }

        public static bool GetIsSensor(RigidBodyId id)
        {
            ref var body = ref GetBodyData(id);
            return body.IsSensor;
        }

        public static void SetIsSensor(RigidBodyId id, bool value)
        {
            ref var body = ref GetBodyData(id);
            body.IsSensor = value;
        }

        public static uint GetCollisionLayer(RigidBodyId id)
        {
            ref var body = ref GetBodyData(id);
            return body.CollisionLayer;
        }

        public static void SetCollisionLayer(RigidBodyId id, uint value)
        {
            ref var body = ref GetBodyData(id);
            body.CollisionLayer = value;
        }

        public static uint GetCollisionMask(RigidBodyId id)
        {
            ref var body = ref GetBodyData(id);
            return body.CollisionMask;
        }

        public static void SetCollisionMask(RigidBodyId id, uint value)
        {
            ref var body = ref GetBodyData(id);
            body.CollisionMask = value;
        }

        public static SizeD GetRectangleColliderSize(RigidBodyId id)
        {
            ref var body = ref GetBodyData(id);
            return body.RectangleColliderSize;
        }

        public static double GetCircleColliderRadius(RigidBodyId id)
        {
            ref var body = ref GetBodyData(id);
            return body.CircleColliderRadius;
        }

        public static AABB2D GetBoundingBox(RigidBodyId id)
        {
            ref var body = ref GetBodyData(id);
            return body.AABB;
        }

        public static int GetContactCount(RigidBodyId id)
        {
            ref var body = ref GetBodyData(id);
            return body.ContactCount;
        }

        public static int GetContacts(RigidBodyId id, Span<Contact> contacts)
        {
            ref var scene = ref PhysicsSceneData.Get(id.PhysicsSceneId);
            ref var body = ref scene.GetBodyData(id);

            if (body.ContactCount == 0)
            {
                return 0;
            }

            var contactsSpan = scene.GetContactsSpan();
            var contactIndex = body.FirstContactIndex;
            var writeCount = 0;

            while (contactIndex != ContactData.Link.NullIndex)
            {
                if (writeCount >= contacts.Length)
                {
                    break;
                }

                ref var contactData = ref contactsSpan[contactIndex];
                contacts[writeCount] = new Contact(contactData.Link1.BodyId, contactData.Link2.BodyId, contactData.ContactManifold);

                ref var link = ref contactData.Link1.BodyId == body.Id ? ref contactData.Link1 : ref contactData.Link2;
                contactIndex = link.NextIndex;

                writeCount++;
            }

            return writeCount;
        }

        public static void SetCircleCollider(RigidBodyId id, double radius)
        {
            ref var scene = ref PhysicsSceneData.Get(id.PhysicsSceneId);
            ref var body = ref scene.GetBodyData(id);

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (body.ColliderType is ColliderType.Circle && body.CircleColliderRadius == radius)
            {
                return;
            }

            if (body.ColliderType is ColliderType.Tile && body.EnableCollisionDetection)
            {
                scene.TileMap.RemoveTile(ref body);
            }

            body.ColliderType = ColliderType.Circle;
            body.CircleColliderRadius = radius;
            body.RectangleColliderSize = default;
            body.RecomputeCollider();
        }

        public static void SetRectangleCollider(RigidBodyId id, in SizeD size)
        {
            ref var scene = ref PhysicsSceneData.Get(id.PhysicsSceneId);
            ref var body = ref scene.GetBodyData(id);

            if (body.ColliderType is ColliderType.Rectangle && body.RectangleColliderSize == size)
            {
                return;
            }

            if (body.ColliderType is ColliderType.Tile && body.EnableCollisionDetection)
            {
                scene.TileMap.RemoveTile(ref body);
            }

            body.ColliderType = ColliderType.Rectangle;
            body.CircleColliderRadius = 0;
            body.RectangleColliderSize = size;
            body.RecomputeCollider();
        }

        public static void SetTileCollider(RigidBodyId id)
        {
            ref var scene = ref PhysicsSceneData.Get(id.PhysicsSceneId);
            ref var body = ref scene.GetBodyData(id);

            if (body.ColliderType is ColliderType.Tile)
            {
                return;
            }

            if (body.Type is BodyType.Kinematic)
            {
                throw new InvalidOperationException("Kinematic body cannot be Tile collider.");
            }

            if (body.ColliderType is not ColliderType.Tile)
            {
                body.ColliderType = ColliderType.Tile;
                body.Position = scene.TileMap.AlignPosition(body.Position);

                if (body.EnableCollisionDetection)
                {
                    scene.TileMap.CreateTile(ref body);
                }
            }

            body.CircleColliderRadius = 0;
            body.RectangleColliderSize = scene.TileSize;
            body.RecomputeCollider();
        }

        public static bool ContainsPoint(RigidBodyId id, in Vector2 point)
        {
            ref var body = ref GetBodyData(id);
            return body.ContainsPoint(point);
        }

        public static bool Overlaps(RigidBodyId id, in AABB2D aabb)
        {
            ref var body = ref GetBodyData(id);
            return body.Overlaps(aabb);
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

        internal static ref RigidBodyData GetBodyData(RigidBodyId id)
        {
            ref var scene = ref PhysicsSceneData.Get(id.PhysicsSceneId);
            return ref scene.GetBodyData(id);
        }
    }
}