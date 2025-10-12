namespace Geisha.Engine.Rendering.Backend;

/// <summary>
///     Provides information about the rendering backend and the graphics adapter it uses.
/// </summary>
/// <remarks>
///     Values provided by <see cref="RenderingBackendInfo" /> are intended for diagnostics, logging and display in tools.
/// </remarks>
/// <param name="Name">
///     Human-readable name of the rendering backend (for example: "DirectX 11", "OpenGL", "Vulkan").
/// </param>
/// <param name="GraphicsAdapterName">
///     Name of the graphics adapter used by the backend (for example: GPU model name reported by the driver).
/// </param>
/// <param name="VideoMemorySize">
///     Total dedicated video memory size of the adapter in bytes. This value is reported by the underlying graphics API
///     and may be an estimate provided by the driver.
/// </param>
/// <param name="FeatureLevel">
///     Graphics API feature level or capability string (for example: Direct3D feature level like "Level_11_0").
/// </param>
public sealed record RenderingBackendInfo(
    string Name,
    string GraphicsAdapterName,
    long VideoMemorySize,
    string FeatureLevel
)
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    ///     Video memory size expressed in gigabytes (GB) using decimal units (1 GB = 1,000,000,000 bytes).
    /// </summary>
    /// <remarks>
    ///     This uses SI units for readability in logs and UI. If you need binary gibibytes (GiB), convert using
    ///     1 GiB = 1,073,741,824 bytes.
    /// </remarks>
    public int VideoMemorySizeGB => (int)(VideoMemorySize / 1_000_000_000);
}