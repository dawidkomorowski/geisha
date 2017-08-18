using System.Collections.Generic;
using System.Linq;

namespace Geisha.Editor.Core.Models.Domain.ProjectHandling
{
    public class Project
    {
        private readonly List<ProjectItem> _projectItems;

        public string Name { get; }

        public IReadOnlyList<ProjectItem> ProjectItems => _projectItems.AsReadOnly();

        public Project(string name, IEnumerable<ProjectItem> projectItems)
        {
            Name = name;
            _projectItems = projectItems.ToList();
        }
    }
}