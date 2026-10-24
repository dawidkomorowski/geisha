using System.Collections.Generic;
using SharpDX;
using SharpDX.DirectWrite;

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

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Key.Dispose();
        }

        base.Dispose(disposing);
    }

    private sealed class Enumerator : CallbackBase, FontFileEnumerator
    {
        private readonly Factory _factory;
        private readonly IEnumerator<string> _fontPathsEnumerator;
        private bool _sourceEnumeratorDisposed;
        private FontFile? _currentFontFile;

        public Enumerator(Factory factory, IEnumerator<string> fontPathsEnumerator)
        {
            _factory = factory;
            _fontPathsEnumerator = fontPathsEnumerator;
        }

        public bool MoveNext()
        {
            if (_sourceEnumeratorDisposed)
            {
                return false;
            }

            _currentFontFile?.Dispose();
            _currentFontFile = null;

            var hasElement = _fontPathsEnumerator.MoveNext();
            if (!hasElement)
            {
                _fontPathsEnumerator.Dispose();
                _sourceEnumeratorDisposed = true;
                return false;
            }

            _currentFontFile = new FontFile(_factory, _fontPathsEnumerator.Current);

            return true;
        }

        public FontFile? CurrentFontFile
        {
            get
            {
                if (_currentFontFile is null)
                {
                    return null;
                }

                ((IUnknown)_currentFontFile).AddReference();
                return _currentFontFile;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !_sourceEnumeratorDisposed)
            {
                _currentFontFile?.Dispose();
                _currentFontFile = null;

                _fontPathsEnumerator.Dispose();
                _sourceEnumeratorDisposed = true;
            }

            base.Dispose(disposing);
        }
    }
}