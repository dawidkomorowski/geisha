using System.Linq;
using System.Threading;
using Geisha.Editor.Core;
using Geisha.Editor.Core.Properties;
using Geisha.Editor.SceneEditor.Model;
using Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor;
using Geisha.Editor.SceneEditor.UserInterface.SceneOutline.SceneOutlineItem;
using Geisha.Editor.SceneEditor.UserInterface.ScenePropertiesEditor;
using Geisha.Engine.Core.SceneModel;
using Geisha.TestUtils;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.SceneEditor.UserInterface.SceneOutline.SceneOutlineItem
{
    [TestFixture]
    public class SceneRootViewModelTests
    {
        private IEventBus _eventBus = null!;
        private IEntityPropertiesEditorViewModelFactory _entityPropertiesEditorViewModelFactory = null!;

        [SetUp]
        public void SetUp()
        {
            _eventBus = new EventBus();
            _entityPropertiesEditorViewModelFactory = Substitute.For<IEntityPropertiesEditorViewModelFactory>();
        }

        private SceneRootViewModel CreateSceneRootViewModel(SceneModel sceneModel)
        {
            return new SceneRootViewModel(sceneModel, _eventBus, _entityPropertiesEditorViewModelFactory);
        }

        [Test]
        public void Constructor_ShouldCreateSceneRootViewModelWithNameAndItems()
        {
            Assert.Fail("TODO");

            // Arrange
            var entity1 = new Entity {Name = "Entity 1"};
            var entity2 = new Entity {Name = "Entity 2"};
            var entity3 = new Entity {Name = "Entity 3"};

            var scene = TestSceneFactory.Create();
            scene.AddEntity(entity1);
            scene.AddEntity(entity2);
            scene.AddEntity(entity3);

            // TODO var sceneModel = new SceneModel(scene);

            // Act
            // TODO var sceneRootViewModel = CreateSceneRootViewModel(sceneModel);

            // Assert
            //Assert.That(sceneRootViewModel.Name, Is.EqualTo("Scene"));
            //Assert.That(sceneRootViewModel.Items, Has.Count.EqualTo(3));

            //var sceneOutlineItemViewModel1 = sceneRootViewModel.Items.ElementAt(0);
            //var sceneOutlineItemViewModel2 = sceneRootViewModel.Items.ElementAt(1);
            //var sceneOutlineItemViewModel3 = sceneRootViewModel.Items.ElementAt(2);
            //Assert.That(sceneOutlineItemViewModel1.Name, Is.EqualTo("Entity 1"));
            //Assert.That(sceneOutlineItemViewModel2.Name, Is.EqualTo("Entity 2"));
            //Assert.That(sceneOutlineItemViewModel3.Name, Is.EqualTo("Entity 3"));
            //Assert.That(sceneOutlineItemViewModel1.Items, Has.Count.Zero);
            //Assert.That(sceneOutlineItemViewModel2.Items, Has.Count.Zero);
            //Assert.That(sceneOutlineItemViewModel3.Items, Has.Count.Zero);
        }

        [Test]
        public void ContextMenu_AddEntity_ShouldAddEntityInSceneModelAndUpdateViewModelItems()
        {
            Assert.Fail("TODO");

            // Arrange
            var scene = TestSceneFactory.Create();
            // TODO var sceneModel = new SceneModel(scene);
            // TODO var sceneRootViewModel = CreateSceneRootViewModel(sceneModel);
            // TODO var addEntityContextMenuItem = sceneRootViewModel.ContextMenuItems.Single(i => i.Name == "Add entity");

            // Act
            // TODO addEntityContextMenuItem.Command.Execute(null);

            // Assert
            // TODO Assert.That(sceneModel.RootEntities, Has.Count.EqualTo(1));
            // TODO Assert.That(sceneRootViewModel.Items, Has.Count.EqualTo(1));
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void OnSelected_ShouldSendPropertiesSubjectChangedEventWithScenePropertiesEditor()
        {
            Assert.Fail("TODO");

            // Arrange
            var scene = TestSceneFactory.CreateWithBehaviorFactoriesFor("Scene behavior name");
            // TODO scene.SceneBehaviorName = "Scene behavior name";
            // TODO var sceneModel = new SceneModel(scene);
            // TODO var sceneRootViewModel = CreateSceneRootViewModel(sceneModel);

            PropertiesSubjectChangedEvent? @event = null;
            _eventBus.RegisterEventHandler<PropertiesSubjectChangedEvent>(e => @event = e);

            // Act
            // TODO sceneRootViewModel.OnSelected();

            // Assert
            Assert.That(@event, Is.Not.Null);
            Assert.That(@event!.ViewModel, Is.Not.Null);
            Assert.That(@event.ViewModel, Is.TypeOf<ScenePropertiesEditorViewModel>());
            var viewModel = (ScenePropertiesEditorViewModel) @event.ViewModel;
            Assert.That(viewModel.SceneBehaviorName, Is.EqualTo("Scene behavior name"));
        }
    }
}