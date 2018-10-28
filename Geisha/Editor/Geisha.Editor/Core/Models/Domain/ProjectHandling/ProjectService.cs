using System;
using System.ComponentModel.Composition;

namespace Geisha.Editor.Core.Models.Domain.ProjectHandling
{
    public interface IProjectService
    {
        bool IsProjectOpen { get; }
        IProject CurrentProject { get; }

        event EventHandler CurrentProjectChanged;

        void CreateNewProject(string projectName, string projectLocation);
        void OpenProject(string projectFilePath);
        void SaveProject();
        void CloseProject();
    }

    [Export(typeof(IProjectService))]
    internal class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private Project _currentProjectInternal;

        public bool IsProjectOpen => CurrentProject != null;
        public IProject CurrentProject => CurrentProjectInternal;

        private Project CurrentProjectInternal
        {
            get => _currentProjectInternal;
            set
            {
                if (_currentProjectInternal != value)
                {
                    _currentProjectInternal = value;
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
            if (IsProjectOpen) CloseProject();

            CurrentProjectInternal = _projectRepository.CreateProject(projectName, projectLocation);
        }

        public void OpenProject(string projectFilePath)
        {
            if (IsProjectOpen) CloseProject();

            CurrentProjectInternal = _projectRepository.OpenProject(projectFilePath);
        }

        public void SaveProject()
        {
            _projectRepository.SaveProject(CurrentProjectInternal);
        }

        public void CloseProject()
        {
            CurrentProjectInternal = null;
        }
    }
}