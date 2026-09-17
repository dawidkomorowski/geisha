using Geisha.Engine.Core.Math;

namespace Geisha.Engine.Windowing;

// TODO: Add documentation.
public sealed record WindowingConfiguration
{
    // TODO: Add documentation.
    public bool AllowWindowResizing { get; init; } = false;
    public DisplayMode DisplayMode { get; init; } = DisplayMode.Windowed;
    public Size WindowClientSize { get; init; } = new(1280, 720);
}