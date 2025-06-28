using System;
using Geisha.Engine.Core.Diagnostics;
using Geisha.Engine.Core.Math;

namespace Geisha.TestUtils;

public interface IDebugRendererForTests : IDebugRenderer, IDisposable
{
    void BeginDraw(double scale = 1d);
    void EndDrawUsingTestContext(int stage = 0);
    void EndDraw(string path, string fileName);
}

internal sealed class DebugRendererForTests : IDebugRendererForTests
{
    private readonly IDebugRenderer? _externalDebugRenderer;
    private readonly bool _enabled;
    private IVisualOutput _visualOutput;

    public DebugRendererForTests(IDebugRenderer? externalDebugRenderer, bool enabled)
    {
        _externalDebugRenderer = externalDebugRenderer;
        _enabled = enabled;
        _visualOutput = TestKit.CreateVisualOutput(1.0, false);
    }

    #region Implementation of IDebugRendererForTests

    public void BeginDraw(double scale = 1d)
    {
        _visualOutput = TestKit.CreateVisualOutput(scale, _enabled);
    }

    public void EndDrawUsingTestContext(int stage = 0)
    {
        _visualOutput.SaveToFileUsingTestContext($"[{stage}]");
        _visualOutput.Dispose();
        _visualOutput = TestKit.CreateVisualOutput(1.0, false);
    }

    public void EndDraw(string path, string fileName)
    {
        _visualOutput.SaveToFile(path, fileName);
        _visualOutput.Dispose();
        _visualOutput = TestKit.CreateVisualOutput(1.0, false);
    }

    #endregion

    #region Implementation of IDebugRenderer

    public void DrawCircle(in Circle circle, Color color)
    {
        _externalDebugRenderer?.DrawCircle(in circle, color);

        _visualOutput.DrawCircle(circle, color);
    }

    public void DrawRectangle(in AxisAlignedRectangle rectangle, Color color, in Matrix3x3 transform)
    {
        _externalDebugRenderer?.DrawRectangle(in rectangle, color, in transform);

        _visualOutput.DrawRectangle(rectangle.ToRectangle().Transform(transform), color);
    }

    #endregion

    public void Dispose()
    {
        _visualOutput.Dispose();
    }
}