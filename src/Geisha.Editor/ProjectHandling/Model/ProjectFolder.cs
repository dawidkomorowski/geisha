using System;
using System.Collections.Generic;
using System.IO;

namespace Geisha.Editor.ProjectHandling.Model
{
    public interface IProjectFolder
    {
        string Name { get; }
        string Path { get; }
        IReadOnlyCollection<IProjectFolder> Folders { get; }

        event EventHandler<ProjectFolderAddedEventArgs> FolderAdded;

        IProjectFolder AddFolder(string name);
    }

    public sealed class ProjectFolder : IProjectFolder
    {
        private readonly List<ProjectFolder> _folders = new List<ProjectFolder>();

        public ProjectFolder(string path)
        {
            Name = System.IO.Path.GetFileName(path);
            Path = path;

            foreach (var folderPath in Directory.EnumerateDirectories(Path))
            {
                _folders.Add(new ProjectFolder(folderPath));
            }
        }

        public string Name { get; }
        public string Path { get; }
        public IReadOnlyCollection<IProjectFolder> Folders => _folders.AsReadOnly();

        public event EventHandler<ProjectFolderAddedEventArgs> FolderAdded;

        public IProjectFolder AddFolder(string name)
        {
            var folderPath = System.IO.Path.Combine(Path, name);
            Directory.CreateDirectory(folderPath);
            var newFolder = new ProjectFolder(folderPath);
            _folders.Add(newFolder);
            FolderAdded?.Invoke(this, new ProjectFolderAddedEventArgs(newFolder));
            return newFolder;
        }
    }
}