using Geisha.Editor.Core.Docking;

namespace Geisha.Editor.SceneEditor.UserInterface.SceneEditor
{
    internal sealed class SceneEditorDocument : Document
    {
        public SceneEditorDocument() : base("Scene editor", new SceneEditorView(), new SceneEditorViewModel())
        {
        }
    }
}