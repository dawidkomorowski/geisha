using System;
using System.Linq;
using Geisha.Engine.Rendering.Backend;
using SharpDX.DirectWrite;

namespace Geisha.Engine.Rendering.DirectX.Text;

internal sealed class DirectWriteContext : IDisposable
{
    private readonly Factory _dwFactory;
    private readonly FontManager _fontManager;
    private CachedTextFormat _cachedTextFormat;

    public DirectWriteContext()
    {
        _dwFactory = new Factory(FactoryType.Shared);
        _fontManager = new FontManager(_dwFactory);

        _fontManager.RegisterCustomFonts(Enumerable.Empty<string>());
    }

    public ITextLayout CreateTextLayout(string text, string fontFamilyName, FontSize fontSize, double maxWidth, double maxHeight)
    {
        var dwTextFormat = new TextFormat(
            _dwFactory,
            fontFamilyName,
            _fontManager.GetFontCollection(fontFamilyName),
            FontWeight.Normal,
            FontStyle.Normal,
            FontStretch.Normal,
            (float)fontSize.Dips
        );
        var dwTextLayout = new SharpDX.DirectWrite.TextLayout(_dwFactory, text, dwTextFormat, (float)maxWidth, (float)maxHeight);
        return new TextLayout(dwTextFormat, dwTextLayout, text);
    }

    public TextFormat GetCachedTextFormat(string fontFamilyName, FontSize fontSize)
    {
        if (_cachedTextFormat.TextFormat is not null && _cachedTextFormat.FontFamilyName == fontFamilyName && _cachedTextFormat.FontSize == fontSize)
        {
            return _cachedTextFormat.TextFormat;
        }

        _cachedTextFormat.FontFamilyName = fontFamilyName;
        _cachedTextFormat.FontSize = fontSize;

        _cachedTextFormat.TextFormat?.Dispose();
        _cachedTextFormat.TextFormat = new TextFormat(
            _dwFactory,
            fontFamilyName,
            _fontManager.GetFontCollection(fontFamilyName),
            FontWeight.Normal,
            FontStyle.Normal,
            FontStretch.Normal,
            (float)fontSize.Dips
        );

        return _cachedTextFormat.TextFormat;
    }

    public void Dispose()
    {
        _cachedTextFormat.TextFormat?.Dispose();
        _fontManager.Dispose();
        _dwFactory.Dispose();
    }

    private struct CachedTextFormat
    {
        public string? FontFamilyName;
        public FontSize FontSize;
        public TextFormat? TextFormat;
    }
}