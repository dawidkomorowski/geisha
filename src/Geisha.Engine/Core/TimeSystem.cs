using System;

namespace Geisha.Engine.Core;

public interface ITimeSystem
{
    DateTime StartUpTime { get; }
    TimeSpan TimeSinceStartUp { get; }
    int FramesSinceStartUp { get; }
    TimeSpan FixedDeltaTime { get; }
    double TimeScale { get; set; }
}

internal interface ITimeSystemInternal : ITimeSystem
{
    TimeStep NextTimeStep();
}

// TODO: Once migrated to .NET 8 a TimeProvider abstraction can be used instead of custom approach.
internal sealed class TimeSystem : ITimeSystemInternal
{
    #region Implementation of ITimeSystem

    public DateTime StartUpTime { get; }
    public TimeSpan TimeSinceStartUp { get; }
    public int FramesSinceStartUp { get; }
    public TimeSpan FixedDeltaTime { get; }
    public double TimeScale { get; set; }

    #endregion

    #region Implementation of ITimeSystemInternal

    public TimeStep NextTimeStep()
    {
        throw new NotImplementedException();
    }

    #endregion
}