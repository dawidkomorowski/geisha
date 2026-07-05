using Geisha.Engine.Core.Memory;

namespace Geisha.Engine.Physics.PhysicsEngine2D.Internal;

internal struct SimulationParameters : IUnmanaged<SimulationParameters>
{
    public int Substeps;
    public int VelocityIterations;
    public int PositionIterations;
    public double PenetrationTolerance;
}