using Geisha.Editor.Core;
using Geisha.Editor.ProjectHandling.Model;

namespace Geisha.Editor.ProjectHandling.UserInterface.NewProjectDialog
{
    public interface INewProjectDialogViewModelFactory
    {
        NewProjectDialogViewModel Create();
    }

    public class NewProjectDialogViewModelFactory : INewProjectDialogViewModelFactory
    {
        private readonly IOpenFileDialogService _openFileDialogService;
        private readonly IProjectService _projectService;

        public NewProjectDialogViewModelFactory(IOpenFileDialogService openFileDialogService, IProjectService projectService)
        {
            _openFileDialogService = openFileDialogService;
            _projectService = projectService;
        }

        public NewProjectDialogViewModel Create()
        {
            return new NewProjectDialogViewModel(_openFileDialogService, _projectService);
        }
    }
}