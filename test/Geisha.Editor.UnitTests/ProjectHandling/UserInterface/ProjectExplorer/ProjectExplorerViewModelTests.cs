using System.Linq;
using Geisha.Editor.Core.ViewModels.Infrastructure;
using Geisha.Editor.ProjectHandling.Model;
using Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer;
using Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem;
using Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem.ContextMenuItems.Add;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.ProjectHandling.UserInterface.ProjectExplorer
{
    [TestFixture]
    public class ProjectExplorerViewModelTests
    {
        [SetUp]
        public void SetUp()
        {
            _projectExplorerItemViewModelFactory = Substitute.For<IProjectExplorerItemViewModelFactory>();
            _projectService = Substitute.For<IProjectService>();
            _window = Substitute.For<IWindow>();
            _addContextMenuItemFactory = Substitute.For<IAddContextMenuItemFactory>();
        }

        private IProjectExplorerItemViewModelFactory _projectExplorerItemViewModelFactory;
        private IProjectService _projectService;
        private IWindow _window;
        private IAddContextMenuItemFactory _addContextMenuItemFactory;

        private ProjectExplorerViewModel GetViewModel()
        {
            return new ProjectExplorerViewModel(_projectExplorerItemViewModelFactory, _projectService)
            {
                Window = _window
            };
        }

        [Test]
        public void OnCurrentProjectChanged_AddsProjectItems_WhenProjectIsOpened()
        {
            // Arrange
            var vm = GetViewModel();

            var project = Substitute.For<IProject>();
            var projectItemViewModel = new ProjectRootViewModel(project, _projectExplorerItemViewModelFactory, _addContextMenuItemFactory, _window);

            _projectExplorerItemViewModelFactory.Create(project, _window).Returns(projectItemViewModel);
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
            var projectItemViewModel = new ProjectRootViewModel(project, _projectExplorerItemViewModelFactory, _addContextMenuItemFactory, _window);
            vm.Items.Add(projectItemViewModel);

            _projectService.ProjectIsOpen.Returns(false);

            // Act
            _projectService.CurrentProjectChanged += Raise.Event();

            // Assert
            Assert.That(vm.Items, Has.Count.EqualTo(0));
        }
    }
}