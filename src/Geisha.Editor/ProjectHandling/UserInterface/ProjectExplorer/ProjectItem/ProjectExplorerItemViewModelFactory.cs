using System.Collections.Generic;
using System.Linq;
using Geisha.Editor.Core.ViewModels;
using Geisha.Editor.ProjectHandling.Model;
using Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem.ContextMenuItems.Add;

namespace Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem
{
    public interface IProjectExplorerItemViewModelFactory
    {
        ProjectRootViewModel Create(IProject project, IWindow window);
        IReadOnlyCollection<ProjectExplorerItemViewModel> Create(IEnumerable<IProjectFolder> folders, IEnumerable<IProjectFile> files, IWindow window);
    }

    public class ProjectExplorerItemViewModelFactory : IProjectExplorerItemViewModelFactory
    {
        private readonly IAddContextMenuItemFactory _addContextMenuItemFactory;

        public ProjectExplorerItemViewModelFactory(IAddContextMenuItemFactory addContextMenuItemFactory)
        {
            _addContextMenuItemFactory = addContextMenuItemFactory;
        }

        public ProjectRootViewModel Create(IProject project, IWindow window)
        {
            return new ProjectRootViewModel(project, this, _addContextMenuItemFactory, window);
        }

        public IReadOnlyCollection<ProjectExplorerItemViewModel> Create(IEnumerable<IProjectFolder> folders, IEnumerable<IProjectFile> files, IWindow window)
        {
            var foldersVMs = folders.OrderBy(f => f.Name).Select(f => Create(f, window));
            var filesVMs = files.OrderBy(f => f.Name).Select(f => Create(f, window));
            return foldersVMs.Concat(filesVMs).ToList().AsReadOnly();
        }

        private ProjectExplorerItemViewModel Create(IProjectFolder folder, IWindow window)
        {
            return new FolderViewModel(folder, this, _addContextMenuItemFactory, window);
        }

        private ProjectExplorerItemViewModel Create(IProjectFile file, IWindow window)
        {
            return new FileViewModel(file);
        }
    }
}