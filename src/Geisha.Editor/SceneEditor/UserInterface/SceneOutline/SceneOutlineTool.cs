using Geisha.Editor.Core.Docking;

namespace Geisha.Editor.SceneEditor.UserInterface.SceneOutline
{
    internal sealed class SceneOutlineTool : Tool
    {
        public SceneOutlineTool() : base("Scene Outline", new SceneOutlineView(), new SceneOutlineViewModel(), true)
        {
        }
    }
}