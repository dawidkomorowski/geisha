using System;
using System.Diagnostics;
using System.Linq;
using Geisha.Editor.SceneEditor.Model;
using Geisha.Engine.Core.SceneModel;
using Geisha.TestUtils;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.Extensions;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.SceneEditor.Model
{
    [TestFixture]
    public class SceneModelTests
    {
        private ISceneBehaviorFactoryProvider _sceneBehaviorFactoryProvider = null!;

        [SetUp]
        public void SetUp()
        {
            _sceneBehaviorFactoryProvider = Substitute.For<ISceneBehaviorFactoryProvider>();
            _sceneBehaviorFactoryProvider.Get(Arg.Any<string>()).ThrowsForAnyArgs(new InvalidOperationException("Missing substitute configuration."));
        }

        #region Constructor

        [Test]
        public void Constructor_ShouldCreateSceneModelWithEntitiesHierarchy()
        {
            // Arrange
            var scene = TestSceneFactory.Create();

            var entity1 = scene.CreateEntity();
            entity1.Name = "Entity 1";

            var entity11 = entity1.CreateChildEntity();
            entity11.Name = "Entity 1.1";

            var entity111 = entity11.CreateChildEntity();
            entity111.Name = "Entity 1.1.1";

            var entity12 = entity1.CreateChildEntity();
            entity12.Name = "Entity 1.2";

            var entity2 = scene.CreateEntity();
            entity2.Name = "Entity 2";

            // Act
            var sceneModel = CreateSceneModel(scene);

            // Assert
            Assert.That(sceneModel.RootEntities, Has.Count.EqualTo(2));

            var entityModel1 = sceneModel.RootEntities.First();
            var entityModel2 = sceneModel.RootEntities.Last();
            Assert.That(entityModel1.Name, Is.EqualTo("Entity 1"));
            Assert.That(entityModel2.Name, Is.EqualTo("Entity 2"));
            Assert.That(entityModel1.Children, Has.Count.EqualTo(2));
            Assert.That(entityModel2.Children, Has.Count.Zero);

            var entityModel11 = entityModel1.Children.First();
            var entityModel12 = entityModel1.Children.Last();
            Assert.That(entityModel11.Name, Is.EqualTo("Entity 1.1"));
            Assert.That(entityModel12.Name, Is.EqualTo("Entity 1.2"));
            Assert.That(entityModel11.Children, Has.Count.EqualTo(1));
            Assert.That(entityModel12.Children, Has.Count.Zero);

            var entityModel111 = entityModel11.Children.Single();
            Assert.That(entityModel111.Name, Is.EqualTo("Entity 1.1.1"));
            Assert.That(entityModel111.Children, Has.Count.Zero);
        }

        [Test]
        public void Constructor_ShouldCreateSceneModelWithNoAvailableSceneBehaviors()
        {
            // Arrange
            var scene = TestSceneFactory.Create();

            // Act
            var sceneModel = CreateSceneModel(scene);

            // Assert
            Assert.That(sceneModel.AvailableSceneBehaviors, Is.Empty);
        }

        [Test]
        public void Constructor_ShouldCreateSceneModelWithAvailableSceneBehaviors()
        {
            // Arrange
            var scene = TestSceneFactory.Create();

            var sceneBehavior1 = new SceneBehaviorName("Behavior 1");
            var sceneBehavior2 = new SceneBehaviorName("Behavior 2");
            var sceneBehavior3 = new SceneBehaviorName("Behavior 3");

            // Act
            var sceneModel = CreateSceneModel(scene, sceneBehavior1, sceneBehavior2, sceneBehavior3);

            // Assert
            Assert.That(sceneModel.AvailableSceneBehaviors, Has.Count.EqualTo(3));
            Assert.That(sceneModel.AvailableSceneBehaviors.Select(b => b.Value), Is.EquivalentTo(new[] { "Behavior 1", "Behavior 2", "Behavior 3" }));
        }

        [Test]
        public void Constructor_ShouldCreateSceneModelWithSceneBehaviorMatchingSceneBehaviorOfScene()
        {
            // Arrange
            var scene = TestSceneFactory.Create();
            var sceneBehavior = Substitute.ForPartsOf<SceneBehavior>(scene);
            sceneBehavior.Name.Returns("Scene behavior");
            scene.SceneBehavior = sceneBehavior;

            // Act
            var sceneModel = CreateSceneModel(scene);

            // Assert
            Assert.That(sceneModel.SceneBehavior.Value, Is.EquivalentTo("Scene behavior"));
        }

        #endregion

        [Test]
        public void AddEntity_ShouldAddNewRootEntityAndNotifyWithEvent()
        {
            // Arrange
            var scene = TestSceneFactory.Create();
            var sceneModel = CreateSceneModel(scene);

            object? eventSender = null;
            EntityAddedEventArgs? eventArgs = null;
            sceneModel.EntityAdded += (sender, args) =>
            {
                eventSender = sender;
                eventArgs = args;
            };

            // Act
            sceneModel.AddEntity();

            // Assert
            Assert.That(sceneModel.RootEntities, Has.Count.EqualTo(1));
            Assert.That(scene.RootEntities, Has.Count.EqualTo(1));

            var entityModel = sceneModel.RootEntities.Single();
            var entity = scene.RootEntities.Single();
            Assert.That(entityModel.Name, Is.EqualTo("Entity 1"));
            Assert.That(entity.Name, Is.EqualTo("Entity 1"));

            Assert.That(eventSender, Is.EqualTo(sceneModel));
            Debug.Assert(eventArgs != null, nameof(eventArgs) + " != null");
            Assert.That(eventArgs.EntityModel, Is.EqualTo(entityModel));
        }

        [Test]
        public void AddEntity_ShouldAddEntitiesWithIncrementingDefaultNames_WhenSceneInitiallyEmpty()
        {
            // Arrange
            var scene = TestSceneFactory.Create();
            var sceneModel = CreateSceneModel(scene);

            // Act
            sceneModel.AddEntity();
            sceneModel.AddEntity();
            sceneModel.AddEntity();

            // Assert
            Assert.That(sceneModel.RootEntities.Select(e => e.Name), Is.EquivalentTo(new[] { "Entity 1", "Entity 2", "Entity 3" }));
        }

        [Test]
        public void SceneBehavior_ShouldUpdateSceneBehaviorOfTheScene_WhenChanged()
        {
            // Arrange
            const string oldSceneBehaviorName = "Old scene behavior";
            const string newSceneBehaviorName = "New scene behavior";

            var scene = TestSceneFactory.Create();

            var oldSceneBehavior = Substitute.ForPartsOf<SceneBehavior>(scene);
            oldSceneBehavior.Name.Returns(oldSceneBehaviorName);

            var newSceneBehavior = Substitute.ForPartsOf<SceneBehavior>(scene);
            newSceneBehavior.Name.Returns(newSceneBehaviorName);

            var newSceneBehaviorFactory = Substitute.For<ISceneBehaviorFactory>();
            newSceneBehaviorFactory.Create(scene).Returns(newSceneBehavior);

            _sceneBehaviorFactoryProvider.Configure().Get(newSceneBehaviorName).Returns(newSceneBehaviorFactory);

            scene.SceneBehavior = oldSceneBehavior;
            var sceneModel = CreateSceneModel(scene);

            // Act
            sceneModel.SceneBehavior = new SceneBehaviorName(newSceneBehaviorName);

            // Assert
            Assert.That(scene.SceneBehavior, Is.EqualTo(newSceneBehavior));
            Assert.That(sceneModel.SceneBehavior.Value, Is.EqualTo("New scene behavior"));
        }

        #region Helpers

        private SceneModel CreateSceneModel(Scene scene, params SceneBehaviorName[] availableSceneBehaviors)
        {
            return new SceneModel(scene, availableSceneBehaviors, _sceneBehaviorFactoryProvider);
        }

        #endregion
    }
}