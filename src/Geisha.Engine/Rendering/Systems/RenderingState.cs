using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering.Backend;
using Geisha.Engine.Rendering.Components;

namespace Geisha.Engine.Rendering.Systems;

internal sealed class RenderingState
{
    private readonly Dictionary<Entity, TrackedEntity> _trackedEntities = new();
    private readonly List<SortingLayer> _sortingLayers = new();
    private readonly IRenderingContext2D _renderingContext2D;

    public RenderingState(IRenderingContext2D renderingContext2D, RenderingConfiguration renderingConfiguration)
    {
        _renderingContext2D = renderingContext2D;

        foreach (var sortingLayerName in renderingConfiguration.SortingLayersOrder)
        {
            _sortingLayers.Add(new SortingLayer(sortingLayerName));
        }
    }

    public CameraNode? CameraNode { get; private set; }

    public ReadOnlySpan<SortingLayer> GetSortingLayersSpan() => CollectionsMarshal.AsSpan(_sortingLayers);

    public void CreateStateFor(Transform2DComponent transform2DComponent)
    {
        var trackedEntity = GetOrCreateTrackedEntity(transform2DComponent.Entity);

        if (trackedEntity.Transform is not null)
        {
            throw new InvalidOperationException("Only single transform component per entity is supported.");
        }

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
        var trackedEntity = _trackedEntities[transform2DComponent.Entity];
        trackedEntity.Transform = null;

        RemoveNodes(trackedEntity);
        RemoveTrackedEntityIfNoLongerNeeded(trackedEntity);
    }

    public void RemoveStateFor(Renderer2DComponent renderer2DComponent)
    {
        var trackedEntity = _trackedEntities[renderer2DComponent.Entity];
        trackedEntity.Renderer2DComponent = null;

        RemoveNodes(trackedEntity);
        RemoveTrackedEntityIfNoLongerNeeded(trackedEntity);
    }

    public void RemoveStateFor(CameraComponent cameraComponent)
    {
        var trackedEntity = _trackedEntities[cameraComponent.Entity];
        trackedEntity.Camera = null;

        RemoveNodes(trackedEntity);
        RemoveTrackedEntityIfNoLongerNeeded(trackedEntity);
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

    private void RemoveTrackedEntityIfNoLongerNeeded(TrackedEntity trackedEntity)
    {
        if (trackedEntity.ShouldBeRemoved)
        {
            _trackedEntities.Remove(trackedEntity.Entity);
        }
    }

    private void CreateNodes(TrackedEntity trackedEntity)
    {
        if (trackedEntity.IsRenderNode && trackedEntity.RenderNode is null)
        {
            var renderNode = CreateRenderNode(trackedEntity.Transform, trackedEntity.Renderer2DComponent);
            AddNodeToLayer(renderNode, renderNode.SortingLayerName);
            renderNode.SortingLayerNameChangedCallback = UpdateNodeLayer;
            trackedEntity.RenderNode = renderNode;
        }

        if (trackedEntity.IsCameraNode && trackedEntity.CameraNode is null)
        {
            if (CameraNode != null)
            {
                throw new InvalidOperationException("Only single camera is supported per scene.");
            }

            CameraNode = CreateCameraNode(trackedEntity.Transform, trackedEntity.Camera);
            trackedEntity.CameraNode = CameraNode;
        }
    }

    private void RemoveNodes(TrackedEntity trackedEntity)
    {
        if (!trackedEntity.IsRenderNode && trackedEntity.RenderNode is not null)
        {
            RemoveNodeFromLayer(trackedEntity.RenderNode, trackedEntity.RenderNode.SortingLayerName);
            trackedEntity.RenderNode.Dispose();
            trackedEntity.RenderNode = null;
        }

        if (!trackedEntity.IsCameraNode && CameraNode?.Entity == trackedEntity.Entity)
        {
            CameraNode?.Dispose();
            CameraNode = null;
            trackedEntity.CameraNode = null;
        }
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

    private CameraNode CreateCameraNode(Transform2DComponent transform, CameraComponent cameraComponent)
    {
        var cameraNode = new CameraNode(transform, cameraComponent)
        {
            ScreenWidth = _renderingContext2D.ScreenSize.Width,
            ScreenHeight = _renderingContext2D.ScreenSize.Height
        };
        return cameraNode;
    }

    private void AddNodeToLayer(RenderNode renderNode, string sortingLayerName)
    {
        if (string.IsNullOrEmpty(sortingLayerName)) return;

        foreach (var sortingLayer in _sortingLayers)
        {
            if (sortingLayer.Name == sortingLayerName)
            {
                sortingLayer.Add(renderNode);
                return;
            }
        }

        throw new ArgumentException("Sorting layer could not be found.", nameof(sortingLayerName));
    }

    private void RemoveNodeFromLayer(RenderNode renderNode, string sortingLayerName)
    {
        if (string.IsNullOrEmpty(sortingLayerName)) return;

        foreach (var sortingLayer in _sortingLayers)
        {
            if (sortingLayer.Name == sortingLayerName)
            {
                sortingLayer.Remove(renderNode);
                return;
            }
        }
    }

    private void UpdateNodeLayer(RenderNode renderNode, string newLayerName, string oldLayerName)
    {
        RemoveNodeFromLayer(renderNode, oldLayerName);
        AddNodeToLayer(renderNode, newLayerName);
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
        public bool IsRenderNode => Transform is not null && Renderer2DComponent is not null;

        [MemberNotNullWhen(true, nameof(Transform), nameof(Camera))]
        public bool IsCameraNode => Transform is not null && Camera is not null;

        public bool ShouldBeRemoved => Transform is null && Renderer2DComponent is null && Camera is null;
    }
}