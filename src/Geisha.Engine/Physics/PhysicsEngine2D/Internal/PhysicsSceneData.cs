using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Math;

namespace Geisha.Engine.Physics.PhysicsEngine2D.Internal;

internal struct PhysicsSceneData
{
    private static readonly List<PhysicsSceneData> Scenes = new();
    private static Span<PhysicsSceneData> ScenesSpan => CollectionsMarshal.AsSpan(Scenes);

    public static PhysicsSceneId Create(in PhysicsScene2DDefinition sceneDefinition)
    {
        var scene = new PhysicsSceneData
        {
            Index = Scenes.Count,
            Version = 1,
            SimulationParameters = new SimulationParameters
            {
                Substeps = sceneDefinition.Substeps > 0 ? sceneDefinition.Substeps : 1,
                VelocityIterations = sceneDefinition.VelocityIterations > 0 ? sceneDefinition.VelocityIterations : 4,
                PositionIterations = sceneDefinition.PositionIterations > 0 ? sceneDefinition.PositionIterations : 4,
                PenetrationTolerance = sceneDefinition.PenetrationTolerance >= 0 ? sceneDefinition.PenetrationTolerance : 0.01
            },
            TileSize = sceneDefinition.TileSize,
            TileMap = new TileMap(sceneDefinition.TileSize),
            BodyIndices = new List<BodyIndex>(),
            Bodies = new List<RigidBodyData>(),
            StaticBodyIndices = new List<int>(),
            KinematicBodyIndices = new List<int>(),
            Contacts = new List<ContactData>(256),
            SensorOverlapCache = new SensorOverlapCache(256),
            SensorOverlapEvents = new List<SensorOverlapEvent>(256)
        };

        Scenes.Add(scene);

        if (Scenes.Count > 2000)
        {
            // TODO: Implement deletion of allocated physics scene by physics system.
            // TODO: Reuse list slots.
            throw new InvalidOperationException();
        }

        return scene.PhysicsSceneId;
    }

    public static void Destroy(PhysicsSceneId id)
    {
        throw new NotImplementedException();
    }

    public static ref PhysicsSceneData Get(PhysicsSceneId id)
    {
        if (!id.IsValid)
        {
            throw new ArgumentException("Invalid scene ID.");
        }

        // TODO: Validate ID.
        return ref ScenesSpan[id.Index];
    }

    private PhysicsSceneId PhysicsSceneId => new(Index, Version);

    public int Index;
    public int Version;

    public SimulationParameters SimulationParameters;

    public SizeD TileSize;
    public TileMap TileMap;

    public List<BodyIndex> BodyIndices;
    public Span<BodyIndex> BodyIndicesSpan => CollectionsMarshal.AsSpan(BodyIndices);
    public List<RigidBodyData> Bodies;
    public Span<RigidBodyData> BodiesSpan => CollectionsMarshal.AsSpan(Bodies);
    public List<int> StaticBodyIndices;
    public List<int> KinematicBodyIndices;

    public List<ContactData> Contacts;
    public Span<ContactData> ContactsSpan => CollectionsMarshal.AsSpan(Contacts);

    public SensorOverlapCache SensorOverlapCache;
    public List<SensorOverlapEvent> SensorOverlapEvents;

    public ref RigidBodyData CreateBody(BodyType bodyType)
    {
        var rigidBodyId = new RigidBodyId(PhysicsSceneId, BodyIndices.Count, 1);

        // TODO: Reuse index slots.
        BodyIndices.Add(default);
        ref var bodyIndex = ref BodyIndicesSpan[rigidBodyId.Index];
        bodyIndex.DenseIndex = Bodies.Count;
        bodyIndex.Version = rigidBodyId.Version;

        var body = new RigidBodyData
        {
            RuntimeId = RuntimeId.Next(),
            Id = rigidBodyId,
            Type = bodyType,
            CollisionNormalFilter = CollisionNormalFilter.None,
            EnableCollisionDetection = true,
            CollisionLayer = uint.MaxValue,
            CollisionMask = uint.MaxValue,
            ContactCount = 0,
            FirstContactIndex = ContactData.Link.NullIndex,
            LastContactIndex = ContactData.Link.NullIndex
        };

        Bodies.Add(body);

        switch (bodyType)
        {
            case BodyType.Static:
                StaticBodyIndices.Add(bodyIndex.DenseIndex);
                break;
            case BodyType.Kinematic:
                KinematicBodyIndices.Add(bodyIndex.DenseIndex);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(bodyType), bodyType, null);
        }

        return ref BodiesSpan[Bodies.Count - 1];
    }

    public void DestroyBody(RigidBodyId id)
    {
        ref var body = ref GetBodyData(id);

        if (body.ColliderType is ColliderType.Tile && body.EnableCollisionDetection)
        {
            TileMap.RemoveTile(ref body);
        }

        switch (body.Type)
        {
            case BodyType.Static:
                StaticBodyIndices.Remove(id.Index);
                break;
            case BodyType.Kinematic:
                KinematicBodyIndices.Remove(id.Index);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        var denseIndex = BodyIndicesSpan[id.Index].DenseIndex;

        if (denseIndex == Bodies.Count - 1)
        {
            // If the last element is being removed, just remove it.
            Bodies.RemoveAt(denseIndex);
        }
        else
        {
            // Otherwise swap-remove with last element.
            BodiesSpan[denseIndex] = BodiesSpan[^1];
            Bodies.RemoveAt(Bodies.Count - 1);

            // Update index pointer.
            var movedBodyIndex = BodiesSpan[denseIndex].Id.Index;
            BodyIndicesSpan[movedBodyIndex].DenseIndex = denseIndex;
        }

        BodyIndicesSpan[id.Index].Version++;
        BodyIndicesSpan[id.Index].DenseIndex = BodyIndex.NullIndex;
    }

    public ref RigidBodyData GetBodyData(RigidBodyId id)
    {
        if (!id.IsValid)
        {
            throw new ArgumentException("Invalid body ID.");
        }

        var bodyIndex = BodyIndicesSpan[id.Index];
        if (bodyIndex.Version != id.Version)
        {
            throw new ArgumentException("Version mismatch detected. The ID is no longer valid.");
        }

        return ref BodiesSpan[bodyIndex.DenseIndex];
    }

    public struct BodyIndex
    {
        public const int NullIndex = -1;

        public int DenseIndex;
        public int Version;
    }
}