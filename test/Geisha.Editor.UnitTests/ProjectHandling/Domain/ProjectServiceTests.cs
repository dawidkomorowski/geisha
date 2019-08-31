using Geisha.Editor.ProjectHandling.Domain;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.ProjectHandling.Domain
{
    [TestFixture]
    public class ProjectServiceTests
    {
        private ProjectService CreateProjectService()
        {
            return new ProjectService();
        }

        [Test]
        public void NewProjectServiceDoesNotHaveOpenProject()
        {
            // Arrange
            // Act
            var projectService = CreateProjectService();

            // Assert
            Assert.That(projectService.ProjectIsOpen, Is.False);
        }

        [Test]
        public void CurrentProject_ThrowsException_WhenProjectIsNotOpen()
        {
            // Arrange
            var projectService = CreateProjectService();

            // Assume
            Assume.That(projectService.ProjectIsOpen, Is.False);

            // Act
            // Assert
            Assert.That(() =>
            {
                var _ = projectService.CurrentProject;
            }, Throws.TypeOf<ProjectNotOpenException>());
        }

        #region CreateNewProject

        private ProjectService CreateNewProjectScenarioSetUp()
        {
            // Arrange
            const string projectName = "Project name";
            const string projectLocation = @"C:\some_location";
            var projectService = CreateProjectService();

            // Act
            projectService.CreateNewProject(projectName, projectLocation);

            return projectService;
        }

        [Test]
        public void CreateNewProject_ShouldMake_ProjectIsOpen_ReturnTrue()
        {
            var projectService = CreateNewProjectScenarioSetUp();

            // Assert
            Assert.That(projectService.ProjectIsOpen, Is.True);
        }

        //[Test]
        //public void CreateNewProject_ShouldSet_CurrentProject_ToCreatedProject()
        //{
        //    var projectService = CreateNewProjectScenarioSetUp();

        //    // Assert
        //    var project = projectService.CurrentProject;
        //    Assert.That(project.Name, Is.EqualTo());
        //}

        #endregion
    }
}