using Geisha.Common.Math.Serialization;
using Geisha.Engine.Core.SceneModel.Serialization;

namespace Geisha.Engine.Rendering.Components.Serialization
{
    /// <summary>
    ///     Represents serializable <see cref="RectangleRendererComponent" /> that is used in a scene file content.
    /// </summary>
    public sealed class SerializableRectangleRendererComponent : ISerializableComponent
    {
        /// <summary>
        ///     Represents <see cref="RectangleRendererComponent.Visible" /> property of <see cref="RectangleRendererComponent" />.
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        ///     Represents <see cref="RectangleRendererComponent.SortingLayerName" /> property of
        ///     <see cref="RectangleRendererComponent" />.
        /// </summary>
        public string SortingLayerName { get; set; }

        /// <summary>
        ///     Represents <see cref="RectangleRendererComponent.OrderInLayer" /> property of
        ///     <see cref="RectangleRendererComponent" />.
        /// </summary>
        public int OrderInLayer { get; set; }

        /// <summary>
        ///     Represents <see cref="RectangleRendererComponent.Dimension" /> property of
        ///     <see cref="RectangleRendererComponent" />.
        /// </summary>
        public SerializableVector2 Dimension { get; set; }

        /// <summary>
        ///     Represents <see cref="RectangleRendererComponent.Color" /> property of <see cref="RectangleRendererComponent" />.
        /// </summary>
        public int ColorArgb { get; set; }

        /// <summary>
        ///     Represents <see cref="RectangleRendererComponent.FillInterior" /> property of
        ///     <see cref="RectangleRendererComponent" />.
        /// </summary>
        public bool FillInterior { get; set; }
    }
}