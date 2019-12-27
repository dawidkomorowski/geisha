using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Geisha.Editor.ProjectHandling.Model
{
    public interface IProject
    {
        string ProjectName { get; }
        string ProjectFilePath { get; }
        string FolderPath { get; }
        IReadOnlyCollection<IProjectFolder> Folders { get; }
        IReadOnlyCollection<IProjectFile> Files { get; }

        event EventHandler<ProjectFolderAddedEventArgs> FolderAdded;
        event EventHandler<ProjectFileAddedEventArgs> FileAdded;

        IProjectFolder AddFolder(string name);
        IProjectFile AddFile(string name, Stream fileContent);
    }

    internal sealed class Project : IProject
    {
        private readonly List<ProjectFolder> _folders = new List<ProjectFolder>();
        private readonly List<ProjectFile> _files = new List<ProjectFile>();

        private Project(string projectFilePath)
        {
            ProjectName = Path.GetFileNameWithoutExtension(projectFilePath);
            ProjectFilePath = projectFilePath;
            FolderPath = Path.GetDirectoryName(projectFilePath);

            foreach (var folderPath in Directory.EnumerateDirectories(FolderPath))
            {
                _folders.Add(new ProjectFolder(folderPath));
            }

            foreach (var filePath in Directory.EnumerateFiles(FolderPath).Where(f => Path.GetExtension(f) != ProjectHandlingConstants.ProjectFileExtension))
            {
                _files.Add(new ProjectFile(filePath));
            }
        }

        public string ProjectName { get; }
        public string ProjectFilePath { get; }
        public string FolderPath { get; }
        public IReadOnlyCollection<IProjectFolder> Folders => _folders.AsReadOnly();
        public IReadOnlyCollection<IProjectFile> Files => _files.AsReadOnly();

        public event EventHandler<ProjectFolderAddedEventArgs> FolderAdded;
        public event EventHandler<ProjectFileAddedEventArgs> FileAdded;

        public IProjectFolder AddFolder(string name)
        {
            var folderPath = Path.Combine(FolderPath, name);
            Directory.CreateDirectory(folderPath);
            var newFolder = new ProjectFolder(folderPath);
            _folders.Add(newFolder);
            FolderAdded?.Invoke(this, new ProjectFolderAddedEventArgs(newFolder));
            return newFolder;
        }

        public IProjectFile AddFile(string name, Stream fileContent)
        {
            var filePath = Path.Combine(FolderPath, name);

            using (var fileStream = File.Create(filePath))
            {
                fileContent.CopyTo(fileStream);
            }

            var newFile = new ProjectFile(filePath);
            _files.Add(newFile);
            FileAdded?.Invoke(this, new ProjectFileAddedEventArgs(newFile));
            return newFile;
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