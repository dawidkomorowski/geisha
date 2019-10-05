using Geisha.Editor.Core;
using Geisha.Editor.SceneEditor.Model;

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
    }

    internal sealed class SceneRootViewModel : SceneOutlineItemViewModel
    {
        private readonly SceneModel _sceneModel;

        public SceneRootViewModel(SceneModel sceneModel)
        {
            _sceneModel = sceneModel;

            Name = "Scene";
        }
    }
}