using System;
using System.Collections.Generic;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Backend;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Rendering.Systems.RenderingSystemTests;

[TestFixture]
public class TextRendererComponentTests : RenderingSystemTestsBase
{
    [Test]
    public void RenderingSystem_ShouldKeepTextRendererComponentState_WhenRenderingSystemIsRemovedFromSceneObserversOfScene()
    {
        // Arrange
        // Renderer2DComponent
        const bool visible = false;
        const string sortingLayerName = "some sorting layer";
        const int orderInLayer = 12;
        // TextRendererComponent
        const string text = "Sample text";
        const string fontFamilyName = "Calibri";
        var fontSize = FontSize.FromDips(30);
        var color = Color.Red;
        const double maxWidth = 200;
        const double maxHeight = 400;
        const TextAlignment textAlignment = TextAlignment.Center;
        const ParagraphAlignment paragraphAlignment = ParagraphAlignment.Center;
        var pivot = new Vector2(100, 200);
        const bool clipToLayoutBox = true;

        MockCreateTextLayout();

        var context = CreateRenderingTestContext(new RenderingConfiguration
            { SortingLayersOrder = new[] { RenderingConfiguration.DefaultSortingLayerName, sortingLayerName } });
        var (_, textRendererComponent) = context.AddText();

        // Renderer2DComponent
        textRendererComponent.Visible = visible;
        textRendererComponent.SortingLayerName = sortingLayerName;
        textRendererComponent.OrderInLayer = orderInLayer;
        // TextRendererComponent
        textRendererComponent.Text = text;
        textRendererComponent.FontFamilyName = fontFamilyName;
        textRendererComponent.FontSize = fontSize;
        textRendererComponent.Color = color;
        textRendererComponent.MaxWidth = maxWidth;
        textRendererComponent.MaxHeight = maxHeight;
        textRendererComponent.TextAlignment = textAlignment;
        textRendererComponent.ParagraphAlignment = paragraphAlignment;
        textRendererComponent.Pivot = pivot;
        textRendererComponent.ClipToLayoutBox = clipToLayoutBox;

        // Assume
        Assert.That(textRendererComponent.IsManagedByRenderingSystem, Is.True);

        // Act
        context.Scene.RemoveObserver(context.RenderingSystem);

        // Assert
        // Renderer2DComponent
        Assert.That(textRendererComponent.IsManagedByRenderingSystem, Is.False);
        Assert.That(textRendererComponent.Visible, Is.EqualTo(visible));
        Assert.That(textRendererComponent.SortingLayerName, Is.EqualTo(sortingLayerName));
        Assert.That(textRendererComponent.OrderInLayer, Is.EqualTo(orderInLayer));
        // TextRendererComponent
        Assert.That(textRendererComponent.Text, Is.EqualTo(text));
        Assert.That(textRendererComponent.FontFamilyName, Is.EqualTo(fontFamilyName));
        Assert.That(textRendererComponent.FontSize, Is.EqualTo(fontSize));
        Assert.That(textRendererComponent.Color, Is.EqualTo(color));
        Assert.That(textRendererComponent.MaxWidth, Is.EqualTo(maxWidth));
        Assert.That(textRendererComponent.MaxHeight, Is.EqualTo(maxHeight));
        Assert.That(textRendererComponent.TextAlignment, Is.EqualTo(textAlignment));
        Assert.That(textRendererComponent.ParagraphAlignment, Is.EqualTo(paragraphAlignment));
        Assert.That(textRendererComponent.Pivot, Is.EqualTo(pivot));
        Assert.That(textRendererComponent.ClipToLayoutBox, Is.EqualTo(clipToLayoutBox));
    }

    [Test]
    public void RenderingSystem_ShouldKeepTextRendererComponentState_WhenRenderingSystemIsAddedToSceneObserversOfScene()
    {
        // Arrange
        // Renderer2DComponent
        const bool visible = false;
        const string sortingLayerName = "some sorting layer";
        const int orderInLayer = 12;
        // TextRendererComponent
        const string text = "Sample text";
        const string fontFamilyName = "Calibri";
        var fontSize = FontSize.FromDips(30);
        var color = Color.Red;
        const double maxWidth = 200;
        const double maxHeight = 400;
        const TextAlignment textAlignment = TextAlignment.Center;
        const ParagraphAlignment paragraphAlignment = ParagraphAlignment.Center;
        var pivot = new Vector2(100, 200);
        const bool clipToLayoutBox = true;

        MockCreateTextLayout();

        var context = CreateRenderingTestContext(new RenderingConfiguration
            { SortingLayersOrder = new[] { RenderingConfiguration.DefaultSortingLayerName, sortingLayerName } });
        context.Scene.RemoveObserver(context.RenderingSystem);

        var (_, textRendererComponent) = context.AddText();

        // Renderer2DComponent
        textRendererComponent.Visible = visible;
        textRendererComponent.SortingLayerName = sortingLayerName;
        textRendererComponent.OrderInLayer = orderInLayer;
        // TextRendererComponent
        textRendererComponent.Text = text;
        textRendererComponent.FontFamilyName = fontFamilyName;
        textRendererComponent.FontSize = fontSize;
        textRendererComponent.Color = color;
        textRendererComponent.MaxWidth = maxWidth;
        textRendererComponent.MaxHeight = maxHeight;
        textRendererComponent.TextAlignment = textAlignment;
        textRendererComponent.ParagraphAlignment = paragraphAlignment;
        textRendererComponent.Pivot = pivot;
        textRendererComponent.ClipToLayoutBox = clipToLayoutBox;

        // Assume
        Assert.That(textRendererComponent.IsManagedByRenderingSystem, Is.False);

        // Act
        context.Scene.AddObserver(context.RenderingSystem);

        // Assert
        // Renderer2DComponent
        Assert.That(textRendererComponent.IsManagedByRenderingSystem, Is.True);
        Assert.That(textRendererComponent.Visible, Is.EqualTo(visible));
        Assert.That(textRendererComponent.SortingLayerName, Is.EqualTo(sortingLayerName));
        Assert.That(textRendererComponent.OrderInLayer, Is.EqualTo(orderInLayer));
        // TextRendererComponent
        Assert.That(textRendererComponent.Text, Is.EqualTo(text));
        Assert.That(textRendererComponent.FontFamilyName, Is.EqualTo(fontFamilyName));
        Assert.That(textRendererComponent.FontSize, Is.EqualTo(fontSize));
        Assert.That(textRendererComponent.Color, Is.EqualTo(color));
        Assert.That(textRendererComponent.MaxWidth, Is.EqualTo(maxWidth));
        Assert.That(textRendererComponent.MaxHeight, Is.EqualTo(maxHeight));
        Assert.That(textRendererComponent.TextAlignment, Is.EqualTo(textAlignment));
        Assert.That(textRendererComponent.ParagraphAlignment, Is.EqualTo(paragraphAlignment));
        Assert.That(textRendererComponent.Pivot, Is.EqualTo(pivot));
        Assert.That(textRendererComponent.ClipToLayoutBox, Is.EqualTo(clipToLayoutBox));
    }

    [Test]
    public void RenderingSystem_ShouldDisposeTextLayout_WhenTextRendererComponentIsRemovedFromEntity()
    {
        // Arrange
        var getTextLayout = MockCreateTextLayout();

        var context = CreateRenderingTestContext();
        var (entity, textRendererComponent) = context.AddText();
        var textLayout = getTextLayout();

        textLayout.ClearReceivedCalls();

        // Act
        entity.RemoveComponent(textRendererComponent);

        // Assert
        textLayout.Received(1).Dispose();
    }

    [Test]
    public void RenderScene_ShouldDrawTextLayout_WhenSceneContainsEntityWithTextRendererAndTransform()
    {
        // Arrange
        const string text = "Sample text";
        const string fontFamilyName = "Calibri";
        var fontSize = FontSize.FromDips(20);
        var color = Color.Red;
        const double maxWidth = 200;
        const double maxHeight = 400;
        const TextAlignment textAlignment = TextAlignment.Center;
        const ParagraphAlignment paragraphAlignment = ParagraphAlignment.Center;
        var pivot = new Vector2(100, 200);
        const bool clipToLayoutBox = true;

        var getTextLayout = MockCreateTextLayout();

        var context = CreateRenderingTestContext();
        context.AddCamera();
        var (entity, textRendererComponent) = context.AddText();

        textRendererComponent.FontFamilyName = fontFamilyName;
        textRendererComponent.FontSize = fontSize;
        textRendererComponent.Color = color;
        textRendererComponent.MaxWidth = maxWidth;
        textRendererComponent.MaxHeight = maxHeight;
        textRendererComponent.TextAlignment = textAlignment;
        textRendererComponent.ParagraphAlignment = paragraphAlignment;
        textRendererComponent.Pivot = pivot;
        textRendererComponent.ClipToLayoutBox = clipToLayoutBox;
        // Force recreation of ITextLayout
        textRendererComponent.Text = text;

        // Act
        context.RenderingSystem.RenderScene();

        // Assert
        var textLayout = getTextLayout();
        textLayout.Received(1).TextAlignment = textAlignment;
        textLayout.Received(1).ParagraphAlignment = paragraphAlignment;
        RenderingContext2D.Received(1).DrawTextLayout(textLayout, color, pivot, entity.Get2DTransformationMatrix(), clipToLayoutBox);
    }

    [Test]
    public void RenderScene_ShouldDrawTextLayout_WhenSceneContainsEntityWithTextRendererAndTransform_AfterLayoutIsUpdated()
    {
        // Arrange
        const string text = "Sample text";
        const string fontFamilyName = "Calibri";
        var fontSize = FontSize.FromDips(20);
        var color = Color.Red;
        const double maxWidth = 200;
        const double maxHeight = 400;
        const TextAlignment textAlignment = TextAlignment.Center;
        const ParagraphAlignment paragraphAlignment = ParagraphAlignment.Center;
        var pivot = new Vector2(100, 200);
        const bool clipToLayoutBox = true;

        var getTextLayout = MockCreateTextLayout();

        var context = CreateRenderingTestContext();
        context.AddCamera();
        var (entity, textRendererComponent) = context.AddText();

        textRendererComponent.Text = text;
        textRendererComponent.FontFamilyName = fontFamilyName;
        textRendererComponent.FontSize = fontSize;
        textRendererComponent.Color = color;
        textRendererComponent.MaxWidth = maxWidth;
        textRendererComponent.MaxHeight = maxHeight;
        textRendererComponent.TextAlignment = textAlignment;
        textRendererComponent.ParagraphAlignment = paragraphAlignment;
        textRendererComponent.Pivot = pivot;
        textRendererComponent.ClipToLayoutBox = clipToLayoutBox;

        // Act
        context.RenderingSystem.RenderScene();

        // Assert
        var textLayout = getTextLayout();
        textLayout.Received(1).FontFamilyName = fontFamilyName;
        textLayout.Received(1).FontSize = fontSize;
        textLayout.Received(1).MaxWidth = maxWidth;
        textLayout.Received(1).MaxHeight = maxHeight;
        textLayout.Received(1).TextAlignment = textAlignment;
        textLayout.Received(1).ParagraphAlignment = paragraphAlignment;
        RenderingContext2D.Received(1).DrawTextLayout(textLayout, color, pivot, entity.Get2DTransformationMatrix(), clipToLayoutBox);
    }

    [Test]
    public void RenderScene_ShouldDrawTextLayout_TransformedWithParentTransform_WhenEntityHasParentWithTransform2DComponent()
    {
        // Arrange
        const string text = "Sample text";
        const string fontFamilyName = "Calibri";
        var fontSize = FontSize.FromDips(20);
        var color = Color.Red;
        const double maxWidth = 200;
        const double maxHeight = 400;
        const TextAlignment textAlignment = TextAlignment.Center;
        const ParagraphAlignment paragraphAlignment = ParagraphAlignment.Center;
        var pivot = new Vector2(100, 200);
        const bool clipToLayoutBox = true;

        var getTextLayout = MockCreateTextLayout();

        var context = CreateRenderingTestContext();
        context.AddCamera();

        var parent = context.Scene.CreateEntity();
        var parentTransform = parent.CreateComponent<Transform2DComponent>();
        parentTransform.Translation = new Vector2(10, 20);
        parentTransform.Rotation = 30;
        parentTransform.Scale = new Vector2(2, 4);

        var (child, textRendererComponent) = context.AddText();
        child.Parent = parent;

        textRendererComponent.Text = text;
        textRendererComponent.FontFamilyName = fontFamilyName;
        textRendererComponent.FontSize = fontSize;
        textRendererComponent.Color = color;
        textRendererComponent.MaxWidth = maxWidth;
        textRendererComponent.MaxHeight = maxHeight;
        textRendererComponent.TextAlignment = textAlignment;
        textRendererComponent.ParagraphAlignment = paragraphAlignment;
        textRendererComponent.Pivot = pivot;
        textRendererComponent.ClipToLayoutBox = clipToLayoutBox;

        var expectedTransform = parentTransform.ToMatrix() * child.Get2DTransformationMatrix();

        // Act
        context.RenderingSystem.RenderScene();

        // Assert
        RenderingContext2D.Received(1).DrawTextLayout(getTextLayout(), color, pivot, expectedTransform, clipToLayoutBox);
    }

    [Test]
    public void RenderScene_ShouldDrawTextLayout_WhenTransformIsInterpolated()
    {
        // Arrange
        const string text = "Sample text";
        const string fontFamilyName = "Calibri";
        var fontSize = FontSize.FromDips(20);
        var color = Color.Red;
        const double maxWidth = 200;
        const double maxHeight = 400;
        const TextAlignment textAlignment = TextAlignment.Center;
        const ParagraphAlignment paragraphAlignment = ParagraphAlignment.Center;
        var pivot = new Vector2(100, 200);
        const bool clipToLayoutBox = true;

        var getTextLayout = MockCreateTextLayout();

        var context = CreateRenderingTestContext();
        context.AddCamera();

        var (entity, textRendererComponent) = context.AddText(new Vector2(10, 20), 30, new Vector2(1, 2));
        var transform2DComponent = entity.GetComponent<Transform2DComponent>();
        transform2DComponent.IsInterpolated = true;

        textRendererComponent.Text = text;
        textRendererComponent.FontFamilyName = fontFamilyName;
        textRendererComponent.FontSize = fontSize;
        textRendererComponent.Color = color;
        textRendererComponent.MaxWidth = maxWidth;
        textRendererComponent.MaxHeight = maxHeight;
        textRendererComponent.TextAlignment = textAlignment;
        textRendererComponent.ParagraphAlignment = paragraphAlignment;
        textRendererComponent.Pivot = pivot;
        textRendererComponent.ClipToLayoutBox = clipToLayoutBox;

        context.TransformInterpolationSystem.SnapshotTransforms();

        transform2DComponent.Translation = new Vector2(20, 40);
        transform2DComponent.Rotation = 60;
        transform2DComponent.Scale = new Vector2(2, 4);

        context.TransformInterpolationSystem.SnapshotTransforms();

        context.TransformInterpolationSystem.InterpolateTransforms(0.5);

        // Assume
        Assert.That(transform2DComponent.InterpolatedTransform, Is.Not.EqualTo(transform2DComponent.Transform));

        // Act
        context.RenderingSystem.RenderScene();

        // Assert
        RenderingContext2D.Received(1).DrawTextLayout(getTextLayout(), color, pivot, transform2DComponent.InterpolatedTransform.ToMatrix(), clipToLayoutBox);
    }

    [Test]
    public void RenderScene_ShouldDrawTextLayout_TransformedWithParentTransform_WhenTransformIsInterpolated()
    {
        // Arrange
        const string text = "Sample text";
        const string fontFamilyName = "Calibri";
        var fontSize = FontSize.FromDips(20);
        var color = Color.Red;
        const double maxWidth = 200;
        const double maxHeight = 400;
        const TextAlignment textAlignment = TextAlignment.Center;
        const ParagraphAlignment paragraphAlignment = ParagraphAlignment.Center;
        var pivot = new Vector2(100, 200);
        const bool clipToLayoutBox = true;

        var getTextLayout = MockCreateTextLayout();

        var context = CreateRenderingTestContext();
        context.AddCamera();

        var parent = context.Scene.CreateEntity();
        var parentTransform = parent.CreateComponent<Transform2DComponent>();
        parentTransform.IsInterpolated = true;
        parentTransform.Translation = new Vector2(10, 20);
        parentTransform.Rotation = 2;
        parentTransform.Scale = new Vector2(1, 2);

        var (child, textRendererComponent) = context.AddText(new Vector2(20, 30), 3, new Vector2(2, 3));
        child.Parent = parent;
        var childTransform = child.GetComponent<Transform2DComponent>();
        childTransform.IsInterpolated = true;

        textRendererComponent.Text = text;
        textRendererComponent.FontFamilyName = fontFamilyName;
        textRendererComponent.FontSize = fontSize;
        textRendererComponent.Color = color;
        textRendererComponent.MaxWidth = maxWidth;
        textRendererComponent.MaxHeight = maxHeight;
        textRendererComponent.TextAlignment = textAlignment;
        textRendererComponent.ParagraphAlignment = paragraphAlignment;
        textRendererComponent.Pivot = pivot;
        textRendererComponent.ClipToLayoutBox = clipToLayoutBox;

        context.TransformInterpolationSystem.SnapshotTransforms();

        parentTransform.Translation = new Vector2(20, 40);
        parentTransform.Rotation = 4;
        parentTransform.Scale = new Vector2(2, 4);

        childTransform.Translation = new Vector2(30, 60);
        childTransform.Rotation = 6;
        childTransform.Scale = new Vector2(3, 6);

        context.TransformInterpolationSystem.SnapshotTransforms();

        context.TransformInterpolationSystem.InterpolateTransforms(0.5);

        // Assume
        Assert.That(parentTransform.InterpolatedTransform, Is.Not.EqualTo(parentTransform.Transform));
        Assert.That(childTransform.InterpolatedTransform, Is.Not.EqualTo(childTransform.Transform));

        var expectedTransform = parentTransform.InterpolatedTransform.ToMatrix() * childTransform.InterpolatedTransform.ToMatrix();

        // Act
        context.RenderingSystem.RenderScene();

        // Assert
        RenderingContext2D.Received(1).DrawTextLayout(getTextLayout(), color, pivot, expectedTransform, clipToLayoutBox);
    }

    [Test]
    public void TextRendererComponent_BoundingRectangle_ShouldReturnDefaultValue_WhenRenderingSystemIsNotAddedToSceneObservers()
    {
        // Arrange
        var textMetrics = new TextMetrics
        {
            Left = 0,
            Top = 0,
            Width = 100,
            Height = 200,
            LayoutWidth = 150,
            LayoutHeight = 250,
            LineCount = 10
        };
        var textLayout = Substitute.For<ITextLayout>();
        textLayout.Metrics.Returns(textMetrics);

        RenderingContext2D.CreateTextLayout(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<FontSize>(), Arg.Any<double>(), Arg.Any<double>())
            .Returns(textLayout);

        var context = CreateRenderingTestContext();
        var (_, textRendererComponent) = context.AddText(new Vector2(10, 20), 0, new Vector2(2, 2));
        context.Scene.RemoveObserver(context.RenderingSystem);
        textRendererComponent.MaxWidth = 150;
        textRendererComponent.MaxHeight = 250;
        textRendererComponent.Pivot = new Vector2(50, 100);

        // Act
        var actual = textRendererComponent.BoundingRectangle;

        // Assert
        Assert.That(textRendererComponent.IsManagedByRenderingSystem, Is.False);
        Assert.That(actual, Is.EqualTo(new AxisAlignedRectangle()));
    }

    [Test]
    public void TextRendererComponent_BoundingRectangle_ShouldReturnComputedValue_WhenRenderingSystemIsAddedToSceneObservers()
    {
        // Arrange
        var textMetrics = new TextMetrics
        {
            Left = 0,
            Top = 0,
            Width = 100,
            Height = 200,
            LayoutWidth = 150,
            LayoutHeight = 250,
            LineCount = 10
        };
        var textLayout = Substitute.For<ITextLayout>();
        textLayout.Metrics.Returns(textMetrics);

        RenderingContext2D.CreateTextLayout(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<FontSize>(), Arg.Any<double>(), Arg.Any<double>())
            .Returns(textLayout);

        var context = CreateRenderingTestContext();
        var (_, textRendererComponent) = context.AddText(new Vector2(10, 20), 0, new Vector2(2, 2));
        textRendererComponent.MaxWidth = 150;
        textRendererComponent.MaxHeight = 250;
        textRendererComponent.Pivot = new Vector2(50, 100);

        // Act
        var actual = textRendererComponent.BoundingRectangle;

        // Assert
        Assert.That(textRendererComponent.IsManagedByRenderingSystem, Is.True);
        Assert.That(actual, Is.EqualTo(new AxisAlignedRectangle(10, 20, 200, 400)));
    }

    [Test]
    public void TextRendererComponent_BoundingRectangle_ShouldReturnComputedValue_WhenTransformIsInterpolated()
    {
        // Arrange
        var textMetrics = new TextMetrics
        {
            Left = 0,
            Top = 0,
            Width = 100,
            Height = 200,
            LayoutWidth = 150,
            LayoutHeight = 250,
            LineCount = 10
        };
        var textLayout = Substitute.For<ITextLayout>();
        textLayout.Metrics.Returns(textMetrics);

        RenderingContext2D.CreateTextLayout(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<FontSize>(), Arg.Any<double>(), Arg.Any<double>())
            .Returns(textLayout);

        var context = CreateRenderingTestContext();
        var (entity, textRendererComponent) = context.AddText(new Vector2(10, 20), 0, new Vector2(2, 2));
        textRendererComponent.MaxWidth = 150;
        textRendererComponent.MaxHeight = 250;
        textRendererComponent.Pivot = new Vector2(50, 100);

        var transform2DComponent = entity.GetComponent<Transform2DComponent>();
        transform2DComponent.IsInterpolated = true;

        context.TransformInterpolationSystem.SnapshotTransforms();

        transform2DComponent.Translation = new Vector2(20, 40);
        transform2DComponent.Scale = new Vector2(4, 4);

        context.TransformInterpolationSystem.SnapshotTransforms();

        context.TransformInterpolationSystem.InterpolateTransforms(0.5);

        // Assume
        Assert.That(transform2DComponent.InterpolatedTransform, Is.Not.EqualTo(transform2DComponent.Transform));

        // Act
        var actual = textRendererComponent.BoundingRectangle;

        // Assert
        Assert.That(textRendererComponent.IsManagedByRenderingSystem, Is.True);
        Assert.That(actual, Is.EqualTo(new AxisAlignedRectangle(15, 30, 300, 600)));
    }

    [Test]
    public void TextRendererComponent_TextMetrics_ShouldReturnDefaultValue_WhenRenderingSystemIsNotAddedToSceneObservers()
    {
        // Arrange
        var textMetrics = new TextMetrics
        {
            Left = 100
        };
        var textLayout = Substitute.For<ITextLayout>();
        textLayout.Metrics.Returns(textMetrics);

        RenderingContext2D.CreateTextLayout(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<FontSize>(), Arg.Any<double>(), Arg.Any<double>())
            .Returns(textLayout);

        var context = CreateRenderingTestContext();
        var (_, textRendererComponent) = context.AddText();
        context.Scene.RemoveObserver(context.RenderingSystem);

        // Act
        var actual = textRendererComponent.TextMetrics;

        // Assert
        Assert.That(textRendererComponent.IsManagedByRenderingSystem, Is.False);
        Assert.That(actual, Is.EqualTo(new TextMetrics()));
    }

    [Test]
    public void TextRendererComponent_TextMetrics_ShouldReturnValueFromTextLayout_WhenRenderingSystemIsAddedToSceneObservers()
    {
        // Arrange
        var textMetrics = new TextMetrics
        {
            Left = 100
        };
        var textLayout = Substitute.For<ITextLayout>();
        textLayout.Metrics.Returns(textMetrics);

        RenderingContext2D.CreateTextLayout(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<FontSize>(), Arg.Any<double>(), Arg.Any<double>())
            .Returns(textLayout);

        var context = CreateRenderingTestContext();
        var (_, textRendererComponent) = context.AddText();

        // Act
        var actual = textRendererComponent.TextMetrics;

        // Assert
        Assert.That(textRendererComponent.IsManagedByRenderingSystem, Is.True);
        Assert.That(actual, Is.EqualTo(textMetrics));
    }

    [Test]
    public void TextRendererComponent_LayoutRectangle_ShouldReturnDefaultValue_WhenRenderingSystemIsNotAddedToSceneObservers()
    {
        // Arrange
        var textMetrics = new TextMetrics
        {
            Left = 10,
            Top = 20,
            Width = 100,
            Height = 200,
            LayoutWidth = 150,
            LayoutHeight = 250,
            LineCount = 10
        };
        var textLayout = Substitute.For<ITextLayout>();
        textLayout.Metrics.Returns(textMetrics);

        RenderingContext2D.CreateTextLayout(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<FontSize>(), Arg.Any<double>(), Arg.Any<double>())
            .Returns(textLayout);

        var context = CreateRenderingTestContext();
        var (_, textRendererComponent) = context.AddText();
        textRendererComponent.MaxWidth = 150;
        textRendererComponent.MaxHeight = 250;
        textRendererComponent.Pivot = new Vector2(5, 15);

        context.Scene.RemoveObserver(context.RenderingSystem);

        // Act
        var actual = textRendererComponent.LayoutRectangle;

        // Assert
        Assert.That(textRendererComponent.IsManagedByRenderingSystem, Is.False);
        Assert.That(actual, Is.EqualTo(new AxisAlignedRectangle()));
    }

    [Test]
    public void TextRendererComponent_LayoutRectangle_ShouldReturnComputedValue_WhenRenderingSystemIsAddedToSceneObservers()
    {
        // Arrange
        var textMetrics = new TextMetrics
        {
            Left = 10,
            Top = 20,
            Width = 100,
            Height = 200,
            LayoutWidth = 150,
            LayoutHeight = 250,
            LineCount = 10
        };
        var textLayout = Substitute.For<ITextLayout>();
        textLayout.Metrics.Returns(textMetrics);

        RenderingContext2D.CreateTextLayout(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<FontSize>(), Arg.Any<double>(), Arg.Any<double>())
            .Returns(textLayout);

        var context = CreateRenderingTestContext();
        var (_, textRendererComponent) = context.AddText();
        textRendererComponent.MaxWidth = 150;
        textRendererComponent.MaxHeight = 250;
        textRendererComponent.Pivot = new Vector2(5, 15);

        // Act
        var actual = textRendererComponent.LayoutRectangle;

        // Assert
        Assert.That(textRendererComponent.IsManagedByRenderingSystem, Is.True);
        Assert.That(actual, Is.EqualTo(new AxisAlignedRectangle(70, -110, 150, 250)));
    }

    [Test]
    public void TextRendererComponent_TextRectangle_ShouldReturnDefaultValue_WhenRenderingSystemIsNotAddedToSceneObservers()
    {
        // Arrange
        var textMetrics = new TextMetrics
        {
            Left = 10,
            Top = 20,
            Width = 100,
            Height = 200,
            LayoutWidth = 150,
            LayoutHeight = 250,
            LineCount = 10
        };
        var textLayout = Substitute.For<ITextLayout>();
        textLayout.Metrics.Returns(textMetrics);

        RenderingContext2D.CreateTextLayout(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<FontSize>(), Arg.Any<double>(), Arg.Any<double>())
            .Returns(textLayout);

        var context = CreateRenderingTestContext();
        var (_, textRendererComponent) = context.AddText();
        textRendererComponent.MaxWidth = 150;
        textRendererComponent.MaxHeight = 250;
        textRendererComponent.Pivot = new Vector2(5, 15);

        context.Scene.RemoveObserver(context.RenderingSystem);

        // Act
        var actual = textRendererComponent.TextRectangle;

        // Assert
        Assert.That(textRendererComponent.IsManagedByRenderingSystem, Is.False);
        Assert.That(actual, Is.EqualTo(new AxisAlignedRectangle()));
    }

    [Test]
    public void TextRendererComponent_TextRectangle_ShouldReturnComputedValue_WhenRenderingSystemIsAddedToSceneObservers()
    {
        // Arrange
        var textMetrics = new TextMetrics
        {
            Left = 10,
            Top = 20,
            Width = 100,
            Height = 200,
            LayoutWidth = 150,
            LayoutHeight = 250,
            LineCount = 10
        };
        var textLayout = Substitute.For<ITextLayout>();
        textLayout.Metrics.Returns(textMetrics);

        RenderingContext2D.CreateTextLayout(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<FontSize>(), Arg.Any<double>(), Arg.Any<double>())
            .Returns(textLayout);

        var context = CreateRenderingTestContext();
        var (_, textRendererComponent) = context.AddText();
        textRendererComponent.MaxWidth = 150;
        textRendererComponent.MaxHeight = 250;
        textRendererComponent.Pivot = new Vector2(5, 15);

        // Act
        var actual = textRendererComponent.TextRectangle;

        // Assert
        Assert.That(textRendererComponent.IsManagedByRenderingSystem, Is.True);
        Assert.That(actual, Is.EqualTo(new AxisAlignedRectangle(55, -105, 100, 200)));
    }

    private Func<ITextLayout> MockCreateTextLayout()
    {
        var textLayouts = new List<ITextLayout>();

        RenderingContext2D.CreateTextLayout(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<FontSize>(), Arg.Any<double>(), Arg.Any<double>())
            .Returns(ci =>
            {
                var text = ci.ArgAt<string>(0);
                var fontFamilyName = ci.ArgAt<string>(1);
                var fontSize = ci.ArgAt<FontSize>(2);
                var maxWidth = ci.ArgAt<double>(3);
                var maxHeight = ci.ArgAt<double>(4);

                var textLayout = Substitute.For<ITextLayout>();
                textLayouts.Add(textLayout);

                textLayout.Text.Returns(text);
                textLayout.FontFamilyName.Returns(fontFamilyName);
                textLayout.FontSize.Returns(fontSize);
                textLayout.MaxWidth.Returns(maxWidth);
                textLayout.MaxHeight.Returns(maxHeight);

                var textMetrics = new TextMetrics
                {
                    Left = 0,
                    Top = 0,
                    Width = maxWidth,
                    Height = maxHeight,
                    LayoutWidth = maxWidth,
                    LayoutHeight = maxHeight,
                    LineCount = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length // Simple line count
                };
                textLayout.Metrics.Returns(textMetrics);

                return textLayout;
            });

        return () => textLayouts[^1];
    }
}