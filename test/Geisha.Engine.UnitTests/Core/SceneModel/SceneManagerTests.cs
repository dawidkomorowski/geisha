﻿using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.SceneModel;
using Geisha.TestUtils;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.SceneModel
{
    [TestFixture]
    public class SceneManagerTests
    {
        private IAssetStore _assetStore = null!;
        private ISceneLoader _sceneLoader = null!;
        private ISceneFactory _sceneFactory = null!;
        private SceneManager _sceneManager = null!;

        [SetUp]
        public void SetUp()
        {
            _assetStore = Substitute.For<IAssetStore>();
            _sceneLoader = Substitute.For<ISceneLoader>();
            _sceneFactory = Substitute.For<ISceneFactory>();
            _sceneManager = new SceneManager(_assetStore, _sceneLoader, _sceneFactory);
        }

        #region LoadEmptyScene

        [Test]
        public void LoadEmptyScene_ShouldNotLoadEmptySceneAndSetAsCurrent_WhenOnNextFrameWasNotCalledAfter()
        {
            // Arrange
            const string sceneBehaviorName = "scene behavior";

            // Act
            _sceneManager.LoadEmptyScene(sceneBehaviorName);

            // Assert
            Assert.That(_sceneManager.CurrentScene, Is.Null);
        }

        [Test]
        public void LoadEmptyScene_And_OnNextFrame_ShouldLoadEmptySceneAndSetAsCurrent_GivenSceneBehaviorName()
        {
            // Arrange
            const string sceneBehaviorName = "scene behavior";
            var scene = TestSceneFactory.CreateWithBehaviorFactoriesFor(sceneBehaviorName);

            _sceneFactory.Create().Returns(scene);

            // Act
            _sceneManager.LoadEmptyScene(sceneBehaviorName);
            _sceneManager.OnNextFrame();

            // Assert
            Assert.That(_sceneManager.CurrentScene, Is.EqualTo(scene));
        }

        [Test]
        public void LoadEmptyScene_And_OnNextFrame_ShouldExecuteOnLoadedOfSceneBehaviorForLoadedScene()
        {
            // Arrange
            const string sceneBehaviorName = "Scene Behavior";

            var sceneBehaviorFactory = Substitute.For<ISceneBehaviorFactory>();
            sceneBehaviorFactory.BehaviorName.Returns(sceneBehaviorName);

            var scene = TestSceneFactory.Create(new[] {sceneBehaviorFactory});

            var sceneBehavior = Substitute.ForPartsOf<SceneBehavior>(scene);
            sceneBehaviorFactory.Create(scene).Returns(sceneBehavior);

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
            const string sceneBehaviorName = "Scene Behavior";
            var scene = TestSceneFactory.CreateWithBehaviorFactoriesFor(sceneBehaviorName);

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
            const string sceneBehaviorName = "Scene Behavior";
            var scene = TestSceneFactory.CreateWithBehaviorFactoriesFor(sceneBehaviorName);

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
            const string sceneBehaviorName = "Scene Behavior";

            var scene1 = TestSceneFactory.CreateWithBehaviorFactoriesFor(sceneBehaviorName);
            _sceneFactory.Create().Returns(scene1);

            _sceneManager.LoadEmptyScene(sceneBehaviorName);
            _sceneManager.OnNextFrame();

            Assume.That(_sceneManager.CurrentScene, Is.EqualTo(scene1));

            var scene2 = TestSceneFactory.CreateWithBehaviorFactoriesFor(sceneBehaviorName);
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

            var scene = TestSceneFactory.CreateWithBehaviorFactoriesFor(sceneBehaviorName1, sceneBehaviorName2);
            _sceneFactory.Create().Returns(scene);

            _sceneManager.LoadEmptyScene(sceneBehaviorName1);
            _sceneManager.LoadEmptyScene(sceneBehaviorName2);

            // Act
            _sceneManager.OnNextFrame();

            // Assert
            Assert.That(_sceneManager.CurrentScene, Is.EqualTo(scene));
            Assert.That(_sceneManager.CurrentScene?.SceneBehaviorName, Is.EqualTo(sceneBehaviorName2));
            _sceneFactory.Received(1).Create();
        }

        #endregion

        #region LoadScene

        [Test]
        public void LoadScene_ShouldNotLoadSceneAndSetAsCurrent_WhenOnNextFrameWasNotCalledAfter()
        {
            // Arrange
            const string sceneFilePath = "start up scene";

            // Act
            _sceneManager.LoadScene(sceneFilePath);

            // Assert
            Assert.That(_sceneManager.CurrentScene, Is.Null);
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
        public void LoadScene_And_OnNextFrame_ShouldExecuteOnLoadedOfSceneBehaviorForLoadedScene()
        {
            // Arrange
            const string sceneFilePath = "start up scene";
            const string sceneBehaviorName = "Scene Behavior";

            var sceneBehaviorFactory = Substitute.For<ISceneBehaviorFactory>();
            sceneBehaviorFactory.BehaviorName.Returns(sceneBehaviorName);

            var scene = TestSceneFactory.Create(new[] {sceneBehaviorFactory});

            var sceneBehavior = Substitute.ForPartsOf<SceneBehavior>(scene);
            sceneBehaviorFactory.Create(scene).Returns(sceneBehavior);

            scene.SceneBehaviorName = sceneBehaviorName;

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
            Assert.That(_sceneManager.CurrentScene, Is.Null);
        }
    }
}