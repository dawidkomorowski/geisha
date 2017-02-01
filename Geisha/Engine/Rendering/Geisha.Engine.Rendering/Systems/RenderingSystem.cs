using System.ComponentModel.Composition;
using System.Linq;
using Geisha.Common.Geometry;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.Systems;
using Geisha.Engine.Rendering.Components;
using Geisha.Framework.Rendering;

namespace Geisha.Engine.Rendering.Systems
{
    [Export(typeof(ISystem))]
    public class RenderingSystem : ISystem
    {
        private readonly IRenderer2D _renderer2D;

        public int Priority { get; set; }
        public Scene Scene { get; set; }

        [ImportingConstructor]
        public RenderingSystem(IRenderer2D renderer2D)
        {
            _renderer2D = renderer2D;
        }

        public void Update(double deltaTime)
        {
            _renderer2D.Clear();

            foreach (var entity in Scene.RootEntity.GetChildrenRecursivelyIncludingRoot().ToList())
            {
                if (entity.HasComponent<SpriteRenderer>() && entity.HasComponent<Transform>())
                {
                    var sprite = entity.GetComponent<SpriteRenderer>().Sprite;
                    var position = entity.GetComponent<Transform>().Translation.ToVector2();
                    var transform = Matrix3.Identity;

                    _renderer2D.Render(sprite, transform);
                }
            }
        }

        public void FixedUpdate()
        {
            Update(0);
        }
    }
}