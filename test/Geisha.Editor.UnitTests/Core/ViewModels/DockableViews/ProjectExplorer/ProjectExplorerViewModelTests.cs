using System.Linq;
using Geisha.Editor.Core.Models.Domain.ProjectHandling;
using Geisha.Editor.Core.ViewModels.DockableViews.ProjectExplorer;
using Geisha.Editor.Core.ViewModels.DockableViews.ProjectExplorer.ProjectItem;
using Geisha.Editor.Core.ViewModels.DockableViews.ProjectExplorer.ProjectItem.ContextMenuItems.Add;
using Geisha.Editor.Core.ViewModels.Infrastructure;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.Core.ViewModels.DockableViews.ProjectExplorer
{
    [TestFixture]
    public class ProjectExplorerViewModelTests
    {
        [SetUp]
        public void SetUp()
        {
            _projectItemViewModelFactory = Substitute.For<IProjectItemViewModelFactory>();
            _projectService = Substitute.For<IProjectService>();
            _window = Substitute.For<IWindow>();
            _addContextMenuItemFactory = Substitute.For<IAddContextMenuItemFactory>();
        }

        private IProjectItemViewModelFactory _projectItemViewModelFactory;
        private IProjectService _projectService;
        private IWindow _window;
        private IAddContextMenuItemFactory _addContextMenuItemFactory;

        private ProjectExplorerViewModel GetViewModel()
        {
            return new ProjectExplorerViewModel(_projectItemViewModelFactory, _projectService)
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
            var projectItemViewModel = new ProjectProjectItemViewModel(project, _projectItemViewModelFactory, _addContextMenuItemFactory, _window);

            _projectItemViewModelFactory.Create(project, _window).Returns(projectItemViewModel);
            _projectService.CurrentProject.Returns(project);
            _projectService.IsProjectOpen.Returns(true);

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
            var projectItemViewModel = new ProjectProjectItemViewModel(project, _projectItemViewModelFactory, _addContextMenuItemFactory, _window);
            vm.Items.Add(projectItemViewModel);

            _projectService.IsProjectOpen.Returns(false);

            // Act
            _projectService.CurrentProjectChanged += Raise.Event();

            // Assert
            Assert.That(vm.Items, Has.Count.EqualTo(0));
        }
    }
}