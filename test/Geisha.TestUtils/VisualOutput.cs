using System;
using System.IO;
using System.Numerics;
using Geisha.Engine.Core.Math;
using NUnit.Framework;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Color = Geisha.Engine.Core.Math.Color;
using Rectangle = Geisha.Engine.Core.Math.Rectangle;
using Vector2 = Geisha.Engine.Core.Math.Vector2;

namespace Geisha.TestUtils;

public sealed class VisualOutput : IDisposable
{
    private readonly float _scale;
    private readonly Image _image;

    internal VisualOutput(double scale)
    {
        _scale = (float)scale;
        _image = new Image<Rgba32>(1280, 768);
        _image.Mutate(ctx => ctx
            .Fill(SixLabors.ImageSharp.Color.White)
            .DrawLine(SixLabors.ImageSharp.Color.Black, 1, new PointF(0, _image.Height / 2f), new PointF(_image.Width, _image.Height / 2f))
            .DrawLine(SixLabors.ImageSharp.Color.Black, 1, new PointF(_image.Width / 2f, 0), new PointF(_image.Width / 2f, _image.Height))
        );
    }

    public void DrawCircle(Circle circle, Color color)
    {
        var drawingOptions = CreateDrawingOptions();
        var pen = Pens.Solid(ConvertColor(color));
        var path = new EllipsePolygon(ConvertPoint(circle.Center), (float)circle.Radius);
        _image.Mutate(ctx => ctx.Draw(drawingOptions, pen, path));
    }

    public void DrawRectangle(Rectangle rectangle, Color color)
    {
        var drawingOptions = CreateDrawingOptions();
        var pen = Pens.Solid(ConvertColor(color));
        var path = new Polygon(new[]
        {
            ConvertPoint(rectangle.LowerLeft),
            ConvertPoint(rectangle.LowerRight),
            ConvertPoint(rectangle.UpperRight),
            ConvertPoint(rectangle.UpperLeft)
        });
        _image.Mutate(ctx => ctx.Draw(drawingOptions, pen, path));
    }

    public void SaveToFile()
    {
        var outputDirectory = System.IO.Path.Combine(Utils.TestDirectory, "VisualOutput");
        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }

        var filePath = System.IO.Path.Combine(outputDirectory, $"{TestContext.CurrentContext.Test.Name}.png");
        TestContext.Out.WriteLine($"Visual output saved to file: {filePath}");
        _image.Save(filePath, new PngEncoder());
    }

    private static SixLabors.ImageSharp.Color ConvertColor(Color color) => new(new Rgba32(color.R, color.G, color.B, color.A));
    private static PointF ConvertPoint(Vector2 point) => new((float)point.X, (float)point.Y);

    private DrawingOptions CreateDrawingOptions() => new()
    {
        Transform = Matrix3x2.CreateTranslation(_image.Width / (2 * _scale), -_image.Height / (2 * _scale)) * Matrix3x2.CreateScale(_scale, -_scale)
    };

    public void Dispose()
    {
        _image.Dispose();
    }
}