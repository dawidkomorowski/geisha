using System.Linq;
using Geisha.Editor.Core;
using Geisha.Editor.SceneEditor.Model;
using Geisha.Editor.SceneEditor.UserInterface.SceneEditor;
using Geisha.Engine.Core.SceneModel;
using Geisha.TestUtils;
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
        private ISceneModelFactory _sceneModelFactory = null!;

        [SetUp]
        public void SetUp()
        {
            _eventBus = new EventBus();
            _sceneLoader = Substitute.For<ISceneLoader>();
            _sceneModelFactory = Substitute.For<ISceneModelFactory>();
        }

        [Test]
        public void Constructor_ShouldLoadTheSceneFromFile()
        {
            // Arrange
            _sceneLoader.Load(SceneFilePath).Returns(TestSceneFactory.Create());

            // Act
            _ = new SceneEditorViewModel(SceneFilePath, _eventBus, _sceneLoader, _sceneModelFactory);

            // Assert
            _sceneLoader.Received(1).Load(SceneFilePath);
        }

        [Test]
        public void OnDocumentSelected_ShouldSendEventWithSceneModel()
        {
            // Arrange
            var scene = TestSceneFactory.Create();
            var sceneModel = TestSceneModelFactory.Create(scene);

            _sceneLoader.Load(SceneFilePath).Returns(scene);
            _sceneModelFactory.Create(scene).Returns(sceneModel);

            var sceneEditorViewModel = new SceneEditorViewModel(SceneFilePath, _eventBus, _sceneLoader, _sceneModelFactory);

            SelectedSceneModelChangedEvent? @event = null;
            _eventBus.RegisterEventHandler<SelectedSceneModelChangedEvent>(e => @event = e);

            // Act
            sceneEditorViewModel.OnDocumentSelected();

            // Assert
            Assert.That(@event, Is.Not.Null);
            Assert.That(@event!.SceneModel, Is.EqualTo(sceneModel));
        }

        [Test]
        public void SaveDocument_ShouldSaveSceneToFile()
        {
            // Arrange
            var scene = TestSceneFactory.Create();
            _sceneLoader.Load(SceneFilePath).Returns(scene);

            var sceneEditorViewModel = new SceneEditorViewModel(SceneFilePath, _eventBus, _sceneLoader, _sceneModelFactory);

            // Act
            sceneEditorViewModel.SaveDocument();

            // Assert
            _sceneLoader.Received(1).Save(scene, SceneFilePath);
        }
    }
}