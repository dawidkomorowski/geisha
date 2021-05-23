using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.SceneModel.Serialization;

namespace Geisha.Engine.Core.SceneModel
{
    /// <summary>
    ///     Base class of all components to be supported by engine.
    /// </summary>
    public abstract class Component
    {
        protected Component()
        {
            ComponentId = ComponentId.Of(GetType());
        }

        public ComponentId ComponentId { get; }

        protected internal virtual void Serialize(IComponentDataWriter componentDataWriter, IAssetStore assetStore)
        {
        }

        protected internal virtual void Deserialize(IComponentDataReader componentDataReader, IAssetStore assetStore)
        {
        }
    }
}