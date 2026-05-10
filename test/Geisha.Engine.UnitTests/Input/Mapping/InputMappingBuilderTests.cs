using Geisha.Engine.Input;
using Geisha.Engine.Input.Mapping;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Input.Mapping;

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
    public void Build_Multiple_ActionMappings()
    {
        // Arrange
        var builder = InputMapping.CreateBuilder();

        // Act
        builder.MapAction("Key action 1", Key.D1);
        builder.MapAction("Key action 2", Key.D2);
        builder.MapAction("Mouse button action 1", MouseButton.Left);
        builder.MapAction("Mouse button action 2", MouseButton.Right);

        var actual = builder.Build();

        // Assert
        Assert.That(actual.ActionMappings, Has.Length.EqualTo(4));
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

        var actual = builder.Build();

        // Assert
        Assert.That(actual.ActionMappings, Has.Length.EqualTo(1));
        Assert.That(actual.AxisMappings, Is.Empty);

        Assert.That(actual.ActionMappings[0].ActionName, Is.EqualTo("Action 1"));
        Assert.That(actual.ActionMappings[0].HardwareActions, Has.Length.EqualTo(4));
        Assert.That(actual.ActionMappings[0].HardwareActions[0].InputSource, Is.EqualTo(InputSource.Create(Key.D1)));
        Assert.That(actual.ActionMappings[0].HardwareActions[1].InputSource, Is.EqualTo(InputSource.Create(Key.D2)));
        Assert.That(actual.ActionMappings[0].HardwareActions[2].InputSource, Is.EqualTo(InputSource.Create(MouseButton.Left)));
        Assert.That(actual.ActionMappings[0].HardwareActions[3].InputSource, Is.EqualTo(InputSource.Create(MouseButton.Right)));
    }
}