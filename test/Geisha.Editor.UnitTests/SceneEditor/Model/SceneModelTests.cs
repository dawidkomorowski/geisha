using System.Linq;
using Geisha.Editor.SceneEditor.Model;
using Geisha.Engine.Core.SceneModel;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.SceneEditor.Model
{
    [TestFixture]
    public class SceneModelTests
    {
        [Test]
        public void Constructor_ShouldCreateSceneModelWithEntitiesHierarchy()
        {
            // Arrange
            var entity1 = new Entity {Name = "Entity 1"};
            var entity11 = new Entity {Name = "Entity 1.1", Parent = entity1};
            _ = new Entity {Name = "Entity 1.1.1", Parent = entity11};
            _ = new Entity {Name = "Entity 1.2", Parent = entity1};
            var entity2 = new Entity {Name = "Entity 2"};

            var scene = new Scene();
            scene.AddEntity(entity1);
            scene.AddEntity(entity2);

            // Act
            var sceneModel = new SceneModel(scene);

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
    }
}