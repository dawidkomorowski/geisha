using System;
using System.ComponentModel.Composition;

namespace Geisha.Editor.Core.Models.Domain.ProjectHandling
{
    public interface IProjectService
    {
        bool IsProjectOpen { get; }
        Project CurrentProject { get; }

        event EventHandler CurrentProjectChanged;

        void CreateNewProject(string projectName, string projectLocation);
        void OpenProject(string projectFilePath);
        void CloseProject();
    }

    [Export(typeof(IProjectService))]
    internal class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private Project _project;

        public bool IsProjectOpen => CurrentProject != null;

        public Project CurrentProject
        {
            get { return _project; }
            private set
            {
                if (_project != value)
                {
                    _project = value;
                    CurrentProjectChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        [ImportingConstructor]
        public ProjectService(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public event EventHandler CurrentProjectChanged;

        public void CreateNewProject(string projectName, string projectLocation)
        {
            CurrentProject = _projectRepository.CreateProject(projectName, projectLocation);
        }

        public void OpenProject(string projectFilePath)
        {
            CurrentProject = _projectRepository.OpenProject(projectFilePath);
        }

        public void CloseProject()
        {
            CurrentProject = null;
        }
    }
}