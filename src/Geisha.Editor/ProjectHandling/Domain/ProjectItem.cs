using System;
using System.Collections.Generic;
using System.Linq;

namespace Geisha.Editor.ProjectHandling.Domain
{
    public interface IProjectItem
    {
        string Name { get; }
        string Path { get; }
        ProjectItemType Type { get; }
        IReadOnlyList<IProjectItem> ProjectItems { get; }

        event EventHandler ProjectItemsChanged;

        void AddFolder(string name);
    }

    public class ProjectItem : IProjectItem
    {
        private readonly List<ProjectItem> _projectItems;
        private readonly List<ProjectItem> _projectItemsPendingToSave = new List<ProjectItem>();

        public ProjectItem(string path, ProjectItemType type, IEnumerable<ProjectItem> projectItems)
        {
            Name = System.IO.Path.GetFileName(path);
            Path = path;
            Type = type;
            _projectItems = projectItems.ToList();
        }

        public IReadOnlyList<ProjectItem> ProjectItems => _projectItems.AsReadOnly();

        public IReadOnlyList<ProjectItem> ProjectItemsPendingToAdd => _projectItemsPendingToSave.AsReadOnly();

        public string Name { get; }
        public string Path { get; }
        public ProjectItemType Type { get; }
        IReadOnlyList<IProjectItem> IProjectItem.ProjectItems => ProjectItems;

        public event EventHandler ProjectItemsChanged;

        public void AddFolder(string name)
        {
            if (Type == ProjectItemType.File) throw new GeishaEditorException("File cannot contain folder.");

            var path = System.IO.Path.Combine(Path, name);
            var newFolder = new ProjectItem(path, ProjectItemType.Directory, Enumerable.Empty<ProjectItem>());

            _projectItems.Add(newFolder);
            _projectItemsPendingToSave.Add(newFolder);

            ProjectItemsChanged?.Invoke(this, EventArgs.Empty);
        }

        public void ClearStatePendingToSave()
        {
            _projectItemsPendingToSave.Clear();
        }
    }

    public enum ProjectItemType
    {
        File,
        Directory
    }
}