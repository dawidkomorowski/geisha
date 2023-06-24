using System;

namespace Geisha.Engine.Rendering.Backend
{
    /// <summary>
    ///     Represents a block of text after it has been fully analyzed and formatted as described by the font and paragraph
    ///     properties used to format text.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         To create <see cref="ITextLayout" /> use <see cref="IRenderingContext2D.CreateTextLayout" />. To render text
    ///         block defined by <see cref="ITextLayout" /> use <see cref="IRenderingContext2D.DrawTextLayout" />.
    ///     </para>
    ///     <para><see cref="ITextLayout" /> automatically handles text wrapping to fit in layout box.</para>
    /// </remarks>
    public interface ITextLayout : IDisposable
    {
        /// <summary>
        ///     Gets text content of layout object.
        /// </summary>
        public string Text { get; }

        /// <summary>
        ///     Gets or sets font family name.
        /// </summary>
        public string FontFamilyName { get; set; }

        /// <summary>
        ///     Gets or sets font size.
        /// </summary>
        public FontSize FontSize { get; set; }

        /// <summary>
        ///     Gets or sets maximum width of layout box.
        /// </summary>
        public double MaxWidth { get; set; }

        /// <summary>
        ///     Gets or sets maximum height of layout box.
        /// </summary>
        public double MaxHeight { get; set; }

        /// <summary>
        ///     Gets or sets alignment of text in paragraph, relative to the leading and trailing edge of the layout box.
        /// </summary>
        public TextAlignment TextAlignment { get; set; }

        /// <summary>
        ///     Gets or sets alignment option of a paragraph relative to the layout box's top and bottom edge.
        /// </summary>
        public ParagraphAlignment ParagraphAlignment { get; set; }

        /// <summary>
        ///     Gets overall metrics for the formatted text content.
        /// </summary>
        public TextMetrics Metrics { get; }
    }
}