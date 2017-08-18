using System.Collections.Generic;
using System.Linq;

namespace Geisha.Editor.Core.Models.Domain.ProjectHandling
{
    public class ProjectItem
    {
        private readonly List<ProjectItem> _projectItems;

        public string Name { get; }
        public string Path { get; }
        public ProjectItemType Type { get; }
        public IReadOnlyList<ProjectItem> ProjectItems => _projectItems.AsReadOnly();

        public ProjectItem(string path, ProjectItemType type, IEnumerable<ProjectItem> projectItems)
        {
            Name = System.IO.Path.GetFileName(path);
            Path = path;
            Type = type;
            _projectItems = projectItems.ToList();
        }

        public enum ProjectItemType
        {
            File,
            Directory
        }
    }
}