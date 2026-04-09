using System;
using System.Diagnostics;

namespace Geisha.Engine.Core;

// TODO: InputSystem was greatly optimized, but now it does not correctly support mutating InputMapping.
//       - Consider making InputMapping immutable to avoid issues.
//       - Consider using ImmutableArray for storing mappings in InputMapping to avoid issues with mutability and to improve performance.
//       - InputSystem was optimized but IInputProvider implementations may still be inefficient, consider optimizing them as well.
// TODO: Report ticket for introducing utility factory methods for HardwareAction and HardwareAxis to make it less verbose to create them, e.g. HardwareAction.CreateKeyboardVariant(Key key) and HardwareAxis.CreateGamepadVariant(GamepadAxis gamepadAxis).
// TODO: Report ticket to consider separating mouse buttons from mouse axes in hardware input variants to avoid confusion (MouseVariant).
// TODO: Add missing docs for new APIs.
// TODO: Report ticket for showcasing TimeScale in Demo Application.
// TODO: Add tests for timescale to be correctly respected by game loop and other systems.

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

internal sealed class TimeSystem : ITimeSystemInternal
{
    private readonly Stopwatch _stopwatch = new();

    // TODO: Once migrated to .NET 8 a TimeProvider abstraction can be used instead of custom approach.
    private readonly Func<DateTime> _now;

    public TimeSystem(CoreConfiguration configuration) : this(configuration, () => DateTime.Now)
    {
    }

    public TimeSystem(CoreConfiguration configuration, Func<DateTime> now)
    {
        _now = now;

        if (configuration.FixedUpdatesPerSecond <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(configuration), configuration.FixedUpdatesPerSecond,
                $"{nameof(CoreConfiguration)}.{nameof(CoreConfiguration.FixedUpdatesPerSecond)} must be greater than zero.");
        }

        StartUpTime = _now();
        FixedDeltaTime = TimeSpan.FromSeconds(1.0 / configuration.FixedUpdatesPerSecond);
        TimeStep.FixedDeltaTime = FixedDeltaTime;
    }

    #region Implementation of ITimeSystem

    public DateTime StartUpTime { get; }
    public TimeSpan TimeSinceStartUp => _now() - StartUpTime;
    public int FramesSinceStartUp { get; private set; }
    public TimeSpan FixedDeltaTime { get; }
    public double TimeScale { get; set; } = 1.0;

    #endregion

    #region Implementation of ITimeSystemInternal

    public TimeStep NextTimeStep()
    {
        FramesSinceStartUp++;

        var elapsed = _stopwatch.Elapsed;
        _stopwatch.Restart();
        return new TimeStep(elapsed, TimeScale);
    }

    #endregion
}