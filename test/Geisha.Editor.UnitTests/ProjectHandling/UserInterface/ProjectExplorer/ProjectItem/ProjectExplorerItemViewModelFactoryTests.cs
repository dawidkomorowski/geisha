using System.Linq;
using Geisha.Editor.ProjectHandling.Model;
using Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem;
using Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem.ContextMenuItems.Add;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem
{
    [TestFixture]
    public class ProjectExplorerItemViewModelFactoryTests
    {
        [SetUp]
        public void SetUp()
        {
            _addContextMenuItemFactory = Substitute.For<IAddContextMenuItemFactory>();
        }

        private IAddContextMenuItemFactory _addContextMenuItemFactory;

        private ProjectExplorerItemViewModelFactory GetVmFactory()
        {
            return new ProjectExplorerItemViewModelFactory(_addContextMenuItemFactory);
        }

        private static IProjectFile GetFileProjectItem(string name = "")
        {
            var file = Substitute.For<IProjectFile>();
            file.Name.Returns(name);

            return file;
        }

        private static IProjectFolder GetDirectoryProjectItem(string name = "")
        {
            var folder = Substitute.For<IProjectFolder>();
            folder.Name.Returns(name);

            return folder;
        }

        [Test]
        public void Create_ShouldCreateDirectoryProjectItemViewModel_GivenDirectoryProjectItem()
        {
            // Arrange
            var factory = GetVmFactory();

            var folder = GetDirectoryProjectItem();

            // Act
            var vms = factory.Create(new[] {folder}, Enumerable.Empty<IProjectFile>()).ToList();

            // Assert
            Assert.That(vms, Is.Not.Empty);
            Assert.That(vms.Single(), Is.TypeOf<FolderViewModel>());
        }

        [Test]
        public void Create_ShouldCreateFileProjectItemViewModel_GivenFileProjectItem()
        {
            // Arrange
            var factory = GetVmFactory();

            var file = GetFileProjectItem();

            // Act
            var vms = factory.Create(Enumerable.Empty<IProjectFolder>(), new[] {file}).ToList();

            // Assert
            Assert.That(vms, Is.Not.Empty);
            Assert.That(vms.Single(), Is.TypeOf<FileViewModel>());
        }

        [Test]
        public void Create_ShouldCreateProjectProjectItemViewModel_GivenProject()
        {
            // Arrange
            var factory = GetVmFactory();

            var project = Substitute.For<IProject>();

            // Act
            var vm = factory.Create(project);

            // Assert
            Assert.That(vm, Is.Not.Null);
        }

        [Test]
        public void Create_ShouldOrderViewModelsByTypeThenByName()
        {
            // Arrange
            var factory = GetVmFactory();

            var folders = new[]
            {
                GetDirectoryProjectItem("bbb"),
                GetDirectoryProjectItem("fff"),
                GetDirectoryProjectItem("ddd")
            };

            var files = new[]
            {
                GetFileProjectItem("aaa"),
                GetFileProjectItem("eee"),
                GetFileProjectItem("ccc")
            };

            // Act
            var vms = factory.Create(folders, files).ToList();

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