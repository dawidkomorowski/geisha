using System;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using AlphaMode = SharpDX.Direct2D1.AlphaMode;
using Device3 = SharpDX.Direct2D1.Device3;
using DeviceContext3 = SharpDX.Direct2D1.DeviceContext3;
using Factory4 = SharpDX.Direct2D1.Factory4;
using Size = Geisha.Engine.Core.Math.Size;

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

    public Texture2D CreateTexture(Size size, BindFlags bindFlags, int samples)
    {
        var textureDescription = new Texture2DDescription
        {
            Width = size.Width,
            Height = size.Height,
            MipLevels = 1,
            ArraySize = 1,
            Format = Format.B8G8R8A8_UNorm,
            SampleDescription = new SampleDescription(samples, 0),
            Usage = ResourceUsage.Default,
            BindFlags = bindFlags
        };
        return new Texture2D(D3D11Device, textureDescription);
    }

    public Bitmap1 CreateBitmap(Texture2D texture, BitmapOptions bitmapOptions)
    {
        using var surface = texture.QueryInterface<Surface>();
        return CreateBitmap(surface, bitmapOptions);
    }

    public Bitmap1 CreateBitmap(Surface surface, BitmapOptions bitmapOptions)
    {
        var bitmapProperties = new BitmapProperties1(
            new PixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied),
            D2D1DeviceContext.DotsPerInch.Width,
            D2D1DeviceContext.DotsPerInch.Height,
            bitmapOptions
        );
        return new Bitmap1(D2D1DeviceContext, surface, bitmapProperties);
    }

    public Bitmap1 CreateBitmap(Size size, DataStream dataStream, int pitch, BitmapOptions bitmapOptions)
    {
        var bitmapProperties = new BitmapProperties1(
            new PixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied),
            D2D1DeviceContext.DotsPerInch.Width,
            D2D1DeviceContext.DotsPerInch.Height,
            bitmapOptions
        );
        return new Bitmap1(D2D1DeviceContext, size.ToSize2(), dataStream, pitch, bitmapProperties);
    }

    public Bitmap1 CreateBitmap(Size size, BitmapOptions bitmapOptions)
    {
        var bitmapProperties = new BitmapProperties1(
            new PixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied),
            D2D1DeviceContext.DotsPerInch.Width,
            D2D1DeviceContext.DotsPerInch.Height,
            bitmapOptions
        );
        return new Bitmap1(D2D1DeviceContext, size.ToSize2(), bitmapProperties);
    }

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