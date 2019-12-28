using System.IO;

namespace Geisha.Editor.ProjectHandling.Model
{
    public interface IProject : IProjectFolder
    {
        string ProjectName { get; }
        string ProjectFilePath { get; }
    }

    internal sealed class Project : ProjectFolder, IProject
    {
        private Project(string projectFilePath) : base(Path.GetDirectoryName(projectFilePath), FileAndFolderFilter)
        {
            ProjectName = Path.GetFileNameWithoutExtension(projectFilePath);
            ProjectFilePath = projectFilePath;
        }

        public string ProjectName { get; }
        public string ProjectFilePath { get; }

        public static Project Create(string projectName, string projectLocation)
        {
            var projectDirectoryPath = Path.Combine(projectLocation, projectName);
            var projectFilePath = Path.Combine(projectDirectoryPath, $"{projectName}{ProjectHandlingConstants.ProjectFileExtension}");

            Directory.CreateDirectory(projectDirectoryPath);
            File.WriteAllText(projectFilePath, string.Empty);

            return new Project(projectFilePath);
        }

        public static Project Open(string projectFilePath)
        {
            return new Project(projectFilePath);
        }

        private static bool FileAndFolderFilter(string path)
        {
            return Path.GetExtension(path) != ProjectHandlingConstants.ProjectFileExtension;
        }
    }
}