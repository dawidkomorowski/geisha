using System;

namespace Geisha.Editor.ProjectHandling.Model
{
    public interface IProjectService
    {
        bool ProjectIsOpen { get; }
        IProject CurrentProject { get; }

        event EventHandler CurrentProjectChanged;

        void CreateNewProject(string projectName, string projectLocation);
        void OpenProject(string projectFilePath);
        void CloseProject();
    }

    internal sealed class ProjectService : IProjectService
    {
        private IProject _currentProject = null;
        public bool ProjectIsOpen => _currentProject != null;
        public IProject CurrentProject => _currentProject ?? throw new ProjectNotOpenException();

        public event EventHandler CurrentProjectChanged;

        public void CreateNewProject(string projectName, string projectLocation)
        {
            _currentProject = Project.Create(projectName, projectLocation);
            CurrentProjectChanged?.Invoke(this, EventArgs.Empty);
        }

        public void OpenProject(string projectFilePath)
        {
            _currentProject = Project.Open(projectFilePath);
            CurrentProjectChanged?.Invoke(this, EventArgs.Empty);
        }

        public void CloseProject()
        {
            _currentProject = null;
            CurrentProjectChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    ///     The exception that is thrown when accessing <see cref="IProjectService.CurrentProject" /> when no project was
    ///     opened.
    /// </summary>
    public sealed class ProjectNotOpenException : Exception
    {
        public ProjectNotOpenException() : base(
            "No project is open. Either open project was already closed or no project was yet opened.")
        {
        }
    }
}