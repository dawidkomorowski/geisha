using System.IO;
using Geisha.Editor.ProjectHandling.Infrastructure;

namespace Geisha.Editor.ProjectHandling.Model
{
    public interface IProject
    {
        string Name { get; }
        string FilePath { get; }
        string DirectoryPath { get; }
    }

    internal sealed class Project : IProject
    {
        public string Name { get; }
        public string FilePath { get; }
        public string DirectoryPath { get; }

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

        private Project(string projectFilePath)
        {
            Name = Path.GetFileNameWithoutExtension(projectFilePath);
            FilePath = projectFilePath;
            DirectoryPath = Path.GetDirectoryName(projectFilePath);
        }
    }
}