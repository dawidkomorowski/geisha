using Geisha.Engine.Core.Configuration;
using Geisha.Engine.Core.SceneModel;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.Core.UnitTests.SceneModel
{
    [TestFixture]
    public class SceneManagerTests
    {
        private ISceneLoader _sceneLoader;
        private IConfigurationManager _configurationManager;
        private IStartUpTask _startUpTask;

        [SetUp]
        public void SetUp()
        {
            _sceneLoader = Substitute.For<ISceneLoader>();
            _configurationManager = Substitute.For<IConfigurationManager>();
            _startUpTask = Substitute.For<IStartUpTask>();
        }

        [Test]
        public void Constructor_ShouldLoadSceneAndSetAsCurrent_ThatIsConfiguredAsStartUpScene()
        {
            // Arrange
            const string startUpScene = "start up scene";
            var scene = new Scene();

            _configurationManager.GetConfiguration<CoreConfiguration>().Returns(new CoreConfiguration {StartUpScene = startUpScene});
            _sceneLoader.Load(startUpScene).Returns(scene);

            // Act
            var actual = new SceneManager(_sceneLoader, _configurationManager, _startUpTask);

            // Assert
            Assert.That(actual.CurrentScene, Is.EqualTo(scene));
        }

        [Test]
        public void Constructor_ShouldExecuteConstructionScriptForLoadedScene()
        {
            // Arrange
            const string startUpScene = "start up scene";
            var constructionScript = Substitute.For<ISceneConstructionScript>();
            var scene = new Scene {ConstructionScript = constructionScript};

            _configurationManager.GetConfiguration<CoreConfiguration>().Returns(new CoreConfiguration {StartUpScene = startUpScene});
            _sceneLoader.Load(startUpScene).Returns(scene);

            // Act
            var actual = new SceneManager(_sceneLoader, _configurationManager, _startUpTask);

            // Assert
            constructionScript.Received().Execute(scene);
        }
    }
}