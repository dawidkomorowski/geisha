using System;
using System.Linq;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.SceneModel;
using Geisha.TestUtils;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.Extensions;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.SceneModel
{
    [TestFixture]
    public class SceneManagerTests
    {
        private IAssetStore _assetStore = null!;
        private ISceneLoader _sceneLoader = null!;
        private ISceneFactory _sceneFactory = null!;
        private ISceneBehaviorFactoryProvider _sceneBehaviorFactoryProvider = null!;
        private ISceneObserver _sceneObserver1 = null!;
        private ISceneObserver _sceneObserver2 = null!;
        private Scene _initialScene = null!;
        private SceneManager _sceneManager = null!;

        [SetUp]
        public void SetUp()
        {
            _assetStore = Substitute.For<IAssetStore>();
            _sceneLoader = Substitute.For<ISceneLoader>();
            _sceneFactory = Substitute.For<ISceneFactory>();
            _sceneBehaviorFactoryProvider = Substitute.For<ISceneBehaviorFactoryProvider>();
            _sceneBehaviorFactoryProvider.Get(Arg.Any<string>()).ThrowsForAnyArgs(new InvalidOperationException("Missing substitute configuration."));

            _initialScene = TestSceneFactory.Create();
            _sceneFactory.Create().Returns(_initialScene);

            _sceneObserver1 = Substitute.For<ISceneObserver>();
            _sceneObserver2 = Substitute.For<ISceneObserver>();

            _sceneManager = new SceneManager(_assetStore, _sceneLoader, _sceneFactory, _sceneBehaviorFactoryProvider);
            _sceneManager.Initialize(new[] { _sceneObserver1, _sceneObserver2 });

            _sceneFactory.ClearReceivedCalls();
        }

        [Test]
        public void Constructor_ShouldSetCurrentSceneToEmptyScene()
        {
            // Arrange
            // Act
            // Assert
            Assert.That(_sceneManager.CurrentScene, Is.EqualTo(_initialScene));
        }

        [Test]
        public void Initialize_ShouldThrowException_WhenSceneManagerAlreadyInitialized()
        {
            // Arrange
            // Act
            // Assert
            Assert.That(() => _sceneManager.Initialize(Enumerable.Empty<ISceneObserver>()), Throws.InvalidOperationException);
        }

        #region LoadEmptyScene

        [Test]
        public void LoadEmptyScene_ShouldThrowException_WhenSceneManagerIsNotInitialized()
        {
            // Arrange
            var sceneManager = new SceneManager(_assetStore, _sceneLoader, _sceneFactory, _sceneBehaviorFactoryProvider);

            // Act
            // Assert
            Assert.That(() => sceneManager.LoadEmptyScene(string.Empty), Throws.InvalidOperationException);
        }

        [Test]
        public void LoadEmptyScene_ShouldNotLoadEmptySceneAndSetAsCurrent_WhenOnNextFrameWasNotCalledAfter()
        {
            // Arrange
            const string sceneBehaviorName = "Behavior name";

            // Act
            _sceneManager.LoadEmptyScene(sceneBehaviorName);

            // Assert
            Assert.That(_sceneManager.CurrentScene, Is.EqualTo(_initialScene));
        }

        [Test]
        public void LoadEmptyScene_And_OnNextFrame_ShouldLoadEmptySceneAndSetAsCurrent_GivenSceneBehaviorName()
        {
            // Arrange
            const string sceneBehaviorName = "Behavior name";
            var scene = TestSceneFactory.Create();

            var sceneBehavior = SetUpSceneBehavior(sceneBehaviorName, scene);
            _sceneFactory.Create().Returns(scene);

            // Act
            _sceneManager.LoadEmptyScene(sceneBehaviorName);
            _sceneManager.OnNextFrame();

            // Assert
            Assert.That(_sceneManager.CurrentScene, Is.EqualTo(scene));
            Assert.That(_sceneManager.CurrentScene.SceneBehavior, Is.EqualTo(sceneBehavior));
        }

        [Test]
        public void LoadEmptyScene_And_OnNextFrame_ShouldRemoveObserversFromCurrentScene_And_AddObserversToNewScene()
        {
            // Arrange
            const string sceneBehaviorName1 = "Behavior name 1";
            var scene1 = TestSceneFactory.Create();
            var entity1 = scene1.CreateEntity();

            _ = SetUpSceneBehavior(sceneBehaviorName1, scene1);
            _sceneFactory.Create().Returns(scene1);

            _sceneManager.LoadEmptyScene(sceneBehaviorName1);
            _sceneManager.OnNextFrame();

            const string sceneBehaviorName2 = "Behavior name 2";
            var scene2 = TestSceneFactory.Create();
            var entity2 = scene2.CreateEntity();

            _ = SetUpSceneBehavior(sceneBehaviorName2, scene2);
            _sceneFactory.Create().Returns(scene2);

            _sceneObserver1.ClearReceivedCalls();
            _sceneObserver2.ClearReceivedCalls();

            // Act
            _sceneManager.LoadEmptyScene(sceneBehaviorName2);
            _sceneManager.OnNextFrame();

            // Assert
            Received.InOrder(() =>
            {
                _sceneObserver1.OnEntityRemoved(entity1);
                _sceneObserver2.OnEntityRemoved(entity1);

                _sceneObserver1.OnEntityCreated(entity2);
                _sceneObserver2.OnEntityCreated(entity2);
            });
        }

        [Test]
        public void LoadEmptyScene_And_OnNextFrame_ShouldExecuteOnLoadedOfSceneBehaviorForLoadedScene()
        {
            // Arrange
            const string sceneBehaviorName = "Behavior name";
            var scene = TestSceneFactory.Create();

            var sceneBehavior = SetUpSceneBehavior(sceneBehaviorName, scene);
            _sceneFactory.Create().Returns(scene);

            // Act
            _sceneManager.LoadEmptyScene(sceneBehaviorName);
            _sceneManager.OnNextFrame();

            // Assert
            sceneBehavior.Received(1).OnLoaded();
        }

        [Test]
        public void LoadEmptyScene_And_OnNextFrame_ShouldUnloadAssets_GivenUnloadAssetsSceneLoadMode()
        {
            // Arrange
            const string sceneBehaviorName = "Behavior name";
            var scene = TestSceneFactory.Create();

            SetUpSceneBehavior(sceneBehaviorName, scene);
            _sceneFactory.Create().Returns(scene);

            // Act
            _sceneManager.LoadEmptyScene(sceneBehaviorName, SceneLoadMode.UnloadAssets);
            _sceneManager.OnNextFrame();

            // Assert
            _assetStore.Received().UnloadAssets();
        }

        [Test]
        public void LoadEmptyScene_And_OnNextFrame_ShouldNotUnloadAssets_GivenPreserveAssetsSceneLoadMode()
        {
            // Arrange
            const string sceneBehaviorName = "Behavior name";
            var scene = TestSceneFactory.Create();

            SetUpSceneBehavior(sceneBehaviorName, scene);
            _sceneFactory.Create().Returns(scene);

            // Act
            // ReSharper disable once RedundantArgumentDefaultValue
            _sceneManager.LoadEmptyScene(sceneBehaviorName, SceneLoadMode.PreserveAssets);
            _sceneManager.OnNextFrame();

            // Assert
            _assetStore.DidNotReceive().UnloadAssets();
        }

        [Test]
        public void OnNextFrame_HandlesLoadEmptySceneOnlyOnce()
        {
            // Arrange
            const string sceneBehaviorName = "Behavior name";
            var scene1 = TestSceneFactory.Create();
            SetUpSceneBehavior(sceneBehaviorName, scene1);
            _sceneFactory.Create().Returns(scene1);

            _sceneManager.LoadEmptyScene(sceneBehaviorName);
            _sceneManager.OnNextFrame();

            Assume.That(_sceneManager.CurrentScene, Is.EqualTo(scene1));

            var scene2 = TestSceneFactory.Create();
            SetUpSceneBehavior(sceneBehaviorName, scene2);
            _sceneFactory.Create().Returns(scene2);

            // Act
            _sceneManager.OnNextFrame();

            // Assert
            Assert.That(_sceneManager.CurrentScene, Is.EqualTo(scene1));
        }

        [Test]
        public void OnNextFrame_HandlesLatestLoadEmptySceneOnly()
        {
            // Arrange
            const string sceneBehaviorName1 = "Scene Behavior 1";
            const string sceneBehaviorName2 = "Scene Behavior 2";

            var scene = TestSceneFactory.Create();
            SetUpSceneBehavior(sceneBehaviorName1, scene);
            var sceneBehavior2 = SetUpSceneBehavior(sceneBehaviorName2, scene);
            _sceneFactory.Create().Returns(scene);

            _sceneManager.LoadEmptyScene(sceneBehaviorName1);
            _sceneManager.LoadEmptyScene(sceneBehaviorName2);

            // Act
            _sceneManager.OnNextFrame();

            // Assert
            Assert.That(_sceneManager.CurrentScene, Is.EqualTo(scene));
            Assert.That(_sceneManager.CurrentScene.SceneBehavior, Is.EqualTo(sceneBehavior2));
            _sceneFactory.Received(1).Create();
        }

        #endregion

        #region LoadScene

        [Test]
        public void LoadScene_ShouldThrowException_WhenSceneManagerIsNotInitialized()
        {
            // Arrange
            var sceneManager = new SceneManager(_assetStore, _sceneLoader, _sceneFactory, _sceneBehaviorFactoryProvider);

            // Act
            // Assert
            Assert.That(() => sceneManager.LoadScene(string.Empty), Throws.InvalidOperationException);
        }

        [Test]
        public void LoadScene_ShouldNotLoadSceneAndSetAsCurrent_WhenOnNextFrameWasNotCalledAfter()
        {
            // Arrange
            const string sceneFilePath = "start up scene";

            // Act
            _sceneManager.LoadScene(sceneFilePath);

            // Assert
            Assert.That(_sceneManager.CurrentScene, Is.EqualTo(_initialScene));
        }

        [Test]
        public void LoadScene_And_OnNextFrame_ShouldLoadSceneAndSetAsCurrent_GivenPathToSceneFile()
        {
            // Arrange
            const string sceneFilePath = "start up scene";
            var scene = TestSceneFactory.Create();

            _sceneLoader.Load(sceneFilePath).Returns(scene);

            // Act
            _sceneManager.LoadScene(sceneFilePath);
            _sceneManager.OnNextFrame();

            // Assert
            Assert.That(_sceneManager.CurrentScene, Is.EqualTo(scene));
        }

        [Test]
        public void LoadScene_And_OnNextFrame_ShouldRemoveObserversFromCurrentScene_And_AddObserversToNewScene()
        {
            // Arrange
            const string sceneFilePath1 = "start up scene 1";
            var scene1 = TestSceneFactory.Create();
            var entity1 = scene1.CreateEntity();

            _sceneLoader.Load(sceneFilePath1).Returns(scene1);

            _sceneManager.LoadScene(sceneFilePath1);
            _sceneManager.OnNextFrame();

            const string sceneFilePath2 = "start up scene 2";
            var scene2 = TestSceneFactory.Create();
            var entity2 = scene2.CreateEntity();

            _sceneLoader.Load(sceneFilePath2).Returns(scene2);

            _sceneObserver1.ClearReceivedCalls();
            _sceneObserver2.ClearReceivedCalls();

            // Act
            _sceneManager.LoadScene(sceneFilePath2);
            _sceneManager.OnNextFrame();

            // Assert
            Received.InOrder(() =>
            {
                _sceneObserver1.OnEntityRemoved(entity1);
                _sceneObserver2.OnEntityRemoved(entity1);

                _sceneObserver1.OnEntityCreated(entity2);
                _sceneObserver2.OnEntityCreated(entity2);
            });
        }

        [Test]
        public void LoadScene_And_OnNextFrame_ShouldExecuteOnLoadedOfSceneBehaviorForLoadedScene()
        {
            // Arrange
            const string sceneFilePath = "start up scene";

            var scene = TestSceneFactory.Create();
            var sceneBehavior = Substitute.ForPartsOf<SceneBehavior>(scene);
            scene.SceneBehavior = sceneBehavior;

            _sceneLoader.Load(sceneFilePath).Returns(scene);

            // Act
            _sceneManager.LoadScene(sceneFilePath);
            _sceneManager.OnNextFrame();

            // Assert
            sceneBehavior.Received(1).OnLoaded();
        }

        [Test]
        public void LoadScene_And_OnNextFrame_ShouldUnloadAssets_GivenUnloadAssetsSceneLoadMode()
        {
            // Arrange
            const string sceneFilePath = "start up scene";
            var scene = TestSceneFactory.Create();

            _sceneLoader.Load(sceneFilePath).Returns(scene);

            // Act
            _sceneManager.LoadScene(sceneFilePath, SceneLoadMode.UnloadAssets);
            _sceneManager.OnNextFrame();

            // Assert
            _assetStore.Received().UnloadAssets();
        }

        [Test]
        public void LoadScene_And_OnNextFrame_ShouldNotUnloadAssets_GivenPreserveAssetsSceneLoadMode()
        {
            // Arrange
            const string sceneFilePath = "start up scene";
            var scene = TestSceneFactory.Create();

            _sceneLoader.Load(sceneFilePath).Returns(scene);

            // Act
            // ReSharper disable once RedundantArgumentDefaultValue
            _sceneManager.LoadScene(sceneFilePath, SceneLoadMode.PreserveAssets);
            _sceneManager.OnNextFrame();

            // Assert
            _assetStore.DidNotReceive().UnloadAssets();
        }

        [Test]
        public void OnNextFrame_HandlesLoadSceneOnlyOnce()
        {
            // Arrange
            const string sceneFilePath = "start up scene";

            var scene1 = TestSceneFactory.Create();
            _sceneLoader.Load(sceneFilePath).Returns(scene1);

            _sceneManager.LoadScene(sceneFilePath);
            _sceneManager.OnNextFrame();

            Assume.That(_sceneManager.CurrentScene, Is.EqualTo(scene1));

            var scene2 = TestSceneFactory.Create();
            _sceneLoader.Load(sceneFilePath).Returns(scene2);

            // Act
            _sceneManager.OnNextFrame();

            // Assert
            Assert.That(_sceneManager.CurrentScene, Is.EqualTo(scene1));
        }

        [Test]
        public void OnNextFrame_HandlesLatestLoadSceneOnly()
        {
            // Arrange
            const string sceneFilePath1 = "scene 1";
            const string sceneFilePath2 = "scene 2";

            var scene1 = TestSceneFactory.Create();
            var scene2 = TestSceneFactory.Create();

            _sceneLoader.Load(sceneFilePath1).Returns(scene1);
            _sceneLoader.Load(sceneFilePath2).Returns(scene2);

            _sceneManager.LoadScene(sceneFilePath1);
            _sceneManager.LoadScene(sceneFilePath2);

            // Act
            _sceneManager.OnNextFrame();

            // Assert
            Assert.That(_sceneManager.CurrentScene, Is.EqualTo(scene2));
            _sceneLoader.DidNotReceive().Load(sceneFilePath1);
        }

        #endregion

        [Test]
        public void OnNextFrame_ShouldNotLoadSceneAndSetAsCurrent_WhenNeitherLoadEmptySceneNorLoadSceneWasCalledBefore()
        {
            // Arrange
            // Act
            _sceneManager.OnNextFrame();

            // Assert
            Assert.That(_sceneManager.CurrentScene, Is.EqualTo(_initialScene));
        }

        #region Helpers

        private SceneBehavior SetUpSceneBehavior(string behaviorName, Scene scene)
        {
            var sceneBehavior = Substitute.ForPartsOf<SceneBehavior>(scene);
            sceneBehavior.Name.Returns(behaviorName);

            var sceneBehaviorFactory = Substitute.For<ISceneBehaviorFactory>();
            sceneBehaviorFactory.BehaviorName.Returns(behaviorName);
            sceneBehaviorFactory.Create(scene).Returns(sceneBehavior);

            _sceneBehaviorFactoryProvider.Configure().Get(behaviorName).Returns(sceneBehaviorFactory);

            return sceneBehavior;
        }

        #endregion
    }
}