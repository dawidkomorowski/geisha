using System;

namespace Geisha.Editor.ProjectHandling.Model
{
    public sealed class ProjectFileAddedEventArgs : EventArgs
    {
        public ProjectFileAddedEventArgs(IProjectFile file)
        {
            File = file;
        }

        public IProjectFile File { get; }
    }
}