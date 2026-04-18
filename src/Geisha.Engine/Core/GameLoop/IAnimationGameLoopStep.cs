namespace Geisha.Engine.Core.GameLoop
{
    internal interface IAnimationGameLoopStep
    {
        void ProcessAnimations(in TimeStep timeStep);
    }
}