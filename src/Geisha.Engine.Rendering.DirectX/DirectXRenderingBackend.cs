using System;
using System.Windows.Forms;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Rendering.Backend;
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
    private readonly DeviceContext _deviceContext;
    private readonly SwapChainPipeline _swapChainPipeline;
    private readonly RenderingContext2D _renderingContext2D;

    /// <summary>
    ///     Creates new instance of <see cref="DirectXRenderingBackend" /> with specified <see cref="Form" /> as render target.
    /// </summary>
    /// <param name="form"><see cref="Form" /> that serves as render target.</param>
    /// <param name="driverType">Type of driver to use by rendering API.</param>
    public DirectXRenderingBackend(Form form, DriverType driverType)
    {
        _statistics = new Statistics();
        var screenSize = new Size(form.ClientSize.Width, form.ClientSize.Height);

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
        _swapChainPipeline = new SwapChainPipeline(_deviceContext, screenSize, _dxgiSwapChain);

        using var dxgiFactory = _dxgiSwapChain.GetParent<Factory>();
        dxgiFactory.MakeWindowAssociation(form.Handle, WindowAssociationFlags.IgnoreAll); // Ignore all window events.

        using var dxgiDevice = _d3D11Device.QueryInterface<SharpDX.DXGI.Device>();

        _renderingContext2D = new RenderingContext2D(_deviceContext, screenSize, _statistics);

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
        _swapChainPipeline.Present(waitForVSync);
        _statistics.UpdateLastFrameStats();
    }

    /// <summary>
    ///     Releases rendering API resources.
    /// </summary>
    public void Dispose()
    {
        _renderingContext2D.Dispose();
        _swapChainPipeline.Dispose();
        _deviceContext.Dispose();
        _dxgiSwapChain.Dispose();
        _d3D11Device.Dispose();
    }
}