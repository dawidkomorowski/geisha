﻿using System.Collections.Generic;
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

        /// <summary>
        /// Dictionary of entities buffers per sorting layer name. Pre-initialized in constructor and sorted by sorting layers.
        /// </summary>
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
                    var transform = entity.GetComponent<Transform>().Create2DTransformationMatrix();

                    if (entity.HasComponent<SpriteRenderer>())
                    {
                        var sprite = entity.GetComponent<SpriteRenderer>().Sprite;
                        _renderer2D.RenderSprite(sprite, transform);
                    }

                    if (entity.HasComponent<TextRenderer>())
                    {
                        var textRenderer = entity.GetComponent<TextRenderer>();
                        _renderer2D.RenderText(textRenderer.Text, textRenderer.FontSize, textRenderer.Color, transform);
                    }
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
                if (entity.HasComponent<RendererBase>() && entity.HasComponent<Transform>())
                {
                    var spriteRenderer = entity.GetComponent<RendererBase>();
                    if (spriteRenderer.Visible)
                    {
                        _sortingLayersBuffers[spriteRenderer.SortingLayerName].Add(entity);
                    }
                }
            }

            foreach (var buffer in _sortingLayersBuffers.Values)
            {
                buffer.Sort((entity1, entity2) =>
                {
                    var sr1 = entity1.GetComponent<RendererBase>();
                    var sr2 = entity2.GetComponent<RendererBase>();

                    return sr1.OrderInLayer - sr2.OrderInLayer;
                });
            }
        }
    }
}