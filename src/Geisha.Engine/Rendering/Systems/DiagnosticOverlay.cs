using Geisha.Engine.Core.Diagnostics;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Rendering.Backend;

namespace Geisha.Engine.Rendering.Systems;

internal sealed class DiagnosticOverlay
{
    private readonly IRenderingContext2D _renderingContext2D;
    private readonly IAggregatedDiagnosticInfoProvider _aggregatedDiagnosticInfoProvider;

    private const string FontFamily = "Consolas";
    private readonly FontSize _fontSize;

    private readonly double _glyphWidth;
    private readonly double _lineHeight;

    public DiagnosticOverlay(IRenderingContext2D renderingContext2D, IAggregatedDiagnosticInfoProvider aggregatedDiagnosticInfoProvider)
    {
        _renderingContext2D = renderingContext2D;
        _aggregatedDiagnosticInfoProvider = aggregatedDiagnosticInfoProvider;

        _fontSize = FontSize.FromDips(20);

        var screenSize = _renderingContext2D.ScreenSize;
        using var textLayout = _renderingContext2D.CreateTextLayout("X", FontFamily, _fontSize, screenSize.Width, screenSize.Height);
        _glyphWidth = textLayout.Metrics.Width;
        _lineHeight = textLayout.Metrics.Height;
    }

    public void Draw()
    {
        const double margin = 4;
        const double padding = 4;

        var screenSize = _renderingContext2D.ScreenSize;
        var translation = new Vector2(-(screenSize.Width / 2d) + margin, screenSize.Height / 2d - margin);

        foreach (var diagnosticInfo in _aggregatedDiagnosticInfoProvider.GetAllDiagnosticInfo())
        {
            var text = diagnosticInfo.ToString();
            var rectSize = new Vector2(text.Length * _glyphWidth + padding * 2, _lineHeight + padding * 2);
            var rectCenter = translation + new Vector2(rectSize.X * 0.5, -rectSize.Y * 0.5);

            _renderingContext2D.DrawRectangle(new AxisAlignedRectangle(rectCenter, rectSize), Color.Green, true, Matrix3x3.Identity);

            var textTransform = Matrix3x3.CreateTranslation(translation + new Vector2(padding, -padding));
            _renderingContext2D.DrawText(text, FontFamily, _fontSize, Color.White, textTransform);

            translation -= new Vector2(0, rectSize.Y + margin);
        }
    }
}