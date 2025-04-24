using Geisha.Engine.Core.Collections;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Physics.Components;
using Geisha.Engine.Physics;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Geisha.Engine.UnitTests.Physics.Systems.PhysicsSystemTests;

[TestFixture]
[DefaultFloatingPointTolerance(Epsilon)]
public class CollisionContactsTests : PhysicsSystemTestsBase
{
    public sealed class ContactsTestCase
    {
        public AxisAlignedRectangle Rectangle1 { get; init; }
        public AxisAlignedRectangle Rectangle2 { get; init; }

        public double Rotation1 { get; init; }
        public double Rotation2 { get; init; }

        public Vector2 ExpectedCollisionNormal { get; init; }
        public double ExpectedSeparationDepth { get; init; }
        public ReadOnlyFixedList2<ContactPoint2D> ExpectedContactPoints { get; init; }
    }

    public static IEnumerable<TestCaseData> ContactsTestCases => new[]
    {
        // Edges overlapping
        new TestCaseData(new ContactsTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(-1, 2.5), new Vector2(10, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 2.5), new Vector2(10, 5)),
            ExpectedCollisionNormal = new Vector2(-1, 0),
            ExpectedSeparationDepth = 4,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(4, 5), new Vector2(5, 2.5), new Vector2(-1, 2.5)),
                new ContactPoint2D(new Vector2(4, 0), new Vector2(5, -2.5), new Vector2(-1, -2.5)))
        }).SetName($"01_{nameof(RectangleKinematicBody_vs_RectangleStaticBody)}"),
        new TestCaseData(new ContactsTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(5, 5), new Vector2(10, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 2.5), new Vector2(10, 5)),
            ExpectedCollisionNormal = new Vector2(0, 1),
            ExpectedSeparationDepth = 2.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(0, 2.5), new Vector2(-5, -2.5), new Vector2(-5, 0)),
                new ContactPoint2D(new Vector2(10, 2.5), new Vector2(5, -2.5), new Vector2(5, 0)))
        }).SetName($"02_{nameof(RectangleKinematicBody_vs_RectangleStaticBody)}"),
        new TestCaseData(new ContactsTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(11, 2.5), new Vector2(10, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 2.5), new Vector2(10, 5)),
            ExpectedCollisionNormal = new Vector2(1, 0),
            ExpectedSeparationDepth = 4,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(6, 5), new Vector2(-5, 2.5), new Vector2(1, 2.5)),
                new ContactPoint2D(new Vector2(6, 0), new Vector2(-5, -2.5), new Vector2(1, -2.5)))
        }).SetName($"03_{nameof(RectangleKinematicBody_vs_RectangleStaticBody)}"),
        new TestCaseData(new ContactsTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(5, 0), new Vector2(10, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 2.5), new Vector2(10, 5)),
            ExpectedCollisionNormal = new Vector2(0, -1),
            ExpectedSeparationDepth = 2.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(0, 2.5), new Vector2(-5, 2.5), new Vector2(-5, 0)),
                new ContactPoint2D(new Vector2(10, 2.5), new Vector2(5, 2.5), new Vector2(5, 0)))
        }).SetName($"04_{nameof(RectangleKinematicBody_vs_RectangleStaticBody)}"),
        // Edges touching
        new TestCaseData(new ContactsTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(-5, 2.5), new Vector2(10, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 2.5), new Vector2(10, 5)),
            ExpectedCollisionNormal = new Vector2(-1, 0),
            ExpectedSeparationDepth = 0,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(0, 5), new Vector2(5, 2.5), new Vector2(-5, 2.5)),
                new ContactPoint2D(new Vector2(0, 0), new Vector2(5, -2.5), new Vector2(-5, -2.5)))
        }).SetName($"05_{nameof(RectangleKinematicBody_vs_RectangleStaticBody)}"),
        new TestCaseData(new ContactsTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(5, 7.5), new Vector2(10, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 2.5), new Vector2(10, 5)),
            ExpectedCollisionNormal = new Vector2(0, 1),
            ExpectedSeparationDepth = 0,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(0, 5), new Vector2(-5, -2.5), new Vector2(-5, 2.5)),
                new ContactPoint2D(new Vector2(10, 5), new Vector2(5, -2.5), new Vector2(5, 2.5)))
        }).SetName($"06_{nameof(RectangleKinematicBody_vs_RectangleStaticBody)}"),
        new TestCaseData(new ContactsTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(15, 2.5), new Vector2(10, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 2.5), new Vector2(10, 5)),
            ExpectedCollisionNormal = new Vector2(1, 0),
            ExpectedSeparationDepth = 0,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(10, 5), new Vector2(-5, 2.5), new Vector2(5, 2.5)),
                new ContactPoint2D(new Vector2(10, 0), new Vector2(-5, -2.5), new Vector2(5, -2.5)))
        }).SetName($"07_{nameof(RectangleKinematicBody_vs_RectangleStaticBody)}"),
        new TestCaseData(new ContactsTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(5, -2.5), new Vector2(10, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 2.5), new Vector2(10, 5)),
            ExpectedCollisionNormal = new Vector2(0, -1),
            ExpectedSeparationDepth = 0,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(0, 0), new Vector2(-5, 2.5), new Vector2(-5, -2.5)),
                new ContactPoint2D(new Vector2(10, 0), new Vector2(5, 2.5), new Vector2(5, -2.5)))
        }).SetName($"08_{nameof(RectangleKinematicBody_vs_RectangleStaticBody)}"),
        // Single vertex overlapping (kinematic into static)
        new TestCaseData(new ContactsTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(-4, 1), new Vector2(10, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 2.5), new Vector2(10, 5)),
            Rotation1 = Angle.Deg2Rad(45),
            ExpectedCollisionNormal = new Vector2(-1, 0),
            ExpectedSeparationDepth = 1.303300,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(1.303300, 2.767766), new Vector2(5, -2.5), new Vector2(-3.696699, 0.267766)))
        }).SetName($"09_{nameof(RectangleKinematicBody_vs_RectangleStaticBody)}"),
        new TestCaseData(new ContactsTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(5, 9), new Vector2(10, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 2.5), new Vector2(10, 5)),
            Rotation1 = Angle.Deg2Rad(45),
            ExpectedCollisionNormal = new Vector2(0, 1),
            ExpectedSeparationDepth = 1.303300,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(3.232233, 3.696699), new Vector2(-5, -2.5), new Vector2(-1.767766, 1.196699)))
        }).SetName($"10_{nameof(RectangleKinematicBody_vs_RectangleStaticBody)}"),
        new TestCaseData(new ContactsTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(14, 4), new Vector2(10, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 2.5), new Vector2(10, 5)),
            Rotation1 = Angle.Deg2Rad(45),
            ExpectedCollisionNormal = new Vector2(1, 0),
            ExpectedSeparationDepth = 1.303300,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(8.696699, 2.232233), new Vector2(-5, 2.5), new Vector2(3.696699, -0.267766)))
        }).SetName($"11_{nameof(RectangleKinematicBody_vs_RectangleStaticBody)}"),
        new TestCaseData(new ContactsTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(5, -4), new Vector2(10, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 2.5), new Vector2(10, 5)),
            Rotation1 = Angle.Deg2Rad(45),
            ExpectedCollisionNormal = new Vector2(0, -1),
            ExpectedSeparationDepth = 1.303300,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(6.767766, 1.303300), new Vector2(5, 2.5), new Vector2(1.767766, -1.196699)))
        }).SetName($"12_{nameof(RectangleKinematicBody_vs_RectangleStaticBody)}"),
        // Single vertex overlapping (static into kinematic)
        new TestCaseData(new ContactsTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(0, 5), new Vector2(10, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 2.5), new Vector2(10, 5)),
            Rotation1 = Angle.Deg2Rad(45),
            ExpectedCollisionNormal = new Vector2(-0.707106, 0.707106),
            ExpectedSeparationDepth = 2.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(0, 5), new Vector2(0, 0), new Vector2(-5, 2.5)))
        }).SetName($"13_{nameof(RectangleKinematicBody_vs_RectangleStaticBody)}"),
        new TestCaseData(new ContactsTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(10, 5), new Vector2(10, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 2.5), new Vector2(10, 5)),
            Rotation1 = Angle.Deg2Rad(-45),
            ExpectedCollisionNormal = new Vector2(0.707106, 0.707106),
            ExpectedSeparationDepth = 2.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(10, 5), new Vector2(0, 0), new Vector2(5, 2.5)))
        }).SetName($"14_{nameof(RectangleKinematicBody_vs_RectangleStaticBody)}"),
        new TestCaseData(new ContactsTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(10, 0), new Vector2(10, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 2.5), new Vector2(10, 5)),
            Rotation1 = Angle.Deg2Rad(45),
            ExpectedCollisionNormal = new Vector2(0.707106, -0.707106),
            ExpectedSeparationDepth = 2.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(10, 0), new Vector2(0, 0), new Vector2(5, -2.5)))
        }).SetName($"15_{nameof(RectangleKinematicBody_vs_RectangleStaticBody)}"),
        new TestCaseData(new ContactsTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(0, 0), new Vector2(10, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 2.5), new Vector2(10, 5)),
            Rotation1 = Angle.Deg2Rad(-45),
            ExpectedCollisionNormal = new Vector2(-0.707106, -0.707106),
            ExpectedSeparationDepth = 2.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(0, 0), new Vector2(0, 0), new Vector2(-5, -2.5)))
        }).SetName($"16_{nameof(RectangleKinematicBody_vs_RectangleStaticBody)}"),
        // Two contact points without clipping
        new TestCaseData(new ContactsTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(-2, 5), new Vector2(5, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 5), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(-1, 0),
            ExpectedSeparationDepth = 0.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(0.5, 7.5), new Vector2(2.5, 2.5), new Vector2(-4.5, 2.5)),
                new ContactPoint2D(new Vector2(0.5, 2.5), new Vector2(2.5, -2.5), new Vector2(-4.5, -2.5)))
        }).SetName($"17_{nameof(RectangleKinematicBody_vs_RectangleStaticBody)}"),
        new TestCaseData(new ContactsTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(5, 12), new Vector2(5, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 5), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(0, 1),
            ExpectedSeparationDepth = 0.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(2.5, 9.5), new Vector2(-2.5, -2.5), new Vector2(-2.5, 4.5)),
                new ContactPoint2D(new Vector2(7.5, 9.5), new Vector2(2.5, -2.5), new Vector2(2.5, 4.5)))
        }).SetName($"18_{nameof(RectangleKinematicBody_vs_RectangleStaticBody)}"),
        new TestCaseData(new ContactsTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(12, 5), new Vector2(5, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 5), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(1, 0),
            ExpectedSeparationDepth = 0.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(9.5, 7.5), new Vector2(-2.5, 2.5), new Vector2(4.5, 2.5)),
                new ContactPoint2D(new Vector2(9.5, 2.5), new Vector2(-2.5, -2.5), new Vector2(4.5, -2.5)))
        }).SetName($"19_{nameof(RectangleKinematicBody_vs_RectangleStaticBody)}"),
        new TestCaseData(new ContactsTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(5, -2), new Vector2(5, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 5), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(0, -1),
            ExpectedSeparationDepth = 0.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(2.5, 0.5), new Vector2(-2.5, 2.5), new Vector2(-2.5, -4.5)),
                new ContactPoint2D(new Vector2(7.5, 0.5), new Vector2(2.5, 2.5), new Vector2(2.5, -4.5)))
        }).SetName($"20_{nameof(RectangleKinematicBody_vs_RectangleStaticBody)}"),
        // Two contact points and one from clipping
        new TestCaseData(new ContactsTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(-2, 1), new Vector2(5, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 5), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(-1, 0),
            ExpectedSeparationDepth = 0.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(0.5, 3.5), new Vector2(2.5, 2.5), new Vector2(-4.5, -1.5)),
                new ContactPoint2D(new Vector2(0.5, 0), new Vector2(2.5, -1), new Vector2(-4.5, -5)))
        }).SetName($"21_{nameof(RectangleKinematicBody_vs_RectangleStaticBody)}"),
        new TestCaseData(new ContactsTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(-2, 9), new Vector2(5, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 5), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(-1, 0),
            ExpectedSeparationDepth = 0.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(0.5, 10), new Vector2(2.5, 1), new Vector2(-4.5, 5)),
                new ContactPoint2D(new Vector2(0.5, 6.5), new Vector2(2.5, -2.5), new Vector2(-4.5, 1.5)))
        }).SetName($"22_{nameof(RectangleKinematicBody_vs_RectangleStaticBody)}"),
        new TestCaseData(new ContactsTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(1, 12), new Vector2(5, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 5), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(0, 1),
            ExpectedSeparationDepth = 0.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(0, 9.5), new Vector2(-1, -2.5), new Vector2(-5, 4.5)),
                new ContactPoint2D(new Vector2(3.5, 9.5), new Vector2(2.5, -2.5), new Vector2(-1.5, 4.5)))
        }).SetName($"23_{nameof(RectangleKinematicBody_vs_RectangleStaticBody)}"),
        new TestCaseData(new ContactsTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(9, 12), new Vector2(5, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 5), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(0, 1),
            ExpectedSeparationDepth = 0.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(6.5, 9.5), new Vector2(-2.5, -2.5), new Vector2(1.5, 4.5)),
                new ContactPoint2D(new Vector2(10, 9.5), new Vector2(1, -2.5), new Vector2(5, 4.5)))
        }).SetName($"24_{nameof(RectangleKinematicBody_vs_RectangleStaticBody)}"),
        new TestCaseData(new ContactsTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(12, 9), new Vector2(5, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 5), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(1, 0),
            ExpectedSeparationDepth = 0.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(9.5, 10), new Vector2(-2.5, 1), new Vector2(4.5, 5)),
                new ContactPoint2D(new Vector2(9.5, 6.5), new Vector2(-2.5, -2.5), new Vector2(4.5, 1.5)))
        }).SetName($"25_{nameof(RectangleKinematicBody_vs_RectangleStaticBody)}"),
        new TestCaseData(new ContactsTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(12, 1), new Vector2(5, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 5), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(1, 0),
            ExpectedSeparationDepth = 0.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(9.5, 3.5), new Vector2(-2.5, 2.5), new Vector2(4.5, -1.5)),
                new ContactPoint2D(new Vector2(9.5, 0), new Vector2(-2.5, -1), new Vector2(4.5, -5)))
        }).SetName($"26_{nameof(RectangleKinematicBody_vs_RectangleStaticBody)}"),
        new TestCaseData(new ContactsTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(9, -2), new Vector2(5, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 5), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(0, -1),
            ExpectedSeparationDepth = 0.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(6.5, 0.5), new Vector2(-2.5, 2.5), new Vector2(1.5, -4.5)),
                new ContactPoint2D(new Vector2(10, 0.5), new Vector2(1, 2.5), new Vector2(5, -4.5)))
        }).SetName($"27_{nameof(RectangleKinematicBody_vs_RectangleStaticBody)}"),
        new TestCaseData(new ContactsTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(1, -2), new Vector2(5, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 5), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(0, -1),
            ExpectedSeparationDepth = 0.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(0, 0.5), new Vector2(-1, 2.5), new Vector2(-5, -4.5)),
                new ContactPoint2D(new Vector2(3.5, 0.5), new Vector2(2.5, 2.5), new Vector2(-1.5, -4.5)))
        }).SetName($"28_{nameof(RectangleKinematicBody_vs_RectangleStaticBody)}")
    };

    [TestCaseSource(nameof(ContactsTestCases))]
    public void RectangleKinematicBody_vs_RectangleStaticBody(ContactsTestCase testCase)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var kinematicBody = CreateRectangleKinematicBody(testCase.Rectangle1, testCase.Rotation1);
        var staticBody = CreateRectangleStaticBody(testCase.Rectangle2, testCase.Rotation2);

        SaveVisualOutput(physicsSystem, 0, 10);

        // Assume
        Assert.That(kinematicBody.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
        Assert.That(staticBody.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);

        // Act
        physicsSystem.ProcessPhysics();

        SaveVisualOutput(physicsSystem, 1, 10);

        // Assert
        var kinematicBodyCollider = kinematicBody.GetComponent<RectangleColliderComponent>();
        var staticBodyCollider = staticBody.GetComponent<RectangleColliderComponent>();

        Assert.That(kinematicBodyCollider.IsColliding, Is.True);
        Assert.That(kinematicBodyCollider.Contacts, Has.Count.EqualTo(1));
        Assert.That(kinematicBodyCollider.Contacts[0].ThisCollider, Is.EqualTo(kinematicBodyCollider));
        Assert.That(kinematicBodyCollider.Contacts[0].OtherCollider, Is.EqualTo(staticBodyCollider));
        Assert.That(kinematicBodyCollider.Contacts[0].CollisionNormal, Is.EqualTo(testCase.ExpectedCollisionNormal).Using(Vector2Comparer));
        Assert.That(kinematicBodyCollider.Contacts[0].SeparationDepth, Is.EqualTo(testCase.ExpectedSeparationDepth));
        Assert.That(kinematicBodyCollider.Contacts[0].ContactPoints.ToArray(),
            Is.EquivalentTo(testCase.ExpectedContactPoints.ToArray()).Using<ContactPoint2D, ContactPoint2D>(ContactPoint2DComparison));

        Assert.That(staticBodyCollider.IsColliding, Is.True);
        Assert.That(staticBodyCollider.Contacts, Has.Count.EqualTo(1));
        Assert.That(staticBodyCollider.Contacts[0].ThisCollider, Is.EqualTo(staticBodyCollider));
        Assert.That(staticBodyCollider.Contacts[0].OtherCollider, Is.EqualTo(kinematicBodyCollider));
        Assert.That(staticBodyCollider.Contacts[0].CollisionNormal, Is.EqualTo(-testCase.ExpectedCollisionNormal).Using(Vector2Comparer));
        Assert.That(staticBodyCollider.Contacts[0].SeparationDepth, Is.EqualTo(testCase.ExpectedSeparationDepth));

        var reflectedExpectedContactPoints = testCase.ExpectedContactPoints.ToArray()
            .Select(cp => new ContactPoint2D(cp.WorldPosition, cp.OtherLocalPosition, cp.ThisLocalPosition)).ToArray();
        Assert.That(staticBodyCollider.Contacts[0].ContactPoints.ToArray(),
            Is.EquivalentTo(reflectedExpectedContactPoints).Using<ContactPoint2D, ContactPoint2D>(ContactPoint2DComparison));
    }
}