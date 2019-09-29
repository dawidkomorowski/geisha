using System.Windows;
using System.Windows.Controls;

namespace Geisha.Editor.Core.Docking
{
    internal sealed class LayoutItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ToolTemplate { get; set; }
        public DataTemplate DocumentTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is ToolViewModel) return ToolTemplate;
            if (item is DocumentViewModel) return DocumentTemplate;

            return base.SelectTemplate(item, container);
        }
    }
}