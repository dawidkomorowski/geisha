namespace Geisha.Engine.Core.GameLoop
{
    internal interface IPhysicsGameLoopStep
    {
        void ProcessPhysics();
        void PreparePhysicsDebugInformation();
    }
}