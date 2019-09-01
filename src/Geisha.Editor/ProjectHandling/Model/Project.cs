using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Geisha.Editor.ProjectHandling.Model
{
    public interface IProject
    {
        string Name { get; }
        string FilePath { get; }
        string DirectoryPath { get; }
        IReadOnlyCollection<IProjectFolder> Folders { get; }
        IReadOnlyCollection<IProjectFile> Files { get; }

        event EventHandler<ProjectFolderAddedEventArgs> FolderAdded;

        IProjectFolder AddFolder(string name);
    }

    internal sealed class Project : IProject
    {
        private readonly List<ProjectFolder> _folders = new List<ProjectFolder>();
        private readonly List<ProjectFile> _files = new List<ProjectFile>();

        private Project(string projectFilePath)
        {
            Name = Path.GetFileNameWithoutExtension(projectFilePath);
            FilePath = projectFilePath;
            DirectoryPath = Path.GetDirectoryName(projectFilePath);

            foreach (var folderPath in Directory.EnumerateDirectories(DirectoryPath))
            {
                _folders.Add(new ProjectFolder(folderPath));
            }

            foreach (var filePath in Directory.EnumerateFiles(DirectoryPath).Where(f => Path.GetExtension(f) != ProjectHandlingConstants.ProjectFileExtension))
            {
                _files.Add(new ProjectFile(filePath));
            }
        }

        public string Name { get; }
        public string FilePath { get; }
        public string DirectoryPath { get; }
        public IReadOnlyCollection<IProjectFolder> Folders => _folders.AsReadOnly();
        public IReadOnlyCollection<IProjectFile> Files => _files.AsReadOnly();

        public event EventHandler<ProjectFolderAddedEventArgs> FolderAdded;

        public IProjectFolder AddFolder(string name)
        {
            var folderPath = Path.Combine(DirectoryPath, name);
            Directory.CreateDirectory(folderPath);
            var newFolder = new ProjectFolder(folderPath);
            _folders.Add(newFolder);
            FolderAdded?.Invoke(this, new ProjectFolderAddedEventArgs(newFolder));
            return newFolder;
        }

        public static Project Create(string projectName, string projectLocation)
        {
            var projectDirectoryPath = Path.Combine(projectLocation, projectName);
            var projectFilePath = Path.Combine(projectDirectoryPath, $"{projectName}{ProjectHandlingConstants.ProjectFileExtension}");

            Directory.CreateDirectory(projectDirectoryPath);
            File.WriteAllText(projectFilePath, string.Empty);

            return new Project(projectFilePath);
        }

        public static Project Open(string projectFilePath)
        {
            return new Project(projectFilePath);
        }
    }
}