namespace Geisha.Editor.ProjectHandling.Model
{
    public interface IProjectFile
    {
        string Name { get; }
        string Path { get; }
    }

    internal sealed class ProjectFile : IProjectFile
    {
        public ProjectFile(string path)
        {
            Name = System.IO.Path.GetFileName(path);
            Path = path;
        }

        public string Name { get; }
        public string Path { get; }
    }
}