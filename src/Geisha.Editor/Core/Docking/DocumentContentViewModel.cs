namespace Geisha.Editor.Core.Docking
{
    public abstract class DocumentContentViewModel : ViewModel
    {
        public abstract void OnDocumentSelected();
        public abstract void SaveDocument();
    }
}