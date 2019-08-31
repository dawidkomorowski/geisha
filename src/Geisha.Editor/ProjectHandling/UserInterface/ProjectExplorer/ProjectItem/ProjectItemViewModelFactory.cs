using System;
using System.Collections.Generic;
using System.Linq;
using Geisha.Editor.Core.ViewModels.Infrastructure;
using Geisha.Editor.ProjectHandling.Model;
using Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem.ContextMenuItems.Add;

namespace Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem
{
    public interface IProjectItemViewModelFactory
    {
        ProjectProjectItemViewModel Create(IProjectObsolete project, IWindow window);
        IEnumerable<ProjectItemViewModel> Create(IEnumerable<IProjectItemObsolete> projectItems, IWindow window);
    }

    public class ProjectItemViewModelFactory : IProjectItemViewModelFactory
    {
        private readonly IAddContextMenuItemFactory _addContextMenuItemFactory;

        public ProjectItemViewModelFactory(IAddContextMenuItemFactory addContextMenuItemFactory)
        {
            _addContextMenuItemFactory = addContextMenuItemFactory;
        }

        public ProjectProjectItemViewModel Create(IProjectObsolete project, IWindow window)
        {
            return new ProjectProjectItemViewModel(project, this, _addContextMenuItemFactory, window);
        }

        public IEnumerable<ProjectItemViewModel> Create(IEnumerable<IProjectItemObsolete> projectItems, IWindow window)
        {
            return projectItems.OrderByDescending(pi => pi.Type).ThenBy(pi => pi.Name).Select(pi => Create(pi, window));
        }

        private ProjectItemViewModel Create(IProjectItemObsolete projectItem, IWindow window)
        {
            switch (projectItem.Type)
            {
                case ProjectItemType.Directory:
                    return new DirectoryProjectItemViewModel(projectItem, this, _addContextMenuItemFactory, window);
                case ProjectItemType.File:
                    return new FileProjectItemViewModel(projectItem);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}