using System;

namespace Geisha.Editor.ProjectHandling.Model
{
    public interface IProjectServiceObsolete
    {
        bool IsProjectOpen { get; }
        IProjectObsolete CurrentProject { get; }

        event EventHandler CurrentProjectChanged;

        void CreateNewProject(string projectName, string projectLocation);
        void OpenProject(string projectFilePath);
        void SaveProject();
        void CloseProject();
    }

    internal class ProjectServiceObsolete : IProjectServiceObsolete
    {
        private readonly IProjectRepository _projectRepository;
        private ProjectObsolete _currentProjectInternal;

        public bool IsProjectOpen => CurrentProject != null;
        public IProjectObsolete CurrentProject => CurrentProjectInternal;

        private ProjectObsolete CurrentProjectInternal
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

        public ProjectServiceObsolete(IProjectRepository projectRepository)
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