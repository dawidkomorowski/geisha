using System.Linq;
using Geisha.Editor.Core;
using Geisha.Editor.SceneEditor.Model;
using Geisha.Editor.SceneEditor.UserInterface.SceneOutline.SceneOutlineItem;
using Geisha.Engine.Core.SceneModel;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.SceneEditor.UserInterface.SceneOutline.SceneOutlineItem
{
    [TestFixture]
    public class SceneRootViewModelTests
    {
        private IEventBus _eventBus;

        [SetUp]
        public void SetUp()
        {
            _eventBus = new EventBus();
        }

        [Test]
        public void Constructor_ShouldCreateSceneRootViewModelWithNameAndItems()
        {
            // Arrange
            var entity1 = new Entity {Name = "Entity 1"};
            var entity2 = new Entity {Name = "Entity 2"};
            var entity3 = new Entity {Name = "Entity 3"};

            var scene = new Scene();
            scene.AddEntity(entity1);
            scene.AddEntity(entity2);
            scene.AddEntity(entity3);

            var sceneModel = new SceneModel(scene);

            // Act
            var sceneRootViewModel = new SceneRootViewModel(sceneModel, _eventBus);

            // Assert
            Assert.That(sceneRootViewModel.Name, Is.EqualTo("Scene"));
            Assert.That(sceneRootViewModel.Items, Has.Count.EqualTo(3));

            var sceneOutlineItemViewModel1 = sceneRootViewModel.Items.ElementAt(0);
            var sceneOutlineItemViewModel2 = sceneRootViewModel.Items.ElementAt(1);
            var sceneOutlineItemViewModel3 = sceneRootViewModel.Items.ElementAt(2);
            Assert.That(sceneOutlineItemViewModel1.Name, Is.EqualTo("Entity 1"));
            Assert.That(sceneOutlineItemViewModel2.Name, Is.EqualTo("Entity 2"));
            Assert.That(sceneOutlineItemViewModel3.Name, Is.EqualTo("Entity 3"));
            Assert.That(sceneOutlineItemViewModel1.Items, Has.Count.Zero);
            Assert.That(sceneOutlineItemViewModel2.Items, Has.Count.Zero);
            Assert.That(sceneOutlineItemViewModel3.Items, Has.Count.Zero);
        }

        [Test]
        public void ContextMenu_AddEntity_ShouldAddEntityInSceneModelAndUpdateViewModelItems()
        {
            // Arrange
            var scene = new Scene();
            var sceneModel = new SceneModel(scene);
            var sceneRootViewModel = new SceneRootViewModel(sceneModel, _eventBus);
            var addEntityContextMenuItem = sceneRootViewModel.ContextMenuItems.Single(i => i.Name == "Add entity");

            // Act
            addEntityContextMenuItem.Command.Execute(null);

            // Assert
            Assert.That(sceneModel.RootEntities, Has.Count.EqualTo(1));
            Assert.That(sceneRootViewModel.Items, Has.Count.EqualTo(1));
        }
    }
}