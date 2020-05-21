using System.Windows;
using System.Windows.Controls;

namespace Geisha.Editor.Core.Docking
{
    internal sealed class LayoutItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? ToolTemplate { get; set; }
        public DataTemplate? DocumentTemplate { get; set; }

        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        {
            return item switch
            {
                ToolViewModel _ => ToolTemplate,
                DocumentViewModel _ => DocumentTemplate,
                _ => base.SelectTemplate(item, container)
            };
        }
    }
}