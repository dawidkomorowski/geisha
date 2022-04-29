using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Diagnostics;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.Systems;
using Geisha.Engine.Rendering.Backend;
using Geisha.Engine.Rendering.Components;

namespace Geisha.Engine.Rendering.Systems
{
    internal sealed class RenderingSystem : IRenderingSystem, ISceneObserver
    {
        private readonly RenderingState _renderingState = new RenderingState();
        private readonly Renderer _renderer;

        public RenderingSystem(IRenderingBackend renderingBackend, RenderingConfiguration renderingConfiguration,
            IAggregatedDiagnosticInfoProvider aggregatedDiagnosticInfoProvider, IDebugRendererForRenderingSystem debugRendererForRenderingSystem)
        {
            _renderer = new Renderer(renderingBackend.Renderer2D, renderingConfiguration, aggregatedDiagnosticInfoProvider, debugRendererForRenderingSystem,
                _renderingState);
        }

        #region Implementation of IRenderingSystem

        public void RenderScene()
        {
            _renderer.RenderScene();
        }

        #endregion

        #region Implementation of ISceneObserver

        public void OnEntityCreated(Entity entity)
        {
        }

        public void OnEntityRemoved(Entity entity)
        {
        }

        public void OnEntityParentChanged(Entity entity, Entity? oldParent, Entity? newParent)
        {
        }

        public void OnComponentCreated(Component component)
        {
            switch (component)
            {
                case Transform2DComponent transform2DComponent:
                    _renderingState.CreateStateFor(transform2DComponent);
                    break;
                case Renderer2DComponent renderer2DComponent:
                    _renderingState.CreateStateFor(renderer2DComponent);
                    break;
                case CameraComponent cameraComponent:
                    _renderingState.CreateStateFor(cameraComponent);
                    break;
            }
        }

        public void OnComponentRemoved(Component component)
        {
        }

        #endregion
    }
}