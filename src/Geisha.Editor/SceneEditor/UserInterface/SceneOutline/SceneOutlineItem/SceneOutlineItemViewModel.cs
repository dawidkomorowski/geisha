using System.Collections.ObjectModel;
using Geisha.Editor.Core;

namespace Geisha.Editor.SceneEditor.UserInterface.SceneOutline.SceneOutlineItem
{
    internal abstract class SceneOutlineItemViewModel : ViewModel
    {
        private readonly IProperty<string> _name;

        protected SceneOutlineItemViewModel()
        {
            _name = CreateProperty<string>(nameof(Name));
        }

        public string Name
        {
            get => _name.Get();
            protected set => _name.Set(value);
        }

        public ObservableCollection<SceneOutlineItemViewModel> Items { get; } = new ObservableCollection<SceneOutlineItemViewModel>();
    }
}