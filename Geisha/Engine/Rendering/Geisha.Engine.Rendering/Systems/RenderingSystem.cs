using System.ComponentModel.Composition;
using System.Linq;
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

        public int Priority { get; set; } = 2;
        public UpdateMode UpdateMode { get; set; } = UpdateMode.Variable;


        [ImportingConstructor]
        public RenderingSystem(IRenderer2D renderer2D)
        {
            _renderer2D = renderer2D;
        }

        public void Update(Scene scene, double deltaTime)
        {
            _renderer2D.Clear();

            foreach (var entity in scene.RootEntity.GetChildrenRecursivelyIncludingRoot().ToList())
            {
                if (entity.HasComponent<SpriteRenderer>() && entity.HasComponent<Transform>())
                {
                    var sprite = entity.GetComponent<SpriteRenderer>().Sprite;
                    var transform = entity.GetComponent<Transform>().Create2DTransformationMatrix();

                    _renderer2D.Render(sprite, transform);
                }
            }
        }

        public void FixedUpdate(Scene scene)
        {
            Update(scene, 0);
        }
    }
}