using System.Windows;
using System.Windows.Controls;
using Geisha.Editor.Core.ViewModels;

namespace Geisha.Editor.Core.Views
{
    public class LayoutItemStyleSelector : StyleSelector
    {
        public Style DockableViewStyle { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is DockableViewViewModel) return DockableViewStyle;

            return base.SelectStyle(item, container);
        }
    }
}