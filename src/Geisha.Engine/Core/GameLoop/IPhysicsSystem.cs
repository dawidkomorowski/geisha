namespace Geisha.Engine.Core.GameLoop
{
    internal interface IPhysicsSystem
    {
        void ProcessPhysics();
        void PreparePhysicsDebugInformation();
    }
}