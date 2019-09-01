using System.Collections.Generic;
using System.Linq;
using Geisha.Editor.Core.ViewModels.Infrastructure;
using Geisha.Editor.ProjectHandling.Model;
using Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem.ContextMenuItems.Add;

namespace Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem
{
    public interface IProjectItemViewModelFactory
    {
        ProjectProjectItemViewModel Create(IProject project, IWindow window);
        IReadOnlyCollection<ProjectItemViewModel> Create(IEnumerable<IProjectFolder> folders, IEnumerable<IProjectFile> files, IWindow window);
    }

    public class ProjectItemViewModelFactory : IProjectItemViewModelFactory
    {
        private readonly IAddContextMenuItemFactory _addContextMenuItemFactory;

        public ProjectItemViewModelFactory(IAddContextMenuItemFactory addContextMenuItemFactory)
        {
            _addContextMenuItemFactory = addContextMenuItemFactory;
        }

        public ProjectProjectItemViewModel Create(IProject project, IWindow window)
        {
            return new ProjectProjectItemViewModel(project, this, _addContextMenuItemFactory, window);
        }

        public IReadOnlyCollection<ProjectItemViewModel> Create(IEnumerable<IProjectFolder> folders, IEnumerable<IProjectFile> files, IWindow window)
        {
            var foldersVMs = folders.OrderBy(f => f.Name).Select(f => Create(f, window));
            var filesVMs = files.OrderBy(f => f.Name).Select(f => Create(f, window));
            return foldersVMs.Concat(filesVMs).ToList().AsReadOnly();
        }

        private ProjectItemViewModel Create(IProjectFolder folder, IWindow window)
        {
            return new DirectoryProjectItemViewModel(folder, this, _addContextMenuItemFactory, window);
        }

        private ProjectItemViewModel Create(IProjectFile file, IWindow window)
        {
            return new FileProjectItemViewModel(file);
        }
    }
}