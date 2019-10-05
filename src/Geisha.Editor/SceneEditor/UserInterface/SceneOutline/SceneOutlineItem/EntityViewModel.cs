using Geisha.Editor.SceneEditor.Model;

namespace Geisha.Editor.SceneEditor.UserInterface.SceneOutline.SceneOutlineItem
{
    internal sealed class EntityViewModel : SceneOutlineItemViewModel
    {
        private readonly EntityModel _entityModel;

        public EntityViewModel(EntityModel entityModel)
        {
            _entityModel = entityModel;

            Name = _entityModel.Name;

            foreach (var model in _entityModel.Children)
            {
                Items.Add(new EntityViewModel(model));
            }
        }
    }
}