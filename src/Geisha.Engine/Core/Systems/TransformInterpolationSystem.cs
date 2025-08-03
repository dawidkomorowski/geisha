using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.GameLoop;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.Systems;

internal readonly record struct TransformInterpolationId(int Id)
{
    public static TransformInterpolationId Invalid { get; } = new(-1);
}

internal sealed class TransformInterpolationSystem : ITransformInterpolationGameLoopStep, ISceneObserver
{
    private readonly List<TransformData> _transforms = new();

    internal TransformInterpolationId CreateTransform(Transform2DComponent transform2DComponent)
    {
        Debug.Assert(HasTransformData(transform2DComponent) is false, "Transform2DComponent is already added to TransformInterpolationSystem.");

        var id = new TransformInterpolationId(_transforms.Count);
        _transforms.Add(new TransformData
        {
            TransformComponent = transform2DComponent,
            PreviousTransform = transform2DComponent.Transform,
            CurrentTransform = transform2DComponent.Transform,
            InterpolatedTransform = transform2DComponent.Transform
        });
        return id;
    }

    internal void DeleteTransform(TransformInterpolationId id)
    {
        Debug.Assert(id.Id >= 0 && id.Id < _transforms.Count, "Invalid TransformInterpolationId.");

        if (id.Id == _transforms.Count - 1)
        {
            // If the last transform is being deleted, just remove it.
            _transforms.RemoveAt(id.Id);
        }
        else
        {
            _transforms[id.Id] = _transforms[^1]; // Move last transform to the deleted position.
            _transforms.RemoveAt(_transforms.Count - 1); // Remove last transform.
            _transforms[id.Id].TransformComponent.TransformInterpolationId = id; // Update ID of the moved transform.    
        }
    }

    internal Transform2D GetInterpolatedTransform(TransformInterpolationId id)
    {
        Debug.Assert(id.Id >= 0 && id.Id < _transforms.Count, "Invalid TransformInterpolationId.");
        return _transforms[id.Id].InterpolatedTransform;
    }

    internal void SetTransformImmediate(TransformInterpolationId id, in Transform2D transform)
    {
        Debug.Assert(id.Id >= 0 && id.Id < _transforms.Count, "Invalid TransformInterpolationId.");
        ref var transformData = ref CollectionsMarshal.AsSpan(_transforms)[id.Id];
        transformData.PreviousTransform = transform;
        transformData.CurrentTransform = transform;
        transformData.InterpolatedTransform = transform;
    }

    /// <summary>
    ///     Determines whether the specified <see cref="Transform2DComponent" /> exists in the system. This API is intended for
    ///     runtime assertions and tests only.
    /// </summary>
    /// <param name="transform2DComponent">The <see cref="Transform2DComponent" /> to locate in the system.</param>
    /// <returns>
    ///     <see langword="true" /> if the specified <see cref="Transform2DComponent" /> is found in the system;
    ///     otherwise, <see langword="false" />.
    /// </returns>
    internal bool HasTransformData(Transform2DComponent transform2DComponent)
    {
        return _transforms.Any(t => t.TransformComponent == transform2DComponent);
    }

    #region Implementation of ITransformInterpolationGameLoopStep

    public void SnapshotTransforms()
    {
        foreach (ref var transformData in CollectionsMarshal.AsSpan(_transforms))
        {
            transformData.PreviousTransform = transformData.CurrentTransform;
            transformData.CurrentTransform = transformData.TransformComponent.Transform;
        }
    }

    public void InterpolateTransforms(double interpolationFactor)
    {
        foreach (ref var transformData in CollectionsMarshal.AsSpan(_transforms))
        {
            transformData.InterpolatedTransform = Transform2D.Lerp(transformData.PreviousTransform, transformData.CurrentTransform, interpolationFactor);
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
            transform2DComponent.TransformInterpolationSystem = this;
            if (transform2DComponent.IsInterpolated && transform2DComponent.TransformInterpolationId == TransformInterpolationId.Invalid)
            {
                transform2DComponent.TransformInterpolationId = CreateTransform(transform2DComponent);
            }
        }
    }

    public void OnComponentRemoved(Component component)
    {
        if (component is Transform2DComponent transform2DComponent)
        {
            transform2DComponent.TransformInterpolationSystem = null;
            if (transform2DComponent.IsInterpolated && transform2DComponent.TransformInterpolationId != TransformInterpolationId.Invalid)
            {
                DeleteTransform(transform2DComponent.TransformInterpolationId);
                transform2DComponent.TransformInterpolationId = TransformInterpolationId.Invalid;
            }
        }
    }

    #endregion

    private struct TransformData
    {
        public Transform2DComponent TransformComponent;
        public Transform2D PreviousTransform;
        public Transform2D CurrentTransform;
        public Transform2D InterpolatedTransform;
    }
}