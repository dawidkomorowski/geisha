namespace Geisha.Engine.Rendering
{
    /// <summary>
    ///     Specifies the alignment of paragraph text along the reading direction axis, relative to the leading and trailing
    ///     edge of the layout box.
    /// </summary>
    /// <remarks>
    ///     Geisha Engine only supports reading direction from left to right so alignment option <see cref="Leading" />
    ///     refers to left and <see cref="Trailing" /> refers to right.
    /// </remarks>
    public enum TextAlignment
    {
        /// <summary>
        ///     The leading edge of the paragraph text is aligned to the leading edge of the layout box.
        /// </summary>
        Leading,

        /// <summary>
        ///     The trailing edge of the paragraph text is aligned to the trailing edge of the layout box.
        /// </summary>
        Trailing,

        /// <summary>
        ///     The center of the paragraph text is aligned to the center of the layout box.
        /// </summary>
        Center,

        /// <summary>
        ///     Align text to the leading side, and also justify text to fill the lines.
        /// </summary>
        Justified
    }
}