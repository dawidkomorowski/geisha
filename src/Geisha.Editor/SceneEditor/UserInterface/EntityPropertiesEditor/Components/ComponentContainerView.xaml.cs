using System.Windows;
using System.Windows.Controls;
using Geisha.Editor.Core;

namespace Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor.Components
{
    /// <summary>
    /// Interaction logic for ComponentContainerView.xaml
    /// </summary>
    public partial class ComponentContainerView : UserControl
    {
        public ComponentContainerView()
        {
            InitializeComponent();

            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is ViewModel viewModel)
            {
                var view = ViewRepository.Default.CreateView(viewModel);
                view.DataContext = viewModel;
                ContentControl.Content = view;
            }
        }
    }
}