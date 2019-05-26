using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Geisha.Editor.Core.Models.Domain.ProjectHandling
{
    public interface IProject
    {
        string Name { get; }
        string FilePath { get; }
        string DirectoryPath { get; }
        IReadOnlyList<IProjectItem> ProjectItems { get; }

        event EventHandler ProjectItemsChanged;

        void AddFolder(string name);
    }

    public class Project : IProject
    {
        private readonly List<ProjectItem> _projectItems;
        private readonly List<ProjectItem> _projectItemsPendingToSave = new List<ProjectItem>();

        public Project(string path, IEnumerable<ProjectItem> projectItems)
        {
            FilePath = path;
            DirectoryPath = Path.GetDirectoryName(FilePath);
            Name = Path.GetFileNameWithoutExtension(FilePath);

            _projectItems = projectItems.ToList();
        }

        public IReadOnlyList<ProjectItem> ProjectItems => _projectItems.AsReadOnly();

        public IReadOnlyList<ProjectItem> ProjectItemsPendingToAdd => _projectItemsPendingToSave.AsReadOnly();

        public string Name { get; }
        public string FilePath { get; }
        public string DirectoryPath { get; }
        IReadOnlyList<IProjectItem> IProject.ProjectItems => ProjectItems;

        public event EventHandler ProjectItemsChanged;

        public void AddFolder(string name)
        {
            var path = Path.Combine(DirectoryPath, name);
            var newFolder = new ProjectItem(path, ProjectItemType.Directory, Enumerable.Empty<ProjectItem>());

            _projectItems.Add(newFolder);
            _projectItemsPendingToSave.Add(newFolder);

            ProjectItemsChanged?.Invoke(this, EventArgs.Empty);
        }

        public void ClearStatePendingToSave()
        {
            _projectItemsPendingToSave.Clear();

            foreach (var projectItem in _projectItems)
                projectItem.ClearStatePendingToSave();
        }
    }
}