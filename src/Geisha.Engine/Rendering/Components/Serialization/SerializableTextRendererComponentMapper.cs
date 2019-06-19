using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Framework.Rendering;

namespace Geisha.Engine.Rendering.Components.Serialization
{
    internal sealed class SerializableTextRendererComponentMapper : SerializableComponentMapperAdapter<TextRendererComponent, SerializableTextRendererComponent>
    {
        protected override SerializableTextRendererComponent MapToSerializable(TextRendererComponent component)
        {
            return new SerializableTextRendererComponent
            {
                Visible = component.Visible,
                SortingLayerName = component.SortingLayerName,
                OrderInLayer = component.OrderInLayer,
                Text = component.Text,
                FontSize = component.FontSize.Points,
                ColorArgb = component.Color.ToArgb()
            };
        }

        protected override TextRendererComponent MapFromSerializable(SerializableTextRendererComponent serializableComponent)
        {
            return new TextRendererComponent
            {
                Visible = serializableComponent.Visible,
                SortingLayerName = serializableComponent.SortingLayerName,
                OrderInLayer = serializableComponent.OrderInLayer,
                Text = serializableComponent.Text,
                FontSize = FontSize.FromPoints(serializableComponent.FontSize),
                Color = Color.FromArgb(serializableComponent.ColorArgb)
            };
        }
    }
}