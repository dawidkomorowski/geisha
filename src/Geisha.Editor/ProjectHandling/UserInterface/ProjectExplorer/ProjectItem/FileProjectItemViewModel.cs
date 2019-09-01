using Geisha.Editor.ProjectHandling.Model;

namespace Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem
{
    public class FileProjectItemViewModel : ProjectItemViewModel
    {
        public FileProjectItemViewModel(IProjectFile file) : base(file.Name)
        {
        }
    }
}