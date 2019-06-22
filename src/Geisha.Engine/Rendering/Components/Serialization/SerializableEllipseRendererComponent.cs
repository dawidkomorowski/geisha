using Geisha.Engine.Core.SceneModel.Serialization;

namespace Geisha.Engine.Rendering.Components.Serialization
{
    /// <summary>
    ///     Represents serializable <see cref="EllipseRendererComponent" /> that is used in a scene file content.
    /// </summary>
    public sealed class SerializableEllipseRendererComponent : ISerializableComponent
    {
        /// <summary>
        ///     Represents <see cref="EllipseRendererComponent.Visible" /> property of <see cref="EllipseRendererComponent" />.
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        ///     Represents <see cref="EllipseRendererComponent.SortingLayerName" /> property of
        ///     <see cref="EllipseRendererComponent" />.
        /// </summary>
        public string SortingLayerName { get; set; }

        /// <summary>
        ///     Represents <see cref="EllipseRendererComponent.OrderInLayer" /> property of
        ///     <see cref="EllipseRendererComponent" />.
        /// </summary>
        public int OrderInLayer { get; set; }

        /// <summary>
        ///     Represents <see cref="EllipseRendererComponent.RadiusX" /> property of
        ///     <see cref="EllipseRendererComponent" />.
        /// </summary>
        public double RadiusX { get; set; }

        /// <summary>
        ///     Represents <see cref="EllipseRendererComponent.RadiusY" /> property of
        ///     <see cref="EllipseRendererComponent" />.
        /// </summary>
        public double RadiusY { get; set; }

        /// <summary>
        ///     Represents <see cref="EllipseRendererComponent.Color" /> property of <see cref="EllipseRendererComponent" />.
        /// </summary>
        public int ColorArgb { get; set; }

        /// <summary>
        ///     Represents <see cref="EllipseRendererComponent.FillInterior" /> property of
        ///     <see cref="EllipseRendererComponent" />.
        /// </summary>
        public bool FillInterior { get; set; }
    }
}