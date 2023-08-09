namespace Geisha.Engine.Rendering.Backend;

/// <summary>
///     Represents set of rendering related statistics provided by <see cref="IRenderingBackend" />.
/// </summary>
/// <param name="DrawCalls">Number of draw calls since last call of <see cref="IRenderingBackend.Present" />.</param>
public readonly record struct RenderingStatistics(
    int DrawCalls
);