using Geisha.Editor.ProjectHandling.Model;

namespace Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem
{
    public class FileViewModel : ProjectExplorerItemViewModel
    {
        public FileViewModel(IProjectFile file) : base(file.Name)
        {
        }
    }
}