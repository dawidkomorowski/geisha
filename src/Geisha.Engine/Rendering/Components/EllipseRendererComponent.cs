using System;
using Geisha.Common.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;

namespace Geisha.Engine.Rendering.Components
{
    /// <summary>
    ///     Ellipse renderer component enables entity with ellipse rendering functionality.
    /// </summary>
    [ComponentId("Geisha.Engine.Rendering.EllipseRendererComponent")]
    public sealed class EllipseRendererComponent : Renderer2DComponent
    {
        /// <summary>
        ///     Radius of the circle.
        /// </summary>
        /// <remarks>
        ///     If assigned changes ellipse to circle by modifying <see cref="RadiusX" /> and <see cref="RadiusY" />. When
        ///     accessed returns circle radius if ellipse is circle; otherwise throws <see cref="EllipseIsNotCircleException" />.
        /// </remarks>
        /// <exception cref="EllipseIsNotCircleException">Thrown when ellipse does not represent a circle.</exception>
        public double Radius
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            get => RadiusX == RadiusY ? RadiusX : throw new EllipseIsNotCircleException(RadiusX, RadiusY);
            set
            {
                RadiusX = value;
                RadiusY = value;
            }
        }

        /// <summary>
        ///     X radius of the ellipse.
        /// </summary>
        public double RadiusX { get; set; }

        /// <summary>
        ///     Y radius of the ellipse.
        /// </summary>
        public double RadiusY { get; set; }

        /// <summary>
        ///     Color of the ellipse.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        ///     Specifies whether to fill interior of ellipse or draw only border. If true interior is filled with color.
        /// </summary>
        public bool FillInterior { get; set; }
    }

    /// <summary>
    ///     The exception that is thrown when accessing <see cref="EllipseRendererComponent.Radius" /> when ellipse does not
    ///     represent a circle.
    /// </summary>
    public sealed class EllipseIsNotCircleException : Exception
    {
        public EllipseIsNotCircleException(double radiusX, double radiusY)
            : base($"Ellipse is not a circle. Circle radius cannot be calculated. RadiusX = {radiusX}, RadiusY = {radiusY}.")
        {
            RadiusX = radiusX;
            RadiusY = radiusY;
        }

        /// <summary>
        ///     X radius of the ellipse.
        /// </summary>
        public double RadiusX { get; }

        /// <summary>
        ///     Y radius of the ellipse.
        /// </summary>
        public double RadiusY { get; }
    }

    internal sealed class EllipseRendererComponentFactory : ComponentFactory<EllipseRendererComponent>
    {
        protected override EllipseRendererComponent CreateComponent() => new EllipseRendererComponent();
    }

    internal sealed class EllipseRendererComponentSerializer : ComponentSerializer<EllipseRendererComponent>
    {
        private const string Visible = "Visible";
        private const string SortingLayerName = "SortingLayerName";
        private const string OrderInLayer = "OrderInLayer";
        private const string RadiusX = "RadiusX";
        private const string RadiusY = "RadiusY";
        private const string Color = "Color";
        private const string FillInterior = "FillInterior";

        public EllipseRendererComponentSerializer() : base(new ComponentId())
        {
            throw new InvalidOperationException();
        }

        protected override void Serialize(EllipseRendererComponent component, IComponentDataWriter componentDataWriter)
        {
            componentDataWriter.WriteBool(Visible, component.Visible);
            componentDataWriter.WriteString(SortingLayerName, component.SortingLayerName);
            componentDataWriter.WriteInt(OrderInLayer, component.OrderInLayer);
            componentDataWriter.WriteDouble(RadiusX, component.RadiusX);
            componentDataWriter.WriteDouble(RadiusY, component.RadiusY);
            componentDataWriter.WriteColor(Color, component.Color);
            componentDataWriter.WriteBool(FillInterior, component.FillInterior);
        }

        protected override void Deserialize(EllipseRendererComponent component, IComponentDataReader componentDataReader)
        {
            component.Visible = componentDataReader.ReadBool(Visible);
            component.SortingLayerName = componentDataReader.ReadString(SortingLayerName) ??
                                         throw new InvalidOperationException($"{SortingLayerName} cannot be null.");
            component.OrderInLayer = componentDataReader.ReadInt(OrderInLayer);
            component.RadiusX = componentDataReader.ReadDouble(RadiusX);
            component.RadiusY = componentDataReader.ReadDouble(RadiusY);
            component.Color = componentDataReader.ReadColor(Color);
            component.FillInterior = componentDataReader.ReadBool(FillInterior);
        }
    }
}