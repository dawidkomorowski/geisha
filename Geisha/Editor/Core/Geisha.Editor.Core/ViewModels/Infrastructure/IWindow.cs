namespace Geisha.Editor.Core.ViewModels.Infrastructure
{
    public interface IWindow
    {
        void Close();
        void ShowModalChildWindow(ViewModel dataContext);
    }
}