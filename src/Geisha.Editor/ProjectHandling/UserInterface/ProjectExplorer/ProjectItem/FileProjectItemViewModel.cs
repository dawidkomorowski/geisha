using Geisha.Editor.ProjectHandling.Domain;

namespace Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem
{
    public class FileProjectItemViewModel : ProjectItemViewModel
    {
        public FileProjectItemViewModel(IProjectItem projectItem) : base(projectItem.Name)
        {
        }
    }
}