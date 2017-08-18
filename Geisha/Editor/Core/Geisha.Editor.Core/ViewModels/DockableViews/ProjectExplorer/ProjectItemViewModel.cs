using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Geisha.Editor.Core.ViewModels.DockableViews.ProjectExplorer
{
    // TODO Add context menu 'Add' that allows to add directories and new items (modal dialog with items to select from vide VS).
    public class ProjectItemViewModel
    {
        public string Name { get; }
        public ObservableCollection<ProjectItemViewModel> Items { get; }

        public ProjectItemViewModel(string name, IEnumerable<ProjectItemViewModel> projectItems)
        {
            Name = name;
            Items = new ObservableCollection<ProjectItemViewModel>(projectItems);
        }
    }
}