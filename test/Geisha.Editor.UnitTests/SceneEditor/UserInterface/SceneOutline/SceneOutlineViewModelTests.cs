using System.Linq;
using Geisha.Editor.Core;
using Geisha.Editor.SceneEditor.Model;
using Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor;
using Geisha.Editor.SceneEditor.UserInterface.SceneEditor;
using Geisha.Editor.SceneEditor.UserInterface.SceneOutline;
using Geisha.Editor.SceneEditor.UserInterface.SceneOutline.SceneOutlineItem;
using Geisha.Engine.Core.SceneModel;
using Geisha.TestUtils;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.SceneEditor.UserInterface.SceneOutline
{
    [TestFixture]
    public class SceneOutlineViewModelTests
    {
        private IEventBus _eventBus = null!;
        private IEntityPropertiesEditorViewModelFactory _entityPropertiesEditorViewModelFactory = null!;

        [SetUp]
        public void SetUp()
        {
            _eventBus = new EventBus();
            _entityPropertiesEditorViewModelFactory = Substitute.For<IEntityPropertiesEditorViewModelFactory>();
        }

        private SceneOutlineViewModel CreateSceneOutlineViewModel()
        {
            return new SceneOutlineViewModel(_eventBus, _entityPropertiesEditorViewModelFactory);
        }

        [Test]
        public void Constructor_ShouldCreateSceneOutlineViewModelWithNoItems()
        {
            // Arrange
            // Act
            var sceneOutlineViewModel = CreateSceneOutlineViewModel();

            // Assert
            Assert.That(sceneOutlineViewModel.Items, Has.Count.Zero);
        }

        [Test]
        public void SelectedSceneModelChangedEvent_ShouldAddItems()
        {
            // Arrange
            var sceneOutlineViewModel = CreateSceneOutlineViewModel();

            var scene = TestSceneFactory.Create();
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
            var sceneOutlineViewModel = CreateSceneOutlineViewModel();

            var existingScene = TestSceneFactory.Create();
            existingScene.AddEntity(new Entity {Name = "Existing entity"});
            var existingSceneModel = new SceneModel(existingScene);

            var scene = TestSceneFactory.Create();
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

        [Test]
        public void SelectedItem_ShouldCallOnSelected_WhenValueHasChanged()
        {
            // Arrange
            var sceneOutlineViewModel = CreateSceneOutlineViewModel();

            var selectedItem = Substitute.For<SceneOutlineItemViewModel>();

            // Act
            sceneOutlineViewModel.SelectedItem = selectedItem;

            // Assert
            selectedItem.Received(1).OnSelected();
        }

        [Test]
        public void SelectedItem_ShouldNotThrowException_WhenSetToNull()
        {
            // Arrange
            var sceneOutlineViewModel = CreateSceneOutlineViewModel();

            var selectedItem = Substitute.For<SceneOutlineItemViewModel>();
            sceneOutlineViewModel.SelectedItem = selectedItem;

            // Act
            sceneOutlineViewModel.SelectedItem = null;

            // Assert
            Assert.That(sceneOutlineViewModel.SelectedItem, Is.Null);
        }
    }
}