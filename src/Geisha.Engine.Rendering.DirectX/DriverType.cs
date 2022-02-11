namespace Geisha.Engine.Rendering.DirectX
{
    /// <summary>
    ///     Specifies type of rendering API driver.
    /// </summary>
    public enum DriverType
    {
        /// <summary>
        ///     Hardware driver uses hardware acceleration for maximum rendering performance. Output image may differ depending on
        ///     driver implementation provided by vendor.
        /// </summary>
        Hardware,

        /// <summary>
        ///     Software driver uses software rasterizer for maximum correctness of output image sacrificing performance. It is
        ///     useful for testing purposes.
        /// </summary>
        Software
    }
}