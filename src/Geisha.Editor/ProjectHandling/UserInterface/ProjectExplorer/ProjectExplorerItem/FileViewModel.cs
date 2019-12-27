using System.IO;
using Geisha.Editor.Core;
using Geisha.Editor.CreateTexture;
using Geisha.Editor.CreateTexture.Model;
using Geisha.Editor.ProjectHandling.Model;

namespace Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectExplorerItem
{
    public class FileViewModel : ProjectExplorerItemViewModel
    {
        private readonly IEventBus _eventBus;
        private readonly IProjectFile _file;

        public FileViewModel(IProjectFile file, IEventBus eventBus) : base(file.Name)
        {
            _file = file;
            _eventBus = eventBus;

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
                ContextMenuItems.Add(new ContextMenuItem("Create texture"));
            }
        }
    }
}