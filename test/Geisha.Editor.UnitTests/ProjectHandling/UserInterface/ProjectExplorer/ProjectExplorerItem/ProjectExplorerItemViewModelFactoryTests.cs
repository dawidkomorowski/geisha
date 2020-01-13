﻿using System.Linq;
using Geisha.Editor.Core;
using Geisha.Editor.CreateSound.UserInterface;
using Geisha.Editor.CreateSprite.UserInterface;
using Geisha.Editor.CreateTexture.UserInterface;
using Geisha.Editor.ProjectHandling.Model;
using Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectExplorerItem;
using Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectExplorerItem.ContextMenuItems.Add;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.ProjectHandling.UserInterface.ProjectExplorer.ProjectExplorerItem
{
    [TestFixture]
    public class ProjectExplorerItemViewModelFactoryTests
    {
        private IEventBus _eventBus;
        private IAddContextMenuItemFactory _addContextMenuItemFactory;
        private ICreateTextureCommandFactory _createTextureCommandFactory;
        private ICreateSpriteCommandFactory _createSpriteCommandFactory;
        private ICreateSoundCommandFactory _createSoundCommandFactory;

        [SetUp]
        public void SetUp()
        {
            _eventBus = new EventBus();
            _addContextMenuItemFactory = Substitute.For<IAddContextMenuItemFactory>();
            _createTextureCommandFactory = Substitute.For<ICreateTextureCommandFactory>();
            _createSpriteCommandFactory = Substitute.For<ICreateSpriteCommandFactory>();
            _createSoundCommandFactory = Substitute.For<ICreateSoundCommandFactory>();
        }

        private ProjectExplorerItemViewModelFactory GetVmFactory()
        {
            return new ProjectExplorerItemViewModelFactory(
                _eventBus,
                _addContextMenuItemFactory,
                _createTextureCommandFactory,
                _createSpriteCommandFactory,
                _createSoundCommandFactory);
        }

        private static IProjectFile GetProjectFile(string name = "")
        {
            var file = Substitute.For<IProjectFile>();
            file.Name.Returns(name);

            return file;
        }

        private static IProjectFolder GetProjectFolder(string name = "")
        {
            var folder = Substitute.For<IProjectFolder>();
            folder.FolderName.Returns(name);

            return folder;
        }

        [Test]
        public void Create_ShouldCreateFolderViewModel_GivenProjectFolder()
        {
            // Arrange
            var factory = GetVmFactory();

            var folder = GetProjectFolder();

            // Act
            var vms = factory.Create(new[] {folder}, Enumerable.Empty<IProjectFile>()).ToList();

            // Assert
            Assert.That(vms, Is.Not.Empty);
            Assert.That(vms.Single(), Is.TypeOf<FolderViewModel>());
        }

        [Test]
        public void Create_ShouldCreateFileViewModel_GivenProjectFile()
        {
            // Arrange
            var factory = GetVmFactory();

            var file = GetProjectFile();

            // Act
            var vms = factory.Create(Enumerable.Empty<IProjectFolder>(), new[] {file}).ToList();

            // Assert
            Assert.That(vms, Is.Not.Empty);
            Assert.That(vms.Single(), Is.TypeOf<FileViewModel>());
        }

        [Test]
        public void Create_ShouldCreateProjectRootViewModel_GivenProject()
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
        public void Create_ShouldReturnFirstFoldersThenFilesViewModelsAllOrderedByName()
        {
            // Arrange
            var factory = GetVmFactory();

            var folders = new[]
            {
                GetProjectFolder("bbb-folder"),
                GetProjectFolder("fff-folder"),
                GetProjectFolder("ddd-folder")
            };

            var files = new[]
            {
                GetProjectFile("aaa-file"),
                GetProjectFile("eee-file"),
                GetProjectFile("ccc-file")
            };

            // Act
            var vms = factory.Create(folders, files).ToList();

            // Assert
            Assert.That(vms, Is.Not.Empty);
            Assert.That(vms, Has.Count.EqualTo(6));
            Assert.That(vms.ElementAt(0).Name, Is.EqualTo("bbb-folder"));
            Assert.That(vms.ElementAt(1).Name, Is.EqualTo("ddd-folder"));
            Assert.That(vms.ElementAt(2).Name, Is.EqualTo("fff-folder"));
            Assert.That(vms.ElementAt(3).Name, Is.EqualTo("aaa-file"));
            Assert.That(vms.ElementAt(4).Name, Is.EqualTo("ccc-file"));
            Assert.That(vms.ElementAt(5).Name, Is.EqualTo("eee-file"));
        }
    }
}