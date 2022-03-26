using System;
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
        protected virtual IComponentFactory CustomComponentFactory { get; } = new NullComponentFactory();
        protected IAssetStore AssetStore { get; private set; } = null!;

        [SetUp]
        public void SetUp()
        {
            AssetStore = Substitute.For<IAssetStore>();
        }

        protected TComponent SerializeAndDeserialize<TComponent>(Action<TComponent> prepareAction) where TComponent : Component
        {
            var sceneSerializer = CreateSerializer();

            var sceneToSerialize = TestSceneFactory.Create(new[] { CustomComponentFactory });
            var entity = sceneToSerialize.CreateEntity();
            var component = entity.CreateComponent<TComponent>();

            prepareAction(component);

            var json = sceneSerializer.Serialize(sceneToSerialize);
            var deserializedScene = sceneSerializer.Deserialize(json);

            return deserializedScene.RootEntities.Single().GetComponent<TComponent>();
        }

        private ISceneSerializer CreateSerializer()
        {
            var sceneFactory = Substitute.For<ISceneFactory>();
            sceneFactory.Create().Returns(ci => TestSceneFactory.Create(new[] { CustomComponentFactory }));

            var sceneBehaviorFactoryProvider = Substitute.For<ISceneBehaviorFactoryProvider>();
            sceneBehaviorFactoryProvider.Get(string.Empty).Returns(new EmptySceneBehaviorFactory());

            return new SceneSerializer(sceneFactory, sceneBehaviorFactoryProvider, AssetStore);
        }

        private sealed class NullComponent : Component
        {
        }

        private sealed class NullComponentFactory : ComponentFactory<NullComponent>
        {
            protected override NullComponent CreateComponent() => new NullComponent();
        }
    }
}