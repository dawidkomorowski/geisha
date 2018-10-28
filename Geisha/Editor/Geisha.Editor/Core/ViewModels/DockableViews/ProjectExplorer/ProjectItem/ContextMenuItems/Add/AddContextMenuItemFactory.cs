using System.ComponentModel.Composition;
using Geisha.Editor.Core.Models.Domain.ProjectHandling;
using Geisha.Editor.Core.ViewModels.Infrastructure;

namespace Geisha.Editor.Core.ViewModels.DockableViews.ProjectExplorer.ProjectItem.ContextMenuItems.Add
{
    public interface IAddContextMenuItemFactory
    {
        AddContextMenuItem Create(IProjectItem projectItem, IWindow window);
    }

    [Export(typeof(IAddContextMenuItemFactory))]
    public class AddContextMenuItemFactory : IAddContextMenuItemFactory
    {
        private readonly IAddNewFolderDialogViewModelFactory _addNewFolderDialogViewModelFactory;

        [ImportingConstructor]
        public AddContextMenuItemFactory(IAddNewFolderDialogViewModelFactory addNewFolderDialogViewModelFactory)
        {
            _addNewFolderDialogViewModelFactory = addNewFolderDialogViewModelFactory;
        }

        public AddContextMenuItem Create(IProjectItem projectItem, IWindow window)
        {
            return new AddContextMenuItem(projectItem, _addNewFolderDialogViewModelFactory, window);
        }
    }
}