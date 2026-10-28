using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Components;
using NUnit.Framework;

namespace Geisha.Engine.IntegrationTests.Rendering;

internal sealed class TextRendererComponentIntegrationTestsSut
{
    public TextRendererComponentIntegrationTestsSut(ISceneManager sceneManager)
    {
        SceneManager = sceneManager;
    }

    public ISceneManager SceneManager { get; }
}

[TestFixture]
internal class TextRendererComponentIntegrationTests : IntegrationTests<TextRendererComponentIntegrationTestsSut>
{
    [Test]
    public void TextRendererComponent_BoundingRectangle_ShouldReturnCachedValue_WhenLayoutPropertiesAreReassignedWithSameValues()
    {
        // Arrange
        var scene = SystemUnderTest.SceneManager.CurrentScene;
        var entity = scene.CreateEntity();
        var transform2DComponent = entity.CreateComponent<Transform2DComponent>();
        var textRendererComponent = entity.CreateComponent<TextRendererComponent>();
        textRendererComponent.IsStatic = true;

        // Assume 1
        Assert.That(textRendererComponent.IsManagedByRenderingSystem, Is.True);

        // Act 1
        textRendererComponent.FontSize = FontSize.FromDips(1.1);
        textRendererComponent.MaxWidth = 2.2;
        textRendererComponent.MaxHeight = 3.3;

        // Assert 1 - Get returns exactly what was set despite internal backend float rounding.
        Assert.That(textRendererComponent.FontSize, Is.EqualTo(FontSize.FromDips(1.1)));
        Assert.That(textRendererComponent.MaxWidth, Is.EqualTo(2.2));
        Assert.That(textRendererComponent.MaxHeight, Is.EqualTo(3.3));

        // Assume 2
        var boundingRectangleBefore = textRendererComponent.BoundingRectangle;
        Assert.That(boundingRectangleBefore, Is.Not.EqualTo(new AxisAlignedRectangle()));

        // Act 2
        transform2DComponent.Translation = new Vector2(10, 20);
        textRendererComponent.FontSize = FontSize.FromDips(1.1);
        textRendererComponent.MaxWidth = 2.2;
        textRendererComponent.MaxHeight = 3.3;

        var boundingRectangleAfter = textRendererComponent.BoundingRectangle;

        // Assert 2 - Changing Transform for static renderer should not change cached bounding
        // rectangle unless the property setters fail to skip recaching for the same inputs.
        Assert.That(boundingRectangleAfter, Is.EqualTo(boundingRectangleBefore));
    }
}