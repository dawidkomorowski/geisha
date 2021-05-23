using System.Linq;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.TestUtils;
using NSubstitute;

namespace Geisha.Engine.UnitTests.Core.SceneModel.Serialization
{
    public abstract class ComponentSerializerTestsBase
    {
        protected abstract IComponentFactory ComponentFactory { get; }
        protected abstract IComponentSerializer ComponentSerializer { get; }

        protected TComponent SerializeAndDeserialize<TComponent>(TComponent component) where TComponent : Component
        {
            var sceneSerializer = CreateSerializer(component.ComponentId);

            var sceneToSerialize = TestSceneFactory.Create();
            var entity = new Entity();
            entity.AddComponent(component);
            sceneToSerialize.AddEntity(entity);

            var json = sceneSerializer.Serialize(sceneToSerialize);
            var deserializedScene = sceneSerializer.Deserialize(json);

            return deserializedScene.RootEntities.Single().GetComponent<TComponent>();
        }

        private ISceneSerializer CreateSerializer(ComponentId componentId)
        {
            var sceneFactory = Substitute.For<ISceneFactory>();
            sceneFactory.Create().Returns(ci => TestSceneFactory.Create());

            var sceneBehaviorFactoryProvider = Substitute.For<ISceneBehaviorFactoryProvider>();
            var emptySceneBehaviorFactory = Substitute.For<ISceneBehaviorFactory>();
            emptySceneBehaviorFactory.BehaviorName.Returns(string.Empty);
            emptySceneBehaviorFactory.Create(Arg.Any<Scene>())
                .Returns(ci => SceneBehavior.CreateEmpty(ci.Arg<Scene>()));
            sceneBehaviorFactoryProvider.Get(string.Empty).Returns(emptySceneBehaviorFactory);

            var componentFactoryProvider = Substitute.For<IComponentFactoryProvider>();
            componentFactoryProvider.Get(componentId).Returns(ComponentFactory);

            var componentSerializerProvider = Substitute.For<IComponentSerializerProvider>();
            componentSerializerProvider.Get(componentId).Returns(ComponentSerializer);

            return new SceneSerializer(sceneFactory, sceneBehaviorFactoryProvider, componentFactoryProvider, componentSerializerProvider);
        }
    }
}