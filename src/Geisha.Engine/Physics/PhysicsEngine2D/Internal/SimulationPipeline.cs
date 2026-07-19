using System;

namespace Geisha.Engine.Physics.PhysicsEngine2D.Internal;

internal static class SimulationPipeline
{
    public static void Step(ref PhysicsSceneData scene, TimeSpan timeStep)
    {
        var simulationParameters = scene.SimulationParameters;
        var deltaTimeSeconds = timeStep.TotalSeconds / simulationParameters.Substeps;

        ClearEvents(ref scene);

        for (var substep = 0; substep < simulationParameters.Substeps; substep++)
        {
            // TODO: Consider adding minimum velocity threshold to avoid solving constraints for very small velocities.
            // TODO: SolveVelocityConstraints could return a boolean value indicating whether the velocity constraints were solved. Then further iterations could be stopped.
            for (var i = 0; i < simulationParameters.VelocityIterations; i++)
            {
                ContactSolver.SolveVelocityConstraints(ref scene);
            }

            KinematicIntegration.IntegrateKinematicMotion(ref scene, deltaTimeSeconds);

            // TODO: Recomputation is only needed for bodies that actually moved.
            foreach (ref var body in scene.GetKinematicBodiesSpan())
            {
                body.RecomputeCollider(ref scene);
            }

            CollisionDetection.DetectCollisions(ref scene);

            // TODO: SolvePositionConstraints could return a boolean value indicating whether the position constraints were solved. Then further iterations could be stopped.
            for (var i = 0; i < simulationParameters.PositionIterations; i++)
            {
                ContactSolver.SolvePositionConstraints(ref scene);
            }

            // TODO: Recomputation is only needed for bodies fixed by position constraints solver. If the touched bodies would be tracked only those could be recomputed.
            foreach (ref var body in scene.GetKinematicBodiesSpan())
            {
                body.RecomputeCollider(ref scene);
            }

            GenerateEvents(ref scene);
        }
    }

    private static void ClearEvents(ref PhysicsSceneData scene)
    {
        scene.SensorOverlapEvents.Clear();
    }

    private static void GenerateEvents(ref PhysicsSceneData scene)
    {
        foreach (var sensorOverlap in scene.SensorOverlapCache.GetOverlaps())
        {
            switch (sensorOverlap.CacheStatus)
            {
                case CacheStatus.New:
                    scene.SensorOverlapEvents.Add(new SensorOverlapEvent(sensorOverlap.Body1Id, sensorOverlap.Body2Id, SensorOverlapEvent.EventType.Begin));
                    break;
                case CacheStatus.Updated:
                    break;
                case CacheStatus.Stale:
                    scene.SensorOverlapEvents.Add(new SensorOverlapEvent(sensorOverlap.Body1Id, sensorOverlap.Body2Id, SensorOverlapEvent.EventType.End));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}