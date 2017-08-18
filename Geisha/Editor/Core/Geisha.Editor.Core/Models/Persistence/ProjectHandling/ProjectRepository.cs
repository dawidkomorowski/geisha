using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Geisha.Common.Serialization;
using Geisha.Editor.Core.Infrastructure;
using Geisha.Editor.Core.Models.Domain.ProjectHandling;

namespace Geisha.Editor.Core.Models.Persistence.ProjectHandling
{
    [Export(typeof(IProjectRepository))]
    internal class ProjectRepository : IProjectRepository
    {
        private readonly IVersionProvider _versionProvider;

        [ImportingConstructor]
        public ProjectRepository(IVersionProvider versionProvider)
        {
            _versionProvider = versionProvider;
        }

        public Project CreateProject(string projectName, string projectLocation)
        {
            var projectDirectoryPath = Path.Combine(projectLocation, projectName);
            Directory.CreateDirectory(projectDirectoryPath);

            var projectFileNameWithExtension = $"{projectName}{ProjectHandlingConstants.ProjectFileExtension}";
            var projectFilePath = Path.Combine(projectDirectoryPath, projectFileNameWithExtension);
            using (var fileStream = File.Create(projectFilePath))
            {
                var projectFile = new ProjectFile {Version = _versionProvider.GetCurrentVersion().ToString()};
                var serializedProjectFile = Serializer.SerializeJson(projectFile);
                using (var streamWriter = new StreamWriter(fileStream))
                {
                    streamWriter.Write(serializedProjectFile);
                }
            }

            return OpenProject(projectFilePath);
        }

        public Project OpenProject(string projectFilePath)
        {
            var projectDirectoryPath = Path.GetDirectoryName(projectFilePath);

            var projectFileContent = File.ReadAllText(projectFilePath);
            var projectFile = Serializer.DeserializeJson<ProjectFile>(projectFileContent);
            var projectName = Path.GetFileNameWithoutExtension(projectFilePath);
            var projectItems = CollectProjectItems(projectDirectoryPath);

            return new Project(projectName, projectItems);
        }

        private IList<ProjectItem> CollectProjectItems(string projectDirectoryPath)
        {
            var projectFiles = Directory.EnumerateFiles(projectDirectoryPath)
                .Where(path => Path.GetExtension(path) != ProjectHandlingConstants.ProjectFileExtension)
                .Select(s => new ProjectItem(s, ProjectItem.ProjectItemType.File, Enumerable.Empty<ProjectItem>()));

            var projectDirectories = Directory.EnumerateDirectories(projectDirectoryPath)
                .Select(path => new ProjectItem(path, ProjectItem.ProjectItemType.Directory, CollectProjectItems(path)));

            return projectFiles.Concat(projectDirectories).ToList();
        }
    }
}