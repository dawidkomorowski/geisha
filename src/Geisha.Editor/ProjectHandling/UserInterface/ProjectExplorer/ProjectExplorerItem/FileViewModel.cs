using Geisha.Editor.Core;
using Geisha.Editor.CreateSound.Model;
using Geisha.Editor.CreateSound.UserInterface;
using Geisha.Editor.CreateSprite.Model;
using Geisha.Editor.CreateSprite.UserInterface;
using Geisha.Editor.CreateTexture.Model;
using Geisha.Editor.CreateTexture.UserInterface;
using Geisha.Editor.ProjectHandling.Model;

namespace Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectExplorerItem
{
    internal class FileViewModel : ProjectExplorerItemViewModel
    {
        private readonly IProjectFile _file;
        private readonly IEventBus _eventBus;
        private readonly ICreateTextureCommandFactory _createTextureCommandFactory;
        private readonly ICreateSpriteCommandFactory _createSpriteCommandFactory;
        private readonly ICreateSoundCommandFactory _createSoundCommandFactory;

        public FileViewModel(IProjectFile file, IEventBus eventBus, ICreateTextureCommandFactory createTextureCommandFactory,
            ICreateSpriteCommandFactory createSpriteCommandFactory, ICreateSoundCommandFactory createSoundCommandFactory) : base(file.Name)
        {
            _file = file;
            _eventBus = eventBus;
            _createTextureCommandFactory = createTextureCommandFactory;
            _createSpriteCommandFactory = createSpriteCommandFactory;
            _createSoundCommandFactory = createSoundCommandFactory;

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
                var command = _createTextureCommandFactory.Create(_file);
                ContextMenuItems.Add(new ContextMenuItem("Create texture", command));
            }

            if (CreateSpriteUtils.CanCreateSpriteFromFile(extension))
            {
                var command = _createSpriteCommandFactory.Create(_file);
                ContextMenuItems.Add(new ContextMenuItem("Create sprite", command));
            }

            if (SoundFileFormat.IsSupported(extension))
            {
                var command = _createSoundCommandFactory.Create(_file);
                ContextMenuItems.Add(new ContextMenuItem("Create sound", command));
            }
        }
    }
}