using System.IO;
using Geisha.Editor.Core.Docking;
using Geisha.Editor.SceneEditor.Model;

namespace Geisha.Editor.SceneEditor.UserInterface.SceneEditor
{
    internal sealed class SceneEditorDocumentFactory : IDocumentFactory
    {
        public bool IsApplicableForFile(string filePath)
        {
            return Path.GetExtension(filePath) == SceneEditorConstants.SceneFileExtension;
        }

        public Document CreateDocument(string filePath)
        {
            return new Document(Path.GetFileName(filePath), new SceneEditorView(), new SceneEditorViewModel());
        }
    }
}