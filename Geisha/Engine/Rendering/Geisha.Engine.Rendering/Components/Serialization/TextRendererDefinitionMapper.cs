using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Framework.Rendering;

namespace Geisha.Engine.Rendering.Components.Serialization
{
    internal class TextRendererDefinitionMapper : SerializableComponentMapperAdapter<TextRendererComponent, TextRendererDefinition>
    {
        protected override TextRendererDefinition MapToSerializable(TextRendererComponent component)
        {
            return new TextRendererDefinition
            {
                Visible = component.Visible,
                SortingLayerName = component.SortingLayerName,
                OrderInLayer = component.OrderInLayer,
                Text = component.Text,
                FontSize = component.FontSize.Points,
                ColorArgb = component.Color.ToArgb()
            };
        }

        protected override TextRendererComponent MapFromSerializable(TextRendererDefinition serializableComponent)
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