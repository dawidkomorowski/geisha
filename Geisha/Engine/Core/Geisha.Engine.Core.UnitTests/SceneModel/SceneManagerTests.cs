using Geisha.Engine.Core.SceneModel;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.Core.UnitTests.SceneModel
{
    [TestFixture]
    public class SceneManagerTests
    {
        private ISceneLoader _sceneLoader;
        private ISceneConstructionScriptExecutor _sceneConstructionScriptExecutor;
        private SceneManager _sceneManager;

        [SetUp]
        public void SetUp()
        {
            _sceneLoader = Substitute.For<ISceneLoader>();
            _sceneConstructionScriptExecutor = Substitute.For<ISceneConstructionScriptExecutor>();
            _sceneManager = new SceneManager(_sceneLoader, _sceneConstructionScriptExecutor);
        }

        [Test]
        public void LoadScene_ShouldLoadSceneAndSetAsCurrent_GivenPathToSceneFile()
        {
            // Arrange
            const string sceneFilePath = "start up scene";
            var scene = new Scene();

            _sceneLoader.Load(sceneFilePath).Returns(scene);

            // Act
            _sceneManager.LoadScene(sceneFilePath);

            // Assert
            Assert.That(_sceneManager.CurrentScene, Is.EqualTo(scene));
        }

        [Test]
        public void LoadScene_ShouldExecuteConstructionScriptForLoadedScene()
        {
            // Arrange
            const string sceneFilePath = "start up scene";
            var scene = new Scene();

            _sceneLoader.Load(sceneFilePath).Returns(scene);

            // Act
            _sceneManager.LoadScene(sceneFilePath);

            // Assert
            _sceneConstructionScriptExecutor.Received().Execute(scene);
        }
    }
}