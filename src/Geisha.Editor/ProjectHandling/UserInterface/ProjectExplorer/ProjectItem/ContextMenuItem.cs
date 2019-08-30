using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem
{
    public class ContextMenuItem
    {
        public string Name { get; }
        public ICommand Command { get; }
        public ObservableCollection<ContextMenuItem> Items { get; }

        public ContextMenuItem(string name, ICommand command = null) : this(name, command, Enumerable.Empty<ContextMenuItem>())
        {
        }

        public ContextMenuItem(string name, ICommand command, IEnumerable<ContextMenuItem> contextMenuItems)
        {
            Name = name;
            Command = command;
            Items = new ObservableCollection<ContextMenuItem>(contextMenuItems);
        }
    }
}