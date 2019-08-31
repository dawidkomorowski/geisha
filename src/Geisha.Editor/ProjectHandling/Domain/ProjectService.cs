using System;

namespace Geisha.Editor.ProjectHandling.Domain
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
        private bool _projectIsOpen;

        public bool ProjectIsOpen => _projectIsOpen;
        public IProject CurrentProject => throw new ProjectNotOpenException();

        public event EventHandler CurrentProjectChanged;

        public void CreateNewProject(string projectName, string projectLocation)
        {
            _projectIsOpen = true;
        }

        public void OpenProject(string projectFilePath)
        {
            throw new NotImplementedException();
        }

        public void CloseProject()
        {
            throw new NotImplementedException();
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