using System;
using System.Collections.Generic;
using System.Linq;

namespace Geisha.Editor.ProjectHandling.Model
{
    public interface IProjectItemObsolete
    {
        string Name { get; }
        string Path { get; }
        ProjectItemType Type { get; }
        IReadOnlyList<IProjectItemObsolete> ProjectItems { get; }

        event EventHandler ProjectItemsChanged;

        void AddFolder(string name);
    }

    public class ProjectItemObsolete : IProjectItemObsolete
    {
        private readonly List<ProjectItemObsolete> _projectItems;
        private readonly List<ProjectItemObsolete> _projectItemsPendingToSave = new List<ProjectItemObsolete>();

        public ProjectItemObsolete(string path, ProjectItemType type, IEnumerable<ProjectItemObsolete> projectItems)
        {
            Name = System.IO.Path.GetFileName(path);
            Path = path;
            Type = type;
            _projectItems = projectItems.ToList();
        }

        public IReadOnlyList<ProjectItemObsolete> ProjectItems => _projectItems.AsReadOnly();

        public IReadOnlyList<ProjectItemObsolete> ProjectItemsPendingToAdd => _projectItemsPendingToSave.AsReadOnly();

        public string Name { get; }
        public string Path { get; }
        public ProjectItemType Type { get; }
        IReadOnlyList<IProjectItemObsolete> IProjectItemObsolete.ProjectItems => ProjectItems;

        public event EventHandler ProjectItemsChanged;

        public void AddFolder(string name)
        {
            if (Type == ProjectItemType.File) throw new GeishaEditorException("File cannot contain folder.");

            var path = System.IO.Path.Combine(Path, name);
            var newFolder = new ProjectItemObsolete(path, ProjectItemType.Directory, Enumerable.Empty<ProjectItemObsolete>());

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