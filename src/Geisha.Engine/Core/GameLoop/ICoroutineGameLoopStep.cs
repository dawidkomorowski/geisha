namespace Geisha.Engine.Core.GameLoop
{
    internal interface ICoroutineGameLoopStep
    {
        void ProcessCoroutines();
        void ProcessCoroutines(in TimeStep timeStep);
    }
}