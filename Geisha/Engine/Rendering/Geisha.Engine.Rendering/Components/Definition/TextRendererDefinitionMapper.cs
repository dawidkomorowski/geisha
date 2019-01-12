using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Framework.Rendering;

namespace Geisha.Engine.Rendering.Components.Definition
{
    internal class TextRendererDefinitionMapper : SerializableComponentMapperAdapter<TextRenderer, TextRendererDefinition>
    {
        protected override TextRendererDefinition MapToSerializable(TextRenderer component)
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

        protected override TextRenderer MapFromSerializable(TextRendererDefinition serializableComponent)
        {
            return new TextRenderer
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