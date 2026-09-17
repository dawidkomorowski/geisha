using Geisha.Engine.Core.Math;

namespace Geisha.Engine.Windowing;

// TODO: Add documentation.
public sealed record WindowingConfiguration
{
    // TODO: Add documentation.
    public bool AllowWindowResizing { get; init; } = false;
    public Size WindowClientSize { get; init; } = new(1280, 720);
}