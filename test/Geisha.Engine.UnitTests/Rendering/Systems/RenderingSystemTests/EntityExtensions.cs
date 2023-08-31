using System;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Components;

namespace Geisha.Engine.UnitTests.Rendering.Systems.RenderingSystemTests;

internal static class EntityExtensions
{
    public static Sprite GetSprite(this Entity entity) => entity.GetComponent<SpriteRendererComponent>().Sprite ??
                                                          throw new ArgumentException("Entity must have SpriteRendererComponent with non-null Sprite.");

    public static double GetOpacity(this Entity entity) => entity.GetComponent<SpriteRendererComponent>().Opacity;

    public static Matrix3x3 Get2DTransformationMatrix(this Entity entity) => entity.GetComponent<Transform2DComponent>().ToMatrix();
}