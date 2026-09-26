using System.Collections.Generic;

namespace Geisha.Engine.Rendering;

/// <summary>
///     Configuration of engine rendering subsystem.
/// </summary>
public sealed record RenderingConfiguration
{
    /// <summary>
    ///     Provides name of default sorting layer.
    /// </summary>
    public const string DefaultSortingLayerName = "Default";

    /// <summary>
    ///     Specifies whether VSync is enabled when the rendering system is initialized. When enabled, rendered frames wait
    ///     for vertical synchronization before presentation, limiting the presentation rate to at most the display refresh
    ///     rate. Default is <c>false</c>.
    /// </summary>
    /// <remarks>
    ///     To change VSync at runtime, use <see cref="Systems.IRenderingSystem.VSyncEnabled" />.
    /// </remarks>
    public bool EnableVSync { get; init; } = false;

    /// <summary>
    ///     Specifies whether to display rendering statistics. Default is <c>false</c>.
    /// </summary>
    public bool ShowRenderingStatistics { get; init; } = false;

    /// <summary>
    ///     List of sorting layers in order of rendering that is first layer in the list is rendered first, last layer in the
    ///     list is rendered last (on top of previous layers). Default is <c>["Default"]</c>.
    /// </summary>
    public IReadOnlyList<string> SortingLayersOrder { get; init; } = new List<string> { DefaultSortingLayerName }.AsReadOnly();
}