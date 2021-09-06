using Geisha.Editor.Core;
using Geisha.Editor.CreateSoundAsset.Model;
using Geisha.Editor.CreateSoundAsset.UserInterface;
using Geisha.Editor.CreateSpriteAsset.Model;
using Geisha.Editor.CreateSpriteAsset.UserInterface;
using Geisha.Editor.CreateTextureAsset.Model;
using Geisha.Editor.CreateTextureAsset.UserInterface;
using Geisha.Editor.ProjectHandling.Model;

namespace Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectExplorerItem
{
    internal class FileViewModel : ProjectExplorerItemViewModel
    {
        private readonly IProjectFile _file;
        private readonly IEventBus _eventBus;
        private readonly ICreateTextureAssetCommandFactory _createTextureAssetCommandFactory;
        private readonly ICreateSpriteAssetCommandFactory _createSpriteAssetCommandFactory;
        private readonly ICreateSoundAssetCommandFactory _createSoundAssetCommandFactory;

        public FileViewModel(IProjectFile file, IEventBus eventBus, ICreateTextureAssetCommandFactory createTextureAssetCommandFactory,
            ICreateSpriteAssetCommandFactory createSpriteAssetCommandFactory, ICreateSoundAssetCommandFactory createSoundAssetCommandFactory) : base(file.Name)
        {
            _file = file;
            _eventBus = eventBus;
            _createTextureAssetCommandFactory = createTextureAssetCommandFactory;
            _createSpriteAssetCommandFactory = createSpriteAssetCommandFactory;
            _createSoundAssetCommandFactory = createSoundAssetCommandFactory;

            DoubleClickCommand = RelayCommand.Create(OnDoubleClick);
            CreateContextMenuActions();
        }

        private void OnDoubleClick()
        {
            _eventBus.SendEvent(new OpenFileEditorRequestedEvent(_file));
        }

        private void CreateContextMenuActions()
        {
            var extension = _file.Extension;

            if (TextureFileFormat.IsSupported(extension))
            {
                var command = _createTextureAssetCommandFactory.Create(_file);
                ContextMenuItems.Add(new ContextMenuItem("Create texture asset", command));
            }

            if (CreateSpriteAssetUtils.CanCreateSpriteAssetFromFile(extension))
            {
                var command = _createSpriteAssetCommandFactory.Create(_file);
                ContextMenuItems.Add(new ContextMenuItem("Create sprite asset", command));
            }

            if (SoundFileFormat.IsSupported(extension))
            {
                var command = _createSoundAssetCommandFactory.Create(_file);
                ContextMenuItems.Add(new ContextMenuItem("Create sound asset", command));
            }
        }
    }
}