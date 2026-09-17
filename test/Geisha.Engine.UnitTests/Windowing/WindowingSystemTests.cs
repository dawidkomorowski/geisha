using Geisha.Engine.Core.Math;
using Geisha.Engine.Rendering.Backend;
using Geisha.Engine.Windowing;
using Geisha.Engine.Windowing.Backend;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Windowing;

[TestFixture]
public class WindowingSystemTests
{
    private IWindowingBackend _windowingBackend = null!;
    private IRenderingBackend _renderingBackend = null!;

    [SetUp]
    public void SetUp()
    {
        _windowingBackend = Substitute.For<IWindowingBackend>();
        _renderingBackend = Substitute.For<IRenderingBackend>();

        _windowingBackend.WindowClientSize.Returns(new Size(1280, 720));
        _windowingBackend.CursorVisible.Returns(true);
        _windowingBackend.DisplayMode.Returns(DisplayMode.Windowed);
    }

    [Test]
    public void Constructor_ShouldInitializeWindowingSystem()
    {
        // Arrange
        _windowingBackend.CursorVisible.Returns(false);
        _windowingBackend.DisplayMode.Returns(DisplayMode.Fullscreen);

        // Act
        var windowingSystem = new WindowingSystem(_windowingBackend, _renderingBackend);

        // Assert
        Assert.That(windowingSystem.CursorVisible, Is.False);
        Assert.That(windowingSystem.DisplayMode, Is.EqualTo(DisplayMode.Fullscreen));
    }

    [Test]
    public void HandleWindowState_ShouldSynchronize_CursorVisible()
    {
        // Arrange
        var windowingSystem = new WindowingSystem(_windowingBackend, _renderingBackend);

        // Act
        windowingSystem.CursorVisible = false;
        windowingSystem.HandleWindowState();

        // Assert
        Assert.That(_windowingBackend.CursorVisible, Is.False);
    }

    [Test]
    public void HandleWindowState_ShouldSynchronize_DisplayMode()
    {
        // Arrange
        var windowingSystem = new WindowingSystem(_windowingBackend, _renderingBackend);

        // Act
        windowingSystem.DisplayMode = DisplayMode.Fullscreen;
        windowingSystem.HandleWindowState();

        // Assert
        Assert.That(_windowingBackend.DisplayMode, Is.EqualTo(DisplayMode.Fullscreen));
    }

    [Test]
    public void HandleWindowState_ShouldResizeRenderingBuffers_WhenWindowClientSizeChanged()
    {
        // Arrange
        var windowingSystem = new WindowingSystem(_windowingBackend, _renderingBackend);

        var size = new Size(1281, 721);

        // Act
        _windowingBackend.WindowClientSize.Returns(size);
        windowingSystem.HandleWindowState();

        // Assert
        _renderingBackend.Received(1).ResizeBuffers(size);
    }

    [Test]
    public void HandleWindowState_ShouldNotResizeRenderingBuffers_WhenWindowClientSizeStaysTheSame()
    {
        // Arrange
        var windowingSystem = new WindowingSystem(_windowingBackend, _renderingBackend);

        // Act
        windowingSystem.HandleWindowState();

        // Assert
        _renderingBackend.ReceivedWithAnyArgs(0).ResizeBuffers(Arg.Any<Size>());
    }

    [TestCase(1280, 0)]
    [TestCase(0, 720)]
    [TestCase(0, 0)]
    public void HandleWindowState_ShouldNotResizeRenderingBuffers_WhenWindowClientSizeIsZero(int width, int height)
    {
        // Arrange
        var windowingSystem = new WindowingSystem(_windowingBackend, _renderingBackend);

        var size = new Size(width, height);

        // Act
        _windowingBackend.WindowClientSize = size;
        windowingSystem.HandleWindowState();

        // Assert
        _renderingBackend.ReceivedWithAnyArgs(0).ResizeBuffers(Arg.Any<Size>());
    }
}