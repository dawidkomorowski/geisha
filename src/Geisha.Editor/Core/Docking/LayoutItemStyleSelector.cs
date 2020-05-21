using System.Windows;
using System.Windows.Controls;

namespace Geisha.Editor.Core.Docking
{
    internal sealed class LayoutItemStyleSelector : StyleSelector
    {
        public Style? ToolStyle { get; set; }
        public Style? DocumentStyle { get; set; }

        public override Style? SelectStyle(object item, DependencyObject container)
        {
            return item switch
            {
                ToolViewModel _ => ToolStyle,
                DocumentViewModel _ => DocumentStyle,
                _ => base.SelectStyle(item, container)
            };
        }
    }
}