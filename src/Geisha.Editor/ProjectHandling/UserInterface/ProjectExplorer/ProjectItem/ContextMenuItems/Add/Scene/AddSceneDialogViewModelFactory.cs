using Geisha.Editor.ProjectHandling.Model;
using Geisha.Editor.SceneEditor.Model;

namespace Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem.ContextMenuItems.Add.Scene
{
    internal interface IAddSceneDialogViewModelFactory
    {
        AddSceneDialogViewModel Create(IProject project);
        AddSceneDialogViewModel Create(IProjectFolder folder);
    }

    internal sealed class AddSceneDialogViewModelFactory : IAddSceneDialogViewModelFactory
    {
        private readonly ICreateEmptySceneService _createEmptySceneService;

        public AddSceneDialogViewModelFactory(ICreateEmptySceneService createEmptySceneService)
        {
            _createEmptySceneService = createEmptySceneService;
        }

        public AddSceneDialogViewModel Create(IProject project)
        {
            return new AddSceneDialogViewModel(_createEmptySceneService, project, null);
        }

        public AddSceneDialogViewModel Create(IProjectFolder folder)
        {
            return new AddSceneDialogViewModel(_createEmptySceneService, null, folder);
        }
    }
}