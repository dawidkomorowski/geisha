using Geisha.Engine.Rendering;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Rendering.Systems.RenderingSystemTests;

[TestFixture]
public class RenderingOrderTests : RenderingSystemTestsBase
{
    [Test]
    public void RenderScene_ShouldIgnoreOrderInLayer_WhenEntitiesAreInDifferentSortingLayers()
    {
        // Arrange
        const string otherSortingLayer = "Other";

        var (renderingSystem, renderingScene) = GetRenderingSystem(new RenderingConfiguration
            { SortingLayersOrder = new[] { RenderingConfiguration.DefaultSortingLayerName, otherSortingLayer } });
        renderingScene.AddCamera();
        var entity1 = renderingScene.AddSprite(orderInLayer: 0, sortingLayerName: otherSortingLayer);
        var entity2 = renderingScene.AddSprite(orderInLayer: 1);

        // Act
        renderingSystem.RenderScene();

        // Assert
        Received.InOrder(() =>
        {
            RenderingContext2D.DrawSprite(entity2.GetSprite(), entity2.Get2DTransformationMatrix(), entity2.GetOpacity());
            RenderingContext2D.DrawSprite(entity1.GetSprite(), entity1.Get2DTransformationMatrix(), entity1.GetOpacity());
        });
    }

    [Test]
    public void RenderScene_ShouldRenderInOrderOf_OrderInLayer_WhenEntitiesAreInTheSameSortingLayer()
    {
        // Arrange
        var (renderingSystem, renderingScene) = GetRenderingSystem();
        renderingScene.AddCamera();
        var entity1 = renderingScene.AddSprite(orderInLayer: 1);
        var entity2 = renderingScene.AddSprite(orderInLayer: -1);
        var entity3 = renderingScene.AddSprite(orderInLayer: 0);

        // Act
        renderingSystem.RenderScene();

        // Assert
        Received.InOrder(() =>
        {
            RenderingContext2D.DrawSprite(entity2.GetSprite(), entity2.Get2DTransformationMatrix(), entity2.GetOpacity());
            RenderingContext2D.DrawSprite(entity3.GetSprite(), entity3.Get2DTransformationMatrix(), entity3.GetOpacity());
            RenderingContext2D.DrawSprite(entity1.GetSprite(), entity1.Get2DTransformationMatrix(), entity1.GetOpacity());
        });
    }

    [Test]
    public void RenderScene_ShouldRenderInSortingLayersOrder_Default_Background_Foreground()
    {
        // Arrange
        const string backgroundSortingLayerName = "Background";
        const string foregroundSortingLayerName = "Foreground";

        var (renderingSystem, renderingScene) = GetRenderingSystem(new RenderingConfiguration
            { SortingLayersOrder = new[] { RenderingConfiguration.DefaultSortingLayerName, backgroundSortingLayerName, foregroundSortingLayerName } });
        renderingScene.AddCamera();
        var entity1 = renderingScene.AddSprite(sortingLayerName: foregroundSortingLayerName);
        var entity2 = renderingScene.AddSprite(sortingLayerName: RenderingConfiguration.DefaultSortingLayerName);
        var entity3 = renderingScene.AddSprite(sortingLayerName: backgroundSortingLayerName);

        // Act
        renderingSystem.RenderScene();

        // Assert
        Received.InOrder(() =>
        {
            RenderingContext2D.DrawSprite(entity2.GetSprite(), entity2.Get2DTransformationMatrix(), entity2.GetOpacity());
            RenderingContext2D.DrawSprite(entity3.GetSprite(), entity3.Get2DTransformationMatrix(), entity3.GetOpacity());
            RenderingContext2D.DrawSprite(entity1.GetSprite(), entity1.Get2DTransformationMatrix(), entity1.GetOpacity());
        });
    }

    [Test]
    public void RenderScene_ShouldRenderInSortingLayersOrder_Foreground_Background_Default()
    {
        // Arrange
        const string backgroundSortingLayerName = "Background";
        const string foregroundSortingLayerName = "Foreground";

        var (renderingSystem, renderingScene) = GetRenderingSystem(new RenderingConfiguration
            { SortingLayersOrder = new[] { foregroundSortingLayerName, backgroundSortingLayerName, RenderingConfiguration.DefaultSortingLayerName } });
        renderingScene.AddCamera();
        var entity1 = renderingScene.AddSprite(sortingLayerName: foregroundSortingLayerName);
        var entity2 = renderingScene.AddSprite(sortingLayerName: RenderingConfiguration.DefaultSortingLayerName);
        var entity3 = renderingScene.AddSprite(sortingLayerName: backgroundSortingLayerName);

        // Act
        renderingSystem.RenderScene();

        // Assert
        Received.InOrder(() =>
        {
            RenderingContext2D.DrawSprite(entity1.GetSprite(), entity1.Get2DTransformationMatrix(), entity1.GetOpacity());
            RenderingContext2D.DrawSprite(entity3.GetSprite(), entity3.Get2DTransformationMatrix(), entity3.GetOpacity());
            RenderingContext2D.DrawSprite(entity2.GetSprite(), entity2.Get2DTransformationMatrix(), entity2.GetOpacity());
        });
    }
}