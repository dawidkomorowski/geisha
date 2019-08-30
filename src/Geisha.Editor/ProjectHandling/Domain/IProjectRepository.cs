namespace Geisha.Editor.ProjectHandling.Domain
{
    public interface IProjectRepository
    {
        Project CreateProject(string projectName, string projectLocation);
        Project OpenProject(string projectFilePath);
        void SaveProject(Project project);
    }
}