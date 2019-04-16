﻿using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.SceneModel;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.Core.UnitTests.SceneModel
{
    [TestFixture]
    public class SceneManagerTests
    {
        private IAssetStore _assetStore;
        private ISceneConstructionScriptExecutor _sceneConstructionScriptExecutor;
        private ISceneLoader _sceneLoader;
        private SceneManager _sceneManager;

        [SetUp]
        public void SetUp()
        {
            _assetStore = Substitute.For<IAssetStore>();
            _sceneConstructionScriptExecutor = Substitute.For<ISceneConstructionScriptExecutor>();
            _sceneLoader = Substitute.For<ISceneLoader>();
            _sceneManager = new SceneManager(_assetStore, _sceneConstructionScriptExecutor, _sceneLoader);
        }

        [Test]
        public void LoadScene_ShouldNotLoadSceneAndSetAsCurrent_WhenOnNextFrameWasNotCalledAfter()
        {
            // Arrange
            const string sceneFilePath = "start up scene";
            var scene = new Scene();

            _sceneLoader.Load(sceneFilePath).Returns(scene);

            // Act
            _sceneManager.LoadScene(sceneFilePath);

            // Assert
            Assert.That(_sceneManager.CurrentScene, Is.Not.EqualTo(scene));
        }

        [Test]
        public void LoadScene_And_OnNextFrame_ShouldLoadSceneAndSetAsCurrent_GivenPathToSceneFile()
        {
            // Arrange
            const string sceneFilePath = "start up scene";
            var scene = new Scene();

            _sceneLoader.Load(sceneFilePath).Returns(scene);

            // Act
            _sceneManager.LoadScene(sceneFilePath);
            _sceneManager.OnNextFrame();

            // Assert
            Assert.That(_sceneManager.CurrentScene, Is.EqualTo(scene));
        }

        [Test]
        public void LoadScene_And_OnNextFrame_ShouldExecuteConstructionScriptForLoadedScene()
        {
            // Arrange
            const string sceneFilePath = "start up scene";
            var scene = new Scene();

            _sceneLoader.Load(sceneFilePath).Returns(scene);

            // Act
            _sceneManager.LoadScene(sceneFilePath);
            _sceneManager.OnNextFrame();

            // Assert
            _sceneConstructionScriptExecutor.Received().Execute(scene);
        }

        [Test]
        public void LoadScene_And_OnNextFrame_ShouldUnloadAssets_GivenUnloadAssetsSceneLoadMode()
        {
            // Arrange
            const string sceneFilePath = "start up scene";
            var scene = new Scene();

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
            var scene = new Scene();

            _sceneLoader.Load(sceneFilePath).Returns(scene);

            // Act
            _sceneManager.LoadScene(sceneFilePath, SceneLoadMode.PreserveAssets);
            _sceneManager.OnNextFrame();

            // Assert
            _assetStore.DidNotReceive().UnloadAssets();
        }

        [Test]
        public void OnNextFrame_ShouldNotLoadSceneAndSetAsCurrent_WhenLoadSceneWasCalledNeverBefore()
        {
            // Arrange
            const string sceneFilePath = "start up scene";
            var scene = new Scene();

            _sceneLoader.Load(sceneFilePath).Returns(scene);

            // Act
            _sceneManager.OnNextFrame();

            // Assert
            Assert.That(_sceneManager.CurrentScene, Is.Null);
        }

        [Test]
        public void OnNextFrame_HandlesLoadSceneRequestOnlyOnce()
        {
            // Arrange
            const string sceneFilePath = "start up scene";
            var scene = new Scene();

            _sceneLoader.Load(sceneFilePath).Returns(scene);

            _sceneManager.LoadScene(sceneFilePath);
            _sceneManager.OnNextFrame();

            Assume.That(_sceneManager.CurrentScene, Is.EqualTo(scene));

            // Act
            _sceneManager.OnNextFrame();

            // Assert
            Assert.That(_sceneManager.CurrentScene, Is.EqualTo(scene));
            _sceneConstructionScriptExecutor.Received(1).Execute(scene);
        }

        [Test]
        public void OnNextFrame_HandlesLatestLoadSceneRequestOnly()
        {
            // Arrange
            const string sceneFilePath = "start up scene";
            var scene = new Scene();

            _sceneLoader.Load(sceneFilePath).Returns(scene);

            _sceneManager.LoadScene("some other scene");
            _sceneManager.LoadScene(sceneFilePath);

            // Act
            _sceneManager.OnNextFrame();

            // Assert
            Assert.That(_sceneManager.CurrentScene, Is.EqualTo(scene));
            _sceneConstructionScriptExecutor.Received(1).Execute(scene);
        }
    }
}