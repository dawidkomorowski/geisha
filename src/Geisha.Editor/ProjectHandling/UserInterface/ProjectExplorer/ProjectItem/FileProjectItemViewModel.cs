using Geisha.Editor.ProjectHandling.Model;

namespace Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem
{
    public class FileProjectItemViewModel : ProjectItemViewModel
    {
        public FileProjectItemViewModel(IProjectItemObsolete projectItem) : base(projectItem.Name)
        {
        }
    }
}