using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Geisha.Editor.Core.ViewModels.DockableViews.ProjectExplorer.ProjectItem
{
    // TODO Add context menu 'Add' that allows to add new items (modal dialog with items to select from vide VS).
    public abstract class ProjectItemViewModel
    {
        protected ProjectItemViewModel(string name)
        {
            Name = name;
        }

        public string Name { get; }
        public ObservableCollection<ProjectItemViewModel> Items { get; } = new ObservableCollection<ProjectItemViewModel>();
        public ObservableCollection<ContextMenuItem> ContextMenuItems { get; } = new ObservableCollection<ContextMenuItem>();

        protected void UpdateItems(IEnumerable<ProjectItemViewModel> expectedItems)
        {
            var expectedItemsList = expectedItems.ToList();

            var projectItemsToAdd = expectedItemsList.Where(ei => Items.All(i => i.Name != ei.Name)).ToList();
            var itemsToRemove = Items.Where(i => expectedItemsList.All(ei => ei.Name != i.Name)).ToList();

            foreach (var item in itemsToRemove)
            {
                Items.Remove(item);
            }

            foreach (var projectItemViewModel in projectItemsToAdd)
            {
                Items.Add(projectItemViewModel);
            }

            for (var newIndex = 0; newIndex < expectedItemsList.Count; newIndex++)
            {
                var projectItemViewModel = expectedItemsList[newIndex];
                var oldIndex = Items.ToList().FindIndex(i => i.Name == projectItemViewModel.Name);

                Items.Move(oldIndex, newIndex);
            }
        }
    }
}