using System.Linq;
using System.Threading;
using Geisha.Editor.Core;
using Geisha.Editor.Core.Properties;
using Geisha.Editor.SceneEditor.Model;
using Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor;
using Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor.Components;
using Geisha.Editor.SceneEditor.UserInterface.SceneOutline.SceneOutlineItem;
using Geisha.Engine.Core.SceneModel;
using Geisha.TestUtils;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.SceneEditor.UserInterface.SceneOutline.SceneOutlineItem
{
    [TestFixture]
    public class EntityViewModelTests
    {
        private IEventBus _eventBus = null!;
        private IEntityPropertiesEditorViewModelFactory _entityPropertiesEditorViewModelFactory = null!;
        private Scene Scene { get; set; } = null!;

        [SetUp]
        public void SetUp()
        {
            _eventBus = new EventBus();
            _entityPropertiesEditorViewModelFactory = Substitute.For<IEntityPropertiesEditorViewModelFactory>();
            Scene = TestSceneFactory.Create();
        }

        private EntityViewModel CreateEntityViewModel(EntityModel entityModel)
        {
            return new EntityViewModel(entityModel, _eventBus, _entityPropertiesEditorViewModelFactory);
        }

        [Test]
        public void Constructor_ShouldCreateEntityViewModelWithNameAndItems()
        {
            // Arrange
            var rootEntity = Scene.CreateEntity();
            rootEntity.Name = "Root entity";

            var entity1 = rootEntity.CreateChildEntity();
            entity1.Name = "Entity 1";

            var entity2 = rootEntity.CreateChildEntity();
            entity2.Name = "Entity 2";

            var entityModel = new EntityModel(rootEntity);

            // Act
            var entityViewModel = CreateEntityViewModel(entityModel);

            // Assert
            Assert.That(entityViewModel.Name, Is.EqualTo("Root entity"));
            Assert.That(entityViewModel.Items, Has.Count.EqualTo(2));

            var sceneOutlineItemViewModel1 = entityViewModel.Items.First();
            var sceneOutlineItemViewModel2 = entityViewModel.Items.Last();
            Assert.That(sceneOutlineItemViewModel1.Name, Is.EqualTo("Entity 1"));
            Assert.That(sceneOutlineItemViewModel2.Name, Is.EqualTo("Entity 2"));
            Assert.That(sceneOutlineItemViewModel1.Items, Has.Count.Zero);
            Assert.That(sceneOutlineItemViewModel2.Items, Has.Count.Zero);
        }

        [Test]
        public void ContextMenu_AddChildEntity_ShouldAddChildEntityInEntityModelAndUpdateViewModelItems()
        {
            // Arrange
            var entity = Scene.CreateEntity();
            var entityModel = new EntityModel(entity);
            var entityViewModel = CreateEntityViewModel(entityModel);
            var addChildEntityContextMenuItem = entityViewModel.ContextMenuItems.Single(i => i.Name == "Add child entity");

            // Act
            addChildEntityContextMenuItem.Command.Execute(null);

            // Assert
            Assert.That(entityModel.Children, Has.Count.EqualTo(1));
            Assert.That(entityViewModel.Items, Has.Count.EqualTo(1));
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void OnSelected_ShouldSendPropertiesSubjectChangedEventWithEntityPropertiesEditor()
        {
            // Arrange
            var entity = Scene.CreateEntity();
            entity.Name = "Entity";
            var entityModel = new EntityModel(entity);
            var entityViewModel = CreateEntityViewModel(entityModel);

            PropertiesSubjectChangedEvent? @event = null;
            _eventBus.RegisterEventHandler<PropertiesSubjectChangedEvent>(e => @event = e);

            _entityPropertiesEditorViewModelFactory.Create(entityModel)
                .Returns(new EntityPropertiesEditorViewModel(entityModel, Substitute.For<IComponentPropertiesEditorViewModelFactory>()));

            // Act
            entityViewModel.OnSelected();

            // Assert
            Assert.That(@event, Is.Not.Null);
            Assert.That(@event!.ViewModel, Is.Not.Null);
            Assert.That(@event!.ViewModel, Is.TypeOf<EntityPropertiesEditorViewModel>());
            var viewModel = (EntityPropertiesEditorViewModel)@event.ViewModel;
            Assert.That(viewModel.Name, Is.EqualTo("Entity"));
        }

        [Test]
        public void Name_ShouldBeUpdated_WhenEntityModelNameIsChanged()
        {
            // Arrange
            var entity = Scene.CreateEntity();
            entity.Name = "Old name";
            var entityModel = new EntityModel(entity);
            var entityViewModel = CreateEntityViewModel(entityModel);

            // Act
            entityModel.Name = "New name";

            // Assert
            Assert.That(entityViewModel.Name, Is.EqualTo("New name"));
        }
    }
}