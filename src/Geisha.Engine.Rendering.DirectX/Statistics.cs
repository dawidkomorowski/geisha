using Geisha.Engine.Rendering.Backend;

namespace Geisha.Engine.Rendering.DirectX;

internal sealed class Statistics
{
    private int _drawCalls;

    public RenderingStatistics LastFrameStats { get; private set; }

    public void IncrementDrawCalls()
    {
        _drawCalls++;
    }

    public void UpdateLastFrameStats()
    {
        LastFrameStats = CreateLastFrameStats();
        Reset();
    }

    private RenderingStatistics CreateLastFrameStats() => new()
    {
        DrawCalls = _drawCalls
    };

    private void Reset()
    {
        _drawCalls = 0;
    }
}