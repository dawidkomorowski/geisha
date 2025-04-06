using Geisha.Engine.Core.Diagnostics;

namespace Geisha.TestUtils;

public static class TestKit
{
    public static IVisualOutput CreateVisualOutput(double scale = 1d, bool enabled = true) => enabled ? new VisualOutput(scale) : new NullVisualOutput();

    public static IDebugRendererForTests CreateDebugRenderer(IDebugRenderer? externalDebugRenderer = null, bool enabled = true) =>
        new DebugRendererForTests(externalDebugRenderer, enabled);
}