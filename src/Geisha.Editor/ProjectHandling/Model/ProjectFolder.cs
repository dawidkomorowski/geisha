namespace Geisha.Editor.ProjectHandling.Model
{
    public interface IProjectFolder
    {
        string Name { get; }
        string Path { get; }
    }

    public sealed class ProjectFolder : IProjectFolder
    {
        public ProjectFolder(string path)
        {
            Name = System.IO.Path.GetFileName(path);
            Path = path;
        }

        public string Name { get; }
        public string Path { get; }
    }
}