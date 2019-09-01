using Geisha.Editor.Core.Infrastructure;
using Geisha.Editor.ProjectHandling.Model;
using Geisha.Editor.ProjectHandling.UserInterface.NewProjectDialog;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.ProjectHandling.UserInterface.NewProjectDialog
{
    [TestFixture]
    public class NewProjectDialogViewModelFactoryTests
    {
        [Test]
        public void Create_ShouldCreateNewViewModel()
        {
            // Arrange
            var requestFilePathService = Substitute.For<IRequestFilePathService>();
            var projectService = Substitute.For<IProjectService>();
            var factory = new NewProjectDialogViewModelFactory(requestFilePathService, projectService);

            // Act
            var actual = factory.Create();

            // Assert
            Assert.That(actual, Is.Not.Null);
        }
    }
}