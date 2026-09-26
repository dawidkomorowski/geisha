using System;
using System.Windows.Forms;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Rendering.Backend;
using SharpDX.Direct3D11;
using Device = SharpDX.Direct3D11.Device;
using FeatureLevel = SharpDX.Direct3D.FeatureLevel;

namespace Geisha.Engine.Rendering.DirectX;

/// <summary>
///     Rendering backend implementation using DirectX rendering API. This implementation depends on WinForms.
/// </summary>
public sealed class DirectXRenderingBackend : IRenderingBackend, IDisposable
{
    private readonly Statistics _statistics;
    private readonly Device _d3D11Device;
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
        // TODO: How to consistently handle DPI?
        _statistics = new Statistics();
        var windowClientSize = new Size(form.ClientSize.Width, form.ClientSize.Height);

        var directXDriverType = driverType switch
        {
            DriverType.Hardware => SharpDX.Direct3D.DriverType.Hardware,
            DriverType.Software => SharpDX.Direct3D.DriverType.Warp,
            _ => throw new ArgumentOutOfRangeException(nameof(driverType), driverType, "Unknown driver type.")
        };

        var featureLevels = new[]
        {
            FeatureLevel.Level_11_1,
            FeatureLevel.Level_11_0,
            FeatureLevel.Level_10_1,
            FeatureLevel.Level_10_0
        };

        var deviceCreationFlags = DeviceCreationFlags.BgraSupport;
        // TODO: Investigate DeviceCreationFlags.Debug
        //deviceCreationFlags |= DeviceCreationFlags.Debug;

        _d3D11Device = new Device(directXDriverType, deviceCreationFlags, featureLevels);

        _deviceContext = new DeviceContext(_d3D11Device);
        _swapChainPipeline = new SwapChainPipeline(_deviceContext, windowClientSize, form.Handle);
        _renderingContext2D = new RenderingContext2D(_deviceContext, windowClientSize, _statistics);

        using var dxgiDevice = _d3D11Device.QueryInterface<SharpDX.DXGI.Device>();
        using var dxgiAdapter = dxgiDevice.Adapter;

        Info = new RenderingBackendInfo(
            Name: "DirectX 11",
            GraphicsAdapterName: dxgiAdapter.Description.Description,
            VideoMemorySize: dxgiAdapter.Description.DedicatedVideoMemory,
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

    #region Implementation of IRenderingBackend

    /// <inheritdoc />
    public IRenderingContext2D Context2D => _renderingContext2D;

    /// <inheritdoc />
    public RenderingStatistics Statistics => _statistics.LastFrameStats;

    /// <inheritdoc />
    public RenderingBackendInfo Info { get; }

    /// <inheritdoc />
    public bool VSyncEnabled
    {
        get => _swapChainPipeline.VSyncEnabled;
        set => _swapChainPipeline.VSyncEnabled = value;
    }


    /// <summary>
    ///     Presents a rendered image to the user.
    /// </summary>
    /// <remarks>
    ///     After presenting a frame, this method waits for the swap chain to accept another frame before it returns. The
    ///     wait is performed regardless of <see cref="VSyncEnabled" /> and is limited to one second.
    ///
    ///     When VSync is enabled, presentation is synchronized with the display's vertical refresh, which normally throttles
    ///     the render thread to the display refresh rate.
    /// </remarks>
    public void Present()
    {
        _swapChainPipeline.Present();
        _statistics.UpdateLastFrameStats();
    }

    /// <inheritdoc />
    public void ResizeBuffers(Size size)
    {
        if (size.Width <= 0 || size.Height <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(size), size, "Rendering buffer width and height must be greater than zero.");
        }

        _swapChainPipeline.ResizeBuffers(size);
        _renderingContext2D.UpdateRenderTargetSize(size);
    }

    #endregion

    /// <summary>
    ///     Gets or sets whether rendering buffers are resized after VSync is changed at runtime. Default is <c>false</c>.
    /// </summary>
    /// <remarks>
    ///     Enable this option only on systems affected by GPU driver issues where changing VSync at runtime does not apply
    ///     synchronization correctly or produces visual artifacts. This has been observed on Windows systems using Qualcomm
    ///     Adreno GPUs. Resizing buffers after a VSync change adds work to the transition.
    /// </remarks>
    public bool ResizeBuffersAfterVSyncChange
    {
        get => _swapChainPipeline.ResizeBuffersAfterVSyncChange;
        set => _swapChainPipeline.ResizeBuffersAfterVSyncChange = value;
    }

    /// <summary>
    ///     Releases rendering API resources.
    /// </summary>
    public void Dispose()
    {
        _renderingContext2D.Dispose();
        _swapChainPipeline.Dispose();
        _deviceContext.Dispose();
        _d3D11Device.Dispose();
    }
}