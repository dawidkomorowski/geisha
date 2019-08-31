using Geisha.Editor.ProjectHandling.Model;

namespace Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem.ContextMenuItems.Add
{
    public interface IAddNewFolderDialogViewModelFactory
    {
        AddNewFolderDialogViewModel Create(IProjectItemObsolete projectItem);
    }

    public class AddNewFolderDialogViewModelFactory : IAddNewFolderDialogViewModelFactory
    {
        private readonly IProjectServiceObsolete _projectService;

        public AddNewFolderDialogViewModelFactory(IProjectServiceObsolete projectService)
        {
            _projectService = projectService;
        }

        public AddNewFolderDialogViewModel Create(IProjectItemObsolete projectItem)
        {
            return new AddNewFolderDialogViewModel(projectItem, _projectService);
        }
    }
}