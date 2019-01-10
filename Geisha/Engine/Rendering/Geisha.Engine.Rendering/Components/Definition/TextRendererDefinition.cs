using Geisha.Engine.Core.SceneModel.Serialization;

namespace Geisha.Engine.Rendering.Components.Definition
{
    /// <summary>
    ///     Represents serializable <see cref="TextRenderer" /> that is used in a scene file content.
    /// </summary>
    public sealed class TextRendererDefinition : IComponentDefinition
    {
        /// <summary>
        ///     Defines <see cref="TextRenderer.Visible" /> property of <see cref="TextRenderer" />.
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        ///     Defines <see cref="TextRenderer.SortingLayerName" /> property of <see cref="TextRenderer" />.
        /// </summary>
        public string SortingLayerName { get; set; }

        /// <summary>
        ///     Defines <see cref="TextRenderer.OrderInLayer" /> property of <see cref="TextRenderer" />.
        /// </summary>
        public int OrderInLayer { get; set; }

        /// <summary>
        ///     Defines <see cref="TextRenderer.Text" /> property of <see cref="TextRenderer" />.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        ///     Defines <see cref="TextRenderer.FontSize" /> property of <see cref="TextRenderer" /> in points (unit).
        /// </summary>
        public double FontSize { get; set; }

        /// <summary>
        ///     Defines <see cref="TextRenderer.Color" /> property of <see cref="TextRenderer" />.
        /// </summary>
        public int ColorArgb { get; set; }
    }
}