namespace Geisha.Engine.Core.GameLoop
{
    internal interface ICoroutineGameLoopStep
    {
        void ProcessCoroutines();
        void ProcessCoroutines(GameTime gameTime);
    }
}