using Geisha.Engine.Rendering.Backend;

namespace Geisha.Engine.Rendering.DirectX;

internal sealed class Statistics
{
    public int DrawCalls { get; private set; }

    public void IncrementDrawCalls()
    {
        DrawCalls++;
    }

    public void Reset()
    {
        DrawCalls = 0;
    }

    public RenderingStatistics ToRenderingStatistics() => new()
    {
        DrawCalls = DrawCalls
    };
}