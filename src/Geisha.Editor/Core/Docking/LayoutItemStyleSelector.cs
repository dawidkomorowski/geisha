using System.Windows;
using System.Windows.Controls;

namespace Geisha.Editor.Core.Docking
{
    internal sealed class LayoutItemStyleSelector : StyleSelector
    {
        public Style ToolStyle { get; set; }
        public Style DocumentStyle { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is ToolViewModel) return ToolStyle;
            if (item is DocumentViewModel) return DocumentStyle;

            return base.SelectStyle(item, container);
        }
    }
}