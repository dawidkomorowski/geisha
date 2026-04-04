namespace Geisha.Engine.Core.GameLoop
{
    internal interface IBehaviorGameLoopStep
    {
        void ProcessBehaviorFixedUpdate();
        void ProcessBehaviorUpdate(in TimeStep timeStep);
    }
}