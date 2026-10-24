using System;
using SharpDX.DirectWrite;

namespace Geisha.Engine.Rendering.DirectX.Text;

internal sealed class DirectWriteContext : IDisposable
{
    private readonly Factory _dwFactory;
    private CachedTextFormat _cachedTextFormat;

    public DirectWriteContext()
    {
        _dwFactory = new Factory(FactoryType.Shared);
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
            _dwFactory.GetSystemFontCollection(false),
            FontWeight.Normal,
            FontStyle.Normal,
            FontStretch.Normal,
            (float)fontSize.Dips
        );

        return _cachedTextFormat.TextFormat;
    }

    public void Dispose()
    {
        _dwFactory.Dispose();
    }

    private struct CachedTextFormat
    {
        public string? FontFamilyName;
        public FontSize FontSize;
        public TextFormat? TextFormat;
    }
}