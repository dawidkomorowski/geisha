using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering.Components;

namespace Geisha.Engine.Rendering.Systems
{
    internal sealed class RenderingState
    {
        private readonly Dictionary<Entity, TrackedEntity> _trackedEntities = new();
        private readonly List<RenderNode> _renderNodes = new();

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
                var renderNode = new RenderNode(trackedEntity.Transform, trackedEntity.Renderer2DComponent);
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

        private void RemoveNodes(TrackedEntity trackedEntity)
        {
            if (!trackedEntity.IsRenderNode && trackedEntity.RenderNode is not null)
            {
                _renderNodes.Remove(trackedEntity.RenderNode);
            }

            if (!trackedEntity.IsCameraNode && CameraNode?.Entity == trackedEntity.Entity)
            {
                CameraNode = null;
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