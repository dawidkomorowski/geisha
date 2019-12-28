namespace Geisha.Editor.ProjectHandling.Model
{
    public interface IProjectFile
    {
        string Name { get; }
        string Path { get; }
        IProjectFolder ParentFolder { get; }
    }

    internal sealed class ProjectFile : IProjectFile
    {
        public ProjectFile(string path, IProjectFolder parentFolder)
        {
            Name = System.IO.Path.GetFileName(path);
            Path = path;
            ParentFolder = parentFolder;
        }

        public string Name { get; }
        public string Path { get; }
        public IProjectFolder ParentFolder { get; }
    }
}