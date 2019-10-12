using System.Windows;
using System.Windows.Controls;
using Geisha.Editor.Core.Docking;
using Geisha.Editor.SceneEditor.UserInterface.SceneOutline.SceneOutlineItem;

namespace Geisha.Editor.SceneEditor.UserInterface.SceneOutline
{
    /// <summary>
    /// Interaction logic for SceneOutlineView.xaml
    /// </summary>
    internal partial class SceneOutlineView : UserControl, IView
    {
        public SceneOutlineView()
        {
            InitializeComponent();
        }

        private void TreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ((SceneOutlineViewModel) DataContext).SelectedItem = (SceneOutlineItemViewModel) e.NewValue;
        }
    }
}