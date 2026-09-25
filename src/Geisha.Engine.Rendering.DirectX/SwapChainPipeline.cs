using System;
using System.Diagnostics;
using System.Threading;
using Geisha.Engine.Core.Math;
using Microsoft.Win32.SafeHandles;
using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using Factory5 = SharpDX.DXGI.Factory5;
using Feature = SharpDX.DXGI.Feature;

namespace Geisha.Engine.Rendering.DirectX;

internal sealed class SwapChainPipeline : IDisposable
{
    private const int BufferCount = 2;
    private const Format PixelFormat = Format.B8G8R8A8_UNorm;
    private const SwapChainFlags SwapChainFlags = SharpDX.DXGI.SwapChainFlags.AllowTearing | SharpDX.DXGI.SwapChainFlags.FrameLatencyWaitAbleObject;
    private const int SampleCount = 4;

    private readonly DeviceContext _deviceContext;
    private readonly SwapChain1 _swapChain;
    private readonly SafeWaitHandle _frameLatencyWaitHandle;
    private readonly EventWaitHandle _frameLatencyWaitEvent;

    private Texture2D _msaaTargetTexture;
    private Bitmap1 _msaaTargetBitmap;

    private Texture2D _resolveTexture;
    private Bitmap1 _resolveBitmap;

    private Bitmap1 _backBufferBitmap;

    public SwapChainPipeline(DeviceContext deviceContext, Size resolution, IntPtr windowHandle)
    {
        _deviceContext = deviceContext;

        using var dxgiDevice = _deviceContext.D3D11Device.QueryInterface<SharpDX.DXGI.Device>();
        using var dxgiAdapter = dxgiDevice.Adapter;
        using var dxgiFactory = dxgiAdapter.GetParent<Factory5>();
        dxgiFactory.MakeWindowAssociation(windowHandle, WindowAssociationFlags.IgnoreAll); // Ignore all window events.

        if (!IsTearingSupported(dxgiFactory))
        {
            throw new NotSupportedException("Tearing is not supported on this device.");
        }

        var formatSupport = _deviceContext.D3D11Device.CheckFormatSupport(PixelFormat);
        if (!formatSupport.HasFlag(FormatSupport.MultisampleRenderTarget))
        {
            throw new NotSupportedException("Multisampling is not supported on this device.");
        }

        if (_deviceContext.D3D11Device.CheckMultisampleQualityLevels(PixelFormat, SampleCount) == 0)
        {
            throw new NotSupportedException("Multisampling is not supported on this device.");
        }

        var swapChainDescription = new SwapChainDescription1
        {
            Width = resolution.Width,
            Height = resolution.Height,
            Format = PixelFormat,
            SampleDescription = new SampleDescription(1, 0),
            Usage = Usage.RenderTargetOutput,
            BufferCount = BufferCount,
            Scaling = Scaling.Stretch,
            SwapEffect = SwapEffect.FlipDiscard,
            Flags = SwapChainFlags
        };

        _swapChain = new SwapChain1(dxgiFactory, dxgiDevice, windowHandle, ref swapChainDescription);

        using var swapChain2 = _swapChain.QueryInterface<SwapChain2>();
        var waitableObject = swapChain2.FrameLatencyWaitableObject;
        _frameLatencyWaitHandle = new SafeWaitHandle(waitableObject, false);
        _frameLatencyWaitEvent = new EventWaitHandle(false, EventResetMode.ManualReset);
        _frameLatencyWaitEvent.SafeWaitHandle = _frameLatencyWaitHandle;
        swapChain2.MaximumFrameLatency = 1;

        CreateBitmaps(resolution);

        _deviceContext.D2D1DeviceContext.Target = _msaaTargetBitmap;

        Debug.Assert(_msaaTargetTexture is not null);
        Debug.Assert(_msaaTargetBitmap is not null);
        Debug.Assert(_resolveTexture is not null);
        Debug.Assert(_resolveBitmap is not null);
        Debug.Assert(_backBufferBitmap is not null);
    }

    public bool VSyncEnabled { get; set; }

    public void Present()
    {
        _deviceContext.D2D1DeviceContext.Target = null;

        _deviceContext.D3D11DeviceContext.ResolveSubresource(_msaaTargetTexture, 0, _resolveTexture, 0, PixelFormat);

        _deviceContext.D2D1DeviceContext.Target = _backBufferBitmap;

        _deviceContext.D2D1DeviceContext.BeginDraw();
        _deviceContext.D2D1DeviceContext.Clear(new RawColor4(0, 0, 0, 1));
        _deviceContext.D2D1DeviceContext.Transform = new RawMatrix3x2(1, 0, 0, 1, 0, 0);
        _deviceContext.D2D1DeviceContext.DrawBitmap(_resolveBitmap, 1.0f, SharpDX.Direct2D1.BitmapInterpolationMode.Linear);
        _deviceContext.D2D1DeviceContext.EndDraw();

        _deviceContext.D2D1DeviceContext.Target = _msaaTargetBitmap;

        if (VSyncEnabled)
        {
            _swapChain.Present(1, PresentFlags.None);
        }
        else
        {
            _swapChain.Present(0, PresentFlags.AllowTearing);
        }

        // Wait for the presentation to complete before working on next frame.
        _frameLatencyWaitEvent.WaitOne(1000);
    }

    public void ResizeBuffers(Size size)
    {
        DestroyBitmaps();

        _swapChain.ResizeBuffers(BufferCount, size.Width, size.Height, PixelFormat, SwapChainFlags);

        CreateBitmaps(size);

        _deviceContext.D2D1DeviceContext.Target = _msaaTargetBitmap;
    }

    public void Dispose()
    {
        DestroyBitmaps();

        _frameLatencyWaitEvent.Dispose();
        _frameLatencyWaitHandle.Dispose();

        _swapChain.Dispose();
    }

    private void CreateBitmaps(Size size)
    {
        _msaaTargetTexture = _deviceContext.CreateTexture(size, BindFlags.RenderTarget, SampleCount);
        _msaaTargetBitmap = _deviceContext.CreateBitmap(_msaaTargetTexture, BitmapOptions.Target | BitmapOptions.CannotDraw);

        _resolveTexture = _deviceContext.CreateTexture(size, BindFlags.ShaderResource, 1);
        _resolveBitmap = _deviceContext.CreateBitmap(_resolveTexture, BitmapOptions.None);

        // It is safe to cache the back buffer reference in DX 11.
        using var backBufferSurface = _swapChain.GetBackBuffer<Surface>(0);
        _backBufferBitmap = _deviceContext.CreateBitmap(backBufferSurface, BitmapOptions.Target | BitmapOptions.CannotDraw);
    }

    private void DestroyBitmaps()
    {
        _deviceContext.D2D1DeviceContext.Target = null;

        _backBufferBitmap.Dispose();
        _resolveBitmap.Dispose();
        _resolveTexture.Dispose();
        _msaaTargetBitmap.Dispose();
        _msaaTargetTexture.Dispose();
    }

    private static unsafe bool IsTearingSupported(Factory5 dxgiFactory)
    {
        RawBool allowTearing = false;
        dxgiFactory.CheckFeatureSupport(Feature.PresentAllowTearing, new IntPtr(&allowTearing), sizeof(RawBool));
        return allowTearing;
    }
}