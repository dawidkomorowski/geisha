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
            var projectFilePath = Path.Combine(projectLocation, projectName, $"{projectName}{ProjectHandlingConstants.ProjectFileExtension}");
            return new Project(projectName, projectFilePath);
        }

        private Project(string name, string filePath)
        {
            Name = name;
            FilePath = filePath;
            DirectoryPath = Path.GetDirectoryName(filePath);
        }
    }
}