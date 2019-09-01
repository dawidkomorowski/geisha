namespace Geisha.Editor.Core.ViewModels
{
    public interface IWindow
    {
        void Close();
        void ShowModalChildWindow(ViewModel dataContext);
    }
}