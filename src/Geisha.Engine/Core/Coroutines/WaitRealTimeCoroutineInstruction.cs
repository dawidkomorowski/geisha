using System;

namespace Geisha.Engine.Core.Coroutines;

// TODO: Add tests for Coroutine.WaitRealTime and Coroutine.Wait in regard to time scaling (e.g. when time scale is set to 0, Coroutine.Wait should never complete, while Coroutine.WaitRealTime should complete as normal).
// TODO: Add documentation for Coroutine.WaitRealTime. Note that WaitRealTime is the same as Wait for CoroutineUpdateMode.FixedTimeStep as time scale is handled differently for fixed time step updates.
// TODO: Update documentation for Coroutine.Wait.
internal sealed class WaitRealTimeCoroutineInstruction : CoroutineInstruction
{
    private readonly TimeSpan _waitTime;
    private TimeSpan _timeWaited = TimeSpan.Zero;

    public WaitRealTimeCoroutineInstruction(TimeSpan waitTime)
    {
        _waitTime = waitTime;
    }

    internal override bool IsCompleted(in TimeStep timeStep)
    {
        _timeWaited += timeStep.UnscaledDeltaTime;
        return _timeWaited >= _waitTime;
    }

    internal override void Execute(Coroutine coroutine)
    {
    }
}