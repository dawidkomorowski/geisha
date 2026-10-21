using SharpDX;
using SharpDX.DirectWrite;

namespace Geisha.Engine.Rendering.DirectX;

internal sealed class FontLoader : CallbackBase, FontCollectionLoader
{
    public FontLoader()
    {
        Key = new DataStream(sizeof(int) * 1, true, true);
        Key.Write(0);
        Key.Position = 0;
    }

    public DataStream Key { get; }

    public FontFileEnumerator CreateEnumeratorFromKey(Factory factory, DataPointer collectionKey)
    {
        return new Enumerator(factory);
    }

    private sealed class Enumerator : CallbackBase, FontFileEnumerator
    {
        private readonly Factory _factory;
        private bool _done;

        public Enumerator(Factory factory)
        {
            _factory = factory;
            CurrentFontFile = new FontFile(_factory, @"C:\Users\Dawid\Downloads\kenney_kenney-fonts\Fonts\Kenney Pixel.ttf");
        }

        public bool MoveNext()
        {
            if (_done)
            {
                return false;
            }

            _done = true;
            return true;
        }

        public FontFile CurrentFontFile { get; }
    }
}