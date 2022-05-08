namespace Geisha.Engine.Core.GameLoop
{
    internal interface IBehaviorSystem
    {
        void ProcessBehaviorFixedUpdate();
        void ProcessBehaviorUpdate(GameTime gameTime);
    }
}