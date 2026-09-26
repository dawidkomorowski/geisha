namespace Geisha.Engine.Windows;

// TODO: Add documentation.
public sealed record WindowsApplicationOptions
{
    public DirectXOptions DirectX { get; init; } = new();
}

public sealed record DirectXOptions
{
    public bool ResizeBuffersAfterVSyncChange { get; init; }
}