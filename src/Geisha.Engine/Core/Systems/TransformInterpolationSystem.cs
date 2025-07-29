using System.Collections.Generic;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.GameLoop;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.Systems;

internal sealed class TransformInterpolationSystem : ITransformInterpolationGameLoopStep, ISceneObserver
{
    private readonly Dictionary<Transform2DComponent, TransformData> _transforms = new();

    #region Implementation of ITransformInterpolationGameLoopStep

    public void SnapshotTransforms()
    {
        foreach (var transformData in _transforms.Values)
        {
            transformData.PreviousTransform = transformData.CurrentTransform;
            transformData.CurrentTransform = transformData.TransformComponent.Transform;
        }
    }

    public void InterpolateTransforms(double interpolationFactor)
    {
        foreach (var transformData in _transforms.Values)
        {
            var previousTransform = transformData.PreviousTransform;
            var currentTransform = transformData.CurrentTransform;
            transformData.InterpolatedTransform = Transform2D.Lerp(previousTransform, currentTransform, interpolationFactor);
            transformData.TransformComponent.InterpolatedTransform = transformData.InterpolatedTransform;
        }
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
        if (component is Transform2DComponent transform2DComponent)
        {
            _transforms[transform2DComponent] = new TransformData
            {
                TransformComponent = transform2DComponent,
                PreviousTransform = transform2DComponent.Transform,
                CurrentTransform = transform2DComponent.Transform,
                InterpolatedTransform = transform2DComponent.Transform
            };
        }
    }

    public void OnComponentRemoved(Component component)
    {
        if (component is Transform2DComponent transform2DComponent)
        {
            _transforms.Remove(transform2DComponent);
        }
    }

    #endregion

    private class TransformData
    {
        public Transform2DComponent TransformComponent;
        public Transform2D PreviousTransform;
        public Transform2D CurrentTransform;
        public Transform2D InterpolatedTransform;
    }
}