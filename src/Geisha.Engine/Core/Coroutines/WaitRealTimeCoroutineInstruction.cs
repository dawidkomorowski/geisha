using System;

namespace Geisha.Engine.Core.Coroutines;

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