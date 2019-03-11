using System.Collections.Generic;
using System.IO;
using System.Linq;
using Geisha.Common.Serialization;
using Geisha.Editor.Core.Infrastructure;
using Geisha.Editor.Core.Models.Domain.ProjectHandling;

namespace Geisha.Editor.Core.Models.Persistence.ProjectHandling
{
    internal class ProjectRepository : IProjectRepository
    {
        private readonly IJsonSerializer _jsonSerializer;
        private readonly IVersionProvider _versionProvider;

        public ProjectRepository(IVersionProvider versionProvider, IJsonSerializer jsonSerializer)
        {
            _versionProvider = versionProvider;
            _jsonSerializer = jsonSerializer;
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
                var serializedProjectFile = _jsonSerializer.Serialize(projectFile);
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
            var projectFile = _jsonSerializer.Deserialize<ProjectFile>(projectFileContent);
            var projectItems = CollectProjectItems(projectDirectoryPath);

            return new Project(projectFilePath, projectItems);
        }

        public void SaveProject(Project project)
        {
            if (!EnsureProjectExists(project)) throw new GeishaEditorException("Cannot save not existent project.");

            foreach (var projectItem in project.ProjectItemsPendingToAdd)
            {
                Directory.CreateDirectory(projectItem.Path);
            }

            foreach (var projectItem in project.ProjectItems)
            {
                SaveProjectItem(projectItem);
            }

            project.ClearStatePendingToSave();
        }

        private void SaveProjectItem(ProjectItem projectItem)
        {
            foreach (var item in projectItem.ProjectItemsPendingToAdd)
            {
                Directory.CreateDirectory(item.Path);
            }

            foreach (var item in projectItem.ProjectItems)
            {
                SaveProjectItem(item);
            }
        }

        private IList<ProjectItem> CollectProjectItems(string projectDirectoryPath)
        {
            var projectFiles = Directory.EnumerateFiles(projectDirectoryPath)
                .Where(path => Path.GetExtension(path) != ProjectHandlingConstants.ProjectFileExtension)
                .Select(s => new ProjectItem(s, ProjectItemType.File, Enumerable.Empty<ProjectItem>()));

            var projectDirectories = Directory.EnumerateDirectories(projectDirectoryPath)
                .Select(path => new ProjectItem(path, ProjectItemType.Directory, CollectProjectItems(path)));

            return projectFiles.Concat(projectDirectories).ToList();
        }

        private bool EnsureProjectExists(Project project)
        {
            return File.Exists(project.FilePath);
        }
    }
}