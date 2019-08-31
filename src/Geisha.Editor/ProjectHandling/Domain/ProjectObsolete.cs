using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Geisha.Editor.ProjectHandling.Domain
{
    public interface IProjectObsolete
    {
        string Name { get; }
        string FilePath { get; }
        string DirectoryPath { get; }
        IReadOnlyList<IProjectItemObsolete> ProjectItems { get; }

        event EventHandler ProjectItemsChanged;

        void AddFolder(string name);
    }

    public class ProjectObsolete : IProjectObsolete
    {
        private readonly List<ProjectItemObsolete> _projectItems;
        private readonly List<ProjectItemObsolete> _projectItemsPendingToSave = new List<ProjectItemObsolete>();

        public ProjectObsolete(string path, IEnumerable<ProjectItemObsolete> projectItems)
        {
            FilePath = path;
            DirectoryPath = Path.GetDirectoryName(FilePath);
            Name = Path.GetFileNameWithoutExtension(FilePath);

            _projectItems = projectItems.ToList();
        }

        public IReadOnlyList<ProjectItemObsolete> ProjectItems => _projectItems.AsReadOnly();

        public IReadOnlyList<ProjectItemObsolete> ProjectItemsPendingToAdd => _projectItemsPendingToSave.AsReadOnly();

        public string Name { get; }
        public string FilePath { get; }
        public string DirectoryPath { get; }
        IReadOnlyList<IProjectItemObsolete> IProjectObsolete.ProjectItems => ProjectItems;

        public event EventHandler ProjectItemsChanged;

        public void AddFolder(string name)
        {
            var path = Path.Combine(DirectoryPath, name);
            var newFolder = new ProjectItemObsolete(path, ProjectItemType.Directory, Enumerable.Empty<ProjectItemObsolete>());

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