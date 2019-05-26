using Geisha.Editor.Core.Models.Domain.ProjectHandling;

namespace Geisha.Editor.Core.ViewModels.DockableViews.ProjectExplorer.ProjectItem.ContextMenuItems.Add
{
    public interface IAddNewFolderDialogViewModelFactory
    {
        AddNewFolderDialogViewModel Create(IProjectItem projectItem);
    }

    public class AddNewFolderDialogViewModelFactory : IAddNewFolderDialogViewModelFactory
    {
        private readonly IProjectService _projectService;

        public AddNewFolderDialogViewModelFactory(IProjectService projectService)
        {
            _projectService = projectService;
        }

        public AddNewFolderDialogViewModel Create(IProjectItem projectItem)
        {
            return new AddNewFolderDialogViewModel(projectItem, _projectService);
        }
    }
}