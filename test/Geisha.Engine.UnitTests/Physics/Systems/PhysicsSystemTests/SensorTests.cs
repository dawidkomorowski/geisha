using System;
using System.Collections.Generic;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Physics;
using Geisha.Engine.Physics.Components;
using Geisha.Engine.Physics.Systems;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Physics.Systems.PhysicsSystemTests;

// TODO: Test sensor overlap cache cleanup/index integrity across frames (stale removal + swap-remove updates do not orphan or corrupt pairs).

[TestFixture]
public class SensorTests : PhysicsSystemTestsBase
{
    [TestCase(false, true, false)]
    [TestCase(false, false, true)]
    [TestCase(false, true, true)]
    [TestCase(true, true, false)]
    [TestCase(true, false, true)]
    [TestCase(true, true, true)]
    public void Sensor_ShouldProduceNoCollisionAndInvokeOverlapBegin_WhenAtLeastOneColliderIsSensor(bool secondBodyIsKinematic, bool firstBodyIsSensor,
        bool secondBodyIsSensor)
    {
        var context = CreateOverlappingSensorContext(secondBodyIsKinematic);

        context.SensorCollider.IsSensor = firstBodyIsSensor;
        context.VisitorCollider.IsSensor = secondBodyIsSensor;

        // Assume
        Assert.That(context.SensorCollider.IsColliding, Is.False);
        Assert.That(context.VisitorCollider.IsColliding, Is.False);

        Assert.That(context.SensorCollider.Enabled, Is.True);
        Assert.That(context.VisitorCollider.Enabled, Is.True);

        // Act
        context.PhysicsSystem.ProcessPhysics();

        // Assert
        SaveVisualOutput(context.PhysicsSystem);

        Assert.That(context.SensorCollider.IsColliding, Is.False);
        Assert.That(context.SensorCollider.ContactCount, Is.Zero);
        var firstTransform = context.SensorCollider.Entity.GetComponent<Transform2DComponent>();
        Assert.That(firstTransform.Translation, Is.EqualTo(new Vector2(0, 0)));
        Assert.That(firstTransform.Rotation, Is.Zero);
        Assert.That(firstTransform.Scale, Is.EqualTo(Vector2.One));

        Assert.That(context.VisitorCollider.IsColliding, Is.False);
        Assert.That(context.VisitorCollider.ContactCount, Is.Zero);
        var secondTransform = context.VisitorCollider.Entity.GetComponent<Transform2DComponent>();
        Assert.That(secondTransform.Translation, Is.EqualTo(new Vector2(150, 0)));
        Assert.That(secondTransform.Rotation, Is.Zero);
        Assert.That(secondTransform.Scale, Is.EqualTo(Vector2.One));

        Assert.That(context.SensorBeginEvents, Has.Count.EqualTo(1));
        Assert.That(context.SensorBeginEvents[0], Is.EqualTo(context.VisitorCollider));
        Assert.That(context.VisitorBeginEvents, Has.Count.EqualTo(1));
        Assert.That(context.VisitorBeginEvents[0], Is.EqualTo(context.SensorCollider));
        Assert.That(context.SensorEndEvents, Is.Empty);
        Assert.That(context.VisitorEndEvents, Is.Empty);
    }

    [TestCase(true, false)]
    [TestCase(false, true)]
    [TestCase(true, true)]
    public void Sensor_ShouldNotInvokeOverlapCallbacks_WhenBothCollidersAreStatic(bool firstBodyIsSensor, bool secondBodyIsSensor)
    {
        var context = CreateOverlappingSensorContext(false, false);

        context.SensorCollider.IsSensor = firstBodyIsSensor;
        context.VisitorCollider.IsSensor = secondBodyIsSensor;

        // Act 0
        context.PhysicsSystem.ProcessPhysics();
        SaveVisualOutput(context.PhysicsSystem, 0);

        // Assert 0
        Assert.That(context.SensorCollider.IsColliding, Is.False);
        Assert.That(context.VisitorCollider.IsColliding, Is.False);
        AssertNoCallbacks(context);

        // Act 1
        context.PhysicsSystem.ProcessPhysics();
        SaveVisualOutput(context.PhysicsSystem, 1);

        // Assert 1
        AssertNoCallbacks(context);
    }

    [TestCase(true)]
    [TestCase(false)]
    public void Sensor_ShouldHandle_NullOverlapCallbacks(bool configureCallbacks)
    {
        var context = CreateOverlappingSensorContext(false);
        var visitorTransform = context.VisitorCollider.Entity.GetComponent<Transform2DComponent>();

        visitorTransform.Translation = new Vector2(300, 0);

        if (!configureCallbacks)
        {
            context.SensorCollider.OnOverlapBegin = null;
            context.SensorCollider.OnOverlapEnd = null;
            context.VisitorCollider.OnOverlapBegin = null;
            context.VisitorCollider.OnOverlapEnd = null;
        }

        // Act 0
        context.PhysicsSystem.ProcessPhysics();
        SaveVisualOutput(context.PhysicsSystem, 0);

        // Assert 0
        Assert.That(context.SensorCollider.IsColliding, Is.False);
        Assert.That(context.VisitorCollider.IsColliding, Is.False);
        AssertNoCallbacks(context);

        // Act 1
        visitorTransform.Translation = new Vector2(150, 0);

        context.PhysicsSystem.ProcessPhysics();
        SaveVisualOutput(context.PhysicsSystem, 1);

        // Assert 1
        Assert.That(context.SensorCollider.IsColliding, Is.False);
        Assert.That(context.VisitorCollider.IsColliding, Is.False);

        // Act 2
        visitorTransform.Translation = new Vector2(300, 0);

        context.PhysicsSystem.ProcessPhysics();
        SaveVisualOutput(context.PhysicsSystem, 2);

        // Assert 2
        Assert.That(context.SensorCollider.IsColliding, Is.False);
        Assert.That(context.VisitorCollider.IsColliding, Is.False);

        if (configureCallbacks)
        {
            Assert.That(context.SensorBeginEvents, Has.Count.EqualTo(1));
            Assert.That(context.SensorBeginEvents[0], Is.EqualTo(context.VisitorCollider));
            Assert.That(context.VisitorBeginEvents, Has.Count.EqualTo(1));
            Assert.That(context.VisitorBeginEvents[0], Is.EqualTo(context.SensorCollider));
            Assert.That(context.SensorEndEvents, Has.Count.EqualTo(1));
            Assert.That(context.SensorEndEvents[0], Is.EqualTo(context.VisitorCollider));
            Assert.That(context.VisitorEndEvents, Has.Count.EqualTo(1));
            Assert.That(context.VisitorEndEvents[0], Is.EqualTo(context.SensorCollider));
        }
        else
        {
            AssertNoCallbacks(context);
        }
    }

    [TestCase(false)]
    [TestCase(true)]
    public void Sensor_ShouldNotInvokeOverlapCallbacks_WhenNoColliderIsSensor(bool visitorIsKinematic)
    {
        var context = CreateOverlappingSensorContext(visitorIsKinematic);
        DisableCollisionResponseIfKinematic(context.SensorCollider);
        DisableCollisionResponseIfKinematic(context.VisitorCollider);

        context.SensorCollider.IsSensor = false;
        context.VisitorCollider.IsSensor = false;

        // Act 0
        context.PhysicsSystem.ProcessPhysics();
        SaveVisualOutput(context.PhysicsSystem, 0);

        // Assert 0
        Assert.That(context.SensorCollider.IsColliding, Is.True);
        Assert.That(context.VisitorCollider.IsColliding, Is.True);
        AssertNoCallbacks(context);

        // Act 1
        context.PhysicsSystem.ProcessPhysics();
        SaveVisualOutput(context.PhysicsSystem, 1);

        // Assert 1
        Assert.That(context.SensorCollider.IsColliding, Is.True);
        Assert.That(context.VisitorCollider.IsColliding, Is.True);
        AssertNoCallbacks(context);
    }

    [TestCase(false)]
    [TestCase(true)]
    public void Sensor_ShouldNotInvokeOverlapCallbacks_WhenOnlyAabbOverlaps_ButShapesDoNot(bool visitorIsKinematic)
    {
        var context = CreateOverlappingSensorContext(visitorIsKinematic);
        var visitorTransform = context.VisitorCollider.Entity.GetComponent<Transform2DComponent>();

        visitorTransform.Translation = new Vector2(170, 170);

        // Act
        context.PhysicsSystem.ProcessPhysics();
        SaveVisualOutput(context.PhysicsSystem, 0, postDrawAction: debugRenderer =>
        {
            debugRenderer.DrawRectangle(context.SensorCollider.BoundingRectangle, Color.Gray, Matrix3x3.Identity);
            debugRenderer.DrawRectangle(context.VisitorCollider.BoundingRectangle, Color.Gray, Matrix3x3.Identity);
        });

        // Assert
        AssertNoCallbacks(context);
    }

    [TestCase(false)]
    [TestCase(true)]
    public void Sensor_ShouldInvoke_OnOverlapBegin_And_OnOverlapEnd_WhenGeometryOverlapChangesDueToPosition(bool visitorIsKinematic)
    {
        var context = CreateOverlappingSensorContext(visitorIsKinematic);
        var visitorTransform = context.VisitorCollider.Entity.GetComponent<Transform2DComponent>();

        visitorTransform.Translation = new Vector2(300, 0);

        // Act 0
        context.PhysicsSystem.ProcessPhysics(); // No overlap -> should invoke no overlap callbacks.
        SaveVisualOutput(context.PhysicsSystem, 0);

        // Assert 0
        AssertNoCallbacks(context);

        // Act 1
        visitorTransform.Translation = new Vector2(150, 0);

        context.PhysicsSystem.ProcessPhysics(); // Overlap detected -> should invoke OnOverlapBegin.
        SaveVisualOutput(context.PhysicsSystem, 1);

        // Assert 1
        Assert.That(context.SensorBeginEvents, Has.Count.EqualTo(1));
        Assert.That(context.SensorBeginEvents[0], Is.EqualTo(context.VisitorCollider));
        Assert.That(context.VisitorBeginEvents, Has.Count.EqualTo(1));
        Assert.That(context.VisitorBeginEvents[0], Is.EqualTo(context.SensorCollider));
        Assert.That(context.SensorEndEvents, Is.Empty);
        Assert.That(context.VisitorEndEvents, Is.Empty);

        // Act 2
        context.PhysicsSystem.ProcessPhysics(); // Overlap stays -> should invoke no overlap callbacks.
        SaveVisualOutput(context.PhysicsSystem, 2);

        // Assert 2
        Assert.That(context.SensorBeginEvents, Has.Count.EqualTo(1));
        Assert.That(context.VisitorBeginEvents, Has.Count.EqualTo(1));
        Assert.That(context.SensorEndEvents, Is.Empty);
        Assert.That(context.VisitorEndEvents, Is.Empty);

        // Act 3
        visitorTransform.Translation = new Vector2(300, 0);

        context.PhysicsSystem.ProcessPhysics(); // Overlap removed -> should invoke OnOverlapEnd.
        SaveVisualOutput(context.PhysicsSystem, 3);

        // Assert 3
        Assert.That(context.SensorBeginEvents, Has.Count.EqualTo(1));
        Assert.That(context.VisitorBeginEvents, Has.Count.EqualTo(1));
        Assert.That(context.SensorEndEvents, Has.Count.EqualTo(1));
        Assert.That(context.SensorEndEvents[0], Is.EqualTo(context.VisitorCollider));
        Assert.That(context.VisitorEndEvents, Has.Count.EqualTo(1));
        Assert.That(context.VisitorEndEvents[0], Is.EqualTo(context.SensorCollider));

        // Act 4
        context.PhysicsSystem.ProcessPhysics(); // No overlap -> should invoke no overlap callbacks.
        SaveVisualOutput(context.PhysicsSystem, 4);

        // Assert 4
        Assert.That(context.SensorBeginEvents, Has.Count.EqualTo(1));
        Assert.That(context.VisitorBeginEvents, Has.Count.EqualTo(1));
        Assert.That(context.SensorEndEvents, Has.Count.EqualTo(1));
        Assert.That(context.VisitorEndEvents, Has.Count.EqualTo(1));
    }

    [TestCase(false, true)]
    [TestCase(false, false)]
    [TestCase(true, true)]
    [TestCase(true, false)]
    public void Sensor_ShouldInvoke_OnOverlapEnd_WhenOverlappingBodyIsRemoved(bool visitorIsKinematic, bool removeSensorCollider)
    {
        var context = CreateOverlappingSensorContext(visitorIsKinematic);

        // Act 0
        context.PhysicsSystem.ProcessPhysics(); // Pair is overlapping.
        SaveVisualOutput(context.PhysicsSystem, 0);

        // Assert 0
        Assert.That(context.SensorBeginEvents, Has.Count.EqualTo(1));
        Assert.That(context.SensorBeginEvents[0], Is.EqualTo(context.VisitorCollider));
        Assert.That(context.VisitorBeginEvents, Has.Count.EqualTo(1));
        Assert.That(context.VisitorBeginEvents[0], Is.EqualTo(context.SensorCollider));
        Assert.That(context.SensorEndEvents, Is.Empty);
        Assert.That(context.VisitorEndEvents, Is.Empty);

        var removedCollider = removeSensorCollider ? context.SensorCollider : context.VisitorCollider;
        var remainingCollider = removeSensorCollider ? context.VisitorCollider : context.SensorCollider;

        // Act 1
        removedCollider.Entity.RemoveComponent(removedCollider); // Removing collider component disposes underlying physics body.

        context.PhysicsSystem.ProcessPhysics(); // Cached overlap is stale -> OnOverlapEnd expected.
        SaveVisualOutput(context.PhysicsSystem, 1);

        // Assert 1
        Assert.That(context.SensorBeginEvents, Has.Count.EqualTo(1));
        Assert.That(context.VisitorBeginEvents, Has.Count.EqualTo(1));
        Assert.That(context.SensorEndEvents, Has.Count.EqualTo(1));
        Assert.That(context.SensorEndEvents[0], Is.EqualTo(context.VisitorCollider));
        Assert.That(context.VisitorEndEvents, Has.Count.EqualTo(1));
        Assert.That(context.VisitorEndEvents[0], Is.EqualTo(context.SensorCollider));

        var removedColliderEndEvents = removeSensorCollider ? context.SensorEndEvents : context.VisitorEndEvents;
        var remainingColliderEndEvents = removeSensorCollider ? context.VisitorEndEvents : context.SensorEndEvents;

        Assert.That(removedColliderEndEvents[0], Is.EqualTo(remainingCollider));
        Assert.That(remainingColliderEndEvents[0], Is.EqualTo(removedCollider));

        // Act 2
        context.PhysicsSystem.ProcessPhysics(); // Overlap already ended.
        SaveVisualOutput(context.PhysicsSystem, 2);

        // Assert 2
        Assert.That(context.SensorBeginEvents, Has.Count.EqualTo(1));
        Assert.That(context.VisitorBeginEvents, Has.Count.EqualTo(1));
        Assert.That(context.SensorEndEvents, Has.Count.EqualTo(1));
        Assert.That(context.VisitorEndEvents, Has.Count.EqualTo(1));
    }

    [TestCase(false, true)]
    [TestCase(false, false)]
    [TestCase(true, true)]
    [TestCase(true, false)]
    public void Sensor_ShouldHandle_RuntimeEnabledToggle_ForSensorAndVisitorBodies(bool visitorIsKinematic, bool disableSensor)
    {
        var context = CreateOverlappingSensorContext(visitorIsKinematic);
        var toggledCollider = disableSensor ? context.SensorCollider : context.VisitorCollider;

        toggledCollider.Enabled = false;

        // Act 0
        context.PhysicsSystem.ProcessPhysics(); // Pair overlaps geometrically, but one body is disabled.
        SaveVisualOutput(context.PhysicsSystem, 0);

        // Assert 0
        AssertNoCallbacks(context);

        // Act 1
        toggledCollider.Enabled = true;

        context.PhysicsSystem.ProcessPhysics(); // Pair is enabled and overlapping -> OnOverlapBegin expected.
        SaveVisualOutput(context.PhysicsSystem, 1);

        // Assert 1
        Assert.That(context.SensorBeginEvents, Has.Count.EqualTo(1));
        Assert.That(context.SensorBeginEvents[0], Is.EqualTo(context.VisitorCollider));
        Assert.That(context.VisitorBeginEvents, Has.Count.EqualTo(1));
        Assert.That(context.VisitorBeginEvents[0], Is.EqualTo(context.SensorCollider));
        Assert.That(context.SensorEndEvents, Is.Empty);
        Assert.That(context.VisitorEndEvents, Is.Empty);

        // Act 2
        context.PhysicsSystem.ProcessPhysics(); // Pair remains enabled and overlapping.
        SaveVisualOutput(context.PhysicsSystem, 2);

        // Assert 2
        Assert.That(context.SensorBeginEvents, Has.Count.EqualTo(1));
        Assert.That(context.VisitorBeginEvents, Has.Count.EqualTo(1));
        Assert.That(context.SensorEndEvents, Is.Empty);
        Assert.That(context.VisitorEndEvents, Is.Empty);

        // Act 3
        toggledCollider.Enabled = false;

        context.PhysicsSystem.ProcessPhysics(); // Overlap removed by disabling while still geometrically overlapping.
        SaveVisualOutput(context.PhysicsSystem, 3);

        // Assert 3
        Assert.That(context.SensorBeginEvents, Has.Count.EqualTo(1));
        Assert.That(context.VisitorBeginEvents, Has.Count.EqualTo(1));
        Assert.That(context.SensorEndEvents, Has.Count.EqualTo(1));
        Assert.That(context.SensorEndEvents[0], Is.EqualTo(context.VisitorCollider));
        Assert.That(context.VisitorEndEvents, Has.Count.EqualTo(1));
        Assert.That(context.VisitorEndEvents[0], Is.EqualTo(context.SensorCollider));

        // Act 4
        context.PhysicsSystem.ProcessPhysics(); // Still disabled.
        SaveVisualOutput(context.PhysicsSystem, 4);

        // Assert 4
        Assert.That(context.SensorBeginEvents, Has.Count.EqualTo(1));
        Assert.That(context.VisitorBeginEvents, Has.Count.EqualTo(1));
        Assert.That(context.SensorEndEvents, Has.Count.EqualTo(1));
        Assert.That(context.VisitorEndEvents, Has.Count.EqualTo(1));
    }

    [TestCase(false, true)]
    [TestCase(false, false)]
    [TestCase(true, true)]
    [TestCase(true, false)]
    public void Sensor_ShouldHandle_RuntimeIsSensorToggle_WhileOverlappingAndSeparated(bool visitorIsKinematic, bool toggleSensor)
    {
        var context = CreateOverlappingSensorContext(visitorIsKinematic);
        DisableCollisionResponseIfKinematic(context.SensorCollider);
        DisableCollisionResponseIfKinematic(context.VisitorCollider);

        var toggledCollider = toggleSensor ? context.SensorCollider : context.VisitorCollider;
        var otherCollider = toggleSensor ? context.VisitorCollider : context.SensorCollider;

        toggledCollider.IsSensor = true;
        otherCollider.IsSensor = false;

        // Act 0
        context.PhysicsSystem.ProcessPhysics(); // Pair is overlapping and one collider is sensor.
        SaveVisualOutput(context.PhysicsSystem, 0);

        // Assert 0
        Assert.That(context.SensorBeginEvents, Has.Count.EqualTo(1));
        Assert.That(context.SensorBeginEvents[0], Is.EqualTo(context.VisitorCollider));
        Assert.That(context.VisitorBeginEvents, Has.Count.EqualTo(1));
        Assert.That(context.VisitorBeginEvents[0], Is.EqualTo(context.SensorCollider));
        Assert.That(context.SensorEndEvents, Is.Empty);
        Assert.That(context.VisitorEndEvents, Is.Empty);

        // Act 1
        toggledCollider.IsSensor = false;

        context.PhysicsSystem.ProcessPhysics(); // Pair remains overlapping but with no sensors -> OnOverlapEnd expected.
        SaveVisualOutput(context.PhysicsSystem, 1);

        // Assert 1
        Assert.That(context.SensorBeginEvents, Has.Count.EqualTo(1));
        Assert.That(context.VisitorBeginEvents, Has.Count.EqualTo(1));
        Assert.That(context.SensorEndEvents, Has.Count.EqualTo(1));
        Assert.That(context.SensorEndEvents[0], Is.EqualTo(context.VisitorCollider));
        Assert.That(context.VisitorEndEvents, Has.Count.EqualTo(1));
        Assert.That(context.VisitorEndEvents[0], Is.EqualTo(context.SensorCollider));

        // Act 2
        context.PhysicsSystem.ProcessPhysics(); // No sensor and still overlapping.
        SaveVisualOutput(context.PhysicsSystem, 2);

        // Assert 2
        Assert.That(context.SensorBeginEvents, Has.Count.EqualTo(1));
        Assert.That(context.VisitorBeginEvents, Has.Count.EqualTo(1));
        Assert.That(context.SensorEndEvents, Has.Count.EqualTo(1));
        Assert.That(context.VisitorEndEvents, Has.Count.EqualTo(1));

        // Act 3
        toggledCollider.IsSensor = true;

        context.PhysicsSystem.ProcessPhysics(); // Pair remains overlapping and one collider becomes sensor -> OnOverlapBegin expected.
        SaveVisualOutput(context.PhysicsSystem, 3);

        // Assert 3
        Assert.That(context.SensorBeginEvents, Has.Count.EqualTo(2));
        Assert.That(context.SensorBeginEvents[1], Is.EqualTo(context.VisitorCollider));
        Assert.That(context.VisitorBeginEvents, Has.Count.EqualTo(2));
        Assert.That(context.VisitorBeginEvents[1], Is.EqualTo(context.SensorCollider));
        Assert.That(context.SensorEndEvents, Has.Count.EqualTo(1));
        Assert.That(context.VisitorEndEvents, Has.Count.EqualTo(1));

        // Act 4
        var visitorTransform = context.VisitorCollider.Entity.GetComponent<Transform2DComponent>();
        visitorTransform.Translation = new Vector2(300, 0);

        context.PhysicsSystem.ProcessPhysics(); // Pair becomes separated with one collider still sensor -> OnOverlapEnd expected.
        SaveVisualOutput(context.PhysicsSystem, 4);

        // Assert 4
        Assert.That(context.SensorBeginEvents, Has.Count.EqualTo(2));
        Assert.That(context.VisitorBeginEvents, Has.Count.EqualTo(2));
        Assert.That(context.SensorEndEvents, Has.Count.EqualTo(2));
        Assert.That(context.SensorEndEvents[1], Is.EqualTo(context.VisitorCollider));
        Assert.That(context.VisitorEndEvents, Has.Count.EqualTo(2));
        Assert.That(context.VisitorEndEvents[1], Is.EqualTo(context.SensorCollider));

        // Act 5
        toggledCollider.IsSensor = false;

        context.PhysicsSystem.ProcessPhysics(); // Toggle while separated should not create callbacks.
        SaveVisualOutput(context.PhysicsSystem, 5);

        // Assert 5
        Assert.That(context.SensorBeginEvents, Has.Count.EqualTo(2));
        Assert.That(context.VisitorBeginEvents, Has.Count.EqualTo(2));
        Assert.That(context.SensorEndEvents, Has.Count.EqualTo(2));
        Assert.That(context.VisitorEndEvents, Has.Count.EqualTo(2));

        // Act 6
        toggledCollider.IsSensor = true;

        context.PhysicsSystem.ProcessPhysics(); // Toggle while separated should not create callbacks.
        SaveVisualOutput(context.PhysicsSystem, 6);

        // Assert 6
        Assert.That(context.SensorBeginEvents, Has.Count.EqualTo(2));
        Assert.That(context.VisitorBeginEvents, Has.Count.EqualTo(2));
        Assert.That(context.SensorEndEvents, Has.Count.EqualTo(2));
        Assert.That(context.VisitorEndEvents, Has.Count.EqualTo(2));
    }

    [TestCase(false)]
    [TestCase(true)]
    public void Sensor_ShouldHandle_RuntimeCollisionFilteringToggle_ForLayerAndMask(bool visitorIsKinematic)
    {
        var context = CreateOverlappingSensorContext(visitorIsKinematic);

        // Act 0
        context.PhysicsSystem.ProcessPhysics(); // Pair is enabled and overlapping with matching filters.
        SaveVisualOutput(context.PhysicsSystem, 0);

        // Assert 0
        Assert.That(context.SensorBeginEvents, Has.Count.EqualTo(1));
        Assert.That(context.SensorBeginEvents[0], Is.EqualTo(context.VisitorCollider));
        Assert.That(context.VisitorBeginEvents, Has.Count.EqualTo(1));
        Assert.That(context.VisitorBeginEvents[0], Is.EqualTo(context.SensorCollider));
        Assert.That(context.SensorEndEvents, Is.Empty);
        Assert.That(context.VisitorEndEvents, Is.Empty);

        // Act 1
        context.SensorCollider.CollisionMask = CollisionBitmask.None;

        context.PhysicsSystem.ProcessPhysics(); // Filters reject overlapping pair -> OnOverlapEnd expected.
        SaveVisualOutput(context.PhysicsSystem, 1);

        // Assert 1
        Assert.That(context.SensorBeginEvents, Has.Count.EqualTo(1));
        Assert.That(context.VisitorBeginEvents, Has.Count.EqualTo(1));
        Assert.That(context.SensorEndEvents, Has.Count.EqualTo(1));
        Assert.That(context.SensorEndEvents[0], Is.EqualTo(context.VisitorCollider));
        Assert.That(context.VisitorEndEvents, Has.Count.EqualTo(1));
        Assert.That(context.VisitorEndEvents[0], Is.EqualTo(context.SensorCollider));

        // Act 2
        context.PhysicsSystem.ProcessPhysics(); // Filters still reject pair.
        SaveVisualOutput(context.PhysicsSystem, 2);

        // Assert 2
        Assert.That(context.SensorBeginEvents, Has.Count.EqualTo(1));
        Assert.That(context.VisitorBeginEvents, Has.Count.EqualTo(1));
        Assert.That(context.SensorEndEvents, Has.Count.EqualTo(1));
        Assert.That(context.VisitorEndEvents, Has.Count.EqualTo(1));

        // Act 3
        context.SensorCollider.CollisionMask = CollisionBitmask.All;

        context.PhysicsSystem.ProcessPhysics(); // Filters accept pair again -> OnOverlapBegin expected.
        SaveVisualOutput(context.PhysicsSystem, 3);

        // Assert 3
        Assert.That(context.SensorBeginEvents, Has.Count.EqualTo(2));
        Assert.That(context.SensorBeginEvents[1], Is.EqualTo(context.VisitorCollider));
        Assert.That(context.VisitorBeginEvents, Has.Count.EqualTo(2));
        Assert.That(context.VisitorBeginEvents[1], Is.EqualTo(context.SensorCollider));
        Assert.That(context.SensorEndEvents, Has.Count.EqualTo(1));
        Assert.That(context.VisitorEndEvents, Has.Count.EqualTo(1));

        // Act 4
        context.VisitorCollider.CollisionLayer = CollisionBitmask.None;

        context.PhysicsSystem.ProcessPhysics(); // Filters reject pair via layer -> OnOverlapEnd expected.
        SaveVisualOutput(context.PhysicsSystem, 4);

        // Assert 4
        Assert.That(context.SensorBeginEvents, Has.Count.EqualTo(2));
        Assert.That(context.VisitorBeginEvents, Has.Count.EqualTo(2));
        Assert.That(context.SensorEndEvents, Has.Count.EqualTo(2));
        Assert.That(context.SensorEndEvents[1], Is.EqualTo(context.VisitorCollider));
        Assert.That(context.VisitorEndEvents, Has.Count.EqualTo(2));
        Assert.That(context.VisitorEndEvents[1], Is.EqualTo(context.SensorCollider));

        // Act 5
        context.VisitorCollider.CollisionLayer = CollisionBitmask.All;

        context.PhysicsSystem.ProcessPhysics(); // Filters accept pair again -> OnOverlapBegin expected.
        SaveVisualOutput(context.PhysicsSystem, 5);

        // Assert 5
        Assert.That(context.SensorBeginEvents, Has.Count.EqualTo(3));
        Assert.That(context.SensorBeginEvents[2], Is.EqualTo(context.VisitorCollider));
        Assert.That(context.VisitorBeginEvents, Has.Count.EqualTo(3));
        Assert.That(context.VisitorBeginEvents[2], Is.EqualTo(context.SensorCollider));
        Assert.That(context.SensorEndEvents, Has.Count.EqualTo(2));
        Assert.That(context.VisitorEndEvents, Has.Count.EqualTo(2));
    }

    [Test]
    public void Sensor_ShouldInvokeBeginAndEndExactlyOnce_PerLogicalTransition_WithSubsteps()
    {
        var physicsConfiguration = new PhysicsConfiguration { Substeps = 8, EnableDebugRendering = true };
        var context = CreateOverlappingSensorContext(sensorIsKinematic: true, visitorIsKinematic: false, physicsConfiguration);
        var visitorTransform = context.VisitorCollider.Entity.GetComponent<Transform2DComponent>();

        visitorTransform.Translation = new Vector2(300, 0);

        // Act 0
        context.PhysicsSystem.ProcessPhysics();
        SaveVisualOutput(context.PhysicsSystem, 0);

        // Assert 0
        AssertNoCallbacks(context);

        // Act 1
        visitorTransform.Translation = new Vector2(150, 0);

        context.PhysicsSystem.ProcessPhysics();
        SaveVisualOutput(context.PhysicsSystem, 1);

        // Assert 1
        Assert.That(context.SensorBeginEvents, Has.Count.EqualTo(1));
        Assert.That(context.SensorBeginEvents[0], Is.EqualTo(context.VisitorCollider));
        Assert.That(context.VisitorBeginEvents, Has.Count.EqualTo(1));
        Assert.That(context.VisitorBeginEvents[0], Is.EqualTo(context.SensorCollider));
        Assert.That(context.SensorEndEvents, Is.Empty);
        Assert.That(context.VisitorEndEvents, Is.Empty);

        // Act 2
        context.PhysicsSystem.ProcessPhysics();
        SaveVisualOutput(context.PhysicsSystem, 2);

        // Assert 2
        Assert.That(context.SensorBeginEvents, Has.Count.EqualTo(1));
        Assert.That(context.VisitorBeginEvents, Has.Count.EqualTo(1));
        Assert.That(context.SensorEndEvents, Is.Empty);
        Assert.That(context.VisitorEndEvents, Is.Empty);

        // Act 3
        visitorTransform.Translation = new Vector2(300, 0);

        context.PhysicsSystem.ProcessPhysics();
        SaveVisualOutput(context.PhysicsSystem, 3);

        // Assert 3
        Assert.That(context.SensorBeginEvents, Has.Count.EqualTo(1));
        Assert.That(context.VisitorBeginEvents, Has.Count.EqualTo(1));
        Assert.That(context.SensorEndEvents, Has.Count.EqualTo(1));
        Assert.That(context.SensorEndEvents[0], Is.EqualTo(context.VisitorCollider));
        Assert.That(context.VisitorEndEvents, Has.Count.EqualTo(1));
        Assert.That(context.VisitorEndEvents[0], Is.EqualTo(context.SensorCollider));

        // Act 4
        context.PhysicsSystem.ProcessPhysics();
        SaveVisualOutput(context.PhysicsSystem, 4);

        // Assert 4
        Assert.That(context.SensorBeginEvents, Has.Count.EqualTo(1));
        Assert.That(context.VisitorBeginEvents, Has.Count.EqualTo(1));
        Assert.That(context.SensorEndEvents, Has.Count.EqualTo(1));
        Assert.That(context.VisitorEndEvents, Has.Count.EqualTo(1));
    }

    [Test]
    public void Sensor_ShouldInvokeBeginAndEndExactlyOnce_WhenFullTransitionOccursWithinSingleFrameAcrossSubsteps()
    {
        TimeSystem.FixedDeltaTime.Returns(TimeSpan.FromSeconds(1.0 / 60.0));

        var physicsConfiguration = new PhysicsConfiguration { Substeps = 8, EnableDebugRendering = true };
        var context = CreateOverlappingSensorContext(sensorIsKinematic: false, visitorIsKinematic: true, physicsConfiguration);

        var visitorTransform = context.VisitorCollider.Entity.GetComponent<Transform2DComponent>();
        var visitorBody = context.VisitorCollider.Entity.GetComponent<KinematicRigidBody2DComponent>();

        var callbackOrder = new List<string>();
        context.SensorCollider.OnOverlapBegin = collider =>
        {
            context.SensorBeginEvents.Add(collider);
            callbackOrder.Add("begin");
        };
        context.VisitorCollider.OnOverlapBegin = collider =>
        {
            context.VisitorBeginEvents.Add(collider);
            callbackOrder.Add("begin");
        };
        context.SensorCollider.OnOverlapEnd = collider =>
        {
            context.SensorEndEvents.Add(collider);
            callbackOrder.Add("end");
        };
        context.VisitorCollider.OnOverlapEnd = collider =>
        {
            context.VisitorEndEvents.Add(collider);
            callbackOrder.Add("end");
        };

        visitorTransform.Translation = new Vector2(-300, 0);
        // With FixedDeltaTime = 1/60 s and velocity 36000, body travels 600 units in a single frame.
        // Starting at X=-300, the center reaches X=300 after one ProcessPhysics call.
        // With radius 100 for both circles and sensor at X=0, this means: outside overlap -> enter overlap -> exit overlap
        // all within one frame while substeps are processed.
        visitorBody.LinearVelocity = new Vector2(36000, 0);

        // Act 0
        SaveVisualOutput(context.PhysicsSystem, 0);
        context.PhysicsSystem.ProcessPhysics();
        SaveVisualOutput(context.PhysicsSystem, 1);

        // Assert 0
        Assert.That(context.SensorBeginEvents, Has.Count.EqualTo(1));
        Assert.That(context.SensorBeginEvents[0], Is.EqualTo(context.VisitorCollider));
        Assert.That(context.VisitorBeginEvents, Has.Count.EqualTo(1));
        Assert.That(context.VisitorBeginEvents[0], Is.EqualTo(context.SensorCollider));
        Assert.That(context.SensorEndEvents, Has.Count.EqualTo(1));
        Assert.That(context.SensorEndEvents[0], Is.EqualTo(context.VisitorCollider));
        Assert.That(context.VisitorEndEvents, Has.Count.EqualTo(1));
        Assert.That(context.VisitorEndEvents[0], Is.EqualTo(context.SensorCollider));
        Assert.That(callbackOrder, Is.EqualTo(new[] { "begin", "begin", "end", "end" }));

        // Act 1
        visitorBody.LinearVelocity = Vector2.Zero;

        context.PhysicsSystem.ProcessPhysics();
        SaveVisualOutput(context.PhysicsSystem, 2);

        // Assert 1
        Assert.That(context.SensorBeginEvents, Has.Count.EqualTo(1));
        Assert.That(context.VisitorBeginEvents, Has.Count.EqualTo(1));
        Assert.That(context.SensorEndEvents, Has.Count.EqualTo(1));
        Assert.That(context.VisitorEndEvents, Has.Count.EqualTo(1));
        Assert.That(callbackOrder, Has.Count.EqualTo(4));
    }

    private SensorOverlapContext CreateOverlappingSensorContext(bool visitorIsKinematic)
    {
        return CreateOverlappingSensorContext(true, visitorIsKinematic, null);
    }

    private SensorOverlapContext CreateOverlappingSensorContext(bool sensorIsKinematic, bool visitorIsKinematic)
    {
        return CreateOverlappingSensorContext(sensorIsKinematic, visitorIsKinematic, null);
    }

    private SensorOverlapContext CreateOverlappingSensorContext(bool sensorIsKinematic, bool visitorIsKinematic,
        PhysicsConfiguration? physicsConfiguration)
    {
        var physicsSystem = physicsConfiguration is null ? GetPhysicsSystem() : GetPhysicsSystem(physicsConfiguration);

        var sensorBody = CreateBody(sensorIsKinematic, 0, 0, 100);
        var visitorBody = CreateBody(visitorIsKinematic, 150, 0, 100);

        var sensorCollider = sensorBody.GetComponent<CircleColliderComponent>();
        var visitorCollider = visitorBody.GetComponent<CircleColliderComponent>();

        sensorCollider.IsSensor = true;

        var sensorBeginEvents = new List<Collider2DComponent>();
        var sensorEndEvents = new List<Collider2DComponent>();
        var visitorBeginEvents = new List<Collider2DComponent>();
        var visitorEndEvents = new List<Collider2DComponent>();

        sensorCollider.OnOverlapBegin = sensorBeginEvents.Add;
        sensorCollider.OnOverlapEnd = sensorEndEvents.Add;
        visitorCollider.OnOverlapBegin = visitorBeginEvents.Add;
        visitorCollider.OnOverlapEnd = visitorEndEvents.Add;

        return new SensorOverlapContext(physicsSystem, sensorCollider, visitorCollider, sensorBeginEvents, sensorEndEvents, visitorBeginEvents,
            visitorEndEvents);
    }

    private static void AssertNoCallbacks(SensorOverlapContext context)
    {
        Assert.That(context.SensorBeginEvents, Is.Empty);
        Assert.That(context.VisitorBeginEvents, Is.Empty);
        Assert.That(context.SensorEndEvents, Is.Empty);
        Assert.That(context.VisitorEndEvents, Is.Empty);
    }

    private static void DisableCollisionResponseIfKinematic(Collider2DComponent collider)
    {
        if (collider.Entity.HasComponent<KinematicRigidBody2DComponent>())
        {
            collider.Entity.GetComponent<KinematicRigidBody2DComponent>().EnableCollisionResponse = false;
        }
    }

    private Entity CreateBody(bool isKinematic, double x, double y, double radius)
    {
        return isKinematic
            ? CreateCircleKinematicBody(x, y, radius)
            : CreateCircleStaticBody(x, y, radius);
    }

    private sealed class SensorOverlapContext
    {
        public SensorOverlapContext(PhysicsSystem physicsSystem, CircleColliderComponent sensorCollider, CircleColliderComponent visitorCollider,
            List<Collider2DComponent> sensorBeginEvents, List<Collider2DComponent> sensorEndEvents,
            List<Collider2DComponent> visitorBeginEvents, List<Collider2DComponent> visitorEndEvents)
        {
            PhysicsSystem = physicsSystem;
            SensorCollider = sensorCollider;
            VisitorCollider = visitorCollider;
            SensorBeginEvents = sensorBeginEvents;
            SensorEndEvents = sensorEndEvents;
            VisitorBeginEvents = visitorBeginEvents;
            VisitorEndEvents = visitorEndEvents;
        }

        public PhysicsSystem PhysicsSystem { get; }
        public CircleColliderComponent SensorCollider { get; }
        public CircleColliderComponent VisitorCollider { get; }

        public List<Collider2DComponent> SensorBeginEvents { get; }
        public List<Collider2DComponent> SensorEndEvents { get; }
        public List<Collider2DComponent> VisitorBeginEvents { get; }
        public List<Collider2DComponent> VisitorEndEvents { get; }
    }
}