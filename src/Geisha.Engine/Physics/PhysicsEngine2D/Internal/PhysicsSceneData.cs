using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Geisha.Engine.Physics.PhysicsEngine2D.Internal;

internal struct PhysicsSceneData
{
    private static readonly List<PhysicsSceneData> Scenes = new();
    private static Span<PhysicsSceneData> ScenesSpan => CollectionsMarshal.AsSpan(Scenes);

    public static PhysicsSceneId Create()
    {
        var context = new PhysicsSceneData
        {
            Index = Scenes.Count,
            Version = 1,
            SimulationParameters = new SimulationParameters
            {
                Substeps = 1,
                VelocityIterations = 4,
                PositionIterations = 4,
                PenetrationTolerance = 0.01
            },
            Bodies = new List<RigidBodyData>(),
            StaticBodyIndices = new List<int>(),
            KinematicBodyIndices = new List<int>()
        };

        Scenes.Add(context);

        if (Scenes.Count > 2000)
        {
            // TODO: Implement deletion of allocated physics scene by physics system.
            // TODO: Reuse list slots.
            throw new InvalidOperationException();
        }

        return context.PhysicsSceneId;
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

    public List<RigidBodyData> Bodies;
    public Span<RigidBodyData> BodiesSpan => CollectionsMarshal.AsSpan(Bodies);
    public List<int> StaticBodyIndices;
    public List<int> KinematicBodyIndices;

    public RigidBodyId CreateBody(BodyType bodyType)
    {
        var body = new RigidBodyData
        {
            Type = bodyType,
            CollisionLayer = uint.MaxValue,
            CollisionMask = uint.MaxValue
        };

        Bodies.Add(body);

        var rigidBodyId = new RigidBodyId(PhysicsSceneId, Bodies.Count - 1, 1);

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