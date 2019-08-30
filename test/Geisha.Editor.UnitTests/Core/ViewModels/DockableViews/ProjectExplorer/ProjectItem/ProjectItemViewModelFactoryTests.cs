using System.Collections.Generic;
using System.Linq;
using Geisha.Editor.Core.ViewModels.Infrastructure;
using Geisha.Editor.ProjectHandling.Domain;
using Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem;
using Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem.ContextMenuItems.Add;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.Core.ViewModels.DockableViews.ProjectExplorer.ProjectItem
{
    [TestFixture]
    public class ProjectItemViewModelFactoryTests
    {
        [SetUp]
        public void SetUp()
        {
            _addContextMenuItemFactory = Substitute.For<IAddContextMenuItemFactory>();
        }

        private IAddContextMenuItemFactory _addContextMenuItemFactory;

        private ProjectItemViewModelFactory GetVmFactory()
        {
            return new ProjectItemViewModelFactory(_addContextMenuItemFactory);
        }

        private IProjectItem GetFileProjectItem(string name = "")
        {
            var projectItem = Substitute.For<IProjectItem>();
            projectItem.Type.Returns(ProjectItemType.File);
            projectItem.Name.Returns(name);

            return projectItem;
        }

        private IProjectItem GetDirectoryProjectItem(string name = "")
        {
            var projectItem = Substitute.For<IProjectItem>();
            projectItem.Type.Returns(ProjectItemType.Directory);
            projectItem.Name.Returns(name);

            return projectItem;
        }

        [Test]
        public void Create_ShouldCreateDirectoryProjectItemViewModel_GivenDirectoryProjectItem()
        {
            // Arrange
            var factory = GetVmFactory();

            var projectItem = GetDirectoryProjectItem();
            var window = Substitute.For<IWindow>();

            // Act
            var vms = factory.Create(new[] {projectItem}, window).ToList();

            // Assert
            Assert.That(vms, Is.Not.Empty);
            Assert.That(vms.Single(), Is.TypeOf<DirectoryProjectItemViewModel>());
        }

        [Test]
        public void Create_ShouldCreateFileProjectItemViewModel_GivenFileProjectItem()
        {
            // Arrange
            var factory = GetVmFactory();

            var projectItem = GetFileProjectItem();
            var window = Substitute.For<IWindow>();

            // Act
            var vms = factory.Create(new[] {projectItem}, window).ToList();

            // Assert
            Assert.That(vms, Is.Not.Empty);
            Assert.That(vms.Single(), Is.TypeOf<FileProjectItemViewModel>());
        }

        [Test]
        public void Create_ShouldCreateProjectProjectItemViewModel_GivenProject()
        {
            // Arrange
            var factory = GetVmFactory();

            var project = Substitute.For<IProject>();
            var window = Substitute.For<IWindow>();

            // Act
            var vm = factory.Create(project, window);

            // Assert
            Assert.That(vm, Is.Not.Null);
        }

        [Test]
        public void Create_ShouldOrderViewModelsByTypeThenByName()
        {
            // Arrange
            var factory = GetVmFactory();

            var projectItems = new List<IProjectItem>
            {
                GetFileProjectItem("aaa"),
                GetDirectoryProjectItem("bbb"),
                GetFileProjectItem("eee"),
                GetDirectoryProjectItem("fff"),
                GetFileProjectItem("ccc"),
                GetDirectoryProjectItem("ddd")
            };

            var window = Substitute.For<IWindow>();

            // Act
            var vms = factory.Create(projectItems, window).ToList();

            // Assert
            Assert.That(vms, Is.Not.Empty);
            Assert.That(vms, Has.Count.EqualTo(6));
            Assert.That(vms.ElementAt(0).Name, Is.EqualTo("bbb"));
            Assert.That(vms.ElementAt(1).Name, Is.EqualTo("ddd"));
            Assert.That(vms.ElementAt(2).Name, Is.EqualTo("fff"));
            Assert.That(vms.ElementAt(3).Name, Is.EqualTo("aaa"));
            Assert.That(vms.ElementAt(4).Name, Is.EqualTo("ccc"));
            Assert.That(vms.ElementAt(5).Name, Is.EqualTo("eee"));
        }
    }
}