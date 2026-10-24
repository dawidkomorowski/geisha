using System;
using SharpDX.DirectWrite;

namespace Geisha.Engine.Rendering.DirectX.Text;

internal sealed class DirectWriteContext : IDisposable
{
    private readonly Factory _dwFactory;

    public DirectWriteContext()
    {
        _dwFactory = new Factory(FactoryType.Shared);
    }

    public void Dispose()
    {
        _dwFactory.Dispose();
    }
}