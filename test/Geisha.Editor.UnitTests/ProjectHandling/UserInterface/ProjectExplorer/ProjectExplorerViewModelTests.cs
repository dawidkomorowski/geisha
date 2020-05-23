using System.Linq;
using Geisha.Editor.ProjectHandling.Model;
using Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer;
using Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectExplorerItem;
using Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectExplorerItem.ContextMenuItems.Add;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.ProjectHandling.UserInterface.ProjectExplorer
{
    [TestFixture]
    public class ProjectExplorerViewModelTests
    {
        private IProjectExplorerItemViewModelFactory _projectExplorerItemViewModelFactory = null!;
        private IProjectService _projectService = null!;
        private IAddContextMenuItemFactory _addContextMenuItemFactory = null!;

        [SetUp]
        public void SetUp()
        {
            _projectExplorerItemViewModelFactory = Substitute.For<IProjectExplorerItemViewModelFactory>();
            _projectService = Substitute.For<IProjectService>();
            _addContextMenuItemFactory = Substitute.For<IAddContextMenuItemFactory>();
        }

        private ProjectExplorerViewModel GetViewModel()
        {
            return new ProjectExplorerViewModel(_projectExplorerItemViewModelFactory, _projectService);
        }

        [Test]
        public void OnCurrentProjectChanged_AddsProjectItems_WhenProjectIsOpened()
        {
            // Arrange
            var vm = GetViewModel();

            var project = Substitute.For<IProject>();
            var projectItemViewModel = new ProjectRootViewModel(project, _projectExplorerItemViewModelFactory, _addContextMenuItemFactory);

            _projectExplorerItemViewModelFactory.Create(project).Returns(projectItemViewModel);
            _projectService.CurrentProject.Returns(project);
            _projectService.ProjectIsOpen.Returns(true);

            // Act
            _projectService.CurrentProjectChanged += Raise.Event();

            // Assert
            Assert.That(vm.Items, Has.Count.EqualTo(1));
            Assert.That(vm.Items.Single(), Is.EqualTo(projectItemViewModel));
        }

        [Test]
        public void OnCurrentProjectChanged_ClearsProjectItems_WhenProjectIsClosed()
        {
            // Arrange
            var vm = GetViewModel();

            var project = Substitute.For<IProject>();
            var projectItemViewModel = new ProjectRootViewModel(project, _projectExplorerItemViewModelFactory, _addContextMenuItemFactory);
            vm.Items.Add(projectItemViewModel);

            _projectService.ProjectIsOpen.Returns(false);

            // Act
            _projectService.CurrentProjectChanged += Raise.Event();

            // Assert
            Assert.That(vm.Items, Has.Count.EqualTo(0));
        }
    }
}