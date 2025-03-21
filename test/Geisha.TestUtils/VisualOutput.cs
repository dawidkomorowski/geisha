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

public interface IVisualOutput : IDisposable
{
    void DrawPoint(Vector2 point, Color color);
    void DrawCircle(Circle circle, Color color);
    void DrawRectangle(Rectangle rectangle, Color color);
    void SaveToFile();
}

internal sealed class NullVisualOutput : IVisualOutput
{
    public void DrawPoint(Vector2 point, Color color)
    {
    }

    public void DrawCircle(Circle circle, Color color)
    {
    }

    public void DrawRectangle(Rectangle rectangle, Color color)
    {
    }

    public void SaveToFile()
    {
        TestContext.WriteLine("Visual output is disabled.");
    }

    public void Dispose()
    {
    }
}

internal sealed class VisualOutput : IVisualOutput
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

    public void DrawPoint(Vector2 point, Color color)
    {
        point *= _scale;

        var drawingOptions = CreateDrawingOptions();
        var path = new EllipsePolygon(ConvertPoint(point), 1);
        _image.Mutate(ctx => ctx.Fill(drawingOptions, ConvertColor(color), path));
    }

    public void DrawCircle(Circle circle, Color color)
    {
        circle = circle.Transform(new Transform2D(Vector2.Zero, 0, new Vector2(_scale, _scale)).ToMatrix());

        var drawingOptions = CreateDrawingOptions();
        var pen = Pens.Solid(ConvertColor(color));
        var path = new EllipsePolygon(ConvertPoint(circle.Center), (float)circle.Radius);
        _image.Mutate(ctx => ctx.Draw(drawingOptions, pen, path));
    }

    public void DrawRectangle(Rectangle rectangle, Color color)
    {
        rectangle = rectangle.Transform(new Transform2D(Vector2.Zero, 0, new Vector2(_scale, _scale)).ToMatrix());

        var drawingOptions = CreateDrawingOptions();
        var pen = Pens.Solid(Brushes.Solid(ConvertColor(color)));
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
        _image.Save(filePath, new PngEncoder());
        TestContext.WriteLine($"Visual output saved to file: {filePath}");
    }

    private static SixLabors.ImageSharp.Color ConvertColor(Color color) => new(new Rgba32(color.R, color.G, color.B, color.A));
    private static PointF ConvertPoint(Vector2 point) => new((float)point.X, (float)point.Y);

    private DrawingOptions CreateDrawingOptions() => new()
    {
        Transform = Matrix3x2.CreateTranslation(_image.Width / 2f, -_image.Height / 2f) * Matrix3x2.CreateScale(1, -1)
    };

    public void Dispose()
    {
        _image.Dispose();
    }
}