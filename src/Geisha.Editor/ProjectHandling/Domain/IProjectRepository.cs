namespace Geisha.Editor.ProjectHandling.Domain
{
    public interface IProjectRepository
    {
        ProjectObsolete CreateProject(string projectName, string projectLocation);
        ProjectObsolete OpenProject(string projectFilePath);
        void SaveProject(ProjectObsolete project);
    }
}