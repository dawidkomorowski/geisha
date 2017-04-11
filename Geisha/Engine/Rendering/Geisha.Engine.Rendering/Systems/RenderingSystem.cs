using System.Collections.Generic;
using System.ComponentModel.Composition;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Configuration;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.Systems;
using Geisha.Engine.Rendering.Components;
using Geisha.Engine.Rendering.Configuration;
using Geisha.Framework.Rendering;

namespace Geisha.Engine.Rendering.Systems
{
    [Export(typeof(ISystem))]
    public class RenderingSystem : ISystem
    {
        private readonly IRenderer2D _renderer2D;
        private readonly IConfigurationManager _configurationManager;

        private readonly Dictionary<string, List<Entity>> _sortingLayersBuffers;

        public int Priority { get; set; } = 2;
        public UpdateMode UpdateMode { get; set; } = UpdateMode.Variable;


        [ImportingConstructor]
        public RenderingSystem(IRenderer2D renderer2D, IConfigurationManager configurationManager)
        {
            _renderer2D = renderer2D;
            _configurationManager = configurationManager;

            var sortingLayersOrder = _configurationManager.GetConfiguration<RenderingConfiguration>().SortingLayersOrder;
            _sortingLayersBuffers = CreateSortingLayersBuffers(sortingLayersOrder);
        }

        public void Update(Scene scene, double deltaTime)
        {
            _renderer2D.Clear();

            UpdateSortingLayersBuffers(scene);

            foreach (var buffer in _sortingLayersBuffers.Values)
            {
                foreach (var entity in buffer)
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

        private Dictionary<string, List<Entity>> CreateSortingLayersBuffers(IEnumerable<string> sortingLayersNames)
        {
            var buffers = new Dictionary<string, List<Entity>>();

            foreach (var sortingLayerName in sortingLayersNames)
            {
                buffers[sortingLayerName] = new List<Entity>();
            }

            return buffers;
        }

        private void UpdateSortingLayersBuffers(Scene scene)
        {
            foreach (var buffer in _sortingLayersBuffers.Values)
            {
                buffer.Clear();
            }

            foreach (var entity in scene.AllEntities)
            {
                if (entity.HasComponent<SpriteRenderer>() && entity.HasComponent<Transform>())
                {
                    var spriteRenderer = entity.GetComponent<SpriteRenderer>();
                    _sortingLayersBuffers[spriteRenderer.SortingLayerName].Add(entity);
                }
            }

            foreach (var buffer in _sortingLayersBuffers.Values)
            {
                buffer.Sort((entity1, entity2) =>
                {
                    var sr1 = entity1.GetComponent<SpriteRenderer>();
                    var sr2 = entity2.GetComponent<SpriteRenderer>();

                    return sr1.SortingOrder - sr2.SortingOrder;
                });
            }
        }
    }
}