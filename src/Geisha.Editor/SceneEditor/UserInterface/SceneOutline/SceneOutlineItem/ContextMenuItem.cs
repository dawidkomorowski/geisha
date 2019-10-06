using System.Windows.Input;

namespace Geisha.Editor.SceneEditor.UserInterface.SceneOutline.SceneOutlineItem
{
    internal sealed class ContextMenuItem
    {
        public string Name { get; }
        public ICommand Command { get; }

        public ContextMenuItem(string name, ICommand command)
        {
            Name = name;
            Command = command;
        }
    }
}