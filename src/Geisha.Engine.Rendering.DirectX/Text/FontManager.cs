using System;
using System.Collections.Generic;
using SharpDX.DirectWrite;

namespace Geisha.Engine.Rendering.DirectX.Text;

internal sealed class FontManager : IDisposable
{
    private readonly Factory _dwFactory;
    private readonly FontCollection _systemFontCollection;

    public FontManager(Factory dwFactory)
    {
        _dwFactory = dwFactory;
        _systemFontCollection = _dwFactory.GetSystemFontCollection(true);

        //var fontCollectionLoader = new CustomFontCollectionLoader();
        //_dwFactory.RegisterFontCollectionLoader(fontCollectionLoader);
        //_fontCollection = new FontCollection(_dwFactory, fontCollectionLoader, fontCollectionLoader.Key);
    }

    public void RegisterCustomFonts(IEnumerable<string> fontPaths)
    {
    }

    public FontCollection GetFontCollection(string fontFamilyName)
    {
        return _systemFontCollection;
    }

    public void Dispose()
    {
    }
}