using System.ComponentModel.Composition;
using Geisha.Engine.Core.SceneModel.Definition;
using Geisha.Framework.Rendering;

namespace Geisha.Engine.Rendering.Components.Definition
{
    [Export(typeof(IComponentDefinitionMapper))]
    internal class TextRendererDefinitionMapper : ComponentDefinitionMapperAdapter<TextRenderer, TextRendererDefinition>
    {
        protected override TextRendererDefinition ToDefinition(TextRenderer component)
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

        protected override TextRenderer FromDefinition(TextRendererDefinition componentDefinition)
        {
            return new TextRenderer
            {
                Visible = componentDefinition.Visible,
                SortingLayerName = componentDefinition.SortingLayerName,
                OrderInLayer = componentDefinition.OrderInLayer,
                Text = componentDefinition.Text,
                FontSize = FontSize.FromPoints(componentDefinition.FontSize),
                Color = Color.FromArgb(componentDefinition.ColorArgb)
            };
        }
    }
}