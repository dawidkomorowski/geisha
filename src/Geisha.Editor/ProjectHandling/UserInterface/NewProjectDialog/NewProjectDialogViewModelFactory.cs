using Geisha.Editor.ProjectHandling.Model;

namespace Geisha.Editor.ProjectHandling.UserInterface.NewProjectDialog
{
    public interface INewProjectDialogViewModelFactory
    {
        NewProjectDialogViewModel Create();
    }

    public class NewProjectDialogViewModelFactory : INewProjectDialogViewModelFactory
    {
        private readonly IProjectService _projectService;

        public NewProjectDialogViewModelFactory(IProjectService projectService)
        {
            _projectService = projectService;
        }

        public NewProjectDialogViewModel Create()
        {
            return new NewProjectDialogViewModel(_projectService);
        }
    }
}