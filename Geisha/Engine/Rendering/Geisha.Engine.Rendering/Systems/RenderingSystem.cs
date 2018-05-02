using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Geisha.Common.Logging;
using Geisha.Common.Math;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Configuration;
using Geisha.Engine.Core.Diagnostics;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.Systems;
using Geisha.Engine.Rendering.Components;
using Geisha.Engine.Rendering.Configuration;
using Geisha.Framework.Rendering;

namespace Geisha.Engine.Rendering.Systems
{
    [Export(typeof(IVariableTimeStepSystem))]
    internal class RenderingSystem : IVariableTimeStepSystem
    {
        private static readonly ILog Log = LogFactory.Create(typeof(RenderingSystem));
        private readonly IAggregatedDiagnosticsInfoProvider _aggregatedDiagnosticsInfoProvider;
        private readonly IRenderer2D _renderer2D;

        /// <summary>
        ///     Dictionary of entities buffers per sorting layer name. Pre-initialized in constructor and sorted by sorting layers.
        /// </summary>
        private readonly Dictionary<string, List<Entity>> _sortingLayersBuffers;

        [ImportingConstructor]
        public RenderingSystem(IRenderer2D renderer2D, IConfigurationManager configurationManager,
            IAggregatedDiagnosticsInfoProvider aggregatedDiagnosticsInfoProvider)
        {
            _renderer2D = renderer2D;
            _aggregatedDiagnosticsInfoProvider = aggregatedDiagnosticsInfoProvider;

            var sortingLayersOrder = configurationManager.GetConfiguration<RenderingConfiguration>().SortingLayersOrder;
            _sortingLayersBuffers = CreateSortingLayersBuffers(sortingLayersOrder);
        }

        public int Priority { get; set; } = 3;

        public void Update(Scene scene, double deltaTime)
        {
            _renderer2D.Clear();

            if (TryGetCameraTransformationMatrix(scene, out var cameraTransformationMatrix))
            {
                UpdateSortingLayersBuffers(scene);

                foreach (var buffer in _sortingLayersBuffers.Values)
                {
                    foreach (var entity in buffer)
                    {
                        var transformationMatrix = entity.GetComponent<Transform>().Create2DTransformationMatrix();
                        transformationMatrix = cameraTransformationMatrix * transformationMatrix;

                        if (entity.HasComponent<SpriteRenderer>())
                        {
                            var sprite = entity.GetComponent<SpriteRenderer>().Sprite;
                            _renderer2D.RenderSprite(sprite, transformationMatrix);
                        }

                        if (entity.HasComponent<TextRenderer>())
                        {
                            var textRenderer = entity.GetComponent<TextRenderer>();
                            _renderer2D.RenderText(textRenderer.Text, textRenderer.FontSize, textRenderer.Color, transformationMatrix);
                        }
                    }
                }
            }

            RenderDiagnosticsInfo();
        }

        private static Dictionary<string, List<Entity>> CreateSortingLayersBuffers(IEnumerable<string> sortingLayersNames)
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
                    var renderer = entity.GetComponent<RendererBase>();
                    if (renderer.Visible)
                        _sortingLayersBuffers[renderer.SortingLayerName].Add(entity);
                }
            }

            foreach (var buffer in _sortingLayersBuffers.Values)
            {
                buffer.Sort((entity1, entity2) =>
                {
                    var r1 = entity1.GetComponent<RendererBase>();
                    var r2 = entity2.GetComponent<RendererBase>();

                    return r1.OrderInLayer - r2.OrderInLayer;
                });
            }
        }

        private static bool TryGetCameraTransformationMatrix(Scene scene, out Matrix3 cameraTransformationMatrix)
        {
            cameraTransformationMatrix = Matrix3.Identity;
            var cameraEntity = scene.AllEntities.SingleOrDefault(e => e.HasComponent<Camera>() && e.HasComponent<Transform>());
            if (cameraEntity == null)
            {
                Log.Warn("No camera component found in scene.");
                return false;
            }

            var camera = cameraEntity.GetComponent<Camera>();
            var cameraTransform = cameraEntity.GetComponent<Transform>();
            var cameraScale = cameraTransform.Scale.ToVector2();
            cameraTransformationMatrix = Matrix3.Scale(new Vector2(1 / cameraScale.X, 1 / cameraScale.Y)) * Matrix3.Rotation(-cameraTransform.Rotation.Z) *
                                         Matrix3.Translation(-cameraTransform.Translation.ToVector2()) * Matrix3.Identity;

            return true;
        }

        private void RenderDiagnosticsInfo()
        {
            var width = _renderer2D.RenderingContext.RenderTargetWidth;
            var height = _renderer2D.RenderingContext.RenderTargetHeight;
            var color = Color.FromArgb(255, 0, 255, 0);
            var transform = new Transform
            {
                Translation = new Vector3(-(width / 2) + 1, height / 2 - 1, 0),
                Rotation = Vector3.Zero,
                Scale = Vector3.One
            };

            foreach (var diagnosticsInfo in _aggregatedDiagnosticsInfoProvider.GetDiagnosticsInfo())
            {
                _renderer2D.RenderText(diagnosticsInfo.ToString(), 12, color, transform.Create2DTransformationMatrix());
                transform.Translation = transform.Translation - new Vector3(0, 13, 0);
            }
        }
    }
}