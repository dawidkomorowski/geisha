using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Geisha.Engine.Physics.PhysicsEngine2D.Internal;

internal struct PhysicsSceneData
{
    private static readonly List<PhysicsSceneData> SceneList = new();
    private static Span<PhysicsSceneData> SceneSpan => CollectionsMarshal.AsSpan(SceneList);

    public static PhysicsSceneId Create()
    {
        var context = new PhysicsSceneData
        {
            Index = SceneList.Count,
            Version = 1,
            SimulationParameters = new SimulationParameters
            {
                Substeps = 1,
                VelocityIterations = 4,
                PositionIterations = 4,
                PenetrationTolerance = 0.01
            },
            Bodies = new List<RigidBodyData>()
        };

        SceneList.Add(context);

        if (SceneList.Count > 1000)
        {
            // TODO: Implement deletion of allocated physics scene by physics system.
            // TODO: Reuse list slots.
            throw new InvalidOperationException();
        }

        return context.PhysicsSceneId;
    }

    public static void Delete(PhysicsSceneId id)
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
        return ref SceneSpan[id.Index];
    }

    private PhysicsSceneId PhysicsSceneId => new(Index, Version);

    public int Index;
    public int Version;

    public SimulationParameters SimulationParameters;

    public List<RigidBodyData> Bodies;

    public RigidBodyId CreateBody()
    {
        var body = new RigidBodyData
        {
            CollisionLayer = uint.MaxValue,
            CollisionMask = uint.MaxValue
        };

        Bodies.Add(body);

        return new RigidBodyId(PhysicsSceneId, Bodies.Count - 1, 1);
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