using System.Collections.ObjectModel;
using Geisha.Editor.Core;
using Geisha.Editor.SceneEditor.UserInterface.SceneEditor;
using Geisha.Editor.SceneEditor.UserInterface.SceneOutline.SceneOutlineItem;

namespace Geisha.Editor.SceneEditor.UserInterface.SceneOutline
{
    internal sealed class SceneOutlineViewModel : ViewModel
    {
        private readonly IEventBus _eventBus;
        private readonly IProperty<SceneOutlineItemViewModel> _selectedItem;

        public SceneOutlineViewModel(IEventBus eventBus)
        {
            _eventBus = eventBus;

            _selectedItem = CreateProperty<SceneOutlineItemViewModel>(nameof(SelectedItem));
            _selectedItem.Subscribe(vm => vm?.OnSelected());

            _eventBus.RegisterEventHandler<SelectedSceneModelChangedEvent>(SelectedSceneModelChangedEventHandler);
        }

        public ObservableCollection<SceneOutlineItemViewModel> Items { get; } = new ObservableCollection<SceneOutlineItemViewModel>();

        public SceneOutlineItemViewModel SelectedItem
        {
            get => _selectedItem.Get();
            set => _selectedItem.Set(value);
        }

        private void SelectedSceneModelChangedEventHandler(SelectedSceneModelChangedEvent @event)
        {
            Items.Clear();
            Items.Add(new SceneRootViewModel(@event.SceneModel, _eventBus));
        }
    }
}