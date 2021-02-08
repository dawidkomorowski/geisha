using System.Linq;
using Geisha.Engine.Core.SceneModel;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.SceneModel
{
    [TestFixture]
    public class SceneTests
    {
        [Test]
        public void AddEntity_ShouldAddNewRootEntity()
        {
            // Arrange
            var scene = CreateScene();
            var entity = new Entity();

            // Act
            scene.AddEntity(entity);

            // Assert
            Assert.That(scene.RootEntities, Is.EquivalentTo(new[] {entity}));
        }

        [Test]
        public void AddEntity_ShouldSetSceneOnEntity()
        {
            // Arrange
            var scene = CreateScene();
            var entity = new Entity();

            // Act
            scene.AddEntity(entity);

            // Assert
            Assert.That(entity.Scene, Is.EqualTo(scene));
        }

        [Test]
        public void RemoveEntity_ShouldRemoveEntityFromRootEntities()
        {
            // Arrange
            var scene = CreateScene();
            var entity = new Entity();

            scene.AddEntity(entity);

            // Act
            scene.RemoveEntity(entity);

            // Assert
            Assert.That(scene.RootEntities, Is.Empty);
        }

        [Test]
        public void RemoveEntity_ShouldUnsetSceneOnEntity()
        {
            // Arrange
            var scene = CreateScene();
            var entity = new Entity();

            scene.AddEntity(entity);

            // Act
            scene.RemoveEntity(entity);

            // Assert
            Assert.That(entity.Scene, Is.Null);
        }

        [Test]
        public void RemoveEntity_ShouldRemoveChildOfOtherEntity_WhenEntityIsNotRoot()
        {
            // Arrange
            var scene = CreateScene();

            var rootEntity = new Entity();
            var childOfRoot = new Entity {Parent = rootEntity};

            scene.AddEntity(rootEntity);

            // Act
            scene.RemoveEntity(childOfRoot);

            // Assert
            Assert.That(scene.AllEntities.Count(), Is.EqualTo(1));
            Assert.That(scene.AllEntities, Does.Not.Contains(childOfRoot));
            Assert.That(rootEntity.Children, Has.Count.EqualTo(0));
            Assert.That(childOfRoot.Parent, Is.Null);
            Assert.That(childOfRoot.Scene, Is.Null);
        }

        [Test]
        public void AllEntities_ShouldReturnAllEntitiesInWholeSceneGraph()
        {
            // Arrange
            var scene = CreateScene();

            var rootEntity1 = new Entity();
            var rootEntity2 = new Entity();
            var child1OfRoot1 = new Entity {Parent = rootEntity1};
            var child2OfRoot1 = new Entity {Parent = rootEntity1};
            var child1OfRoot2 = new Entity {Parent = rootEntity2};
            var child1OfChild1OfRoot1 = new Entity {Parent = child1OfRoot1};

            scene.AddEntity(rootEntity1);
            scene.AddEntity(rootEntity2);

            // Act
            var allEntities = scene.AllEntities;

            // Assert
            Assert.That(allEntities, Is.EquivalentTo(new[] {rootEntity1, rootEntity2, child1OfRoot1, child2OfRoot1, child1OfRoot2, child1OfChild1OfRoot1}));
        }

        [Test]
        public void SceneBehaviorName_ShouldBeSetToDefault_WhenSceneConstructed()
        {
            // Arrange
            var scene = CreateScene();

            // Act
            var actual = scene.SceneBehaviorName;

            // Assert
            Assert.That(actual, Is.EqualTo(DefaultSceneBehavior.Name));
        }

        [Test]
        public void SceneBehaviorName_ShouldSetSceneBehavior_GivenBehaviorName()
        {
            // Arrange
            var scene = CreateScene();

            var sceneBehavior1 = Substitute.ForPartsOf<SceneBehavior>(scene);
            var sceneBehavior2 = Substitute.ForPartsOf<SceneBehavior>(scene);
            var sceneBehavior3 = Substitute.ForPartsOf<SceneBehavior>(scene);

            var sceneBehaviorFactory1 = Substitute.For<ISceneBehaviorFactory>();
            var sceneBehaviorFactory2 = Substitute.For<ISceneBehaviorFactory>();
            var sceneBehaviorFactory3 = Substitute.For<ISceneBehaviorFactory>();

            sceneBehaviorFactory1.BehaviorName.Returns("Scene Behavior Factory 1");
            sceneBehaviorFactory2.BehaviorName.Returns("Scene Behavior Factory 2");
            sceneBehaviorFactory3.BehaviorName.Returns("Scene Behavior Factory 3");

            sceneBehaviorFactory1.Create(scene).Returns(sceneBehavior1);
            sceneBehaviorFactory2.Create(scene).Returns(sceneBehavior2);
            sceneBehaviorFactory3.Create(scene).Returns(sceneBehavior3);

            var genericSceneBehaviorFactory = new GenericSceneBehaviorFactory(new[] {sceneBehaviorFactory1, sceneBehaviorFactory2, sceneBehaviorFactory3});

            // Act
            var actual = genericSceneBehaviorFactory.Create("Scene Behavior Factory 2", scene);
            scene.SceneBehaviorName = "Scene Behavior Factory 2";
            scene.OnLoaded();

            // Assert
            Assert.That(scene.SceneBehaviorName, Is.EqualTo("Scene Behavior Factory 2"));
            sceneBehavior2.Received(1).OnLoaded();
        }

        private Scene CreateScene()
        {
            return new Scene(Enumerable.Empty<ISceneBehaviorFactory>());
        }
    }
}