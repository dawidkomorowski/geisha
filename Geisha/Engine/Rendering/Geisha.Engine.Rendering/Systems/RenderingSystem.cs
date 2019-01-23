using System.Collections.Generic;
using System.Linq;
using Geisha.Common.Logging;
using Geisha.Common.Math;
using Geisha.Engine.Core;
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
    internal class RenderingSystem : IVariableTimeStepSystem
    {
        private static readonly ILog Log = LogFactory.Create(typeof(RenderingSystem));
        private readonly IAggregatedDiagnosticInfoProvider _aggregatedDiagnosticInfoProvider;
        private readonly IRenderer2D _renderer2D;

        /// <summary>
        ///     Dictionary of entities buffers per sorting layer name. Pre-initialized in constructor and sorted by sorting layers.
        /// </summary>
        private readonly Dictionary<string, List<Entity>> _sortingLayersBuffers;

        public RenderingSystem(IRenderer2D renderer2D, IConfigurationManager configurationManager,
            IAggregatedDiagnosticInfoProvider aggregatedDiagnosticInfoProvider)
        {
            _renderer2D = renderer2D;
            _aggregatedDiagnosticInfoProvider = aggregatedDiagnosticInfoProvider;

            var sortingLayersOrder = configurationManager.GetConfiguration<RenderingConfiguration>().SortingLayersOrder;
            _sortingLayersBuffers = CreateSortingLayersBuffers(sortingLayersOrder);
        }

        public string Name => GetType().FullName;

        public void Update(Scene scene, GameTime gameTime)
        {
            _renderer2D.BeginRendering();

            _renderer2D.Clear(Color.FromArgb(255, 255, 255, 255));

            if (TryGetCameraTransformationMatrix(scene, out var cameraTransformationMatrix))
            {
                UpdateSortingLayersBuffers(scene);


                foreach (var buffer in _sortingLayersBuffers.Values)
                {
                    foreach (var entity in buffer)
                    {
                        var transformationMatrix = entity.GetComponent<TransformComponent>().Create2DTransformationMatrix();
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

            RenderDiagnosticInfo();

            _renderer2D.EndRendering();
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
                if (entity.HasComponent<Renderer2DComponent>() && entity.HasComponent<TransformComponent>())
                {
                    var renderer = entity.GetComponent<Renderer2DComponent>();
                    if (renderer.Visible)
                        _sortingLayersBuffers[renderer.SortingLayerName].Add(entity);
                }
            }

            foreach (var buffer in _sortingLayersBuffers.Values)
            {
                buffer.Sort((entity1, entity2) =>
                {
                    var r1 = entity1.GetComponent<Renderer2DComponent>();
                    var r2 = entity2.GetComponent<Renderer2DComponent>();

                    return r1.OrderInLayer - r2.OrderInLayer;
                });
            }
        }

        // TODO It is inefficient to traverse all entities to find a camera each time.
        private static bool TryGetCameraTransformationMatrix(Scene scene, out Matrix3 cameraTransformationMatrix)
        {
            cameraTransformationMatrix = Matrix3.Identity;
            var cameraEntity = scene.AllEntities.SingleOrDefault(e => e.HasComponent<CameraComponent>() && e.HasComponent<TransformComponent>());
            if (cameraEntity == null)
            {
                Log.Warn("No camera component found in scene.");
                return false;
            }

            var camera = cameraEntity.GetComponent<CameraComponent>();
            var cameraTransform = cameraEntity.GetComponent<TransformComponent>();
            var cameraScale = cameraTransform.Scale.ToVector2();
            cameraTransformationMatrix = Matrix3.Scale(new Vector2(1 / cameraScale.X, 1 / cameraScale.Y)) * Matrix3.Rotation(-cameraTransform.Rotation.Z) *
                                         Matrix3.Translation(-cameraTransform.Translation.ToVector2()) * Matrix3.Identity;

            return true;
        }

        private void RenderDiagnosticInfo()
        {
            var width = _renderer2D.Window.ClientAreaWidth;
            var height = _renderer2D.Window.ClientAreaHeight;
            var color = Color.FromArgb(255, 0, 255, 0);
            var transform = new TransformComponent
            {
                Translation = new Vector3(-(width / 2) + 1, height / 2 - 1, 0),
                Rotation = Vector3.Zero,
                Scale = Vector3.One
            };

            foreach (var diagnosticInfo in _aggregatedDiagnosticInfoProvider.GetAllDiagnosticInfo())
            {
                _renderer2D.RenderText(diagnosticInfo.ToString(), FontSize.FromDips(14), color, transform.Create2DTransformationMatrix());
                transform.Translation = transform.Translation - new Vector3(0, 14, 0);
            }
        }
    }
}