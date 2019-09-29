namespace Geisha.Editor.Core.Docking
{
    internal interface IDocumentFactory
    {
        bool IsApplicableForFile(string filePath);
        Document CreateDocument(string filePath);
    }
}