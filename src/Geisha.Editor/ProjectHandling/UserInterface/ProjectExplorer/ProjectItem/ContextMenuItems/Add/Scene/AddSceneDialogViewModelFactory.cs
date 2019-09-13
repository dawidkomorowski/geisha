namespace Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem.ContextMenuItems.Add.Scene
{
    internal interface IAddSceneDialogViewModelFactory
    {
        AddSceneDialogViewModel Create();
    }

    internal sealed class AddSceneDialogViewModelFactory : IAddSceneDialogViewModelFactory
    {
        public AddSceneDialogViewModel Create()
        {
            return new AddSceneDialogViewModel();
        }
    }
}