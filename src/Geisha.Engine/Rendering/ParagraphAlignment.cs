namespace Geisha.Engine.Rendering
{
    /// <summary>
    ///     Specifies the alignment of paragraph text along the flow direction axis, relative to the top and bottom of the
    ///     flow's layout box.
    /// </summary>
    /// <remarks>
    ///     Geisha Engine only supports flow direction from top to bottom so alignment option <see cref="Near" /> refers
    ///     to top and <see cref="Far" /> refers to bottom.
    /// </remarks>
    public enum ParagraphAlignment
    {
        /// <summary>
        ///     The top of the text flow is aligned to the top edge of the layout box.
        /// </summary>
        Near,

        /// <summary>
        ///     The bottom of the text flow is aligned to the bottom edge of the layout box.
        /// </summary>
        Far,

        /// <summary>
        ///     The center of the flow is aligned to the center of the layout box.
        /// </summary>
        Center
    }
}