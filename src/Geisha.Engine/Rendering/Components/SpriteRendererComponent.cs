using System;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Rendering.Components
{
    /// <summary>
    ///     <see cref="SpriteRendererComponent" /> gives an <see cref="Core.SceneModel.Entity" /> capability of rendering a
    ///     sprite.
    /// </summary>
    public sealed class SpriteRendererComponent : Renderer2DComponent
    {
        public static ComponentId Id { get; } = new ComponentId("Geisha.Engine.Rendering.SpriteRendererComponent");

        public override ComponentId ComponentId => Id;

        /// <summary>
        ///     Sprite to be rendered.
        /// </summary>
        public Sprite? Sprite { get; set; }
    }

    internal sealed class SpriteRendererComponentFactory : IComponentFactory
    {
        public Type ComponentType { get; } = typeof(SpriteRendererComponent);
        public ComponentId ComponentId => SpriteRendererComponent.Id;
        public IComponent Create() => new SpriteRendererComponent();
    }
}