namespace Geisha.TestUtils;

public static class TestKit
{
    public static IVisualOutput CreateVisualOutput(double scale = 1d, bool enabled = true) => enabled ? new VisualOutput(scale) : new NullVisualOutput();
}