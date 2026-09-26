namespace Geisha.Engine.Core.GameLoop
{
    internal interface IRenderingGameLoopStep
    {
        void WaitForFramePacing();
        void RenderScene();
    }
}