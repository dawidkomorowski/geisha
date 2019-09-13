using Geisha.Editor.ProjectHandling.Model;

namespace Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem.ContextMenuItems.Add.NewFolder
{
    internal interface IAddNewFolderDialogViewModelFactory
    {
        AddNewFolderDialogViewModel Create(IProjectFolder folder);
    }

    internal sealed class AddNewFolderDialogViewModelFactory : IAddNewFolderDialogViewModelFactory
    {
        private readonly IProjectService _projectService;

        public AddNewFolderDialogViewModelFactory(IProjectService projectService)
        {
            _projectService = projectService;
        }

        public AddNewFolderDialogViewModel Create(IProjectFolder folder)
        {
            return new AddNewFolderDialogViewModel(folder, _projectService);
        }
    }
}