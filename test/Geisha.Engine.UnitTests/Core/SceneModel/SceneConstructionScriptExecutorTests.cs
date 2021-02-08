using System;
using System.Linq;
using Geisha.Engine.Core.SceneModel;
using Geisha.TestUtils;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.SceneModel
{
    [TestFixture]
    public class SceneConstructionScriptExecutorTests
    {
        [Test]
        public void Execute_ShouldExecuteConstructionScript_WhenThereIsSingleMatchingConstructionScript()
        {
            // Arrange
            var constructionScriptName = Guid.NewGuid().ToString();
            var constructionScript = Substitute.For<ISceneConstructionScript>();
            constructionScript.Name.Returns(constructionScriptName);

            var executor = new SceneConstructionScriptExecutor(new[] {constructionScript});

            var scene = TestSceneFactory.Create();
            scene.ConstructionScript = constructionScriptName;

            // Act
            executor.Execute(scene);

            // Assert
            constructionScript.Received().Execute(scene);
        }

        [Test]
        public void Execute_ShouldThrowException_WhenThereIsNoMatchingConstructionScript()
        {
            // Arrange
            var constructionScriptName = Guid.NewGuid().ToString();
            var constructionScript = Substitute.For<ISceneConstructionScript>();
            constructionScript.Name.Returns(constructionScriptName);

            var executor = new SceneConstructionScriptExecutor(new[] {constructionScript});

            var scene = TestSceneFactory.Create();
            scene.ConstructionScript = "Not existing construction script";

            // Act
            // Assert
            Assert.That(() => { executor.Execute(scene); },
                Throws.InvalidOperationException.With.Message.Contains($"There must be exactly one {nameof(ISceneConstructionScript)}"));
        }

        [Test]
        public void Execute_ShouldThrowException_WhenThereAreMultipleMatchingConstructionScripts()
        {
            // Arrange
            var constructionScriptName = Guid.NewGuid().ToString();
            var constructionScript1 = Substitute.For<ISceneConstructionScript>();
            constructionScript1.Name.Returns(constructionScriptName);
            var constructionScript2 = Substitute.For<ISceneConstructionScript>();
            constructionScript2.Name.Returns(constructionScriptName);

            var executor = new SceneConstructionScriptExecutor(new[] {constructionScript1, constructionScript2});

            var scene = TestSceneFactory.Create();
            scene.ConstructionScript = constructionScriptName;

            // Act
            // Assert
            Assert.That(() => { executor.Execute(scene); },
                Throws.InvalidOperationException.With.Message.Contains($"There must be exactly one {nameof(ISceneConstructionScript)}"));
        }

        [Test]
        public void Execute_ShouldNotExecuteConstructionScript_WhenConstructionScriptIsEmpty()
        {
            // Arrange
            var executor = new SceneConstructionScriptExecutor(Enumerable.Empty<ISceneConstructionScript>());

            var scene = TestSceneFactory.Create();
            scene.ConstructionScript = string.Empty;

            // Act
            // Assert
            Assert.That(() => { executor.Execute(scene); }, Throws.Nothing);
        }
    }
}