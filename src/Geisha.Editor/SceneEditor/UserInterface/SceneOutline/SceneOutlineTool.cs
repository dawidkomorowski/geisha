using Geisha.Editor.Core.Docking;

namespace Geisha.Editor.SceneEditor.UserInterface.SceneOutline
{
    internal sealed class SceneOutlineTool : Tool
    {
        public SceneOutlineTool(SceneOutlineViewModel viewModel) : base("Scene Outline", new SceneOutlineView(), viewModel, true)
        {
        }
    }
}