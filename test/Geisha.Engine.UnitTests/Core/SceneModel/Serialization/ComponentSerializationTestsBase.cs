using System.Linq;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.TestUtils;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.SceneModel.Serialization
{
    public abstract class ComponentSerializationTestsBase
    {
        protected abstract IComponentFactory ComponentFactory { get; }
        protected IAssetStore AssetStore { get; private set; } = null!;

        [SetUp]
        public void SetUp()
        {
            AssetStore = Substitute.For<IAssetStore>();
        }

        protected TComponent SerializeAndDeserialize<TComponent>(TComponent component) where TComponent : Component
        {
            var sceneSerializer = CreateSerializer(component.ComponentId);

            var sceneToSerialize = TestSceneFactory.Create();
            var entity = sceneToSerialize.CreateEntity();
            entity.AddComponent(component);

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

            return new SceneSerializer(sceneFactory, sceneBehaviorFactoryProvider, componentFactoryProvider, AssetStore);
        }
    }
}