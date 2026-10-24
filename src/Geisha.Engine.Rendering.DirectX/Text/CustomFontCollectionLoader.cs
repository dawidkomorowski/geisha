using SharpDX;
using SharpDX.DirectWrite;
using System.Collections.Generic;

namespace Geisha.Engine.Rendering.DirectX.Text;

internal sealed class CustomFontCollectionLoader : CallbackBase, FontCollectionLoader
{
    private readonly IEnumerable<string> _fontPaths;

    public CustomFontCollectionLoader(IEnumerable<string> fontPaths)
    {
        _fontPaths = fontPaths;
    }

    public DataStream Key { get; } = new(4, false, false);

    public FontFileEnumerator CreateEnumeratorFromKey(Factory factory, DataPointer collectionKey)
    {
        return new Enumerator(factory, _fontPaths.GetEnumerator());
    }

    private sealed class Enumerator : CallbackBase, FontFileEnumerator
    {
        private readonly Factory _factory;
        private readonly IEnumerator<string> _fontPathsEnumerator;

        public Enumerator(Factory factory, IEnumerator<string> fontPathsEnumerator)
        {
            _factory = factory;
            _fontPathsEnumerator = fontPathsEnumerator;
        }

        public bool MoveNext()
        {
            var hasElement = _fontPathsEnumerator.MoveNext();
            if (!hasElement)
            {
                return hasElement;
            }

            CurrentFontFile = new FontFile(_factory, _fontPathsEnumerator.Current);

            return hasElement;
        }

        public FontFile? CurrentFontFile { get; private set; }
    }
}