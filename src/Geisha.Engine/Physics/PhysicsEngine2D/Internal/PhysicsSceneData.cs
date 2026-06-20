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
            Bodies = new List<RigidBodyData>(),
            StaticBodyIndices = new List<int>(),
            KinematicBodyIndices = new List<int>(),
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

    public List<RigidBodyData> Bodies;
    public Span<RigidBodyData> BodiesSpan => CollectionsMarshal.AsSpan(Bodies);
    public List<int> StaticBodyIndices;
    public List<int> KinematicBodyIndices;

    public SensorOverlapCache SensorOverlapCache;
    public List<SensorOverlapEvent> SensorOverlapEvents;

    public RigidBodyId CreateBody(BodyType bodyType)
    {
        var rigidBodyId = new RigidBodyId(PhysicsSceneId, Bodies.Count, 1);

        var body = new RigidBodyData
        {
            RuntimeId = RuntimeId.Next(),
            Id = rigidBodyId,
            Type = bodyType,
            CollisionNormalFilter = CollisionNormalFilter.None,
            EnableCollisionDetection = true,
            CollisionLayer = uint.MaxValue,
            CollisionMask = uint.MaxValue
        };

        Bodies.Add(body);

        switch (bodyType)
        {
            case BodyType.Static:
                StaticBodyIndices.Add(rigidBodyId.Index);
                break;
            case BodyType.Kinematic:
                KinematicBodyIndices.Add(rigidBodyId.Index);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(bodyType), bodyType, null);
        }

        return rigidBodyId;
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
                Bodies[id.Index] = default;
                StaticBodyIndices.Remove(id.Index);
                break;
            case BodyType.Kinematic:
                Bodies[id.Index] = default;
                KinematicBodyIndices.Remove(id.Index);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public ref RigidBodyData GetBodyData(RigidBodyId id)
    {
        if (!id.IsValid)
        {
            throw new ArgumentException("Invalid body ID.");
        }

        // TODO: Validate ID.
        var bodies = CollectionsMarshal.AsSpan(Bodies);
        return ref bodies[id.Index];
    }
}