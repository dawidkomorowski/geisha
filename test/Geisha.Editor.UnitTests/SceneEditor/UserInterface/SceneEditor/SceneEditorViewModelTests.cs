using System.Linq;
using Geisha.Editor.Core;
using Geisha.Editor.SceneEditor.UserInterface.SceneEditor;
using Geisha.Engine.Core.SceneModel;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.SceneEditor.UserInterface.SceneEditor
{
    [TestFixture]
    public class SceneEditorViewModelTests
    {
        private const string SceneFilePath = "level1.scene";
        private IEventBus _eventBus = null!;
        private ISceneLoader _sceneLoader = null!;

        [SetUp]
        public void SetUp()
        {
            _eventBus = new EventBus();
            _sceneLoader = Substitute.For<ISceneLoader>();
        }

        [Test]
        public void Constructor_ShouldLoadTheSceneFromFile()
        {
            // Arrange
            _sceneLoader.Load(SceneFilePath).Returns(new Scene());

            // Act
            _ = new SceneEditorViewModel(SceneFilePath, _eventBus, _sceneLoader);

            // Assert
            _sceneLoader.Received(1).Load(SceneFilePath);
        }

        [Test]
        public void OnDocumentSelected_ShouldSendEventWithSceneModel()
        {
            // Arrange
            var scene = new Scene();
            scene.AddEntity(new Entity {Name = "Entity"});
            _sceneLoader.Load(SceneFilePath).Returns(scene);

            var sceneEditorViewModel = new SceneEditorViewModel(SceneFilePath, _eventBus, _sceneLoader);

            SelectedSceneModelChangedEvent? @event = null;
            _eventBus.RegisterEventHandler<SelectedSceneModelChangedEvent>(e => @event = e);

            // Act
            sceneEditorViewModel.OnDocumentSelected();

            // Assert
            Assert.That(@event, Is.Not.Null);
            Assert.That(@event!.SceneModel, Is.Not.Null);
            Assert.That(@event.SceneModel.RootEntities, Has.Count.EqualTo(1));
            Assert.That(@event.SceneModel.RootEntities.Single().Name, Is.EqualTo("Entity"));
        }

        [Test]
        public void SaveDocument_ShouldSaveSceneToFile()
        {
            // Arrange
            var scene = new Scene();
            _sceneLoader.Load(SceneFilePath).Returns(scene);

            var sceneEditorViewModel = new SceneEditorViewModel(SceneFilePath, _eventBus, _sceneLoader);

            // Act
            sceneEditorViewModel.SaveDocument();

            // Assert
            _sceneLoader.Received(1).Save(scene, SceneFilePath);
        }
    }
}