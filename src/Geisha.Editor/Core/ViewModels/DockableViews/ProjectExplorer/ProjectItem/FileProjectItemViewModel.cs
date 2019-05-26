using Geisha.Editor.Core.Models.Domain.ProjectHandling;

namespace Geisha.Editor.Core.ViewModels.DockableViews.ProjectExplorer.ProjectItem
{
    public class FileProjectItemViewModel : ProjectItemViewModel
    {
        public FileProjectItemViewModel(IProjectItem projectItem) : base(projectItem.Name)
        {
        }
    }
}