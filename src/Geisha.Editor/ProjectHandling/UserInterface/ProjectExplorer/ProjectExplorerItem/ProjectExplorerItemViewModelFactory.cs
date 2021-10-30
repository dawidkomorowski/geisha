using System.Collections.Generic;
using System.Linq;
using Geisha.Editor.Core;
using Geisha.Editor.CreateAsset.UserInterface.Sound;
using Geisha.Editor.CreateSpriteAsset.UserInterface;
using Geisha.Editor.CreateTextureAsset.UserInterface;
using Geisha.Editor.ProjectHandling.Model;
using Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectExplorerItem.ContextMenuItems.Add;

namespace Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectExplorerItem
{
    internal interface IProjectExplorerItemViewModelFactory
    {
        ProjectRootViewModel Create(IProject project);
        IReadOnlyCollection<ProjectExplorerItemViewModel> Create(IEnumerable<IProjectFolder> folders, IEnumerable<IProjectFile> files);
    }

    internal class ProjectExplorerItemViewModelFactory : IProjectExplorerItemViewModelFactory
    {
        private readonly IEventBus _eventBus;
        private readonly IAddContextMenuItemFactory _addContextMenuItemFactory;
        private readonly ICreateTextureAssetCommandFactory _createTextureAssetCommandFactory;
        private readonly ICreateSpriteAssetCommandFactory _createSpriteAssetCommandFactory;
        private readonly ICreateSoundAssetCommandFactory _createSoundAssetCommandFactory;

        public ProjectExplorerItemViewModelFactory(IEventBus eventBus, IAddContextMenuItemFactory addContextMenuItemFactory,
            ICreateTextureAssetCommandFactory createTextureAssetCommandFactory, ICreateSpriteAssetCommandFactory createSpriteAssetCommandFactory,
            ICreateSoundAssetCommandFactory createSoundAssetCommandFactory)
        {
            _eventBus = eventBus;
            _addContextMenuItemFactory = addContextMenuItemFactory;
            _createTextureAssetCommandFactory = createTextureAssetCommandFactory;
            _createSpriteAssetCommandFactory = createSpriteAssetCommandFactory;
            _createSoundAssetCommandFactory = createSoundAssetCommandFactory;
        }

        public ProjectRootViewModel Create(IProject project)
        {
            return new ProjectRootViewModel(project, this, _addContextMenuItemFactory);
        }

        public IReadOnlyCollection<ProjectExplorerItemViewModel> Create(IEnumerable<IProjectFolder> folders, IEnumerable<IProjectFile> files)
        {
            var foldersVMs = folders.OrderBy(f => f.FolderName).Select(Create);
            var filesVMs = files.OrderBy(f => f.Name).Select(Create);
            return foldersVMs.Concat(filesVMs).ToList().AsReadOnly();
        }

        private ProjectExplorerItemViewModel Create(IProjectFolder folder)
        {
            return new FolderViewModel(folder, this, _addContextMenuItemFactory);
        }

        private ProjectExplorerItemViewModel Create(IProjectFile file)
        {
            return new FileViewModel(
                file,
                _eventBus,
                _createTextureAssetCommandFactory,
                _createSpriteAssetCommandFactory,
                _createSoundAssetCommandFactory
            );
        }
    }
}