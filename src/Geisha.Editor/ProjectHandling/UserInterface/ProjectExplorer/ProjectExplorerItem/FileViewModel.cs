using System.IO;
using Geisha.Editor.Core;
using Geisha.Editor.CreateSprite.Model;
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

        public FileViewModel(IProjectFile file, IEventBus eventBus, ICreateTextureCommandFactory createTextureCommandFactory) : base(file.Name)
        {
            _file = file;
            _eventBus = eventBus;
            _createTextureCommandFactory = createTextureCommandFactory;

            DoubleClickCommand = new RelayCommand(OnDoubleClick);
            CreateContextMenuActions();
        }

        private void OnDoubleClick()
        {
            _eventBus.SendEvent(new OpenFileEditorRequestedEvent(_file));
        }

        private void CreateContextMenuActions()
        {
            var extension = Path.GetExtension(_file.Name);

            if (TextureFileFormat.IsSupported(extension))
            {
                var command = _createTextureCommandFactory.Create(_file);
                ContextMenuItems.Add(new ContextMenuItem("Create texture", command));
            }

            if (CanCreateSpriteFromFile.Check(extension))
            {
                ContextMenuItems.Add(new ContextMenuItem("Create sprite"));
            }
        }
    }
}