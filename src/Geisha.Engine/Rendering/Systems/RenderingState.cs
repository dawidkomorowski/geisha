using System;
using System.Collections.Generic;
using System.Diagnostics;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering.Components;

namespace Geisha.Engine.Rendering.Systems
{
    internal sealed class RenderingState
    {
        private readonly Dictionary<Entity, TrackedEntity> _trackedEntities = new Dictionary<Entity, TrackedEntity>();
        private readonly List<RenderNode> _renderNodes = new List<RenderNode>();
        private readonly Dictionary<Entity, RenderNode> _renderNodeIndex = new Dictionary<Entity, RenderNode>();

        public CameraNode? CameraNode { get; private set; }

        public IReadOnlyList<RenderNode> GetRenderNodes() => _renderNodes;

        public void CreateStateFor(Transform2DComponent transform2DComponent)
        {
            var entity = transform2DComponent.Entity;

            if (_trackedEntities.TryGetValue(entity, out var trackedEntity))
            {
                trackedEntity.Transform = transform2DComponent;
            }
            else
            {
                trackedEntity = new TrackedEntity(entity)
                {
                    Transform = transform2DComponent
                };
                _trackedEntities.Add(entity, trackedEntity);
            }

            CreateNodes(trackedEntity);
        }

        public void CreateStateFor(Renderer2DComponent renderer2DComponent)
        {
            var entity = renderer2DComponent.Entity;

            if (_trackedEntities.TryGetValue(entity, out var trackedEntity))
            {
                trackedEntity.Renderer2DComponent = renderer2DComponent;
            }
            else
            {
                trackedEntity = new TrackedEntity(entity)
                {
                    Renderer2DComponent = renderer2DComponent
                };
                _trackedEntities.Add(entity, trackedEntity);
            }

            CreateNodes(trackedEntity);
        }

        public void CreateStateFor(CameraComponent cameraComponent)
        {
            var entity = cameraComponent.Entity;

            if (_trackedEntities.TryGetValue(entity, out var trackedEntity))
            {
                trackedEntity.Camera = cameraComponent;
            }
            else
            {
                trackedEntity = new TrackedEntity(entity)
                {
                    Camera = cameraComponent
                };
                _trackedEntities.Add(entity, trackedEntity);
            }

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
            if (trackedEntity.IsRenderNode)
            {
                // TODO Such asserts could be replaced by MemberNotNullWhenAttribute in .NET 5
                Debug.Assert(trackedEntity.Transform != null, "trackedEntity.Transform != null");
                Debug.Assert(trackedEntity.Renderer2DComponent != null, "trackedEntity.Renderer2DComponent != null");
                var renderNode = new RenderNode(trackedEntity.Transform, trackedEntity.Renderer2DComponent);
                _renderNodes.Add(renderNode);
                _renderNodeIndex.Add(trackedEntity.Entity, renderNode);
            }

            if (trackedEntity.IsCameraNode)
            {
                if (CameraNode != null)
                {
                    throw new InvalidOperationException("Only single camera is supported per scene.");
                }

                Debug.Assert(trackedEntity.Transform != null, "trackedEntity.Transform != null");
                Debug.Assert(trackedEntity.Camera != null, "trackedEntity.Camera != null");
                CameraNode = new CameraNode(trackedEntity.Transform, trackedEntity.Camera);
            }
        }

        private void RemoveNodes(TrackedEntity trackedEntity)
        {
            if (!trackedEntity.IsRenderNode && _renderNodeIndex.TryGetValue(trackedEntity.Entity, out var renderNode))
            {
                _renderNodeIndex.Remove(trackedEntity.Entity);
                _renderNodes.Remove(renderNode);
            }

            if (!trackedEntity.IsCameraNode && CameraNode?.Entity == trackedEntity.Entity)
            {
                CameraNode = null;
            }
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

            public bool IsRenderNode => Transform != null && Renderer2DComponent != null;
            public bool IsCameraNode => Transform != null && Camera != null;
            public bool ShouldBeRemoved => Transform is null && Renderer2DComponent is null && Camera is null;
        }
    }
}