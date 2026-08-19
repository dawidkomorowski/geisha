using System;
using System.Threading;
using System.Windows.Forms;
using Geisha.Engine.Rendering.Backend;
using Microsoft.Win32.SafeHandles;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Device = SharpDX.Direct3D11.Device;
using FeatureLevel = SharpDX.Direct3D.FeatureLevel;
using Rational = SharpDX.DXGI.Rational;

namespace Geisha.Engine.Rendering.DirectX;

/// <summary>
///     Rendering backend implementation using DirectX rendering API. This implementation depends on WinForms.
/// </summary>
public sealed class DirectXRenderingBackend : IRenderingBackend, IDisposable
{
    private readonly Statistics _statistics;
    private readonly Device _d3D11Device;
    private readonly SwapChain _dxgiSwapChain;
    private readonly SafeWaitHandle _frameLatencyWaitHandle;
    private readonly EventWaitHandle _frameLatencyWaitEvent;
    private readonly DeviceContext _deviceContext;
    private readonly RenderingContext2D _renderingContext2D;

    /// <summary>
    ///     Creates new instance of <see cref="DirectXRenderingBackend" /> with specified <see cref="Form" /> as render target.
    /// </summary>
    /// <param name="form"><see cref="Form" /> that serves as render target.</param>
    /// <param name="driverType">Type of driver to use by rendering API.</param>
    public DirectXRenderingBackend(Form form, DriverType driverType)
    {
        _statistics = new Statistics();

        // TODO: Check if tearing is supported?

        var swapChainDescription = new SwapChainDescription
        {
            BufferCount = 2,
            ModeDescription = new ModeDescription(form.ClientSize.Width, form.ClientSize.Height, new Rational(60, 1), Format.B8G8R8A8_UNorm),
            IsWindowed = true,
            OutputHandle = form.Handle,
            SampleDescription = new SampleDescription(1, 0),
            SwapEffect = SwapEffect.FlipDiscard,
            Usage = Usage.RenderTargetOutput,
            Flags = SwapChainFlags.AllowTearing | SwapChainFlags.FrameLatencyWaitAbleObject
        };

        var directXDriverType = driverType switch
        {
            DriverType.Hardware => SharpDX.Direct3D.DriverType.Hardware,
            DriverType.Software => SharpDX.Direct3D.DriverType.Warp,
            _ => throw new ArgumentOutOfRangeException(nameof(driverType), driverType, "Unknown driver type.")
        };

        Device.CreateWithSwapChain(
            directXDriverType,
            DeviceCreationFlags.BgraSupport, // TODO Investigate DeviceCreationFlags.Debug
            new[] { FeatureLevel.Level_11_0 },
            swapChainDescription,
            out _d3D11Device,
            out _dxgiSwapChain
        );

        _deviceContext = new DeviceContext(_d3D11Device);

        using var swapChain2 = _dxgiSwapChain.QueryInterface<SwapChain2>();
        var waitableObject = swapChain2.FrameLatencyWaitableObject;
        _frameLatencyWaitHandle = new SafeWaitHandle(waitableObject, false);
        _frameLatencyWaitEvent = new EventWaitHandle(false, EventResetMode.ManualReset);
        _frameLatencyWaitEvent.SafeWaitHandle = _frameLatencyWaitHandle;
        swapChain2.MaximumFrameLatency = 1;

        using var dxgiFactory = _dxgiSwapChain.GetParent<Factory>();
        dxgiFactory.MakeWindowAssociation(form.Handle, WindowAssociationFlags.IgnoreAll); // Ignore all windows events

        using var dxgiDevice = _d3D11Device.QueryInterface<SharpDX.DXGI.Device>();

        _renderingContext2D = new RenderingContext2D(form, _deviceContext, _statistics);

        Info = new RenderingBackendInfo(
            Name: "DirectX 11",
            GraphicsAdapterName: dxgiDevice.Adapter.Description.Description,
            VideoMemorySize: dxgiDevice.Adapter.Description.DedicatedVideoMemory,
            FeatureLevel: _d3D11Device.FeatureLevel.ToString()
        );
    }

    /// <summary>
    ///     Creates new instance of <see cref="DirectXRenderingBackend" /> with default hidden <see cref="Form" /> as a render
    ///     target. This constructor is meant for providing rendering backend services without rendering output e.g. current
    ///     implementation of Geisha.Editor.
    /// </summary>
    public DirectXRenderingBackend() : this(new Form(), DriverType.Hardware)
    {
    }

    /// <inheritdoc />
    public IRenderingContext2D Context2D => _renderingContext2D;

    /// <inheritdoc />
    public RenderingStatistics Statistics => _statistics.LastFrameStats;

    /// <inheritdoc />
    public RenderingBackendInfo Info { get; }

    /// <inheritdoc />
    public void Present(bool waitForVSync)
    {
        using var backBufferSurface = _dxgiSwapChain.GetBackBuffer<Surface>(0);

        _renderingContext2D.DrawToSwapChainSurface(backBufferSurface);

        if (waitForVSync)
        {
            _dxgiSwapChain.Present(1, PresentFlags.None);
        }
        else
        {
            _dxgiSwapChain.Present(0, PresentFlags.AllowTearing);
        }

        _statistics.UpdateLastFrameStats();

        // Wait for the presentation to complete before working on next frame.
        _frameLatencyWaitEvent.WaitOne(1000);
    }

    /// <summary>
    ///     Releases rendering API resources.
    /// </summary>
    public void Dispose()
    {
        _renderingContext2D.Dispose();
        _frameLatencyWaitEvent.Dispose();
        _frameLatencyWaitHandle.Dispose();
        _deviceContext.Dispose();
        _dxgiSwapChain.Dispose();
        _d3D11Device.Dispose();
    }
}