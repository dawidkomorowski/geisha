namespace Geisha.Engine.Core.GameLoop
{
    internal interface IAnimationGameLoopStep
    {
        void ProcessAnimations(GameTime gameTime);
    }
}