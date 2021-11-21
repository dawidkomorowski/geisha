using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Geisha.Editor.ProjectHandling.Model
{
    public interface IProjectFolder
    {
        string FolderName { get; }
        string FolderPath { get; }
        IReadOnlyCollection<IProjectFolder> Folders { get; }
        IReadOnlyCollection<IProjectFile> Files { get; }

        event EventHandler<ProjectFolderAddedEventArgs> FolderAdded;
        event EventHandler<ProjectFileAddedEventArgs> FileAdded;

        IProjectFolder AddFolder(string name);
        IProjectFile AddFile(string name, Stream fileContent);
    }

    internal class ProjectFolder : IProjectFolder
    {
        private readonly List<ProjectFolder> _folders = new List<ProjectFolder>();
        private readonly List<ProjectFile> _files = new List<ProjectFile>();

        private ProjectFolder(string folderPath) : this(folderPath, s => true)
        {
        }

        protected ProjectFolder(string folderPath, Func<string, bool> fileAndFolderFilter)
        {
            FolderName = Path.GetFileName(folderPath);
            FolderPath = folderPath;

            foreach (var subfolderPath in Directory.EnumerateDirectories(FolderPath).Where(fileAndFolderFilter))
            {
                _folders.Add(new ProjectFolder(subfolderPath));
            }

            foreach (var filePath in Directory.EnumerateFiles(FolderPath).Where(fileAndFolderFilter))
            {
                _files.Add(new ProjectFile(filePath, this));
            }
        }

        public string FolderName { get; }
        public string FolderPath { get; }
        public IReadOnlyCollection<IProjectFolder> Folders => _folders.AsReadOnly();
        public IReadOnlyCollection<IProjectFile> Files => _files.AsReadOnly();

        public event EventHandler<ProjectFolderAddedEventArgs>? FolderAdded;
        public event EventHandler<ProjectFileAddedEventArgs>? FileAdded;

        public IProjectFolder AddFolder(string name)
        {
            if (_folders.Any(f => f.FolderName == name)) throw new ArgumentException("Folder with the same name already exists in this folder.");

            var folderPath = Path.Combine(FolderPath, name);
            Directory.CreateDirectory(folderPath);
            var newFolder = new ProjectFolder(folderPath);
            _folders.Add(newFolder);
            FolderAdded?.Invoke(this, new ProjectFolderAddedEventArgs(newFolder));
            return newFolder;
        }

        public IProjectFile AddFile(string name, Stream fileContent)
        {
            if (_files.Any(f => f.Name == name)) throw new ArgumentException("File with the same name already exists in this folder.");

            var filePath = Path.Combine(FolderPath, name);

            using (var fileStream = File.Create(filePath))
            {
                fileContent.CopyTo(fileStream);
            }

            var newFile = new ProjectFile(filePath, this);
            _files.Add(newFile);
            FileAdded?.Invoke(this, new ProjectFileAddedEventArgs(newFile));
            return newFile;
        }
    }
}