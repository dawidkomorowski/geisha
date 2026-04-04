namespace Geisha.Engine.Core.Coroutines
{
    internal sealed class WaitForNextFrameCoroutineInstruction : CoroutineInstruction
    {
        internal override bool IsCompleted(in TimeStep timeStep) => true;

        internal override void Execute(Coroutine coroutine)
        {
        }
    }
}