using Geisha.Editor.ProjectHandling.Model;

namespace Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem.ContextMenuItems.Add
{
    public interface IAddNewFolderDialogViewModelFactory
    {
        AddNewFolderDialogViewModel Create(IProjectFolder folder);
    }

    public class AddNewFolderDialogViewModelFactory : IAddNewFolderDialogViewModelFactory
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