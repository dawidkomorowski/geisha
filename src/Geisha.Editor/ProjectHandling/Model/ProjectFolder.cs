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
        IReadOnlyCollection<IProjectFile> Files { get; }

        event EventHandler<ProjectFolderAddedEventArgs> FolderAdded;
        event EventHandler<ProjectFileAddedEventArgs> FileAdded;

        IProjectFolder AddFolder(string name);
        IProjectFile AddFile(string name, Stream fileContent);
    }

    internal sealed class ProjectFolder : IProjectFolder
    {
        private readonly List<ProjectFolder> _folders = new List<ProjectFolder>();
        private readonly List<ProjectFile> _files = new List<ProjectFile>();

        public ProjectFolder(string path)
        {
            Name = System.IO.Path.GetFileName(path);
            Path = path;

            foreach (var folderPath in Directory.EnumerateDirectories(Path))
            {
                _folders.Add(new ProjectFolder(folderPath));
            }

            foreach (var filePath in Directory.EnumerateFiles(Path))
            {
                _files.Add(new ProjectFile(filePath));
            }
        }

        public string Name { get; }
        public string Path { get; }
        public IReadOnlyCollection<IProjectFolder> Folders => _folders.AsReadOnly();
        public IReadOnlyCollection<IProjectFile> Files => _files.AsReadOnly();

        public event EventHandler<ProjectFolderAddedEventArgs> FolderAdded;
        public event EventHandler<ProjectFileAddedEventArgs> FileAdded;

        public IProjectFolder AddFolder(string name)
        {
            var folderPath = System.IO.Path.Combine(Path, name);
            Directory.CreateDirectory(folderPath);
            var newFolder = new ProjectFolder(folderPath);
            _folders.Add(newFolder);
            FolderAdded?.Invoke(this, new ProjectFolderAddedEventArgs(newFolder));
            return newFolder;
        }

        public IProjectFile AddFile(string name, Stream fileContent)
        {
            var filePath = System.IO.Path.Combine(Path, name);

            using (var fileStream = File.Create(filePath))
            {
                fileContent.CopyTo(fileStream);
            }

            var newFile = new ProjectFile(filePath);
            _files.Add(newFile);
            FileAdded?.Invoke(this, new ProjectFileAddedEventArgs(newFile));
            return newFile;
        }
    }
}