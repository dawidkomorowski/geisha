using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering.Backend;
using Geisha.Engine.Rendering.Components;

namespace Geisha.Engine.Rendering.Systems
{
    internal sealed class RenderingState
    {
        private readonly Dictionary<Entity, TrackedEntity> _trackedEntities = new();
        private readonly List<RenderNode> _renderNodes = new();
        private readonly IRenderingContext2D _renderingContext2D;

        public RenderingState(IRenderingContext2D renderingContext2D)
        {
            _renderingContext2D = renderingContext2D;
        }

        public CameraNode? CameraNode { get; private set; }

        public IReadOnlyList<RenderNode> GetRenderNodes() => _renderNodes;

        public void CreateStateFor(Transform2DComponent transform2DComponent)
        {
            var trackedEntity = GetOrCreateTrackedEntity(transform2DComponent.Entity);
            trackedEntity.Transform = transform2DComponent;

            CreateNodes(trackedEntity);
        }

        public void CreateStateFor(Renderer2DComponent renderer2DComponent)
        {
            var trackedEntity = GetOrCreateTrackedEntity(renderer2DComponent.Entity);

            if (trackedEntity.Renderer2DComponent is not null)
            {
                throw new InvalidOperationException("Only single renderer component per entity is supported.");
            }

            trackedEntity.Renderer2DComponent = renderer2DComponent;

            CreateNodes(trackedEntity);
        }

        public void CreateStateFor(CameraComponent cameraComponent)
        {
            var trackedEntity = GetOrCreateTrackedEntity(cameraComponent.Entity);

            if (trackedEntity.Camera is not null)
            {
                throw new InvalidOperationException("Only single camera component per entity is supported.");
            }

            trackedEntity.Camera = cameraComponent;

            CreateNodes(trackedEntity);
        }

        public void RemoveStateFor(Transform2DComponent transform2DComponent)
        {
            var entity = transform2DComponent.Entity;

            var trackedEntity = _trackedEntities[entity];
            trackedEntity.Transform = null;

            if (trackedEntity.ShouldBeRemoved)
            {
                _trackedEntities.Remove(entity);
            }

            RemoveNodes(trackedEntity);
        }

        public void RemoveStateFor(Renderer2DComponent renderer2DComponent)
        {
            var entity = renderer2DComponent.Entity;

            var trackedEntity = _trackedEntities[entity];
            trackedEntity.Renderer2DComponent = null;

            if (trackedEntity.ShouldBeRemoved)
            {
                _trackedEntities.Remove(entity);
            }

            RemoveNodes(trackedEntity);
        }

        public void RemoveStateFor(CameraComponent cameraComponent)
        {
            var entity = cameraComponent.Entity;

            var trackedEntity = _trackedEntities[entity];
            trackedEntity.Camera = null;

            if (trackedEntity.ShouldBeRemoved)
            {
                _trackedEntities.Remove(entity);
            }

            RemoveNodes(trackedEntity);
        }

        private void CreateNodes(TrackedEntity trackedEntity)
        {
            if (trackedEntity.IsRenderNode && trackedEntity.RenderNode is null)
            {
                var renderNode = CreateRenderNode(trackedEntity.Transform, trackedEntity.Renderer2DComponent);
                _renderNodes.Add(renderNode);
                trackedEntity.RenderNode = renderNode;
            }

            if (trackedEntity.IsCameraNode && trackedEntity.CameraNode is null)
            {
                if (CameraNode != null)
                {
                    throw new InvalidOperationException("Only single camera is supported per scene.");
                }

                CameraNode = new CameraNode(trackedEntity.Transform, trackedEntity.Camera);
                trackedEntity.CameraNode = CameraNode;
            }
        }

        // BUG If renderer was removed and then added again it will not work correctly. Maybe the same issue is with camera.
        private void RemoveNodes(TrackedEntity trackedEntity)
        {
            if (!trackedEntity.IsRenderNode && trackedEntity.RenderNode is not null)
            {
                _renderNodes.Remove(trackedEntity.RenderNode);
                trackedEntity.RenderNode.Dispose();
                trackedEntity.RenderNode = null;
            }

            if (!trackedEntity.IsCameraNode && CameraNode?.Entity == trackedEntity.Entity)
            {
                CameraNode = null;
                trackedEntity.CameraNode = null;
            }
        }

        private TrackedEntity GetOrCreateTrackedEntity(Entity entity)
        {
            if (_trackedEntities.TryGetValue(entity, out var trackedEntity))
            {
                return trackedEntity;
            }

            trackedEntity = new TrackedEntity(entity);
            _trackedEntities.Add(entity, trackedEntity);
            return trackedEntity;
        }

        private RenderNode CreateRenderNode(Transform2DComponent transform, Renderer2DComponent renderer2DComponent)
        {
            if (renderer2DComponent is EllipseRendererComponent ellipseRendererComponent)
            {
                return new EllipseNode(transform, ellipseRendererComponent);
            }

            if (renderer2DComponent is RectangleRendererComponent rectangleRendererComponent)
            {
                return new RectangleNode(transform, rectangleRendererComponent);
            }

            if (renderer2DComponent is SpriteRendererComponent spriteRendererComponent)
            {
                return new SpriteNode(transform, spriteRendererComponent);
            }

            if (renderer2DComponent is TextRendererComponent textRendererComponent)
            {
                return new TextNode(transform, textRendererComponent, _renderingContext2D);
            }

            throw new ArgumentException($"Unsupported type of {nameof(Renderer2DComponent)}: {renderer2DComponent.GetType()}.", nameof(renderer2DComponent));
        }

        private sealed class TrackedEntity
        {
            public TrackedEntity(Entity entity)
            {
                Entity = entity;
            }

            public Entity Entity { get; }

            public Transform2DComponent? Transform { get; set; }
            public Renderer2DComponent? Renderer2DComponent { get; set; }
            public CameraComponent? Camera { get; set; }

            public RenderNode? RenderNode { get; set; }
            public CameraNode? CameraNode { get; set; }

            [MemberNotNullWhen(true, nameof(Transform), nameof(Renderer2DComponent))]
            public bool IsRenderNode => Transform != null && Renderer2DComponent != null;

            [MemberNotNullWhen(true, nameof(Transform), nameof(Camera))]
            public bool IsCameraNode => Transform != null && Camera != null;

            public bool ShouldBeRemoved => Transform is null && Renderer2DComponent is null && Camera is null;
        }
    }
}