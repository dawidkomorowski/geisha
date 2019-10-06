using Geisha.Editor.Core;
using Geisha.Editor.SceneEditor.Model;

namespace Geisha.Editor.SceneEditor.UserInterface.SceneOutline.SceneOutlineItem
{
    internal sealed class SceneRootViewModel : SceneOutlineItemViewModel
    {
        private readonly SceneModel _sceneModel;

        public SceneRootViewModel(SceneModel sceneModel)
        {
            _sceneModel = sceneModel;

            Name = "Scene";

            foreach (var entityModel in _sceneModel.RootEntities)
            {
                Items.Add(new EntityViewModel(entityModel));
            }

            ContextMenuItems.Add(new ContextMenuItem("Add entity", new RelayCommand(AddEntity)));

            _sceneModel.EntityAdded += SceneModelOnEntityAdded;
        }

        private void AddEntity()
        {
            _sceneModel.AddEntity();
        }

        private void SceneModelOnEntityAdded(object sender, EntityAddedEventArgs e)
        {
            Items.Add(new EntityViewModel(e.EntityModel));
        }
    }
}