namespace Geisha.Engine.Rendering.Backend;

/// <summary>
///     Represents set of rendering related statistics provided by <see cref="IRenderingBackend" />.
/// </summary>
/// <param name="DrawCalls">Number of draw calls during last frame.</param>
/// <remarks>
///     Rendering statistics are values captured for last rendered frame that is all rendering up to latest call of
///     <see cref="IRenderingBackend.Present" />.
/// </remarks>
public readonly record struct RenderingStatistics(
    int DrawCalls
);