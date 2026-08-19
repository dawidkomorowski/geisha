using System;
using SharpDX;
using SharpDX.Direct2D1;

namespace Geisha.Engine.Rendering.DirectX;

internal sealed class DeviceContext : IDisposable
{
    public DeviceContext(SharpDX.Direct3D11.Device d3D11Device)
    {
        D3D11Device = d3D11Device;
        D3D11DeviceContext = d3D11Device.ImmediateContext;

        using var dxgiDevice = d3D11Device.QueryInterface<SharpDX.DXGI.Device>();
        using var d2D1Factory = CreateD2D1Factory(FactoryType.SingleThreaded, DebugLevel.None);
        using var d2D1Device = new Device3(d2D1Factory, dxgiDevice);

        D2D1DeviceContext = new DeviceContext3(d2D1Device, DeviceContextOptions.None);
        D2D1DeviceContext.AntialiasMode = AntialiasMode.Aliased;
    }

    public SharpDX.Direct3D11.Device D3D11Device { get; }
    public SharpDX.Direct3D11.DeviceContext D3D11DeviceContext { get; }
    public DeviceContext3 D2D1DeviceContext { get; }

    private static Factory4 CreateD2D1Factory(FactoryType factoryType, DebugLevel debugLevel)
    {
        FactoryOptions? factoryOptionsRef = null;
        if (debugLevel != DebugLevel.None)
        {
            factoryOptionsRef = new FactoryOptions
            {
                DebugLevel = debugLevel
            };
        }

        D2D1.CreateFactory(factoryType, Utilities.GetGuidFromType(typeof(Factory4)), factoryOptionsRef, out var iFactoryOut);
        return new Factory4(iFactoryOut);
    }

    public void Dispose()
    {
        D2D1DeviceContext.Dispose();
        D3D11DeviceContext.Dispose();
    }
}