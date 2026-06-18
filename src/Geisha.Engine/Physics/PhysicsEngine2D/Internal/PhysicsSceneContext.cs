using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Geisha.Engine.Physics.PhysicsEngine2D.Internal;

internal struct PhysicsSceneContext
{
    private static readonly List<PhysicsSceneContext> ContextList = new();
    private static Span<PhysicsSceneContext> ContextSpan => CollectionsMarshal.AsSpan(ContextList);

    public static PhysicsSceneId Create()
    {
        var context = new PhysicsSceneContext
        {
            Index = ContextList.Count,
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

        ContextList.Add(context);

        if (ContextList.Count > 1000)
        {
            // TODO: Implement deletion of allocated physics scene by physics system.
            // TODO: Reuse list slots.
            throw new InvalidOperationException();
        }

        return context.PhysicsSceneId;
    }

    public static void Delete(int id)
    {
        throw new NotImplementedException();
    }

    public static ref PhysicsSceneContext Get(PhysicsSceneId id)
    {
        if (!id.IsValid)
        {
            throw new ArgumentException("Invalid scene ID.");
        }

        // TODO: Validate ID.
        return ref ContextSpan[id.Index];
    }

    private PhysicsSceneId PhysicsSceneId => new(Index, Version);

    public int Index;
    public int Version;

    public SimulationParameters SimulationParameters;

    public List<RigidBodyData> Bodies;

    public RigidBodyId CreateBody()
    {
        Bodies.Add(new RigidBodyData());

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