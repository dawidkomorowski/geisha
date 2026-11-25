using Geisha.Engine.Input;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Input;

[TestFixture]
public class InputMappingBuilderTests
{
    [Test]
    public void Build_EmptyInputMapping()
    {
        // Arrange
        var builder = InputMapping.CreateBuilder();

        // Act
        var actual = builder.Build();

        // Assert
        Assert.That(actual.ActionMappings, Is.Empty);
        Assert.That(actual.AxisMappings, Is.Empty);
    }

    [Test]
    public void Build_One_ActionMapping_Key()
    {
        // Arrange
        var builder = InputMapping.CreateBuilder();

        // Act
        builder.MapAction("Key action 1", Key.D1);

        var actual = builder.Build();

        // Assert
        Assert.That(actual.ActionMappings, Has.Length.EqualTo(1));
        Assert.That(actual.AxisMappings, Is.Empty);

        Assert.That(actual.ActionMappings[0].ActionName, Is.EqualTo("Key action 1"));
        Assert.That(actual.ActionMappings[0].HardwareActions, Has.Length.EqualTo(1));
        Assert.That(actual.ActionMappings[0].HardwareActions[0].InputSource, Is.EqualTo(InputSource.Create(Key.D1)));
    }

    [Test]
    public void Build_One_ActionMapping_MouseButton()
    {
        // Arrange
        var builder = InputMapping.CreateBuilder();

        // Act
        builder.MapAction("Mouse button action 1", MouseButton.Left);

        var actual = builder.Build();

        // Assert
        Assert.That(actual.ActionMappings, Has.Length.EqualTo(1));
        Assert.That(actual.AxisMappings, Is.Empty);

        Assert.That(actual.ActionMappings[0].ActionName, Is.EqualTo("Mouse button action 1"));
        Assert.That(actual.ActionMappings[0].HardwareActions, Has.Length.EqualTo(1));
        Assert.That(actual.ActionMappings[0].HardwareActions[0].InputSource, Is.EqualTo(InputSource.Create(MouseButton.Left)));
    }

    [Test]
    public void Build_One_ActionMapping_ControllerButton()
    {
        // Arrange
        var builder = InputMapping.CreateBuilder();

        // Act
        builder.MapAction("Controller button action 1", ControllerIndex.Four, ControllerButton.Back);

        var actual = builder.Build();

        // Assert
        Assert.That(actual.ActionMappings, Has.Length.EqualTo(1));
        Assert.That(actual.AxisMappings, Is.Empty);

        Assert.That(actual.ActionMappings[0].ActionName, Is.EqualTo("Controller button action 1"));
        Assert.That(actual.ActionMappings[0].HardwareActions, Has.Length.EqualTo(1));
        Assert.That(actual.ActionMappings[0].HardwareActions[0].InputSource, Is.EqualTo(InputSource.Create(ControllerIndex.Four, ControllerButton.Back)));
    }

    [Test]
    public void Build_Multiple_ActionMappings()
    {
        // Arrange
        var builder = InputMapping.CreateBuilder();

        // Act
        builder.MapAction("Key action 1", Key.D1);
        builder.MapAction("Key action 2", Key.D2);
        builder.MapAction("Mouse button action 1", MouseButton.Left);
        builder.MapAction("Mouse button action 2", MouseButton.Right);
        builder.MapAction("Controller button action 1", ControllerIndex.Four, ControllerButton.Start);
        builder.MapAction("Controller button action 2", ControllerIndex.Four, ControllerButton.Back);

        var actual = builder.Build();

        // Assert
        Assert.That(actual.ActionMappings, Has.Length.EqualTo(6));
        Assert.That(actual.AxisMappings, Is.Empty);

        Assert.That(actual.ActionMappings[0].ActionName, Is.EqualTo("Key action 1"));
        Assert.That(actual.ActionMappings[0].HardwareActions, Has.Length.EqualTo(1));
        Assert.That(actual.ActionMappings[0].HardwareActions[0].InputSource, Is.EqualTo(InputSource.Create(Key.D1)));

        Assert.That(actual.ActionMappings[1].ActionName, Is.EqualTo("Key action 2"));
        Assert.That(actual.ActionMappings[1].HardwareActions, Has.Length.EqualTo(1));
        Assert.That(actual.ActionMappings[1].HardwareActions[0].InputSource, Is.EqualTo(InputSource.Create(Key.D2)));

        Assert.That(actual.ActionMappings[2].ActionName, Is.EqualTo("Mouse button action 1"));
        Assert.That(actual.ActionMappings[2].HardwareActions, Has.Length.EqualTo(1));
        Assert.That(actual.ActionMappings[2].HardwareActions[0].InputSource, Is.EqualTo(InputSource.Create(MouseButton.Left)));

        Assert.That(actual.ActionMappings[3].ActionName, Is.EqualTo("Mouse button action 2"));
        Assert.That(actual.ActionMappings[3].HardwareActions, Has.Length.EqualTo(1));
        Assert.That(actual.ActionMappings[3].HardwareActions[0].InputSource, Is.EqualTo(InputSource.Create(MouseButton.Right)));

        Assert.That(actual.ActionMappings[4].ActionName, Is.EqualTo("Controller button action 1"));
        Assert.That(actual.ActionMappings[4].HardwareActions, Has.Length.EqualTo(1));
        Assert.That(actual.ActionMappings[4].HardwareActions[0].InputSource, Is.EqualTo(InputSource.Create(ControllerIndex.Four, ControllerButton.Start)));

        Assert.That(actual.ActionMappings[5].ActionName, Is.EqualTo("Controller button action 2"));
        Assert.That(actual.ActionMappings[5].HardwareActions, Has.Length.EqualTo(1));
        Assert.That(actual.ActionMappings[5].HardwareActions[0].InputSource, Is.EqualTo(InputSource.Create(ControllerIndex.Four, ControllerButton.Back)));
    }

    [Test]
    public void Build_One_ActionMapping_Multiple_InputSources()
    {
        // Arrange
        var builder = InputMapping.CreateBuilder();

        // Act
        builder.MapAction("Action 1", Key.D1);
        builder.MapAction("Action 1", Key.D2);
        builder.MapAction("Action 1", MouseButton.Left);
        builder.MapAction("Action 1", MouseButton.Right);
        builder.MapAction("Action 1", ControllerIndex.Four, ControllerButton.Start);
        builder.MapAction("Action 1", ControllerIndex.Four, ControllerButton.Back);

        var actual = builder.Build();

        // Assert
        Assert.That(actual.ActionMappings, Has.Length.EqualTo(1));
        Assert.That(actual.AxisMappings, Is.Empty);

        Assert.That(actual.ActionMappings[0].ActionName, Is.EqualTo("Action 1"));
        Assert.That(actual.ActionMappings[0].HardwareActions, Has.Length.EqualTo(6));
        Assert.That(actual.ActionMappings[0].HardwareActions[0].InputSource, Is.EqualTo(InputSource.Create(Key.D1)));
        Assert.That(actual.ActionMappings[0].HardwareActions[1].InputSource, Is.EqualTo(InputSource.Create(Key.D2)));
        Assert.That(actual.ActionMappings[0].HardwareActions[2].InputSource, Is.EqualTo(InputSource.Create(MouseButton.Left)));
        Assert.That(actual.ActionMappings[0].HardwareActions[3].InputSource, Is.EqualTo(InputSource.Create(MouseButton.Right)));
        Assert.That(actual.ActionMappings[0].HardwareActions[4].InputSource, Is.EqualTo(InputSource.Create(ControllerIndex.Four, ControllerButton.Start)));
        Assert.That(actual.ActionMappings[0].HardwareActions[5].InputSource, Is.EqualTo(InputSource.Create(ControllerIndex.Four, ControllerButton.Back)));
    }

    [Test]
    public void Build_One_AxisMapping_Key()
    {
        // Arrange
        var builder = InputMapping.CreateBuilder();

        // Act
        builder.MapAxis("Key axis 1", Key.A, 1);

        var actual = builder.Build();

        // Assert
        Assert.That(actual.ActionMappings, Is.Empty);
        Assert.That(actual.AxisMappings, Has.Length.EqualTo(1));

        Assert.That(actual.AxisMappings[0].AxisName, Is.EqualTo("Key axis 1"));
        Assert.That(actual.AxisMappings[0].HardwareAxes, Has.Length.EqualTo(1));
        Assert.That(actual.AxisMappings[0].HardwareAxes[0].InputSource, Is.EqualTo(InputSource.Create(Key.A)));
        Assert.That(actual.AxisMappings[0].HardwareAxes[0].Scale, Is.EqualTo(1));
    }

    [Test]
    public void Build_One_AxisMapping_MouseAxis()
    {
        // Arrange
        var builder = InputMapping.CreateBuilder();

        // Act
        builder.MapAxis("Mouse axis 1", MouseAxis.X, 0.5);

        var actual = builder.Build();

        // Assert
        Assert.That(actual.ActionMappings, Is.Empty);
        Assert.That(actual.AxisMappings, Has.Length.EqualTo(1));

        Assert.That(actual.AxisMappings[0].AxisName, Is.EqualTo("Mouse axis 1"));
        Assert.That(actual.AxisMappings[0].HardwareAxes, Has.Length.EqualTo(1));
        Assert.That(actual.AxisMappings[0].HardwareAxes[0].InputSource, Is.EqualTo(InputSource.Create(MouseAxis.X)));
        Assert.That(actual.AxisMappings[0].HardwareAxes[0].Scale, Is.EqualTo(0.5));
    }

    [Test]
    public void Build_One_AxisMapping_ControllerAxis()
    {
        // Arrange
        var builder = InputMapping.CreateBuilder();

        // Act
        builder.MapAxis("Controller axis 1", ControllerIndex.Four, ControllerAxis.RightTrigger, 0.5);

        var actual = builder.Build();

        // Assert
        Assert.That(actual.ActionMappings, Is.Empty);
        Assert.That(actual.AxisMappings, Has.Length.EqualTo(1));

        Assert.That(actual.AxisMappings[0].AxisName, Is.EqualTo("Controller axis 1"));
        Assert.That(actual.AxisMappings[0].HardwareAxes, Has.Length.EqualTo(1));
        Assert.That(actual.AxisMappings[0].HardwareAxes[0].InputSource, Is.EqualTo(InputSource.Create(ControllerIndex.Four, ControllerAxis.RightTrigger)));
        Assert.That(actual.AxisMappings[0].HardwareAxes[0].Scale, Is.EqualTo(0.5));
    }

    [Test]
    public void Build_Multiple_AxisMappings()
    {
        // Arrange
        var builder = InputMapping.CreateBuilder();

        // Act
        builder.MapAxis("Key axis 1", Key.A, 1);
        builder.MapAxis("Key axis 2", Key.D, -1);
        builder.MapAxis("Mouse axis 1", MouseAxis.X, 0.5);
        builder.MapAxis("Mouse axis 2", MouseAxis.Y, -0.25);
        builder.MapAxis("Controller axis 1", ControllerIndex.Four, ControllerAxis.RightTrigger, 0.1);
        builder.MapAxis("Controller axis 2", ControllerIndex.Four, ControllerAxis.LeftTrigger, -0.1);

        var actual = builder.Build();

        // Assert
        Assert.That(actual.ActionMappings, Is.Empty);
        Assert.That(actual.AxisMappings, Has.Length.EqualTo(6));

        Assert.That(actual.AxisMappings[0].AxisName, Is.EqualTo("Key axis 1"));
        Assert.That(actual.AxisMappings[0].HardwareAxes, Has.Length.EqualTo(1));
        Assert.That(actual.AxisMappings[0].HardwareAxes[0].InputSource, Is.EqualTo(InputSource.Create(Key.A)));
        Assert.That(actual.AxisMappings[0].HardwareAxes[0].Scale, Is.EqualTo(1));

        Assert.That(actual.AxisMappings[1].AxisName, Is.EqualTo("Key axis 2"));
        Assert.That(actual.AxisMappings[1].HardwareAxes, Has.Length.EqualTo(1));
        Assert.That(actual.AxisMappings[1].HardwareAxes[0].InputSource, Is.EqualTo(InputSource.Create(Key.D)));
        Assert.That(actual.AxisMappings[1].HardwareAxes[0].Scale, Is.EqualTo(-1));

        Assert.That(actual.AxisMappings[2].AxisName, Is.EqualTo("Mouse axis 1"));
        Assert.That(actual.AxisMappings[2].HardwareAxes, Has.Length.EqualTo(1));
        Assert.That(actual.AxisMappings[2].HardwareAxes[0].InputSource, Is.EqualTo(InputSource.Create(MouseAxis.X)));
        Assert.That(actual.AxisMappings[2].HardwareAxes[0].Scale, Is.EqualTo(0.5));

        Assert.That(actual.AxisMappings[3].AxisName, Is.EqualTo("Mouse axis 2"));
        Assert.That(actual.AxisMappings[3].HardwareAxes, Has.Length.EqualTo(1));
        Assert.That(actual.AxisMappings[3].HardwareAxes[0].InputSource, Is.EqualTo(InputSource.Create(MouseAxis.Y)));
        Assert.That(actual.AxisMappings[3].HardwareAxes[0].Scale, Is.EqualTo(-0.25));

        Assert.That(actual.AxisMappings[4].AxisName, Is.EqualTo("Controller axis 1"));
        Assert.That(actual.AxisMappings[4].HardwareAxes, Has.Length.EqualTo(1));
        Assert.That(actual.AxisMappings[4].HardwareAxes[0].InputSource, Is.EqualTo(InputSource.Create(ControllerIndex.Four, ControllerAxis.RightTrigger)));
        Assert.That(actual.AxisMappings[4].HardwareAxes[0].Scale, Is.EqualTo(0.1));

        Assert.That(actual.AxisMappings[5].AxisName, Is.EqualTo("Controller axis 2"));
        Assert.That(actual.AxisMappings[5].HardwareAxes, Has.Length.EqualTo(1));
        Assert.That(actual.AxisMappings[5].HardwareAxes[0].InputSource, Is.EqualTo(InputSource.Create(ControllerIndex.Four, ControllerAxis.LeftTrigger)));
        Assert.That(actual.AxisMappings[5].HardwareAxes[0].Scale, Is.EqualTo(-0.1));
    }

    [Test]
    public void Build_One_AxisMapping_Multiple_InputSources()
    {
        // Arrange
        var builder = InputMapping.CreateBuilder();

        // Act
        builder.MapAxis("Axis 1", Key.A, 1);
        builder.MapAxis("Axis 1", Key.D, -1);
        builder.MapAxis("Axis 1", MouseAxis.X, 0.5);
        builder.MapAxis("Axis 1", MouseAxis.Y, -0.25);
        builder.MapAxis("Axis 1", ControllerIndex.Four, ControllerAxis.RightTrigger, 0.1);
        builder.MapAxis("Axis 1", ControllerIndex.Four, ControllerAxis.LeftTrigger, -0.1);

        var actual = builder.Build();

        // Assert
        Assert.That(actual.ActionMappings, Is.Empty);
        Assert.That(actual.AxisMappings, Has.Length.EqualTo(1));

        Assert.That(actual.AxisMappings[0].AxisName, Is.EqualTo("Axis 1"));
        Assert.That(actual.AxisMappings[0].HardwareAxes, Has.Length.EqualTo(6));
        Assert.That(actual.AxisMappings[0].HardwareAxes[0].InputSource, Is.EqualTo(InputSource.Create(Key.A)));
        Assert.That(actual.AxisMappings[0].HardwareAxes[0].Scale, Is.EqualTo(1));
        Assert.That(actual.AxisMappings[0].HardwareAxes[1].InputSource, Is.EqualTo(InputSource.Create(Key.D)));
        Assert.That(actual.AxisMappings[0].HardwareAxes[1].Scale, Is.EqualTo(-1));
        Assert.That(actual.AxisMappings[0].HardwareAxes[2].InputSource, Is.EqualTo(InputSource.Create(MouseAxis.X)));
        Assert.That(actual.AxisMappings[0].HardwareAxes[2].Scale, Is.EqualTo(0.5));
        Assert.That(actual.AxisMappings[0].HardwareAxes[3].InputSource, Is.EqualTo(InputSource.Create(MouseAxis.Y)));
        Assert.That(actual.AxisMappings[0].HardwareAxes[3].Scale, Is.EqualTo(-0.25));
        Assert.That(actual.AxisMappings[0].HardwareAxes[4].InputSource, Is.EqualTo(InputSource.Create(ControllerIndex.Four, ControllerAxis.RightTrigger)));
        Assert.That(actual.AxisMappings[0].HardwareAxes[4].Scale, Is.EqualTo(0.1));
        Assert.That(actual.AxisMappings[0].HardwareAxes[5].InputSource, Is.EqualTo(InputSource.Create(ControllerIndex.Four, ControllerAxis.LeftTrigger)));
        Assert.That(actual.AxisMappings[0].HardwareAxes[5].Scale, Is.EqualTo(-0.1));
    }

    [Test]
    public void Build_Multiple_ActionMappings_And_AxisMappings()
    {
        // Arrange
        var builder = InputMapping.CreateBuilder();

        // Act
        builder.MapAction("Action 1", Key.D1);
        builder.MapAction("Action 1", MouseButton.Left);
        builder.MapAction("Action 1", ControllerIndex.Four, ControllerButton.Start);
        builder.MapAction("Action 2", Key.D2);
        builder.MapAction("Action 2", MouseButton.Right);
        builder.MapAction("Action 2", ControllerIndex.Four, ControllerButton.Back);

        builder.MapAxis("Axis 1", Key.A, 1);
        builder.MapAxis("Axis 1", MouseAxis.X, 0.5);
        builder.MapAxis("Axis 1", ControllerIndex.Four, ControllerAxis.RightTrigger, 0.1);
        builder.MapAxis("Axis 2", Key.D, -1);
        builder.MapAxis("Axis 2", MouseAxis.Y, -0.25);
        builder.MapAxis("Axis 2", ControllerIndex.Four, ControllerAxis.LeftTrigger, -0.1);

        var actual = builder.Build();

        // Assert
        Assert.That(actual.ActionMappings, Has.Length.EqualTo(2));
        Assert.That(actual.AxisMappings, Has.Length.EqualTo(2));

        Assert.That(actual.ActionMappings[0].ActionName, Is.EqualTo("Action 1"));
        Assert.That(actual.ActionMappings[0].HardwareActions, Has.Length.EqualTo(3));
        Assert.That(actual.ActionMappings[0].HardwareActions[0].InputSource, Is.EqualTo(InputSource.Create(Key.D1)));
        Assert.That(actual.ActionMappings[0].HardwareActions[1].InputSource, Is.EqualTo(InputSource.Create(MouseButton.Left)));
        Assert.That(actual.ActionMappings[0].HardwareActions[2].InputSource, Is.EqualTo(InputSource.Create(ControllerIndex.Four, ControllerButton.Start)));

        Assert.That(actual.ActionMappings[1].ActionName, Is.EqualTo("Action 2"));
        Assert.That(actual.ActionMappings[1].HardwareActions, Has.Length.EqualTo(3));
        Assert.That(actual.ActionMappings[1].HardwareActions[0].InputSource, Is.EqualTo(InputSource.Create(Key.D2)));
        Assert.That(actual.ActionMappings[1].HardwareActions[1].InputSource, Is.EqualTo(InputSource.Create(MouseButton.Right)));
        Assert.That(actual.ActionMappings[1].HardwareActions[2].InputSource, Is.EqualTo(InputSource.Create(ControllerIndex.Four, ControllerButton.Back)));

        Assert.That(actual.AxisMappings[0].AxisName, Is.EqualTo("Axis 1"));
        Assert.That(actual.AxisMappings[0].HardwareAxes, Has.Length.EqualTo(3));
        Assert.That(actual.AxisMappings[0].HardwareAxes[0].InputSource, Is.EqualTo(InputSource.Create(Key.A)));
        Assert.That(actual.AxisMappings[0].HardwareAxes[0].Scale, Is.EqualTo(1));
        Assert.That(actual.AxisMappings[0].HardwareAxes[1].InputSource, Is.EqualTo(InputSource.Create(MouseAxis.X)));
        Assert.That(actual.AxisMappings[0].HardwareAxes[1].Scale, Is.EqualTo(0.5));
        Assert.That(actual.AxisMappings[0].HardwareAxes[2].InputSource, Is.EqualTo(InputSource.Create(ControllerIndex.Four, ControllerAxis.RightTrigger)));
        Assert.That(actual.AxisMappings[0].HardwareAxes[2].Scale, Is.EqualTo(0.1));

        Assert.That(actual.AxisMappings[1].AxisName, Is.EqualTo("Axis 2"));
        Assert.That(actual.AxisMappings[1].HardwareAxes, Has.Length.EqualTo(3));
        Assert.That(actual.AxisMappings[1].HardwareAxes[0].InputSource, Is.EqualTo(InputSource.Create(Key.D)));
        Assert.That(actual.AxisMappings[1].HardwareAxes[0].Scale, Is.EqualTo(-1));
        Assert.That(actual.AxisMappings[1].HardwareAxes[1].InputSource, Is.EqualTo(InputSource.Create(MouseAxis.Y)));
        Assert.That(actual.AxisMappings[1].HardwareAxes[1].Scale, Is.EqualTo(-0.25));
        Assert.That(actual.AxisMappings[1].HardwareAxes[2].InputSource, Is.EqualTo(InputSource.Create(ControllerIndex.Four, ControllerAxis.LeftTrigger)));
        Assert.That(actual.AxisMappings[1].HardwareAxes[2].Scale, Is.EqualTo(-0.1));
    }
}