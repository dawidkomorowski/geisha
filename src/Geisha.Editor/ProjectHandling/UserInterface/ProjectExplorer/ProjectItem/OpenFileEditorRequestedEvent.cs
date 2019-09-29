using Geisha.Editor.Core;
using Geisha.Editor.ProjectHandling.Model;

namespace Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem
{
    internal sealed class OpenFileEditorRequestedEvent : IEvent
    {
        public OpenFileEditorRequestedEvent(IProjectFile file)
        {
            File = file;
        }

        public IProjectFile File { get; }
    }
}