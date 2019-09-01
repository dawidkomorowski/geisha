using System;

namespace Geisha.Editor.ProjectHandling.Model
{
    public sealed class ProjectFolderAddedEventArgs : EventArgs
    {
        public ProjectFolderAddedEventArgs(IProjectFolder folder)
        {
            Folder = folder;
        }

        public IProjectFolder Folder { get; }
    }
}