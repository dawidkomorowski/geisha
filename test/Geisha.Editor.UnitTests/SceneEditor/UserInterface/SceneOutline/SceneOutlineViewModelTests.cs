using System.Linq;
using Geisha.Editor.Core;
using Geisha.Editor.SceneEditor.Model;
using Geisha.Editor.SceneEditor.UserInterface.SceneEditor;
using Geisha.Editor.SceneEditor.UserInterface.SceneOutline;
using Geisha.Engine.Core.SceneModel;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.SceneEditor.UserInterface.SceneOutline
{
    [TestFixture]
    public class SceneOutlineViewModelTests
    {
        private IEventBus _eventBus;

        [SetUp]
        public void SetUp()
        {
            _eventBus = new EventBus();
        }

        [Test]
        public void Constructor_ShouldCreateSceneOutlineViewModelWithNoItems()
        {
            // Arrange
            // Act
            var sceneOutlineViewModel = new SceneOutlineViewModel(_eventBus);

            // Assert
            Assert.That(sceneOutlineViewModel.Items, Has.Count.Zero);
        }

        [Test]
        public void SelectedSceneModelChangedEvent_ShouldAddItems()
        {
            // Arrange
            var sceneOutlineViewModel = new SceneOutlineViewModel(_eventBus);

            var scene = new Scene();
            scene.AddEntity(new Entity {Name = "Entity"});
            var sceneModel = new SceneModel(scene);

            // Act
            _eventBus.SendEvent(new SelectedSceneModelChangedEvent(sceneModel));

            // Assert
            Assert.That(sceneOutlineViewModel.Items, Has.Count.EqualTo(1));
            var sceneRootViewModel = sceneOutlineViewModel.Items.Single();
            Assert.That(sceneRootViewModel.Name, Is.EqualTo("Scene"));
            Assert.That(sceneRootViewModel.Items, Has.Count.EqualTo(1));
            var entityViewModel = sceneRootViewModel.Items.Single();
            Assert.That(entityViewModel.Name, Is.EqualTo("Entity"));
            Assert.That(entityViewModel.Items, Has.Count.Zero);
        }

        [Test]
        public void SelectedSceneModelChangedEvent_ShouldClearExistingItemsAndAddNewItems()
        {
            // Arrange
            var sceneOutlineViewModel = new SceneOutlineViewModel(_eventBus);

            var existingScene = new Scene();
            existingScene.AddEntity(new Entity {Name = "Existing entity"});
            var existingSceneModel = new SceneModel(existingScene);

            var scene = new Scene();
            scene.AddEntity(new Entity {Name = "Entity"});
            var sceneModel = new SceneModel(scene);

            _eventBus.SendEvent(new SelectedSceneModelChangedEvent(existingSceneModel));

            // Assume
            Assume.That(sceneOutlineViewModel.Items, Has.Count.EqualTo(1));
            Assume.That(sceneOutlineViewModel.Items.Single().Items.Single().Name, Is.EqualTo("Existing entity"));

            // Act
            _eventBus.SendEvent(new SelectedSceneModelChangedEvent(sceneModel));

            // Assert
            Assert.That(sceneOutlineViewModel.Items, Has.Count.EqualTo(1));
            var sceneRootViewModel = sceneOutlineViewModel.Items.Single();
            Assert.That(sceneRootViewModel.Name, Is.EqualTo("Scene"));
            Assert.That(sceneRootViewModel.Items, Has.Count.EqualTo(1));
            var entityViewModel = sceneRootViewModel.Items.Single();
            Assert.That(entityViewModel.Name, Is.EqualTo("Entity"));
            Assert.That(entityViewModel.Items, Has.Count.Zero);
        }
    }
}