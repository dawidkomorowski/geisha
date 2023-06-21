﻿using Geisha.Engine.Core.Math;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Backend;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Rendering.Systems
{
    public partial class RenderingSystemTests
    {
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

            var textLayout = Substitute.For<ITextLayout>();
            _renderingContext2D.CreateTextLayout(text, fontFamilyName, fontSize, maxWidth, maxHeight).Returns(textLayout);

            var (renderingSystem, renderingScene) = GetRenderingSystem();
            renderingScene.AddCamera();
            var (entity, textRendererComponent) = renderingScene.AddText();

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
            renderingSystem.RenderScene();

            // Assert
            textLayout.Received(1).TextAlignment = textAlignment;
            textLayout.Received(1).ParagraphAlignment = paragraphAlignment;
            textLayout.Received(1).Pivot = pivot;
            _renderingContext2D.Received(1).DrawTextLayout(textLayout, color, entity.Get2DTransformationMatrix(), clipToLayoutBox);
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

            var textLayout = Substitute.For<ITextLayout>();
            _renderingContext2D.CreateTextLayout(text, Arg.Any<string>(), Arg.Any<FontSize>(), Arg.Any<double>(), Arg.Any<double>()).Returns(textLayout);

            var (renderingSystem, renderingScene) = GetRenderingSystem();
            renderingScene.AddCamera();
            var (entity, textRendererComponent) = renderingScene.AddText();

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
            renderingSystem.RenderScene();

            // Assert
            textLayout.Received(1).FontFamilyName = fontFamilyName;
            textLayout.Received(1).FontSize = fontSize;
            textLayout.Received(1).MaxWidth = maxWidth;
            textLayout.Received(1).MaxHeight = maxHeight;
            textLayout.Received(1).TextAlignment = textAlignment;
            textLayout.Received(1).ParagraphAlignment = paragraphAlignment;
            textLayout.Received(1).Pivot = pivot;
            _renderingContext2D.Received(1).DrawTextLayout(textLayout, color, entity.Get2DTransformationMatrix(), clipToLayoutBox);
        }

        [Test]
        public void RenderingSystem_ShouldDisposeTextLayout_WhenTextRendererComponentIsRemovedFromEntity()
        {
            // Arrange
            var textLayout = Substitute.For<ITextLayout>();
            _renderingContext2D.CreateTextLayout(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<FontSize>(), Arg.Any<double>(), Arg.Any<double>())
                .Returns(textLayout);

            var (_, renderingScene) = GetRenderingSystem();
            var (entity, textRendererComponent) = renderingScene.AddText();

            textLayout.ClearReceivedCalls();

            // Act
            entity.RemoveComponent(textRendererComponent);

            // Assert
            textLayout.Received(1).Dispose();
        }

        [Test]
        public void RenderingSystem_ShouldKeepTextRendererComponentState_WhenRenderingSystemIsRemovedFromSceneObserversOfScene()
        {
            // Arrange
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

            var textLayout = Substitute.For<ITextLayout>();
            _renderingContext2D.CreateTextLayout(text, Arg.Any<string>(), Arg.Any<FontSize>(), Arg.Any<double>(), Arg.Any<double>())
                .Returns(textLayout);
            textLayout.Text.Returns(text);

            var (renderingSystem, renderingScene) = GetRenderingSystem();
            var (_, textRendererComponent) = renderingScene.AddText();

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
            renderingScene.Scene.RemoveObserver(renderingSystem);

            // Assert
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

            var textLayout = Substitute.For<ITextLayout>();
            _renderingContext2D.CreateTextLayout(text, Arg.Any<string>(), Arg.Any<FontSize>(), Arg.Any<double>(), Arg.Any<double>())
                .Returns(textLayout);
            textLayout.Text.Returns(text);

            var (renderingSystem, renderingScene) = GetRenderingSystem();
            renderingScene.Scene.RemoveObserver(renderingSystem);

            var (_, textRendererComponent) = renderingScene.AddText();

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
            renderingScene.Scene.AddObserver(renderingSystem);

            // Assert
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
        public void TextRendererComponent_TextMetrics_ShouldReturnDefaultValue_WhenRenderingSystemIsNotAddedToSceneObservers()
        {
            // Arrange
            var textMetrics = new TextMetrics
            {
                Left = 100
            };
            var textLayout = Substitute.For<ITextLayout>();
            textLayout.Metrics.Returns(textMetrics);

            _renderingContext2D.CreateTextLayout(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<FontSize>(), Arg.Any<double>(), Arg.Any<double>())
                .Returns(textLayout);

            var (renderingSystem, renderingScene) = GetRenderingSystem();
            var (_, textRendererComponent) = renderingScene.AddText();
            renderingScene.Scene.RemoveObserver(renderingSystem);

            // Act
            var actual = textRendererComponent.TextMetrics;

            // Assert
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

            _renderingContext2D.CreateTextLayout(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<FontSize>(), Arg.Any<double>(), Arg.Any<double>())
                .Returns(textLayout);

            var (_, renderingScene) = GetRenderingSystem();
            var (_, textRendererComponent) = renderingScene.AddText();

            // Act
            var actual = textRendererComponent.TextMetrics;

            // Assert
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

            _renderingContext2D.CreateTextLayout(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<FontSize>(), Arg.Any<double>(), Arg.Any<double>())
                .Returns(textLayout);

            var (renderingSystem, renderingScene) = GetRenderingSystem();
            var (_, textRendererComponent) = renderingScene.AddText();
            textRendererComponent.MaxWidth = 150;
            textRendererComponent.MaxHeight = 250;
            textRendererComponent.Pivot = new Vector2(5, 15);

            renderingScene.Scene.RemoveObserver(renderingSystem);

            // Act
            var actual = textRendererComponent.LayoutRectangle;

            // Assert
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

            _renderingContext2D.CreateTextLayout(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<FontSize>(), Arg.Any<double>(), Arg.Any<double>())
                .Returns(textLayout);

            var (_, renderingScene) = GetRenderingSystem();
            var (_, textRendererComponent) = renderingScene.AddText();
            textRendererComponent.MaxWidth = 150;
            textRendererComponent.MaxHeight = 250;
            textRendererComponent.Pivot = new Vector2(5, 15);

            // Act
            var actual = textRendererComponent.LayoutRectangle;

            // Assert
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

            _renderingContext2D.CreateTextLayout(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<FontSize>(), Arg.Any<double>(), Arg.Any<double>())
                .Returns(textLayout);

            var (renderingSystem, renderingScene) = GetRenderingSystem();
            var (_, textRendererComponent) = renderingScene.AddText();
            textRendererComponent.MaxWidth = 150;
            textRendererComponent.MaxHeight = 250;
            textRendererComponent.Pivot = new Vector2(5, 15);

            renderingScene.Scene.RemoveObserver(renderingSystem);

            // Act
            var actual = textRendererComponent.TextRectangle;

            // Assert
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

            _renderingContext2D.CreateTextLayout(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<FontSize>(), Arg.Any<double>(), Arg.Any<double>())
                .Returns(textLayout);

            var (_, renderingScene) = GetRenderingSystem();
            var (_, textRendererComponent) = renderingScene.AddText();
            textRendererComponent.MaxWidth = 150;
            textRendererComponent.MaxHeight = 250;
            textRendererComponent.Pivot = new Vector2(5, 15);

            // Act
            var actual = textRendererComponent.TextRectangle;

            // Assert
            Assert.That(actual, Is.EqualTo(new AxisAlignedRectangle(55, -105, 100, 200)));
        }
    }
}