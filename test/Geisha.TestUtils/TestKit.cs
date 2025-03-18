namespace Geisha.TestUtils;

public static class TestKit
{
    public static VisualOutput CreateVisualOutput(double scale = 1d) => new(scale);
}