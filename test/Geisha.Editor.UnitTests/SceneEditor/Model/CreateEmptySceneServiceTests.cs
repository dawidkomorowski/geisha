using System.IO;
using Geisha.Editor.ProjectHandling.Model;
using Geisha.Editor.SceneEditor.Model;
using Geisha.Engine.Core.SceneModel;
using Geisha.TestUtils;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.SceneEditor.Model
{
    [TestFixture]
    public class CreateEmptySceneServiceTests
    {
        private ISceneFactory _sceneFactory = null!;
        private ISceneLoader _sceneLoader = null!;
        private CreateEmptySceneService _createEmptySceneService = null!;

        [SetUp]
        public void SetUp()
        {
            _sceneFactory = Substitute.For<ISceneFactory>();
            _sceneFactory.Create().Returns(ci => TestSceneFactory.Create());
            _sceneLoader = Substitute.For<ISceneLoader>();
            _createEmptySceneService = new CreateEmptySceneService(_sceneFactory, _sceneLoader);
        }

        [Test]
        public void CreateEmptyScene_ShouldAddEmptySceneFileToProject()
        {
            // Arrange
            const string sceneName = "SomeSceneName";
            var project = Substitute.For<IProject>();
            const byte sceneData = 128;

            // Define assertions
            _sceneLoader.Save(Arg.Do<Scene>(scene =>
            {
                Assert.That(scene, Is.Not.Null);
                Assert.That(scene.AllEntities, Is.Empty);
                Assert.That(scene.SceneBehavior.Name, Is.Empty);
            }), Arg.Do<Stream>(stream => { stream.WriteByte(sceneData); }));

            project.AddFile(Arg.Any<string>(), Arg.Do<Stream>(stream =>
            {
                Assert.That(stream.Length, Is.EqualTo(1));
                Assert.That(stream.ReadByte(), Is.EqualTo(sceneData));
            })).ReturnsNull();

            // Act
            _createEmptySceneService.CreateEmptyScene(sceneName, project);

            // Assert
            project.Received().AddFile($"{sceneName}{SceneEditorConstants.SceneFileExtension}", Arg.Any<Stream>());
        }

        [Test]
        public void CreateEmptyScene_ShouldAddEmptySceneFileToFolder()
        {
            // Arrange
            const string sceneName = "SomeSceneName";
            var folder = Substitute.For<IProjectFolder>();
            const byte sceneData = 128;

            // Define assertions
            _sceneLoader.Save(Arg.Do<Scene>(scene =>
            {
                Assert.That(scene, Is.Not.Null);
                Assert.That(scene.AllEntities, Is.Empty);
                Assert.That(scene.SceneBehavior.Name, Is.Empty);
            }), Arg.Do<Stream>(stream => { stream.WriteByte(sceneData); }));

            folder.AddFile(Arg.Any<string>(), Arg.Do<Stream>(stream =>
            {
                Assert.That(stream.Length, Is.EqualTo(1));
                Assert.That(stream.ReadByte(), Is.EqualTo(sceneData));
            })).ReturnsNull();

            // Act
            _createEmptySceneService.CreateEmptyScene(sceneName, folder);

            // Assert
            folder.Received().AddFile($"{sceneName}{SceneEditorConstants.SceneFileExtension}", Arg.Any<Stream>());
        }
    }
}