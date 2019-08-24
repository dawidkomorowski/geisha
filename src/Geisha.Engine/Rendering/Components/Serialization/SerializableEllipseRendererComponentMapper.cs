using Geisha.Engine.Core.SceneModel.Serialization;

namespace Geisha.Engine.Rendering.Components.Serialization
{
    internal sealed class SerializableEllipseRendererComponentMapper
        : SerializableComponentMapperAdapter<EllipseRendererComponent, SerializableEllipseRendererComponent>
    {
        protected override SerializableEllipseRendererComponent MapToSerializable(EllipseRendererComponent component)
        {
            return new SerializableEllipseRendererComponent
            {
                Visible = component.Visible,
                SortingLayerName = component.SortingLayerName,
                OrderInLayer = component.OrderInLayer,
                RadiusX = component.RadiusX,
                RadiusY = component.RadiusY,
                ColorArgb = component.Color.ToArgb(),
                FillInterior = component.FillInterior
            };
        }

        protected override EllipseRendererComponent MapFromSerializable(SerializableEllipseRendererComponent serializableComponent)
        {
            return new EllipseRendererComponent
            {
                Visible = serializableComponent.Visible,
                SortingLayerName = serializableComponent.SortingLayerName,
                OrderInLayer = serializableComponent.OrderInLayer,
                RadiusX = serializableComponent.RadiusX,
                RadiusY = serializableComponent.RadiusY,
                Color = Color.FromArgb(serializableComponent.ColorArgb),
                FillInterior = serializableComponent.FillInterior
            };
        }
    }
}