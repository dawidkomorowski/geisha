using System;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Backend;
using NUnit.Framework;

namespace Geisha.Engine.IntegrationTests.Rendering
{
    internal sealed class TextLayoutIntegrationTestsSut
    {
        public TextLayoutIntegrationTestsSut(IRenderingBackend renderingBackend)
        {
            RenderingBackend = renderingBackend;
        }

        public IRenderingBackend RenderingBackend { get; }
    }

    [TestFixture]
    internal class TextLayoutIntegrationTests : IntegrationTests<TextLayoutIntegrationTestsSut>
    {
        private IRenderingContext2D Context2D => SystemUnderTest.RenderingBackend.Context2D;

        [TestCase("Layout 1", "Arial", 10, 100, 200)]
        [TestCase("Layout 2", "Consolas", 20, 200, 400)]
        public void TextLayout_ShouldBeCreatedWithSpecifiedProperties(string text, string fontFamilyName, double fontSize, double maxWidth, double maxHeight)
        {
            // Arrange
            var fontSizeStruct = FontSize.FromDips(fontSize);

            // Act
            using var textLayout = Context2D.CreateTextLayout(text, fontFamilyName, fontSizeStruct, maxWidth, maxHeight);

            // Assert
            Assert.That(textLayout.Text, Is.EqualTo(text));
            Assert.That(textLayout.FontFamilyName, Is.EqualTo(fontFamilyName));
            Assert.That(textLayout.FontSize, Is.EqualTo(fontSizeStruct));
            Assert.That(textLayout.MaxWidth, Is.EqualTo(maxWidth));
            Assert.That(textLayout.MaxHeight, Is.EqualTo(maxHeight));
            Assert.That(textLayout.TextAlignment, Is.EqualTo(TextAlignment.Leading));
            Assert.That(textLayout.ParagraphAlignment, Is.EqualTo(ParagraphAlignment.Near));
        }

        [Test]
        public void TextLayout_FontFamilyName_Get_ShouldThrowException_WhenTextLayoutIsDisposed()
        {
            // Arrange
            var textLayout = CreateTextLayout();
            textLayout.Dispose();

            // Act
            // Assert
            Assert.That(() => _ = textLayout.FontFamilyName, Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void TextLayout_FontFamilyName_Set_ShouldThrowException_WhenTextLayoutIsDisposed()
        {
            // Arrange
            var textLayout = CreateTextLayout();
            textLayout.Dispose();

            // Act
            // Assert
            Assert.That(() => textLayout.FontFamilyName = "Arial", Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void TextLayout_FontFamilyName_ShouldGetAndSet()
        {
            // Arrange
            using var textLayout = CreateTextLayout();
            var expected1 = "Arial";
            var expected2 = "Calibri";

            // Act
            textLayout.FontFamilyName = expected1;
            var actual1 = textLayout.FontFamilyName;
            textLayout.FontFamilyName = expected2;
            var actual2 = textLayout.FontFamilyName;

            // Assert
            Assert.That(actual1, Is.EqualTo(expected1));
            Assert.That(actual2, Is.EqualTo(expected2));
        }

        [Test]
        public void TextLayout_FontSize_Get_ShouldThrowException_WhenTextLayoutIsDisposed()
        {
            // Arrange
            var textLayout = CreateTextLayout();
            textLayout.Dispose();

            // Act
            // Assert
            Assert.That(() => _ = textLayout.FontSize, Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void TextLayout_FontSize_Set_ShouldThrowException_WhenTextLayoutIsDisposed()
        {
            // Arrange
            var textLayout = CreateTextLayout();
            textLayout.Dispose();

            // Act
            // Assert
            Assert.That(() => textLayout.FontSize = FontSize.FromDips(20), Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void TextLayout_FontSize_ShouldGetAndSet()
        {
            // Arrange
            using var textLayout = CreateTextLayout();
            var expected1 = FontSize.FromDips(10);
            var expected2 = FontSize.FromDips(20);

            // Act
            textLayout.FontSize = expected1;
            var actual1 = textLayout.FontSize;
            textLayout.FontSize = expected2;
            var actual2 = textLayout.FontSize;

            // Assert
            Assert.That(actual1, Is.EqualTo(expected1));
            Assert.That(actual2, Is.EqualTo(expected2));
        }

        [Test]
        public void TextLayout_MaxWidth_Get_ShouldThrowException_WhenTextLayoutIsDisposed()
        {
            // Arrange
            var textLayout = CreateTextLayout();
            textLayout.Dispose();

            // Act
            // Assert
            Assert.That(() => _ = textLayout.MaxWidth, Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void TextLayout_MaxWidth_Set_ShouldThrowException_WhenTextLayoutIsDisposed()
        {
            // Arrange
            var textLayout = CreateTextLayout();
            textLayout.Dispose();

            // Act
            // Assert
            Assert.That(() => textLayout.MaxWidth = 100, Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void TextLayout_MaxWidth_ShouldGetAndSet()
        {
            // Arrange
            using var textLayout = CreateTextLayout();
            var expected1 = 100;
            var expected2 = 200;

            // Act
            textLayout.MaxWidth = expected1;
            var actual1 = textLayout.MaxWidth;
            textLayout.MaxWidth = expected2;
            var actual2 = textLayout.MaxWidth;

            // Assert
            Assert.That(actual1, Is.EqualTo(expected1));
            Assert.That(actual2, Is.EqualTo(expected2));
        }

        [Test]
        public void TextLayout_MaxHeight_Get_ShouldThrowException_WhenTextLayoutIsDisposed()
        {
            // Arrange
            var textLayout = CreateTextLayout();
            textLayout.Dispose();

            // Act
            // Assert
            Assert.That(() => _ = textLayout.MaxHeight, Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void TextLayout_MaxHeight_Set_ShouldThrowException_WhenTextLayoutIsDisposed()
        {
            // Arrange
            var textLayout = CreateTextLayout();
            textLayout.Dispose();

            // Act
            // Assert
            Assert.That(() => textLayout.MaxHeight = 100, Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void TextLayout_MaxHeight_ShouldGetAndSet()
        {
            // Arrange
            using var textLayout = CreateTextLayout();
            var expected1 = 100;
            var expected2 = 200;

            // Act
            textLayout.MaxHeight = expected1;
            var actual1 = textLayout.MaxHeight;
            textLayout.MaxHeight = expected2;
            var actual2 = textLayout.MaxHeight;

            // Assert
            Assert.That(actual1, Is.EqualTo(expected1));
            Assert.That(actual2, Is.EqualTo(expected2));
        }

        [Test]
        public void TextLayout_TextAlignment_Get_ShouldThrowException_WhenTextLayoutIsDisposed()
        {
            // Arrange
            var textLayout = CreateTextLayout();
            textLayout.Dispose();

            // Act
            // Assert
            Assert.That(() => _ = textLayout.TextAlignment, Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void TextLayout_TextAlignment_Set_ShouldThrowException_WhenTextLayoutIsDisposed()
        {
            // Arrange
            var textLayout = CreateTextLayout();
            textLayout.Dispose();

            // Act
            // Assert
            Assert.That(() => textLayout.TextAlignment = TextAlignment.Center, Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void TextLayout_TextAlignment_ShouldGetAndSet()
        {
            // Arrange
            using var textLayout = CreateTextLayout();
            var expected1 = TextAlignment.Center;
            var expected2 = TextAlignment.Justified;

            // Act
            textLayout.TextAlignment = expected1;
            var actual1 = textLayout.TextAlignment;
            textLayout.TextAlignment = expected2;
            var actual2 = textLayout.TextAlignment;

            // Assert
            Assert.That(actual1, Is.EqualTo(expected1));
            Assert.That(actual2, Is.EqualTo(expected2));
        }

        [Test]
        public void TextLayout_ParagraphAlignment_Get_ShouldThrowException_WhenTextLayoutIsDisposed()
        {
            // Arrange
            var textLayout = CreateTextLayout();
            textLayout.Dispose();

            // Act
            // Assert
            Assert.That(() => _ = textLayout.ParagraphAlignment, Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void TextLayout_ParagraphAlignment_Set_ShouldThrowException_WhenTextLayoutIsDisposed()
        {
            // Arrange
            var textLayout = CreateTextLayout();
            textLayout.Dispose();

            // Act
            // Assert
            Assert.That(() => textLayout.ParagraphAlignment = ParagraphAlignment.Center, Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void TextLayout_ParagraphAlignment_ShouldGetAndSet()
        {
            // Arrange
            using var textLayout = CreateTextLayout();
            var expected1 = ParagraphAlignment.Center;
            var expected2 = ParagraphAlignment.Far;

            // Act
            textLayout.ParagraphAlignment = expected1;
            var actual1 = textLayout.ParagraphAlignment;
            textLayout.ParagraphAlignment = expected2;
            var actual2 = textLayout.ParagraphAlignment;

            // Assert
            Assert.That(actual1, Is.EqualTo(expected1));
            Assert.That(actual2, Is.EqualTo(expected2));
        }

        [Test]
        public void TextLayout_Metrics_Get_ShouldThrowException_WhenTextLayoutIsDisposed()
        {
            // Arrange
            var textLayout = CreateTextLayout();
            textLayout.Dispose();

            // Act
            // Assert
            Assert.That(() => _ = textLayout.Metrics, Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void TextLayout_Metrics_Get_ShouldReturnMetricsForSpecifiedTextLayout()
        {
            // Arrange
            var text =
                @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nulla malesuada pharetra mattis. Donec finibus id mi sed congue. Aenean scelerisque a nulla et sollicitudin. Quisque vitae neque laoreet, dapibus mi sed, vulputate ipsum. Sed varius turpis at erat consequat venenatis. Vivamus id risus sed diam auctor feugiat. Suspendisse sodales sem sit amet elementum cursus. Quisque sit amet elementum nibh, vel elementum dolor. Cras ultrices nibh erat, nec ultricies nisi sodales vitae. Duis iaculis vestibulum risus id sodales. Etiam tempor nisl eu nunc bibendum, sed sodales turpis pharetra. Mauris auctor sapien orci, in finibus dolor iaculis id. Aenean ornare tellus ut feugiat aliquam.

Nunc luctus imperdiet urna semper mattis. Donec at tortor dignissim neque luctus iaculis. Maecenas condimentum libero quis dolor dictum mollis. Proin in feugiat nulla. Suspendisse tincidunt, mi varius auctor accumsan, tellus urna vehicula magna, a auctor urna tellus non metus. Donec metus odio, pharetra nec elit et, molestie tincidunt lorem. Cras blandit nibh sodales varius gravida. Suspendisse facilisis porta ipsum, ut lobortis purus vestibulum vitae. Duis rutrum eros ac nulla varius, nec ultricies elit ultricies. Integer et mi dolor. Vestibulum efficitur diam ullamcorper, finibus libero et, fringilla lectus. Donec bibendum sem quam, vel consectetur elit efficitur quis. Quisque in nibh at massa pellentesque convallis. Curabitur mattis rutrum ligula, id varius mi pharetra vitae. Integer quis ultrices risus. ";

            var textLayout = Context2D.CreateTextLayout(text, "Calibri", FontSize.FromDips(20), 1000, 500);

            // Act
            var actual = textLayout.Metrics;

            // Assert
            Assert.That(actual.Left, Is.Zero);
            Assert.That(actual.Top, Is.Zero);
            Assert.That(actual.Width, Is.EqualTo(994.51171875d));
            Assert.That(actual.Height, Is.EqualTo(341.796875d));
            Assert.That(actual.LayoutWidth, Is.EqualTo(1000));
            Assert.That(actual.LayoutHeight, Is.EqualTo(500));
            Assert.That(actual.LineCount, Is.EqualTo(14));
        }

        private ITextLayout CreateTextLayout()
        {
            return Context2D.CreateTextLayout("Text Layout", "Consolas", FontSize.FromDips(100), 2000, 3000);
        }
    }
}