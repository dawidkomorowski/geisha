using System;
using System.Collections.Generic;
using SharpDX.DirectWrite;

namespace Geisha.Engine.Rendering.DirectX.Text;

internal sealed class FontManager : IDisposable
{
    private readonly Factory _dwFactory;
    private readonly FontCollection _systemFontCollection;
    private readonly HashSet<string> _registeredCustomFonts;
    private FontCollection? _customFontCollection;
    private CustomFontCollectionLoader? _customFontCollectionLoader;

    public FontManager(Factory dwFactory)
    {
        _dwFactory = dwFactory;
        _systemFontCollection = _dwFactory.GetSystemFontCollection(true);
        _registeredCustomFonts = new HashSet<string>();
    }

    public void RegisterCustomFonts(IEnumerable<string> fontPaths)
    {
        _customFontCollectionLoader = new CustomFontCollectionLoader();
        _dwFactory.RegisterFontCollectionLoader(_customFontCollectionLoader);
        _customFontCollection = new FontCollection(_dwFactory, _customFontCollectionLoader, _customFontCollectionLoader.Key);

        for (var i = 0; i < _customFontCollection.FontFamilyCount; i++)
        {
            using var fontFamily = _customFontCollection.GetFontFamily(i);
            using var fontFamilyNames = fontFamily.FamilyNames;
            if (fontFamilyNames.Count > 0)
            {
                var fontFamilyName = fontFamilyNames.GetString(0);
                _registeredCustomFonts.Add(fontFamilyName);
            }
        }
    }

    public FontCollection GetFontCollection(string fontFamilyName)
    {
        if (_customFontCollection is not null && _registeredCustomFonts.Contains(fontFamilyName))
        {
            return _customFontCollection;
        }

        return _systemFontCollection;
    }

    public void Dispose()
    {
        _registeredCustomFonts.Clear();
        _customFontCollection?.Dispose();

        if (_customFontCollectionLoader is not null)
        {
            _dwFactory.UnregisterFontCollectionLoader(_customFontCollectionLoader);
            _customFontCollectionLoader.Dispose();
        }
    }
}