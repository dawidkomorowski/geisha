using System.IO;
using Geisha.Editor.Core;
using Geisha.Editor.Core.Docking;
using Geisha.Editor.SceneEditor.Model;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Editor.SceneEditor.UserInterface.SceneEditor
{
    internal sealed class SceneEditorDocumentFactory : IDocumentFactory
    {
        private readonly ISceneLoader _sceneLoader;
        private readonly IEventBus _eventBus;

        public SceneEditorDocumentFactory(ISceneLoader sceneLoader, IEventBus eventBus)
        {
            _sceneLoader = sceneLoader;
            _eventBus = eventBus;
        }

        public bool IsApplicableForFile(string filePath)
        {
            return Path.GetExtension(filePath) == SceneEditorConstants.SceneFileExtension;
        }

        public Document CreateDocument(string filePath)
        {
            return new Document(Path.GetFileName(filePath), new SceneEditorView(), new SceneEditorViewModel(filePath, _eventBus, _sceneLoader));
        }
    }
}