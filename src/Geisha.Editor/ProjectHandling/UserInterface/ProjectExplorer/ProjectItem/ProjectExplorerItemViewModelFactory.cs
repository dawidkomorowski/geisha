using System.Collections.Generic;
using System.Linq;
using Geisha.Editor.ProjectHandling.Model;
using Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem.ContextMenuItems.Add;

namespace Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem
{
    internal interface IProjectExplorerItemViewModelFactory
    {
        ProjectRootViewModel Create(IProject project);
        IReadOnlyCollection<ProjectExplorerItemViewModel> Create(IEnumerable<IProjectFolder> folders, IEnumerable<IProjectFile> files);
    }

    internal class ProjectExplorerItemViewModelFactory : IProjectExplorerItemViewModelFactory
    {
        private readonly IAddContextMenuItemFactory _addContextMenuItemFactory;

        public ProjectExplorerItemViewModelFactory(IAddContextMenuItemFactory addContextMenuItemFactory)
        {
            _addContextMenuItemFactory = addContextMenuItemFactory;
        }

        public ProjectRootViewModel Create(IProject project)
        {
            return new ProjectRootViewModel(project, this, _addContextMenuItemFactory);
        }

        public IReadOnlyCollection<ProjectExplorerItemViewModel> Create(IEnumerable<IProjectFolder> folders, IEnumerable<IProjectFile> files)
        {
            var foldersVMs = folders.OrderBy(f => f.Name).Select(Create);
            var filesVMs = files.OrderBy(f => f.Name).Select(Create);
            return foldersVMs.Concat(filesVMs).ToList().AsReadOnly();
        }

        private ProjectExplorerItemViewModel Create(IProjectFolder folder)
        {
            return new FolderViewModel(folder, this, _addContextMenuItemFactory);
        }

        private static ProjectExplorerItemViewModel Create(IProjectFile file)
        {
            return new FileViewModel(file);
        }
    }
}