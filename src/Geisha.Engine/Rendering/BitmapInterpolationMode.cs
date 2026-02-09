namespace Geisha.Engine.Rendering;

/// <summary>
///     Specifies how bitmap images are interpolated when they are rendered with any transform where the pixels in the
///     bitmap don't line up exactly one-to-one with pixels on screen.
/// </summary>
public enum BitmapInterpolationMode
{
    /// <summary>
    ///     Uses nearest-neighbor sampling, preserving sharp images (pixel-perfect look) at the cost of visible aliasing.
    ///     This mode is a good fit for low-resolution pixel art images.
    /// </summary>
    NearestNeighbor,

    /// <summary>
    ///     Uses linear interpolation, producing smoother results at the cost of slightly blurred images.
    ///     This mode is a good fit for high-resolution images.
    /// </summary>
    Linear
}