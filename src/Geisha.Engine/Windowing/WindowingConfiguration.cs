namespace Geisha.Engine.Windowing;

// TODO: Add documentation.
public sealed record WindowingConfiguration
{
    // TODO: Move this config option from rendering configuration.
    // TODO: Add documentation.
    public bool AllowWindowResizing { get; init; } = false;
}