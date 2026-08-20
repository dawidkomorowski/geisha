using System;
using System.Threading;
using Geisha.Engine.Core.Math;
using Microsoft.Win32.SafeHandles;
using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;

namespace Geisha.Engine.Rendering.DirectX;

internal sealed class SwapChainPipeline : IDisposable
{
    private readonly DeviceContext _deviceContext;
    private readonly SwapChain1 _swapChain;
    private readonly SafeWaitHandle _frameLatencyWaitHandle;
    private readonly EventWaitHandle _frameLatencyWaitEvent;

    private readonly Texture2D _msaaTargetTexture;
    private readonly Bitmap1 _msaaTargetBitmap;

    private readonly Texture2D _resolveTexture;
    private readonly Bitmap1 _resolveBitmap;

    private readonly Bitmap1 _backBufferBitmap;

    public SwapChainPipeline(DeviceContext deviceContext, Size screenSize, SwapChain1 swapChain)
    {
        _deviceContext = deviceContext;
        _swapChain = swapChain;

        using var swapChain2 = _swapChain.QueryInterface<SwapChain2>();
        var waitableObject = swapChain2.FrameLatencyWaitableObject;
        _frameLatencyWaitHandle = new SafeWaitHandle(waitableObject, false);
        _frameLatencyWaitEvent = new EventWaitHandle(false, EventResetMode.ManualReset);
        _frameLatencyWaitEvent.SafeWaitHandle = _frameLatencyWaitHandle;
        swapChain2.MaximumFrameLatency = 1;

        // TODO: How to consistently handle DPI?
        // TODO: Check supported multisample quality levels and use D3D11_STANDARD_MULTISAMPLE_PATTERN?
        _msaaTargetTexture = _deviceContext.CreateTexture(screenSize, BindFlags.RenderTarget, 4);
        _msaaTargetBitmap = _deviceContext.CreateBitmap(_msaaTargetTexture, BitmapOptions.Target | BitmapOptions.CannotDraw);

        _resolveTexture = _deviceContext.CreateTexture(screenSize, BindFlags.ShaderResource, 1);
        _resolveBitmap = _deviceContext.CreateBitmap(_resolveTexture, BitmapOptions.None);

        _deviceContext.D2D1DeviceContext.Target = _msaaTargetBitmap;

        // It is safe to cache the back buffer reference in DX 11.
        using var backBufferSurface = _swapChain.GetBackBuffer<Surface>(0);
        _backBufferBitmap = _deviceContext.CreateBitmap(backBufferSurface, BitmapOptions.Target | BitmapOptions.CannotDraw);
    }

    public void Present(bool waitForVSync)
    {
        _deviceContext.D2D1DeviceContext.Target = null;

        _deviceContext.D3D11DeviceContext.ResolveSubresource(_msaaTargetTexture, 0, _resolveTexture, 0, Format.B8G8R8A8_UNorm);

        _deviceContext.D2D1DeviceContext.Target = _backBufferBitmap;

        _deviceContext.D2D1DeviceContext.BeginDraw();
        _deviceContext.D2D1DeviceContext.Clear(new RawColor4(0, 0, 0, 1));
        _deviceContext.D2D1DeviceContext.Transform = new RawMatrix3x2(1, 0, 0, 1, 0, 0);
        _deviceContext.D2D1DeviceContext.DrawBitmap(_resolveBitmap, 1.0f, SharpDX.Direct2D1.BitmapInterpolationMode.Linear);
        _deviceContext.D2D1DeviceContext.EndDraw();

        _deviceContext.D2D1DeviceContext.Target = _msaaTargetBitmap;

        if (waitForVSync)
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

    public void Dispose()
    {
        _backBufferBitmap.Dispose();
        _resolveBitmap.Dispose();
        _resolveTexture.Dispose();
        _msaaTargetBitmap.Dispose();
        _msaaTargetTexture.Dispose();
        _frameLatencyWaitEvent.Dispose();
        _frameLatencyWaitHandle.Dispose();
    }
}