using System;
using Geisha.Common.Math;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Rendering.Components
{
    // TODO Should Camera be actually a component? Maybe it should be separate thing directly on Scene?
    // TODO what if there are more than one camera? (introduce active flag?)
    // TODO optimization(camera viewing space is good point to optimizing draw calls by clipping things out of camera or not visible by camera)
    // TODO viewing space:
    // TODO     for 2D it is logical rectangle that is observable clipping of the whole scene
    // TODO     for 3D it is frustum space that defines observable clipping polyhedron
    // TODO resolution(is it camera responsibility?)
    // TODO aspect ratio(isn't it defined by viewing space?)
    // TODO projection type (only meaningful for 3D ?)
    /// <summary>
    ///     Represents camera that defines view-port.
    /// </summary>
    public sealed class CameraComponent : IComponent
    {
        // TODO Add xml docs.
        public int ScreenWidth { get; internal set; }

        // TODO Add xml docs.
        public int ScreenHeight { get; internal set; }
    }

    // TODO Should it be part of CameraComponent?
    // TODO Add XML docs.
    public static class CameraExtensions
    {
        public static Vector2 ScreenPointTo2DWorldPoint(this Entity cameraEntity, Vector2 screenPoint)
        {
            if (!cameraEntity.HasComponent<CameraComponent>()) throw new ArgumentException("Entity is not a camera.");

            var cameraComponent = cameraEntity.GetComponent<CameraComponent>();
            var cameraTransform = cameraEntity.GetComponent<TransformComponent>();

            var transformationMatrix = cameraTransform.Create2DTransformationMatrix() * Matrix3x3.CreateScale(new Vector2(1, -1)) *
                                       Matrix3x3.CreateTranslation(new Vector2(-cameraComponent.ScreenWidth / 2.0, -cameraComponent.ScreenHeight / 2.0));

            return (transformationMatrix * screenPoint.Homogeneous).ToVector2();
        }

        public static Matrix3x3 Create2DWorldToScreenMatrix(this Entity cameraEntity)
        {
            if (!cameraEntity.HasComponent<CameraComponent>()) throw new ArgumentException("Entity is not a camera.");

            var cameraTransform = cameraEntity.GetComponent<TransformComponent>();
            var cameraScale = cameraTransform.Scale.ToVector2();
            return Matrix3x3.CreateScale(new Vector2(1 / cameraScale.X, 1 / cameraScale.Y)) *
                   Matrix3x3.CreateRotation(-cameraTransform.Rotation.Z) *
                   Matrix3x3.CreateTranslation(-cameraTransform.Translation.ToVector2()) * Matrix3x3.Identity;
        }
    }
}