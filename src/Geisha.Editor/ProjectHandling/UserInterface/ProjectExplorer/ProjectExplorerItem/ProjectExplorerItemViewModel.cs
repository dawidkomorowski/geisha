using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectExplorerItem
{
    public abstract class ProjectExplorerItemViewModel
    {
        protected ProjectExplorerItemViewModel(string name)
        {
            Name = name;
        }

        public string Name { get; }
        public ObservableCollection<ProjectExplorerItemViewModel> Items { get; } = new ObservableCollection<ProjectExplorerItemViewModel>();
        public ObservableCollection<ContextMenuItem> ContextMenuItems { get; } = new ObservableCollection<ContextMenuItem>();
        public ICommand DoubleClickCommand { get; protected set; }

        protected void UpdateItems(IEnumerable<ProjectExplorerItemViewModel> expectedItems)
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