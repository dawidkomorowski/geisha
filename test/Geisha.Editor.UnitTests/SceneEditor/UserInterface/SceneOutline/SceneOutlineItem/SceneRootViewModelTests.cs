using System.Linq;
using Geisha.Editor.Core;
using Geisha.Editor.Core.Properties;
using Geisha.Editor.SceneEditor.Model;
using Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor;
using Geisha.Editor.SceneEditor.UserInterface.SceneOutline.SceneOutlineItem;
using Geisha.Editor.SceneEditor.UserInterface.ScenePropertiesEditor;
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
            // Arrange
            var scene = TestSceneFactory.Create();

            var entity1 = scene.CreateEntity();
            entity1.Name = "Entity 1";

            var entity2 = scene.CreateEntity();
            entity2.Name = "Entity 2";

            var entity3 = scene.CreateEntity();
            entity3.Name = "Entity 3";

            var sceneModel = TestSceneModelFactory.Create(scene);

            // Act
            var sceneRootViewModel = CreateSceneRootViewModel(sceneModel);

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
            var sceneModel = TestSceneModelFactory.Create();
            var sceneRootViewModel = CreateSceneRootViewModel(sceneModel);
            var addEntityContextMenuItem = sceneRootViewModel.ContextMenuItems.Single(i => i.Name == "Add entity");

            // Act
            addEntityContextMenuItem.Command.Execute(null);

            // Assert
            Assert.That(sceneModel.RootEntities, Has.Count.EqualTo(1));
            Assert.That(sceneRootViewModel.Items, Has.Count.EqualTo(1));
        }

        [Test]
        public void OnSelected_ShouldSendPropertiesSubjectChangedEventWithScenePropertiesEditor()
        {
            // Arrange
            var sceneModel = TestSceneModelFactory.Create("Scene behavior name");
            sceneModel.SceneBehavior = sceneModel.AvailableSceneBehaviors.Single();
            var sceneRootViewModel = CreateSceneRootViewModel(sceneModel);

            PropertiesSubjectChangedEvent? @event = null;
            _eventBus.RegisterEventHandler<PropertiesSubjectChangedEvent>(e => @event = e);

            // Act
            sceneRootViewModel.OnSelected();

            // Assert
            Assert.That(@event, Is.Not.Null);
            Assert.That(@event!.ViewModel, Is.Not.Null);
            Assert.That(@event.ViewModel, Is.TypeOf<ScenePropertiesEditorViewModel>());
            var viewModel = (ScenePropertiesEditorViewModel)@event.ViewModel;
            Assert.That(viewModel.SceneBehavior, Is.EqualTo(sceneModel.SceneBehavior));
        }
    }
}