namespace Geisha.Editor.Core.Models.Domain.ProjectHandling
{
    public interface IProjectRepository
    {
        Project CreateProject(string projectName, string projectLocation);
        Project OpenProject(string projectFilePath);
    }
}