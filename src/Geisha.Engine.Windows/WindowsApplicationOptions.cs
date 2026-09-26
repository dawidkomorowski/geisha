namespace Geisha.Engine.Windows;

/// <summary>
///     Configures Windows-specific behavior when starting an application with <see cref="WindowsApplication" />.
/// </summary>
public sealed record WindowsApplicationOptions
{
    /// <summary>
    ///     Specifies DirectX-specific application options.
    /// </summary>
    public DirectXOptions DirectX { get; init; } = new();
}

/// <summary>
///     Configures DirectX-specific behavior for a Windows application.
/// </summary>
public sealed record DirectXOptions
{
    /// <summary>
    ///     Specifies whether DirectX rendering buffers are resized after VSync is changed at runtime. Default is <c>false</c>.
    /// </summary>
    /// <remarks>
    ///     Enable this option only on systems affected by GPU driver issues where changing VSync at runtime does not apply
    ///     synchronization correctly or produces visual artifacts. This has been observed on Windows systems using Qualcomm
    ///     Adreno GPUs. Resizing buffers after a VSync change adds work to the transition.
    /// </remarks>
    public bool ResizeBuffersAfterVSyncChange { get; init; }
}