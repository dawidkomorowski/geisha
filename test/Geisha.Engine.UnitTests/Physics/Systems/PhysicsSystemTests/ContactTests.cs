using Geisha.Engine.Core.Collections;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Physics.Components;
using Geisha.Engine.Physics;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Geisha.Engine.Core.Components;

namespace Geisha.Engine.UnitTests.Physics.Systems.PhysicsSystemTests;

[TestFixture]
[DefaultFloatingPointTolerance(Epsilon)]
public class ContactTests : PhysicsSystemTestsBase
{
    #region Rectangle and Rectangle

    public sealed class RectangleAndRectangleTestCase
    {
        public AxisAlignedRectangle Rectangle1 { get; init; }
        public AxisAlignedRectangle Rectangle2 { get; init; }

        public double Rotation1 { get; init; }
        public double Rotation2 { get; init; }

        public Vector2 ExpectedCollisionNormal { get; init; }
        public double ExpectedPenetrationDepth { get; init; }
        public ReadOnlyFixedList2<ContactPoint2D> ExpectedContactPoints { get; init; }
    }

    // When updating these test cases, consider updating test cases for Rectangle and Tile collision.
    public static IEnumerable<TestCaseData> RectangleAndRectangleTestCases => new[]
    {
        // Edges overlapping
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(-1, 2.5), new Vector2(10, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 2.5), new Vector2(10, 5)),
            ExpectedCollisionNormal = new Vector2(-1, 0),
            ExpectedPenetrationDepth = 4,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(4, 5), new Vector2(5, 2.5), new Vector2(-1, 2.5)),
                new ContactPoint2D(new Vector2(4, 0), new Vector2(5, -2.5), new Vector2(-1, -2.5)))
        }).SetName($"01_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(5, 5), new Vector2(10, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 2.5), new Vector2(10, 5)),
            ExpectedCollisionNormal = new Vector2(0, 1),
            ExpectedPenetrationDepth = 2.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(0, 2.5), new Vector2(-5, -2.5), new Vector2(-5, 0)),
                new ContactPoint2D(new Vector2(10, 2.5), new Vector2(5, -2.5), new Vector2(5, 0)))
        }).SetName($"02_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(11, 2.5), new Vector2(10, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 2.5), new Vector2(10, 5)),
            ExpectedCollisionNormal = new Vector2(1, 0),
            ExpectedPenetrationDepth = 4,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(6, 5), new Vector2(-5, 2.5), new Vector2(1, 2.5)),
                new ContactPoint2D(new Vector2(6, 0), new Vector2(-5, -2.5), new Vector2(1, -2.5)))
        }).SetName($"03_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(5, 0), new Vector2(10, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 2.5), new Vector2(10, 5)),
            ExpectedCollisionNormal = new Vector2(0, -1),
            ExpectedPenetrationDepth = 2.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(0, 2.5), new Vector2(-5, 2.5), new Vector2(-5, 0)),
                new ContactPoint2D(new Vector2(10, 2.5), new Vector2(5, 2.5), new Vector2(5, 0)))
        }).SetName($"04_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        // Edges touching
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(-5, 2.5), new Vector2(10, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 2.5), new Vector2(10, 5)),
            ExpectedCollisionNormal = new Vector2(-1, 0),
            ExpectedPenetrationDepth = 0,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(0, 5), new Vector2(5, 2.5), new Vector2(-5, 2.5)),
                new ContactPoint2D(new Vector2(0, 0), new Vector2(5, -2.5), new Vector2(-5, -2.5)))
        }).SetName($"05_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(5, 7.5), new Vector2(10, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 2.5), new Vector2(10, 5)),
            ExpectedCollisionNormal = new Vector2(0, 1),
            ExpectedPenetrationDepth = 0,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(0, 5), new Vector2(-5, -2.5), new Vector2(-5, 2.5)),
                new ContactPoint2D(new Vector2(10, 5), new Vector2(5, -2.5), new Vector2(5, 2.5)))
        }).SetName($"06_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(15, 2.5), new Vector2(10, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 2.5), new Vector2(10, 5)),
            ExpectedCollisionNormal = new Vector2(1, 0),
            ExpectedPenetrationDepth = 0,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(10, 5), new Vector2(-5, 2.5), new Vector2(5, 2.5)),
                new ContactPoint2D(new Vector2(10, 0), new Vector2(-5, -2.5), new Vector2(5, -2.5)))
        }).SetName($"07_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(5, -2.5), new Vector2(10, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 2.5), new Vector2(10, 5)),
            ExpectedCollisionNormal = new Vector2(0, -1),
            ExpectedPenetrationDepth = 0,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(0, 0), new Vector2(-5, 2.5), new Vector2(-5, -2.5)),
                new ContactPoint2D(new Vector2(10, 0), new Vector2(5, 2.5), new Vector2(5, -2.5)))
        }).SetName($"08_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        // Single vertex overlapping (kinematic into static)
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(-4, 1), new Vector2(10, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 2.5), new Vector2(10, 5)),
            Rotation1 = Angle.Deg2Rad(45),
            ExpectedCollisionNormal = new Vector2(-1, 0),
            ExpectedPenetrationDepth = 1.303300,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(1.303300, 2.767766), new Vector2(5, -2.5), new Vector2(-3.696699, 0.267766)))
        }).SetName($"09_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(5, 9), new Vector2(10, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 2.5), new Vector2(10, 5)),
            Rotation1 = Angle.Deg2Rad(45),
            ExpectedCollisionNormal = new Vector2(0, 1),
            ExpectedPenetrationDepth = 1.303300,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(3.232233, 3.696699), new Vector2(-5, -2.5), new Vector2(-1.767766, 1.196699)))
        }).SetName($"10_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(14, 4), new Vector2(10, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 2.5), new Vector2(10, 5)),
            Rotation1 = Angle.Deg2Rad(45),
            ExpectedCollisionNormal = new Vector2(1, 0),
            ExpectedPenetrationDepth = 1.303300,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(8.696699, 2.232233), new Vector2(-5, 2.5), new Vector2(3.696699, -0.267766)))
        }).SetName($"11_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(5, -4), new Vector2(10, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 2.5), new Vector2(10, 5)),
            Rotation1 = Angle.Deg2Rad(45),
            ExpectedCollisionNormal = new Vector2(0, -1),
            ExpectedPenetrationDepth = 1.303300,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(6.767766, 1.303300), new Vector2(5, 2.5), new Vector2(1.767766, -1.196699)))
        }).SetName($"12_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        // Single vertex overlapping (static into kinematic)
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(0, 5), new Vector2(10, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 2.5), new Vector2(10, 5)),
            Rotation1 = Angle.Deg2Rad(45),
            ExpectedCollisionNormal = new Vector2(-0.707106, 0.707106),
            ExpectedPenetrationDepth = 2.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(0, 5), new Vector2(0, 0), new Vector2(-5, 2.5)))
        }).SetName($"13_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(10, 5), new Vector2(10, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 2.5), new Vector2(10, 5)),
            Rotation1 = Angle.Deg2Rad(-45),
            ExpectedCollisionNormal = new Vector2(0.707106, 0.707106),
            ExpectedPenetrationDepth = 2.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(10, 5), new Vector2(0, 0), new Vector2(5, 2.5)))
        }).SetName($"14_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(10, 0), new Vector2(10, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 2.5), new Vector2(10, 5)),
            Rotation1 = Angle.Deg2Rad(45),
            ExpectedCollisionNormal = new Vector2(0.707106, -0.707106),
            ExpectedPenetrationDepth = 2.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(10, 0), new Vector2(0, 0), new Vector2(5, -2.5)))
        }).SetName($"15_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(0, 0), new Vector2(10, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 2.5), new Vector2(10, 5)),
            Rotation1 = Angle.Deg2Rad(-45),
            ExpectedCollisionNormal = new Vector2(-0.707106, -0.707106),
            ExpectedPenetrationDepth = 2.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(0, 0), new Vector2(0, 0), new Vector2(-5, -2.5)))
        }).SetName($"16_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        // Two contact points without clipping
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(-2, 5), new Vector2(5, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 5), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(-1, 0),
            ExpectedPenetrationDepth = 0.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(0.5, 7.5), new Vector2(2.5, 2.5), new Vector2(-4.5, 2.5)),
                new ContactPoint2D(new Vector2(0.5, 2.5), new Vector2(2.5, -2.5), new Vector2(-4.5, -2.5)))
        }).SetName($"17_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(5, 12), new Vector2(5, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 5), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(0, 1),
            ExpectedPenetrationDepth = 0.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(2.5, 9.5), new Vector2(-2.5, -2.5), new Vector2(-2.5, 4.5)),
                new ContactPoint2D(new Vector2(7.5, 9.5), new Vector2(2.5, -2.5), new Vector2(2.5, 4.5)))
        }).SetName($"18_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(12, 5), new Vector2(5, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 5), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(1, 0),
            ExpectedPenetrationDepth = 0.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(9.5, 7.5), new Vector2(-2.5, 2.5), new Vector2(4.5, 2.5)),
                new ContactPoint2D(new Vector2(9.5, 2.5), new Vector2(-2.5, -2.5), new Vector2(4.5, -2.5)))
        }).SetName($"19_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(5, -2), new Vector2(5, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 5), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(0, -1),
            ExpectedPenetrationDepth = 0.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(2.5, 0.5), new Vector2(-2.5, 2.5), new Vector2(-2.5, -4.5)),
                new ContactPoint2D(new Vector2(7.5, 0.5), new Vector2(2.5, 2.5), new Vector2(2.5, -4.5)))
        }).SetName($"20_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        // Two contact points and one from clipping
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(-2, 1), new Vector2(5, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 5), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(-1, 0),
            ExpectedPenetrationDepth = 0.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(0.5, 3.5), new Vector2(2.5, 2.5), new Vector2(-4.5, -1.5)),
                new ContactPoint2D(new Vector2(0.5, 0), new Vector2(2.5, -1), new Vector2(-4.5, -5)))
        }).SetName($"21_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(-2, 9), new Vector2(5, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 5), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(-1, 0),
            ExpectedPenetrationDepth = 0.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(0.5, 10), new Vector2(2.5, 1), new Vector2(-4.5, 5)),
                new ContactPoint2D(new Vector2(0.5, 6.5), new Vector2(2.5, -2.5), new Vector2(-4.5, 1.5)))
        }).SetName($"22_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(1, 12), new Vector2(5, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 5), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(0, 1),
            ExpectedPenetrationDepth = 0.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(0, 9.5), new Vector2(-1, -2.5), new Vector2(-5, 4.5)),
                new ContactPoint2D(new Vector2(3.5, 9.5), new Vector2(2.5, -2.5), new Vector2(-1.5, 4.5)))
        }).SetName($"23_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(9, 12), new Vector2(5, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 5), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(0, 1),
            ExpectedPenetrationDepth = 0.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(6.5, 9.5), new Vector2(-2.5, -2.5), new Vector2(1.5, 4.5)),
                new ContactPoint2D(new Vector2(10, 9.5), new Vector2(1, -2.5), new Vector2(5, 4.5)))
        }).SetName($"24_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(12, 9), new Vector2(5, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 5), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(1, 0),
            ExpectedPenetrationDepth = 0.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(9.5, 10), new Vector2(-2.5, 1), new Vector2(4.5, 5)),
                new ContactPoint2D(new Vector2(9.5, 6.5), new Vector2(-2.5, -2.5), new Vector2(4.5, 1.5)))
        }).SetName($"25_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(12, 1), new Vector2(5, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 5), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(1, 0),
            ExpectedPenetrationDepth = 0.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(9.5, 3.5), new Vector2(-2.5, 2.5), new Vector2(4.5, -1.5)),
                new ContactPoint2D(new Vector2(9.5, 0), new Vector2(-2.5, -1), new Vector2(4.5, -5)))
        }).SetName($"26_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(9, -2), new Vector2(5, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 5), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(0, -1),
            ExpectedPenetrationDepth = 0.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(6.5, 0.5), new Vector2(-2.5, 2.5), new Vector2(1.5, -4.5)),
                new ContactPoint2D(new Vector2(10, 0.5), new Vector2(1, 2.5), new Vector2(5, -4.5)))
        }).SetName($"27_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(1, -2), new Vector2(5, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 5), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(0, -1),
            ExpectedPenetrationDepth = 0.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(0, 0.5), new Vector2(-1, 2.5), new Vector2(-5, -4.5)),
                new ContactPoint2D(new Vector2(3.5, 0.5), new Vector2(2.5, 2.5), new Vector2(-1.5, -4.5)))
        }).SetName($"28_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        // Two contact points both from clipping
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(-9, 5), new Vector2(20, 20)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 5), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(-1, 0),
            ExpectedPenetrationDepth = 1,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(1, 10), new Vector2(10, 5), new Vector2(-4, 5)),
                new ContactPoint2D(new Vector2(1, 0), new Vector2(10, -5), new Vector2(-4, -5)))
        }).SetName($"29_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(5, 19), new Vector2(20, 20)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 5), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(0, 1),
            ExpectedPenetrationDepth = 1,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(0, 9), new Vector2(-5, -10), new Vector2(-5, 4)),
                new ContactPoint2D(new Vector2(10, 9), new Vector2(5, -10), new Vector2(5, 4)))
        }).SetName($"30_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(19, 5), new Vector2(20, 20)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 5), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(1, 0),
            ExpectedPenetrationDepth = 1,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(9, 10), new Vector2(-10, 5), new Vector2(4, 5)),
                new ContactPoint2D(new Vector2(9, 0), new Vector2(-10, -5), new Vector2(4, -5)))
        }).SetName($"31_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(5, -9), new Vector2(20, 20)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 5), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(0, -1),
            ExpectedPenetrationDepth = 1,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(0, 1), new Vector2(-5, 10), new Vector2(-5, -4)),
                new ContactPoint2D(new Vector2(10, 1), new Vector2(5, 10), new Vector2(5, -4)))
        }).SetName($"32_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        // Both rectangles rotated
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(5, -5), new Vector2(10, 10)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 5), new Vector2(10, 10)),
            Rotation1 = Angle.Deg2Rad(-30),
            Rotation2 = Angle.Deg2Rad(30),
            ExpectedCollisionNormal = new Vector2(0.499999, -0.866025),
            ExpectedPenetrationDepth = 3.169872,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(3.169872, 1.830127), new Vector2(-5, 5), new Vector2(-3.169873, -1.830127)),
                new ContactPoint2D(new Vector2(2.113248, 0), new Vector2(-5, 2.886751), new Vector2(-5, -2.886751)))
        }).SetName($"33_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        // Vertices touching
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(-5, 15), new Vector2(10, 10)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 5), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(-1, 0),
            ExpectedPenetrationDepth = 0,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(0, 10), new Vector2(5, -5), new Vector2(-5, 5)))
        }).SetName($"34_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(15, 15), new Vector2(10, 10)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 5), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(1, 0),
            ExpectedPenetrationDepth = 0,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(10, 10), new Vector2(-5, -5), new Vector2(5, 5)))
        }).SetName($"35_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(15, -5), new Vector2(10, 10)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 5), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(0, -1),
            ExpectedPenetrationDepth = 0,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(10, 0), new Vector2(-5, 5), new Vector2(5, -5)))
        }).SetName($"36_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(-5, -5), new Vector2(10, 10)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 5), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(-1, 0),
            ExpectedPenetrationDepth = 0,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(0, 0), new Vector2(5, 5), new Vector2(-5, -5)))
        }).SetName($"37_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        // Reproduction of bug: https://github.com/dawidkomorowski/geisha/issues/573
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(-408.70623806035013, 112.21406847569999), new Vector2(100, 100)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(-500, 200), new Vector2(100, 100)),
            Rotation1 = -0.27488825763167818,
            ExpectedCollisionNormal = new Vector2(0.9624555349174285, -0.2714393915901053),
            ExpectedPenetrationDepth = 0,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(-450, 150), new Vector2(-50.000000000000036, 25.158525320208298), new Vector2(50, -50)))
        }).SetName($"38_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}")
    };

    [TestCaseSource(nameof(RectangleAndRectangleTestCases))]
    public void RectangleKinematicBody_And_RectangleStaticBody(RectangleAndRectangleTestCase testCase)
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
        var kinematicBodyContacts = kinematicBodyCollider.GetContacts();
        Assert.That(kinematicBodyContacts, Has.Length.EqualTo(1));
        Assert.That(kinematicBodyContacts[0].ThisCollider, Is.EqualTo(kinematicBodyCollider));
        Assert.That(kinematicBodyContacts[0].OtherCollider, Is.EqualTo(staticBodyCollider));
        Assert.That(kinematicBodyContacts[0].CollisionNormal, Is.EqualTo(testCase.ExpectedCollisionNormal).Using(Vector2Comparer));
        Assert.That(kinematicBodyContacts[0].PenetrationDepth, Is.EqualTo(testCase.ExpectedPenetrationDepth));
        Assert.That(kinematicBodyContacts[0].ContactPoints.ToArray(),
            Is.EquivalentTo(testCase.ExpectedContactPoints.ToArray()).Using<ContactPoint2D, ContactPoint2D>(ContactPoint2DComparison));

        Assert.That(staticBodyCollider.IsColliding, Is.True);
        var staticBodyContacts = staticBodyCollider.GetContacts();
        Assert.That(staticBodyContacts, Has.Length.EqualTo(1));
        Assert.That(staticBodyContacts[0].ThisCollider, Is.EqualTo(staticBodyCollider));
        Assert.That(staticBodyContacts[0].OtherCollider, Is.EqualTo(kinematicBodyCollider));
        Assert.That(staticBodyContacts[0].CollisionNormal, Is.EqualTo(-testCase.ExpectedCollisionNormal).Using(Vector2Comparer));
        Assert.That(staticBodyContacts[0].PenetrationDepth, Is.EqualTo(testCase.ExpectedPenetrationDepth));

        var reflectedExpectedContactPoints = testCase.ExpectedContactPoints.ToArray()
            .Select(cp => new ContactPoint2D(cp.WorldPosition, cp.OtherLocalPosition, cp.ThisLocalPosition)).ToArray();
        Assert.That(staticBodyContacts[0].ContactPoints.ToArray(),
            Is.EquivalentTo(reflectedExpectedContactPoints).Using<ContactPoint2D, ContactPoint2D>(ContactPoint2DComparison));
    }

    #endregion

    #region Circle and Circle

    public sealed class CircleAndCircleTestCase
    {
        public Circle Circle1 { get; init; }
        public Circle Circle2 { get; init; }

        public double Rotation1 { get; init; }
        public double Rotation2 { get; init; }

        public Vector2 ExpectedCollisionNormal { get; init; }
        public double ExpectedPenetrationDepth { get; init; }
        public ContactPoint2D ExpectedContactPoint { get; init; }
    }

    public static IEnumerable<TestCaseData> CircleAndCircleTestCases => new[]
    {
        // Axis aligned overlap
        new TestCaseData(new CircleAndCircleTestCase
        {
            Circle1 = new Circle(new Vector2(-4, 5), 5),
            Circle2 = new Circle(new Vector2(5, 5), 5),
            ExpectedCollisionNormal = new Vector2(-1, 0),
            ExpectedPenetrationDepth = 1,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(0.5, 5), new Vector2(4.5, 0), new Vector2(-4.5, 0))
        }).SetName($"01_{nameof(CircleKinematicBody_And_CircleStaticBody)}"),
        new TestCaseData(new CircleAndCircleTestCase
        {
            Circle1 = new Circle(new Vector2(5, 14), 5),
            Circle2 = new Circle(new Vector2(5, 5), 5),
            ExpectedCollisionNormal = new Vector2(0, 1),
            ExpectedPenetrationDepth = 1,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(5, 9.5), new Vector2(0, -4.5), new Vector2(0, 4.5))
        }).SetName($"02_{nameof(CircleKinematicBody_And_CircleStaticBody)}"),
        new TestCaseData(new CircleAndCircleTestCase
        {
            Circle1 = new Circle(new Vector2(14, 5), 5),
            Circle2 = new Circle(new Vector2(5, 5), 5),
            ExpectedCollisionNormal = new Vector2(1, 0),
            ExpectedPenetrationDepth = 1,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(9.5, 5), new Vector2(-4.5, 0), new Vector2(4.5, 0))
        }).SetName($"03_{nameof(CircleKinematicBody_And_CircleStaticBody)}"),
        new TestCaseData(new CircleAndCircleTestCase
        {
            Circle1 = new Circle(new Vector2(5, -4), 5),
            Circle2 = new Circle(new Vector2(5, 5), 5),
            ExpectedCollisionNormal = new Vector2(0, -1),
            ExpectedPenetrationDepth = 1,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(5, 0.5), new Vector2(0, 4.5), new Vector2(0, -4.5))
        }).SetName($"04_{nameof(CircleKinematicBody_And_CircleStaticBody)}"),
        // Diagonal overlap
        new TestCaseData(new CircleAndCircleTestCase
        {
            Circle1 = new Circle(new Vector2(0, 10), 5),
            Circle2 = new Circle(new Vector2(5, 5), 5),
            ExpectedCollisionNormal = new Vector2(-0.707106, 0.707106),
            ExpectedPenetrationDepth = 2.928932,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(2.5, 7.5), new Vector2(2.5, -2.5), new Vector2(-2.5, 2.5))
        }).SetName($"05_{nameof(CircleKinematicBody_And_CircleStaticBody)}"),
        new TestCaseData(new CircleAndCircleTestCase
        {
            Circle1 = new Circle(new Vector2(10, 10), 5),
            Circle2 = new Circle(new Vector2(5, 5), 5),
            ExpectedCollisionNormal = new Vector2(0.707106, 0.707106),
            ExpectedPenetrationDepth = 2.928932,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(7.5, 7.5), new Vector2(-2.5, -2.5), new Vector2(2.5, 2.5))
        }).SetName($"06_{nameof(CircleKinematicBody_And_CircleStaticBody)}"),
        new TestCaseData(new CircleAndCircleTestCase
        {
            Circle1 = new Circle(new Vector2(10, 0), 5),
            Circle2 = new Circle(new Vector2(5, 5), 5),
            ExpectedCollisionNormal = new Vector2(0.707106, -0.707106),
            ExpectedPenetrationDepth = 2.928932,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(7.5, 2.5), new Vector2(-2.5, 2.5), new Vector2(2.5, -2.5))
        }).SetName($"07_{nameof(CircleKinematicBody_And_CircleStaticBody)}"),
        new TestCaseData(new CircleAndCircleTestCase
        {
            Circle1 = new Circle(new Vector2(0, 0), 5),
            Circle2 = new Circle(new Vector2(5, 5), 5),
            ExpectedCollisionNormal = new Vector2(-0.707106, -0.707106),
            ExpectedPenetrationDepth = 2.928932,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(2.5, 2.5), new Vector2(2.5, 2.5), new Vector2(-2.5, -2.5))
        }).SetName($"08_{nameof(CircleKinematicBody_And_CircleStaticBody)}"),
        // Touching
        new TestCaseData(new CircleAndCircleTestCase
        {
            Circle1 = new Circle(new Vector2(-5, 5), 5),
            Circle2 = new Circle(new Vector2(5, 5), 5),
            ExpectedCollisionNormal = new Vector2(-1, 0),
            ExpectedPenetrationDepth = 0,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(0, 5), new Vector2(5, 0), new Vector2(-5, 0))
        }).SetName($"09_{nameof(CircleKinematicBody_And_CircleStaticBody)}"),
        new TestCaseData(new CircleAndCircleTestCase
        {
            Circle1 = new Circle(new Vector2(5, 15), 5),
            Circle2 = new Circle(new Vector2(5, 5), 5),
            ExpectedCollisionNormal = new Vector2(0, 1),
            ExpectedPenetrationDepth = 0,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(5, 10), new Vector2(0, -5), new Vector2(0, 5))
        }).SetName($"10_{nameof(CircleKinematicBody_And_CircleStaticBody)}"),
        new TestCaseData(new CircleAndCircleTestCase
        {
            Circle1 = new Circle(new Vector2(15, 5), 5),
            Circle2 = new Circle(new Vector2(5, 5), 5),
            ExpectedCollisionNormal = new Vector2(1, 0),
            ExpectedPenetrationDepth = 0,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(10, 5), new Vector2(-5, 0), new Vector2(5, 0))
        }).SetName($"11_{nameof(CircleKinematicBody_And_CircleStaticBody)}"),
        new TestCaseData(new CircleAndCircleTestCase
        {
            Circle1 = new Circle(new Vector2(5, -5), 5),
            Circle2 = new Circle(new Vector2(5, 5), 5),
            ExpectedCollisionNormal = new Vector2(0, -1),
            ExpectedPenetrationDepth = 0,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(5, 0), new Vector2(0, 5), new Vector2(0, -5))
        }).SetName($"12_{nameof(CircleKinematicBody_And_CircleStaticBody)}"),
        // Different sizes
        new TestCaseData(new CircleAndCircleTestCase
        {
            Circle1 = new Circle(new Vector2(-5, 5), 10),
            Circle2 = new Circle(new Vector2(5, 5), 5),
            ExpectedCollisionNormal = new Vector2(-1, 0),
            ExpectedPenetrationDepth = 5,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(2.5, 5), new Vector2(7.5, 0), new Vector2(-2.5, 0))
        }).SetName($"13_{nameof(CircleKinematicBody_And_CircleStaticBody)}"),
        new TestCaseData(new CircleAndCircleTestCase
        {
            Circle1 = new Circle(new Vector2(5, 15), 10),
            Circle2 = new Circle(new Vector2(5, 5), 5),
            ExpectedCollisionNormal = new Vector2(0, 1),
            ExpectedPenetrationDepth = 5,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(5, 7.5), new Vector2(0, -7.5), new Vector2(0, 2.5))
        }).SetName($"14_{nameof(CircleKinematicBody_And_CircleStaticBody)}"),
        new TestCaseData(new CircleAndCircleTestCase
        {
            Circle1 = new Circle(new Vector2(15, 5), 10),
            Circle2 = new Circle(new Vector2(5, 5), 5),
            ExpectedCollisionNormal = new Vector2(1, 0),
            ExpectedPenetrationDepth = 5,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(7.5, 5), new Vector2(-7.5, 0), new Vector2(2.5, 0))
        }).SetName($"15_{nameof(CircleKinematicBody_And_CircleStaticBody)}"),
        new TestCaseData(new CircleAndCircleTestCase
        {
            Circle1 = new Circle(new Vector2(5, -5), 10),
            Circle2 = new Circle(new Vector2(5, 5), 5),
            ExpectedCollisionNormal = new Vector2(0, -1),
            ExpectedPenetrationDepth = 5,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(5, 2.5), new Vector2(0, 7.5), new Vector2(0, -2.5))
        }).SetName($"16_{nameof(CircleKinematicBody_And_CircleStaticBody)}"),
        // Both circles rotated
        new TestCaseData(new CircleAndCircleTestCase
        {
            Circle1 = new Circle(new Vector2(0, 10), 5),
            Circle2 = new Circle(new Vector2(5, 5), 5),
            Rotation1 = Angle.Deg2Rad(45),
            Rotation2 = Angle.Deg2Rad(-45),
            ExpectedCollisionNormal = new Vector2(-0.707106, 0.707106),
            ExpectedPenetrationDepth = 2.928932,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(2.5, 7.5), new Vector2(0, -3.535533), new Vector2(-3.535533, 0))
        }).SetName($"17_{nameof(CircleKinematicBody_And_CircleStaticBody)}")
    };

    [TestCaseSource(nameof(CircleAndCircleTestCases))]
    public void CircleKinematicBody_And_CircleStaticBody(CircleAndCircleTestCase testCase)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var kinematicBody = CreateCircleKinematicBody(testCase.Circle1, testCase.Rotation1);
        var staticBody = CreateCircleStaticBody(testCase.Circle2, testCase.Rotation2);

        SaveVisualOutput(physicsSystem, 0, 10);

        // Assume
        Assert.That(kinematicBody.GetComponent<CircleColliderComponent>().IsColliding, Is.False);
        Assert.That(staticBody.GetComponent<CircleColliderComponent>().IsColliding, Is.False);

        // Act
        physicsSystem.ProcessPhysics();

        SaveVisualOutput(physicsSystem, 1, 10);

        // Assert
        var kinematicBodyCollider = kinematicBody.GetComponent<CircleColliderComponent>();
        var staticBodyCollider = staticBody.GetComponent<CircleColliderComponent>();

        Assert.That(kinematicBodyCollider.IsColliding, Is.True);
        var kinematicBodyContacts = kinematicBodyCollider.GetContacts();
        Assert.That(kinematicBodyContacts, Has.Length.EqualTo(1));
        Assert.That(kinematicBodyContacts[0].ThisCollider, Is.EqualTo(kinematicBodyCollider));
        Assert.That(kinematicBodyContacts[0].OtherCollider, Is.EqualTo(staticBodyCollider));
        Assert.That(kinematicBodyContacts[0].CollisionNormal, Is.EqualTo(testCase.ExpectedCollisionNormal).Using(Vector2Comparer));
        Assert.That(kinematicBodyContacts[0].PenetrationDepth, Is.EqualTo(testCase.ExpectedPenetrationDepth));
        Assert.That(kinematicBodyContacts[0].ContactPoints.Count, Is.EqualTo(1));
        Assert.That(kinematicBodyContacts[0].ContactPoints[0],
            Is.EqualTo(testCase.ExpectedContactPoint).Using<ContactPoint2D, ContactPoint2D>(ContactPoint2DComparison));

        Assert.That(staticBodyCollider.IsColliding, Is.True);
        var staticBodyContacts = staticBodyCollider.GetContacts();
        Assert.That(staticBodyContacts, Has.Length.EqualTo(1));
        Assert.That(staticBodyContacts[0].ThisCollider, Is.EqualTo(staticBodyCollider));
        Assert.That(staticBodyContacts[0].OtherCollider, Is.EqualTo(kinematicBodyCollider));
        Assert.That(staticBodyContacts[0].CollisionNormal, Is.EqualTo(-testCase.ExpectedCollisionNormal).Using(Vector2Comparer));
        Assert.That(staticBodyContacts[0].PenetrationDepth, Is.EqualTo(testCase.ExpectedPenetrationDepth));

        var cp = testCase.ExpectedContactPoint;
        var reflectedContactPoint = new ContactPoint2D(cp.WorldPosition, cp.OtherLocalPosition, cp.ThisLocalPosition);
        Assert.That(staticBodyContacts[0].ContactPoints.Count, Is.EqualTo(1));
        Assert.That(staticBodyContacts[0].ContactPoints[0],
            Is.EqualTo(reflectedContactPoint).Using<ContactPoint2D, ContactPoint2D>(ContactPoint2DComparison));
    }

    #endregion

    #region Rectangle and Circle

    public sealed class RectangleAndCircleTestCase
    {
        public AxisAlignedRectangle Rectangle { get; init; }
        public Circle Circle { get; init; }

        public double RectangleRotation { get; init; }
        public double CircleRotation { get; init; }

        public Vector2 ExpectedCollisionNormal { get; init; }
        public double ExpectedPenetrationDepth { get; init; }
        public ContactPoint2D ExpectedContactPoint { get; init; }
    }

    // When updating these test cases, consider updating test cases for Circle and Tile collision.
    public static IEnumerable<TestCaseData> RectangleAndCircleTestCases => new[]
    {
        // Axis aligned edge overlap
        new TestCaseData(new RectangleAndCircleTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(-4, 5), new Vector2(10, 10)),
            Circle = new Circle(new Vector2(5, 5), 5),
            ExpectedCollisionNormal = new Vector2(-1, 0),
            ExpectedPenetrationDepth = 1,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(0.5, 5), new Vector2(4.5, 0), new Vector2(-4.5, 0))
        }).SetName($"01_{nameof(RectangleKinematicBody_And_CircleStaticBody)}"),
        new TestCaseData(new RectangleAndCircleTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(5, 14), new Vector2(10, 10)),
            Circle = new Circle(new Vector2(5, 5), 5),
            ExpectedCollisionNormal = new Vector2(0, 1),
            ExpectedPenetrationDepth = 1,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(5, 9.5), new Vector2(0, -4.5), new Vector2(0, 4.5))
        }).SetName($"02_{nameof(RectangleKinematicBody_And_CircleStaticBody)}"),
        new TestCaseData(new RectangleAndCircleTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(14, 5), new Vector2(10, 10)),
            Circle = new Circle(new Vector2(5, 5), 5),
            ExpectedCollisionNormal = new Vector2(1, 0),
            ExpectedPenetrationDepth = 1,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(9.5, 5), new Vector2(-4.5, 0), new Vector2(4.5, 0))
        }).SetName($"03_{nameof(RectangleKinematicBody_And_CircleStaticBody)}"),
        new TestCaseData(new RectangleAndCircleTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(5, -4), new Vector2(10, 10)),
            Circle = new Circle(new Vector2(5, 5), 5),
            ExpectedCollisionNormal = new Vector2(0, -1),
            ExpectedPenetrationDepth = 1,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(5, 0.5), new Vector2(0, 4.5), new Vector2(0, -4.5))
        }).SetName($"04_{nameof(RectangleKinematicBody_And_CircleStaticBody)}"),
        // Touching
        new TestCaseData(new RectangleAndCircleTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(-5, 5), new Vector2(10, 10)),
            Circle = new Circle(new Vector2(5, 5), 5),
            ExpectedCollisionNormal = new Vector2(-1, 0),
            ExpectedPenetrationDepth = 0,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(0, 5), new Vector2(5, 0), new Vector2(-5, 0))
        }).SetName($"05_{nameof(RectangleKinematicBody_And_CircleStaticBody)}"),
        new TestCaseData(new RectangleAndCircleTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(5, 15), new Vector2(10, 10)),
            Circle = new Circle(new Vector2(5, 5), 5),
            ExpectedCollisionNormal = new Vector2(0, 1),
            ExpectedPenetrationDepth = 0,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(5, 10), new Vector2(0, -5), new Vector2(0, 5))
        }).SetName($"06_{nameof(RectangleKinematicBody_And_CircleStaticBody)}"),
        new TestCaseData(new RectangleAndCircleTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(15, 5), new Vector2(10, 10)),
            Circle = new Circle(new Vector2(5, 5), 5),
            ExpectedCollisionNormal = new Vector2(1, 0),
            ExpectedPenetrationDepth = 0,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(10, 5), new Vector2(-5, 0), new Vector2(5, 0))
        }).SetName($"07_{nameof(RectangleKinematicBody_And_CircleStaticBody)}"),
        new TestCaseData(new RectangleAndCircleTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(5, -5), new Vector2(10, 10)),
            Circle = new Circle(new Vector2(5, 5), 5),
            ExpectedCollisionNormal = new Vector2(0, -1),
            ExpectedPenetrationDepth = 0,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(5, 0), new Vector2(0, 5), new Vector2(0, -5))
        }).SetName($"08_{nameof(RectangleKinematicBody_And_CircleStaticBody)}"),
        // Diagonal edge overlap
        new TestCaseData(new RectangleAndCircleTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(0, 10), new Vector2(10, 10)),
            Circle = new Circle(new Vector2(5, 5), 5),
            RectangleRotation = Angle.Deg2Rad(-45),
            ExpectedCollisionNormal = new Vector2(-0.707106, 0.707106),
            ExpectedPenetrationDepth = 2.928932,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(2.5, 7.5), new Vector2(3.535533, 0), new Vector2(-2.5, 2.5))
        }).SetName($"09_{nameof(RectangleKinematicBody_And_CircleStaticBody)}"),
        new TestCaseData(new RectangleAndCircleTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(10, 10), new Vector2(10, 10)),
            Circle = new Circle(new Vector2(5, 5), 5),
            RectangleRotation = Angle.Deg2Rad(-45),
            ExpectedCollisionNormal = new Vector2(0.707106, 0.707106),
            ExpectedPenetrationDepth = 2.928932,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(7.5, 7.5), new Vector2(0, -3.535533), new Vector2(2.5, 2.5))
        }).SetName($"10_{nameof(RectangleKinematicBody_And_CircleStaticBody)}"),
        new TestCaseData(new RectangleAndCircleTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(10, 0), new Vector2(10, 10)),
            Circle = new Circle(new Vector2(5, 5), 5),
            RectangleRotation = Angle.Deg2Rad(-45),
            ExpectedCollisionNormal = new Vector2(0.707106, -0.707106),
            ExpectedPenetrationDepth = 2.928932,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(7.5, 2.5), new Vector2(-3.535533, 0), new Vector2(2.5, -2.5))
        }).SetName($"11_{nameof(RectangleKinematicBody_And_CircleStaticBody)}"),
        new TestCaseData(new RectangleAndCircleTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(0, 0), new Vector2(10, 10)),
            Circle = new Circle(new Vector2(5, 5), 5),
            RectangleRotation = Angle.Deg2Rad(-45),
            ExpectedCollisionNormal = new Vector2(-0.707106, -0.707106),
            ExpectedPenetrationDepth = 2.928932,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(2.5, 2.5), new Vector2(0, 3.535533), new Vector2(-2.5, -2.5))
        }).SetName($"12_{nameof(RectangleKinematicBody_And_CircleStaticBody)}"),
        // Axis aligned vertex overlap
        new TestCaseData(new RectangleAndCircleTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(-5, 5), new Vector2(10, 10)),
            Circle = new Circle(new Vector2(5, 5), 5),
            RectangleRotation = Angle.Deg2Rad(-45),
            ExpectedCollisionNormal = new Vector2(-1, 0),
            ExpectedPenetrationDepth = 2.071067,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(1.035533, 5), new Vector2(4.267766, 4.267766), new Vector2(-3.964466, 0))
        }).SetName($"13_{nameof(RectangleKinematicBody_And_CircleStaticBody)}"),
        new TestCaseData(new RectangleAndCircleTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(5, 15), new Vector2(10, 10)),
            Circle = new Circle(new Vector2(5, 5), 5),
            RectangleRotation = Angle.Deg2Rad(-45),
            ExpectedCollisionNormal = new Vector2(0, 1),
            ExpectedPenetrationDepth = 2.071067,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(5, 8.964466), new Vector2(4.267766, -4.267766), new Vector2(0, 3.964466))
        }).SetName($"14_{nameof(RectangleKinematicBody_And_CircleStaticBody)}"),
        new TestCaseData(new RectangleAndCircleTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(15, 5), new Vector2(10, 10)),
            Circle = new Circle(new Vector2(5, 5), 5),
            RectangleRotation = Angle.Deg2Rad(-45),
            ExpectedCollisionNormal = new Vector2(1, 0),
            ExpectedPenetrationDepth = 2.071067,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(8.964466, 5), new Vector2(-4.267766, -4.267766), new Vector2(3.964466, 0))
        }).SetName($"15_{nameof(RectangleKinematicBody_And_CircleStaticBody)}"),
        new TestCaseData(new RectangleAndCircleTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(5, -5), new Vector2(10, 10)),
            Circle = new Circle(new Vector2(5, 5), 5),
            RectangleRotation = Angle.Deg2Rad(-45),
            ExpectedCollisionNormal = new Vector2(0, -1),
            ExpectedPenetrationDepth = 2.071067,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(5, 1.035533), new Vector2(-4.267766, 4.267766), new Vector2(0, -3.964466))
        }).SetName($"16_{nameof(RectangleKinematicBody_And_CircleStaticBody)}"),
        // Diagonal vertex overlap
        new TestCaseData(new RectangleAndCircleTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(-3, 13), new Vector2(10, 10)),
            Circle = new Circle(new Vector2(5, 5), 5),
            ExpectedCollisionNormal = new Vector2(-0.707106, 0.707106),
            ExpectedPenetrationDepth = 0.757359,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(1.732233, 8.267766), new Vector2(4.732233, -4.732233), new Vector2(-3.267766, 3.267766))
        }).SetName($"17_{nameof(RectangleKinematicBody_And_CircleStaticBody)}"),
        new TestCaseData(new RectangleAndCircleTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(13, 13), new Vector2(10, 10)),
            Circle = new Circle(new Vector2(5, 5), 5),
            ExpectedCollisionNormal = new Vector2(0.707106, 0.707106),
            ExpectedPenetrationDepth = 0.757359,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(8.267766, 8.267766), new Vector2(-4.732233, -4.732233), new Vector2(3.267766, 3.267766))
        }).SetName($"18_{nameof(RectangleKinematicBody_And_CircleStaticBody)}"),
        new TestCaseData(new RectangleAndCircleTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(13, -3), new Vector2(10, 10)),
            Circle = new Circle(new Vector2(5, 5), 5),
            ExpectedCollisionNormal = new Vector2(0.707106, -0.707106),
            ExpectedPenetrationDepth = 0.757359,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(8.267766, 1.732233), new Vector2(-4.732233, 4.732233), new Vector2(3.267766, -3.267766))
        }).SetName($"19_{nameof(RectangleKinematicBody_And_CircleStaticBody)}"),
        new TestCaseData(new RectangleAndCircleTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(-3, -3), new Vector2(10, 10)),
            Circle = new Circle(new Vector2(5, 5), 5),
            ExpectedCollisionNormal = new Vector2(-0.707106, -0.707106),
            ExpectedPenetrationDepth = 0.757359,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(1.732233, 1.732233), new Vector2(4.732233, 4.732233), new Vector2(-3.267766, -3.267766))
        }).SetName($"20_{nameof(RectangleKinematicBody_And_CircleStaticBody)}"),
        // Different sizes
        new TestCaseData(new RectangleAndCircleTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(-2, 5), new Vector2(5, 5)),
            Circle = new Circle(new Vector2(5, 5), 5),
            ExpectedCollisionNormal = new Vector2(-1, 0),
            ExpectedPenetrationDepth = 0.5,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(0.25, 5), new Vector2(2.25, 0), new Vector2(-4.75, 0))
        }).SetName($"21_{nameof(RectangleKinematicBody_And_CircleStaticBody)}"),
        new TestCaseData(new RectangleAndCircleTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(-8, 5), new Vector2(20, 20)),
            Circle = new Circle(new Vector2(5, 5), 5),
            ExpectedCollisionNormal = new Vector2(-1, 0),
            ExpectedPenetrationDepth = 2,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(1, 5), new Vector2(9, 0), new Vector2(-4, 0))
        }).SetName($"22_{nameof(RectangleKinematicBody_And_CircleStaticBody)}"),
        // Both rotated
        new TestCaseData(new RectangleAndCircleTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(-2, 8), new Vector2(10, 10)),
            Circle = new Circle(new Vector2(5, 5), 5),
            RectangleRotation = Angle.Deg2Rad(-45),
            CircleRotation = Angle.Deg2Rad(45),
            ExpectedCollisionNormal = new Vector2(-0.707106, 0.707106),
            ExpectedPenetrationDepth = 2.928932,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(2.5, 7.5), new Vector2(3.535533, 2.828427), new Vector2(0, 3.535533))
        }).SetName($"23_{nameof(RectangleKinematicBody_And_CircleStaticBody)}"),
        new TestCaseData(new RectangleAndCircleTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(-4, 6), new Vector2(10, 10)),
            Circle = new Circle(new Vector2(5, 5), 5),
            RectangleRotation = Angle.Deg2Rad(-45),
            CircleRotation = Angle.Deg2Rad(45),
            ExpectedCollisionNormal = new Vector2(-0.887789, 0.460249),
            ExpectedPenetrationDepth = 2.827264,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(1.816059, 6.650623), new Vector2(3.652515, 4.572635), new Vector2(-1.084218, 3.418552))
        }).SetName($"24_{nameof(RectangleKinematicBody_And_CircleStaticBody)}"),
        // Circle rotated
        new TestCaseData(new RectangleAndCircleTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(-4, 5), new Vector2(10, 10)),
            Circle = new Circle(new Vector2(5, 5), 5),
            CircleRotation = Angle.Deg2Rad(45),
            ExpectedCollisionNormal = new Vector2(-1, 0),
            ExpectedPenetrationDepth = 1,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(0.5, 5), new Vector2(4.5, 0), new Vector2(-3.181981, 3.181981))
        }).SetName($"25_{nameof(RectangleKinematicBody_And_CircleStaticBody)}")
    };

    [TestCaseSource(nameof(RectangleAndCircleTestCases))]
    public void RectangleKinematicBody_And_CircleStaticBody(RectangleAndCircleTestCase testCase)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var kinematicBody = CreateRectangleKinematicBody(testCase.Rectangle, testCase.RectangleRotation);
        var staticBody = CreateCircleStaticBody(testCase.Circle, testCase.CircleRotation);

        SaveVisualOutput(physicsSystem, 0, 10);

        // Assume
        Assert.That(kinematicBody.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
        Assert.That(staticBody.GetComponent<CircleColliderComponent>().IsColliding, Is.False);

        // Act
        physicsSystem.ProcessPhysics();

        SaveVisualOutput(physicsSystem, 1, 10);

        // Assert
        var kinematicBodyCollider = kinematicBody.GetComponent<RectangleColliderComponent>();
        var staticBodyCollider = staticBody.GetComponent<CircleColliderComponent>();

        Assert.That(kinematicBodyCollider.IsColliding, Is.True);
        var kinematicBodyContacts = kinematicBodyCollider.GetContacts();
        Assert.That(kinematicBodyContacts, Has.Length.EqualTo(1));
        Assert.That(kinematicBodyContacts[0].ThisCollider, Is.EqualTo(kinematicBodyCollider));
        Assert.That(kinematicBodyContacts[0].OtherCollider, Is.EqualTo(staticBodyCollider));
        Assert.That(kinematicBodyContacts[0].CollisionNormal, Is.EqualTo(testCase.ExpectedCollisionNormal).Using(Vector2Comparer));
        Assert.That(kinematicBodyContacts[0].PenetrationDepth, Is.EqualTo(testCase.ExpectedPenetrationDepth));
        Assert.That(kinematicBodyContacts[0].ContactPoints.Count, Is.EqualTo(1));
        Assert.That(kinematicBodyContacts[0].ContactPoints[0],
            Is.EqualTo(testCase.ExpectedContactPoint).Using<ContactPoint2D, ContactPoint2D>(ContactPoint2DComparison));

        Assert.That(staticBodyCollider.IsColliding, Is.True);
        var staticBodyContacts = staticBodyCollider.GetContacts();
        Assert.That(staticBodyContacts, Has.Length.EqualTo(1));
        Assert.That(staticBodyContacts[0].ThisCollider, Is.EqualTo(staticBodyCollider));
        Assert.That(staticBodyContacts[0].OtherCollider, Is.EqualTo(kinematicBodyCollider));
        Assert.That(staticBodyContacts[0].CollisionNormal, Is.EqualTo(-testCase.ExpectedCollisionNormal).Using(Vector2Comparer));
        Assert.That(staticBodyContacts[0].PenetrationDepth, Is.EqualTo(testCase.ExpectedPenetrationDepth));

        var cp = testCase.ExpectedContactPoint;
        var reflectedContactPoint = new ContactPoint2D(cp.WorldPosition, cp.OtherLocalPosition, cp.ThisLocalPosition);
        Assert.That(staticBodyContacts[0].ContactPoints.Count, Is.EqualTo(1));
        Assert.That(staticBodyContacts[0].ContactPoints[0],
            Is.EqualTo(reflectedContactPoint).Using<ContactPoint2D, ContactPoint2D>(ContactPoint2DComparison));
    }

    public static IEnumerable<TestCaseData> CircleAndRectangleTestCases =>
        RectangleAndCircleTestCases.Select(tc =>
            tc.SetName(tc.TestName?.Replace(nameof(RectangleKinematicBody_And_CircleStaticBody), nameof(CircleKinematicBody_And_RectangleStaticBody))));

    [TestCaseSource(nameof(CircleAndRectangleTestCases))]
    public void CircleKinematicBody_And_RectangleStaticBody(RectangleAndCircleTestCase testCase)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var kinematicBody = CreateCircleKinematicBody(testCase.Circle, testCase.CircleRotation);
        var staticBody = CreateRectangleStaticBody(testCase.Rectangle, testCase.RectangleRotation);

        SaveVisualOutput(physicsSystem, 0, 10);

        // Assume
        Assert.That(kinematicBody.GetComponent<CircleColliderComponent>().IsColliding, Is.False);
        Assert.That(staticBody.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);

        // Act
        physicsSystem.ProcessPhysics();

        SaveVisualOutput(physicsSystem, 1, 10);

        // Assert
        var kinematicBodyCollider = kinematicBody.GetComponent<CircleColliderComponent>();
        var staticBodyCollider = staticBody.GetComponent<RectangleColliderComponent>();

        Assert.That(kinematicBodyCollider.IsColliding, Is.True);
        var kinematicBodyContacts = kinematicBodyCollider.GetContacts();
        Assert.That(kinematicBodyContacts, Has.Length.EqualTo(1));
        Assert.That(kinematicBodyContacts[0].ThisCollider, Is.EqualTo(kinematicBodyCollider));
        Assert.That(kinematicBodyContacts[0].OtherCollider, Is.EqualTo(staticBodyCollider));
        Assert.That(kinematicBodyContacts[0].CollisionNormal, Is.EqualTo(-testCase.ExpectedCollisionNormal).Using(Vector2Comparer));
        Assert.That(kinematicBodyContacts[0].PenetrationDepth, Is.EqualTo(testCase.ExpectedPenetrationDepth));
        Assert.That(kinematicBodyContacts[0].ContactPoints.Count, Is.EqualTo(1));

        var cp = testCase.ExpectedContactPoint;
        var reflectedContactPoint = new ContactPoint2D(cp.WorldPosition, cp.OtherLocalPosition, cp.ThisLocalPosition);
        Assert.That(kinematicBodyContacts[0].ContactPoints[0],
            Is.EqualTo(reflectedContactPoint).Using<ContactPoint2D, ContactPoint2D>(ContactPoint2DComparison));

        Assert.That(staticBodyCollider.IsColliding, Is.True);
        var staticBodyContacts = staticBodyCollider.GetContacts();
        Assert.That(staticBodyContacts, Has.Length.EqualTo(1));
        Assert.That(staticBodyContacts[0].ThisCollider, Is.EqualTo(staticBodyCollider));
        Assert.That(staticBodyContacts[0].OtherCollider, Is.EqualTo(kinematicBodyCollider));
        Assert.That(staticBodyContacts[0].CollisionNormal, Is.EqualTo(testCase.ExpectedCollisionNormal).Using(Vector2Comparer));
        Assert.That(staticBodyContacts[0].PenetrationDepth, Is.EqualTo(testCase.ExpectedPenetrationDepth));


        Assert.That(staticBodyContacts[0].ContactPoints.Count, Is.EqualTo(1));
        Assert.That(staticBodyContacts[0].ContactPoints[0],
            Is.EqualTo(testCase.ExpectedContactPoint).Using<ContactPoint2D, ContactPoint2D>(ContactPoint2DComparison));
    }

    #endregion

    #region Rectangle and Tile

    public sealed class RectangleAndTileTestCase
    {
        public AxisAlignedRectangle Rectangle { get; init; }
        public AxisAlignedRectangle Tile { get; init; }

        public double RectangleRotation { get; init; }

        public Vector2 ExpectedCollisionNormal { get; init; }
        public double ExpectedPenetrationDepth { get; init; }
        public ReadOnlyFixedList2<ContactPoint2D> ExpectedContactPoints { get; init; }
    }

    // These test cases are numbered to match the same test cases for Rectangle and Rectangle collision.
    // Therefore, some numbers are skipped as those test cases are not applicable for Rectangle and Tile collision.
    public static IEnumerable<TestCaseData> RectangleAndTileTestCases => new[]
    {
        // Edges overlapping
        new TestCaseData(new RectangleAndTileTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(4, 5), new Vector2(10, 5)),
            Tile = new AxisAlignedRectangle(new Vector2(10, 5), new Vector2(10, 5)),
            ExpectedCollisionNormal = new Vector2(-1, 0),
            ExpectedPenetrationDepth = 4,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(9, 7.5), new Vector2(5, 2.5), new Vector2(-1, 2.5)),
                new ContactPoint2D(new Vector2(9, 2.5), new Vector2(5, -2.5), new Vector2(-1, -2.5)))
        }).SetName($"01_{nameof(RectangleKinematicBody_And_TileStaticBody)}"),
        new TestCaseData(new RectangleAndTileTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(10, 7.5), new Vector2(10, 5)),
            Tile = new AxisAlignedRectangle(new Vector2(10, 5), new Vector2(10, 5)),
            ExpectedCollisionNormal = new Vector2(0, 1),
            ExpectedPenetrationDepth = 2.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(5, 5), new Vector2(-5, -2.5), new Vector2(-5, 0)),
                new ContactPoint2D(new Vector2(15, 5), new Vector2(5, -2.5), new Vector2(5, 0)))
        }).SetName($"02_{nameof(RectangleKinematicBody_And_TileStaticBody)}"),
        new TestCaseData(new RectangleAndTileTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(16, 5), new Vector2(10, 5)),
            Tile = new AxisAlignedRectangle(new Vector2(10, 5), new Vector2(10, 5)),
            ExpectedCollisionNormal = new Vector2(1, 0),
            ExpectedPenetrationDepth = 4,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(11, 7.5), new Vector2(-5, 2.5), new Vector2(1, 2.5)),
                new ContactPoint2D(new Vector2(11, 2.5), new Vector2(-5, -2.5), new Vector2(1, -2.5)))
        }).SetName($"03_{nameof(RectangleKinematicBody_And_TileStaticBody)}"),
        new TestCaseData(new RectangleAndTileTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(10, 2.5), new Vector2(10, 5)),
            Tile = new AxisAlignedRectangle(new Vector2(10, 5), new Vector2(10, 5)),
            ExpectedCollisionNormal = new Vector2(0, -1),
            ExpectedPenetrationDepth = 2.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(5, 5), new Vector2(-5, 2.5), new Vector2(-5, 0)),
                new ContactPoint2D(new Vector2(15, 5), new Vector2(5, 2.5), new Vector2(5, 0)))
        }).SetName($"04_{nameof(RectangleKinematicBody_And_TileStaticBody)}"),
        // Edges touching
        new TestCaseData(new RectangleAndTileTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(0, 5), new Vector2(10, 5)),
            Tile = new AxisAlignedRectangle(new Vector2(10, 5), new Vector2(10, 5)),
            ExpectedCollisionNormal = new Vector2(-1, 0),
            ExpectedPenetrationDepth = 0,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(5, 7.5), new Vector2(5, 2.5), new Vector2(-5, 2.5)),
                new ContactPoint2D(new Vector2(5, 2.5), new Vector2(5, -2.5), new Vector2(-5, -2.5)))
        }).SetName($"05_{nameof(RectangleKinematicBody_And_TileStaticBody)}"),
        new TestCaseData(new RectangleAndTileTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(10, 10), new Vector2(10, 5)),
            Tile = new AxisAlignedRectangle(new Vector2(10, 5), new Vector2(10, 5)),
            ExpectedCollisionNormal = new Vector2(0, 1),
            ExpectedPenetrationDepth = 0,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(5, 7.5), new Vector2(-5, -2.5), new Vector2(-5, 2.5)),
                new ContactPoint2D(new Vector2(15, 7.5), new Vector2(5, -2.5), new Vector2(5, 2.5)))
        }).SetName($"06_{nameof(RectangleKinematicBody_And_TileStaticBody)}"),
        new TestCaseData(new RectangleAndTileTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(20, 5), new Vector2(10, 5)),
            Tile = new AxisAlignedRectangle(new Vector2(10, 5), new Vector2(10, 5)),
            ExpectedCollisionNormal = new Vector2(1, 0),
            ExpectedPenetrationDepth = 0,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(15, 7.5), new Vector2(-5, 2.5), new Vector2(5, 2.5)),
                new ContactPoint2D(new Vector2(15, 2.5), new Vector2(-5, -2.5), new Vector2(5, -2.5)))
        }).SetName($"07_{nameof(RectangleKinematicBody_And_TileStaticBody)}"),
        new TestCaseData(new RectangleAndTileTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(10, 0), new Vector2(10, 5)),
            Tile = new AxisAlignedRectangle(new Vector2(10, 5), new Vector2(10, 5)),
            ExpectedCollisionNormal = new Vector2(0, -1),
            ExpectedPenetrationDepth = 0,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(5, 2.5), new Vector2(-5, 2.5), new Vector2(-5, -2.5)),
                new ContactPoint2D(new Vector2(15, 2.5), new Vector2(5, 2.5), new Vector2(5, -2.5)))
        }).SetName($"08_{nameof(RectangleKinematicBody_And_TileStaticBody)}"),
        // Single vertex overlapping (kinematic into static)
        new TestCaseData(new RectangleAndTileTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(1, 3.5), new Vector2(10, 5)),
            Tile = new AxisAlignedRectangle(new Vector2(10, 5), new Vector2(10, 5)),
            RectangleRotation = Angle.Deg2Rad(45),
            ExpectedCollisionNormal = new Vector2(-1, 0),
            ExpectedPenetrationDepth = 1.303300,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(6.303300, 5.267766), new Vector2(5, -2.5), new Vector2(-3.696699, 0.267766)))
        }).SetName($"09_{nameof(RectangleKinematicBody_And_TileStaticBody)}"),
        new TestCaseData(new RectangleAndTileTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(10, 11.5), new Vector2(10, 5)),
            Tile = new AxisAlignedRectangle(new Vector2(10, 5), new Vector2(10, 5)),
            RectangleRotation = Angle.Deg2Rad(45),
            ExpectedCollisionNormal = new Vector2(0, 1),
            ExpectedPenetrationDepth = 1.303300,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(8.232233, 6.196699), new Vector2(-5, -2.5), new Vector2(-1.767766, 1.196699)))
        }).SetName($"10_{nameof(RectangleKinematicBody_And_TileStaticBody)}"),
        new TestCaseData(new RectangleAndTileTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(19, 6.5), new Vector2(10, 5)),
            Tile = new AxisAlignedRectangle(new Vector2(10, 5), new Vector2(10, 5)),
            RectangleRotation = Angle.Deg2Rad(45),
            ExpectedCollisionNormal = new Vector2(1, 0),
            ExpectedPenetrationDepth = 1.303300,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(13.696699, 4.732233), new Vector2(-5, 2.5), new Vector2(3.696699, -0.267766)))
        }).SetName($"11_{nameof(RectangleKinematicBody_And_TileStaticBody)}"),
        new TestCaseData(new RectangleAndTileTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(10, -1.5), new Vector2(10, 5)),
            Tile = new AxisAlignedRectangle(new Vector2(10, 5), new Vector2(10, 5)),
            RectangleRotation = Angle.Deg2Rad(45),
            ExpectedCollisionNormal = new Vector2(0, -1),
            ExpectedPenetrationDepth = 1.303300,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(11.767766, 3.803300), new Vector2(5, 2.5), new Vector2(1.767766, -1.196699)))
        }).SetName($"12_{nameof(RectangleKinematicBody_And_TileStaticBody)}"),
        // Single vertex overlapping (static into kinematic)
        new TestCaseData(new RectangleAndTileTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(5, 7.5), new Vector2(10, 5)),
            Tile = new AxisAlignedRectangle(new Vector2(10, 5), new Vector2(10, 5)),
            RectangleRotation = Angle.Deg2Rad(45),
            ExpectedCollisionNormal = new Vector2(-0.707106, 0.707106),
            ExpectedPenetrationDepth = 2.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(5, 7.5), new Vector2(0, 0), new Vector2(-5, 2.5)))
        }).SetName($"13_{nameof(RectangleKinematicBody_And_TileStaticBody)}"),
        new TestCaseData(new RectangleAndTileTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(15, 7.5), new Vector2(10, 5)),
            Tile = new AxisAlignedRectangle(new Vector2(10, 5), new Vector2(10, 5)),
            RectangleRotation = Angle.Deg2Rad(-45),
            ExpectedCollisionNormal = new Vector2(0.707106, 0.707106),
            ExpectedPenetrationDepth = 2.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(15, 7.5), new Vector2(0, 0), new Vector2(5, 2.5)))
        }).SetName($"14_{nameof(RectangleKinematicBody_And_TileStaticBody)}"),
        new TestCaseData(new RectangleAndTileTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(15, 2.5), new Vector2(10, 5)),
            Tile = new AxisAlignedRectangle(new Vector2(10, 5), new Vector2(10, 5)),
            RectangleRotation = Angle.Deg2Rad(45),
            ExpectedCollisionNormal = new Vector2(0.707106, -0.707106),
            ExpectedPenetrationDepth = 2.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(15, 2.5), new Vector2(0, 0), new Vector2(5, -2.5)))
        }).SetName($"15_{nameof(RectangleKinematicBody_And_TileStaticBody)}"),
        new TestCaseData(new RectangleAndTileTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(5, 2.5), new Vector2(10, 5)),
            Tile = new AxisAlignedRectangle(new Vector2(10, 5), new Vector2(10, 5)),
            RectangleRotation = Angle.Deg2Rad(-45),
            ExpectedCollisionNormal = new Vector2(-0.707106, -0.707106),
            ExpectedPenetrationDepth = 2.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(5, 2.5), new Vector2(0, 0), new Vector2(-5, -2.5)))
        }).SetName($"16_{nameof(RectangleKinematicBody_And_TileStaticBody)}"),
        // Two contact points without clipping
        new TestCaseData(new RectangleAndTileTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(3, 10), new Vector2(5, 5)),
            Tile = new AxisAlignedRectangle(new Vector2(10, 10), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(-1, 0),
            ExpectedPenetrationDepth = 0.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(5.5, 12.5), new Vector2(2.5, 2.5), new Vector2(-4.5, 2.5)),
                new ContactPoint2D(new Vector2(5.5, 7.5), new Vector2(2.5, -2.5), new Vector2(-4.5, -2.5)))
        }).SetName($"17_{nameof(RectangleKinematicBody_And_TileStaticBody)}"),
        new TestCaseData(new RectangleAndTileTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(10, 17), new Vector2(5, 5)),
            Tile = new AxisAlignedRectangle(new Vector2(10, 10), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(0, 1),
            ExpectedPenetrationDepth = 0.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(7.5, 14.5), new Vector2(-2.5, -2.5), new Vector2(-2.5, 4.5)),
                new ContactPoint2D(new Vector2(12.5, 14.5), new Vector2(2.5, -2.5), new Vector2(2.5, 4.5)))
        }).SetName($"18_{nameof(RectangleKinematicBody_And_TileStaticBody)}"),
        new TestCaseData(new RectangleAndTileTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(17, 10), new Vector2(5, 5)),
            Tile = new AxisAlignedRectangle(new Vector2(10, 10), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(1, 0),
            ExpectedPenetrationDepth = 0.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(14.5, 12.5), new Vector2(-2.5, 2.5), new Vector2(4.5, 2.5)),
                new ContactPoint2D(new Vector2(14.5, 7.5), new Vector2(-2.5, -2.5), new Vector2(4.5, -2.5)))
        }).SetName($"19_{nameof(RectangleKinematicBody_And_TileStaticBody)}"),
        new TestCaseData(new RectangleAndTileTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(10, 3), new Vector2(5, 5)),
            Tile = new AxisAlignedRectangle(new Vector2(10, 10), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(0, -1),
            ExpectedPenetrationDepth = 0.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(7.5, 5.5), new Vector2(-2.5, 2.5), new Vector2(-2.5, -4.5)),
                new ContactPoint2D(new Vector2(12.5, 5.5), new Vector2(2.5, 2.5), new Vector2(2.5, -4.5)))
        }).SetName($"20_{nameof(RectangleKinematicBody_And_TileStaticBody)}"),
        // Two contact points and one from clipping
        new TestCaseData(new RectangleAndTileTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(3, 6), new Vector2(5, 5)),
            Tile = new AxisAlignedRectangle(new Vector2(10, 10), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(-1, 0),
            ExpectedPenetrationDepth = 0.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(5.5, 8.5), new Vector2(2.5, 2.5), new Vector2(-4.5, -1.5)),
                new ContactPoint2D(new Vector2(5.5, 5), new Vector2(2.5, -1), new Vector2(-4.5, -5)))
        }).SetName($"21_{nameof(RectangleKinematicBody_And_TileStaticBody)}"),
        new TestCaseData(new RectangleAndTileTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(3, 14), new Vector2(5, 5)),
            Tile = new AxisAlignedRectangle(new Vector2(10, 10), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(-1, 0),
            ExpectedPenetrationDepth = 0.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(5.5, 15), new Vector2(2.5, 1), new Vector2(-4.5, 5)),
                new ContactPoint2D(new Vector2(5.5, 11.5), new Vector2(2.5, -2.5), new Vector2(-4.5, 1.5)))
        }).SetName($"22_{nameof(RectangleKinematicBody_And_TileStaticBody)}"),
        new TestCaseData(new RectangleAndTileTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(6, 17), new Vector2(5, 5)),
            Tile = new AxisAlignedRectangle(new Vector2(10, 10), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(0, 1),
            ExpectedPenetrationDepth = 0.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(5, 14.5), new Vector2(-1, -2.5), new Vector2(-5, 4.5)),
                new ContactPoint2D(new Vector2(8.5, 14.5), new Vector2(2.5, -2.5), new Vector2(-1.5, 4.5)))
        }).SetName($"23_{nameof(RectangleKinematicBody_And_TileStaticBody)}"),
        new TestCaseData(new RectangleAndTileTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(14, 17), new Vector2(5, 5)),
            Tile = new AxisAlignedRectangle(new Vector2(10, 10), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(0, 1),
            ExpectedPenetrationDepth = 0.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(11.5, 14.5), new Vector2(-2.5, -2.5), new Vector2(1.5, 4.5)),
                new ContactPoint2D(new Vector2(15, 14.5), new Vector2(1, -2.5), new Vector2(5, 4.5)))
        }).SetName($"24_{nameof(RectangleKinematicBody_And_TileStaticBody)}"),
        new TestCaseData(new RectangleAndTileTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(17, 14), new Vector2(5, 5)),
            Tile = new AxisAlignedRectangle(new Vector2(10, 10), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(1, 0),
            ExpectedPenetrationDepth = 0.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(14.5, 15), new Vector2(-2.5, 1), new Vector2(4.5, 5)),
                new ContactPoint2D(new Vector2(14.5, 11.5), new Vector2(-2.5, -2.5), new Vector2(4.5, 1.5)))
        }).SetName($"25_{nameof(RectangleKinematicBody_And_TileStaticBody)}"),
        new TestCaseData(new RectangleAndTileTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(17, 6), new Vector2(5, 5)),
            Tile = new AxisAlignedRectangle(new Vector2(10, 10), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(1, 0),
            ExpectedPenetrationDepth = 0.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(14.5, 8.5), new Vector2(-2.5, 2.5), new Vector2(4.5, -1.5)),
                new ContactPoint2D(new Vector2(14.5, 5), new Vector2(-2.5, -1), new Vector2(4.5, -5)))
        }).SetName($"26_{nameof(RectangleKinematicBody_And_TileStaticBody)}"),
        new TestCaseData(new RectangleAndTileTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(14, 3), new Vector2(5, 5)),
            Tile = new AxisAlignedRectangle(new Vector2(10, 10), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(0, -1),
            ExpectedPenetrationDepth = 0.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(11.5, 5.5), new Vector2(-2.5, 2.5), new Vector2(1.5, -4.5)),
                new ContactPoint2D(new Vector2(15, 5.5), new Vector2(1, 2.5), new Vector2(5, -4.5)))
        }).SetName($"27_{nameof(RectangleKinematicBody_And_TileStaticBody)}"),
        new TestCaseData(new RectangleAndTileTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(6, 3), new Vector2(5, 5)),
            Tile = new AxisAlignedRectangle(new Vector2(10, 10), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(0, -1),
            ExpectedPenetrationDepth = 0.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(5, 5.5), new Vector2(-1, 2.5), new Vector2(-5, -4.5)),
                new ContactPoint2D(new Vector2(8.5, 5.5), new Vector2(2.5, 2.5), new Vector2(-1.5, -4.5)))
        }).SetName($"28_{nameof(RectangleKinematicBody_And_TileStaticBody)}"),
        // Two contact points both from clipping
        new TestCaseData(new RectangleAndTileTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(-4, 10), new Vector2(20, 20)),
            Tile = new AxisAlignedRectangle(new Vector2(10, 10), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(-1, 0),
            ExpectedPenetrationDepth = 1,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(6, 15), new Vector2(10, 5), new Vector2(-4, 5)),
                new ContactPoint2D(new Vector2(6, 5), new Vector2(10, -5), new Vector2(-4, -5)))
        }).SetName($"29_{nameof(RectangleKinematicBody_And_TileStaticBody)}"),
        new TestCaseData(new RectangleAndTileTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(10, 24), new Vector2(20, 20)),
            Tile = new AxisAlignedRectangle(new Vector2(10, 10), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(0, 1),
            ExpectedPenetrationDepth = 1,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(5, 14), new Vector2(-5, -10), new Vector2(-5, 4)),
                new ContactPoint2D(new Vector2(15, 14), new Vector2(5, -10), new Vector2(5, 4)))
        }).SetName($"30_{nameof(RectangleKinematicBody_And_TileStaticBody)}"),
        new TestCaseData(new RectangleAndTileTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(24, 10), new Vector2(20, 20)),
            Tile = new AxisAlignedRectangle(new Vector2(10, 10), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(1, 0),
            ExpectedPenetrationDepth = 1,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(14, 15), new Vector2(-10, 5), new Vector2(4, 5)),
                new ContactPoint2D(new Vector2(14, 5), new Vector2(-10, -5), new Vector2(4, -5)))
        }).SetName($"31_{nameof(RectangleKinematicBody_And_TileStaticBody)}"),
        new TestCaseData(new RectangleAndTileTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(10, -4), new Vector2(20, 20)),
            Tile = new AxisAlignedRectangle(new Vector2(10, 10), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(0, -1),
            ExpectedPenetrationDepth = 1,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(5, 6), new Vector2(-5, 10), new Vector2(-5, -4)),
                new ContactPoint2D(new Vector2(15, 6), new Vector2(5, 10), new Vector2(5, -4)))
        }).SetName($"32_{nameof(RectangleKinematicBody_And_TileStaticBody)}"),
        // Vertices touching
        new TestCaseData(new RectangleAndTileTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(0, 20), new Vector2(10, 10)),
            Tile = new AxisAlignedRectangle(new Vector2(10, 10), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(-1, 0),
            ExpectedPenetrationDepth = 0,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(5, 15), new Vector2(5, -5), new Vector2(-5, 5)))
        }).SetName($"34_{nameof(RectangleKinematicBody_And_TileStaticBody)}"),
        new TestCaseData(new RectangleAndTileTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(20, 20), new Vector2(10, 10)),
            Tile = new AxisAlignedRectangle(new Vector2(10, 10), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(1, 0),
            ExpectedPenetrationDepth = 0,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(15, 15), new Vector2(-5, -5), new Vector2(5, 5)))
        }).SetName($"35_{nameof(RectangleKinematicBody_And_TileStaticBody)}"),
        new TestCaseData(new RectangleAndTileTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(20, 0), new Vector2(10, 10)),
            Tile = new AxisAlignedRectangle(new Vector2(10, 10), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(0, -1),
            ExpectedPenetrationDepth = 0,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(15, 5), new Vector2(-5, 5), new Vector2(5, -5)))
        }).SetName($"36_{nameof(RectangleKinematicBody_And_TileStaticBody)}"),
        new TestCaseData(new RectangleAndTileTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(0, 0), new Vector2(10, 10)),
            Tile = new AxisAlignedRectangle(new Vector2(10, 10), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(-1, 0),
            ExpectedPenetrationDepth = 0,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(5, 5), new Vector2(5, 5), new Vector2(-5, -5)))
        }).SetName($"37_{nameof(RectangleKinematicBody_And_TileStaticBody)}")
    };

    [TestCaseSource(nameof(RectangleAndTileTestCases))]
    public void RectangleKinematicBody_And_TileStaticBody(RectangleAndTileTestCase testCase)
    {
        // Arrange
        var physicsConfiguration = new PhysicsConfiguration
        {
            TileSize = new SizeD(testCase.Tile.Width, testCase.Tile.Height),
            EnableDebugRendering = true
        };
        var physicsSystem = GetPhysicsSystem(physicsConfiguration);
        var kinematicBody = CreateRectangleKinematicBody(testCase.Rectangle, testCase.RectangleRotation);
        var staticBody = CreateTileStaticBody(testCase.Tile.Center);

        SaveVisualOutput(physicsSystem, 0, 10);

        // Assume
        Assert.That(kinematicBody.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
        Assert.That(staticBody.GetComponent<TileColliderComponent>().IsColliding, Is.False);

        // Act
        physicsSystem.ProcessPhysics();

        SaveVisualOutput(physicsSystem, 1, 10);

        // Assert
        Assert.That(staticBody.GetComponent<Transform2DComponent>().Translation, Is.EqualTo(testCase.Tile.Center), "Tile is misaligned.");

        var kinematicBodyCollider = kinematicBody.GetComponent<RectangleColliderComponent>();
        var staticBodyCollider = staticBody.GetComponent<TileColliderComponent>();

        Assert.That(kinematicBodyCollider.IsColliding, Is.True);
        var kinematicBodyContacts = kinematicBodyCollider.GetContacts();
        Assert.That(kinematicBodyContacts, Has.Length.EqualTo(1));
        Assert.That(kinematicBodyContacts[0].ThisCollider, Is.EqualTo(kinematicBodyCollider));
        Assert.That(kinematicBodyContacts[0].OtherCollider, Is.EqualTo(staticBodyCollider));
        Assert.That(kinematicBodyContacts[0].CollisionNormal, Is.EqualTo(testCase.ExpectedCollisionNormal).Using(Vector2Comparer));
        Assert.That(kinematicBodyContacts[0].PenetrationDepth, Is.EqualTo(testCase.ExpectedPenetrationDepth));
        Assert.That(kinematicBodyContacts[0].ContactPoints.ToArray(),
            Is.EquivalentTo(testCase.ExpectedContactPoints.ToArray()).Using<ContactPoint2D, ContactPoint2D>(ContactPoint2DComparison));

        Assert.That(staticBodyCollider.IsColliding, Is.True);
        var staticBodyContacts = staticBodyCollider.GetContacts();
        Assert.That(staticBodyContacts, Has.Length.EqualTo(1));
        Assert.That(staticBodyContacts[0].ThisCollider, Is.EqualTo(staticBodyCollider));
        Assert.That(staticBodyContacts[0].OtherCollider, Is.EqualTo(kinematicBodyCollider));
        Assert.That(staticBodyContacts[0].CollisionNormal, Is.EqualTo(-testCase.ExpectedCollisionNormal).Using(Vector2Comparer));
        Assert.That(staticBodyContacts[0].PenetrationDepth, Is.EqualTo(testCase.ExpectedPenetrationDepth));

        var reflectedExpectedContactPoints = testCase.ExpectedContactPoints.ToArray()
            .Select(cp => new ContactPoint2D(cp.WorldPosition, cp.OtherLocalPosition, cp.ThisLocalPosition)).ToArray();
        Assert.That(staticBodyContacts[0].ContactPoints.ToArray(),
            Is.EquivalentTo(reflectedExpectedContactPoints).Using<ContactPoint2D, ContactPoint2D>(ContactPoint2DComparison));
    }

    #endregion

    #region Circle and Tile

    public sealed class CircleAndTileTestCase
    {
        public AxisAlignedRectangle Tile { get; init; }
        public Circle Circle { get; init; }

        public double CircleRotation { get; init; }

        public Vector2 ExpectedCollisionNormal { get; init; }
        public double ExpectedPenetrationDepth { get; init; }
        public ContactPoint2D ExpectedContactPoint { get; init; }
    }

    // These test cases are numbered to match the same test cases for Rectangle and Circle collision.
    // Therefore, some numbers are skipped as those test cases are not applicable for Circle and Tile collision.
    public static IEnumerable<TestCaseData> CircleAndTileTestCases => new[]
    {
        // Axis aligned edge overlap
        new TestCaseData(new CircleAndTileTestCase
        {
            Tile = new AxisAlignedRectangle(new Vector2(10, 10), new Vector2(10, 10)),
            Circle = new Circle(new Vector2(19, 10), 5),
            ExpectedCollisionNormal = new Vector2(-1, 0),
            ExpectedPenetrationDepth = 1,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(14.5, 10), new Vector2(4.5, 0), new Vector2(-4.5, 0))
        }).SetName($"01_{nameof(CircleKinematicBody_And_TileStaticBody)}"),
        new TestCaseData(new CircleAndTileTestCase
        {
            Tile = new AxisAlignedRectangle(new Vector2(10, 10), new Vector2(10, 10)),
            Circle = new Circle(new Vector2(10, 1), 5),
            ExpectedCollisionNormal = new Vector2(0, 1),
            ExpectedPenetrationDepth = 1,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(10, 5.5), new Vector2(0, -4.5), new Vector2(0, 4.5))
        }).SetName($"02_{nameof(CircleKinematicBody_And_TileStaticBody)}"),
        new TestCaseData(new CircleAndTileTestCase
        {
            Tile = new AxisAlignedRectangle(new Vector2(10, 10), new Vector2(10, 10)),
            Circle = new Circle(new Vector2(1, 10), 5),
            ExpectedCollisionNormal = new Vector2(1, 0),
            ExpectedPenetrationDepth = 1,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(5.5, 10), new Vector2(-4.5, 0), new Vector2(4.5, 0))
        }).SetName($"03_{nameof(CircleKinematicBody_And_TileStaticBody)}"),
        new TestCaseData(new CircleAndTileTestCase
        {
            Tile = new AxisAlignedRectangle(new Vector2(10, 10), new Vector2(10, 10)),
            Circle = new Circle(new Vector2(10, 19), 5),
            ExpectedCollisionNormal = new Vector2(0, -1),
            ExpectedPenetrationDepth = 1,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(10, 14.5), new Vector2(0, 4.5), new Vector2(0, -4.5))
        }).SetName($"04_{nameof(CircleKinematicBody_And_TileStaticBody)}"),
        // Touching
        new TestCaseData(new CircleAndTileTestCase
        {
            Tile = new AxisAlignedRectangle(new Vector2(10, 10), new Vector2(10, 10)),
            Circle = new Circle(new Vector2(20, 10), 5),
            ExpectedCollisionNormal = new Vector2(-1, 0),
            ExpectedPenetrationDepth = 0,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(15, 10), new Vector2(5, 0), new Vector2(-5, 0))
        }).SetName($"05_{nameof(CircleKinematicBody_And_TileStaticBody)}"),
        new TestCaseData(new CircleAndTileTestCase
        {
            Tile = new AxisAlignedRectangle(new Vector2(10, 10), new Vector2(10, 10)),
            Circle = new Circle(new Vector2(10, 0), 5),
            ExpectedCollisionNormal = new Vector2(0, 1),
            ExpectedPenetrationDepth = 0,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(10, 5), new Vector2(0, -5), new Vector2(0, 5))
        }).SetName($"06_{nameof(CircleKinematicBody_And_TileStaticBody)}"),
        new TestCaseData(new CircleAndTileTestCase
        {
            Tile = new AxisAlignedRectangle(new Vector2(10, 10), new Vector2(10, 10)),
            Circle = new Circle(new Vector2(0, 10), 5),
            ExpectedCollisionNormal = new Vector2(1, 0),
            ExpectedPenetrationDepth = 0,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(5, 10), new Vector2(-5, 0), new Vector2(5, 0))
        }).SetName($"07_{nameof(CircleKinematicBody_And_TileStaticBody)}"),
        new TestCaseData(new CircleAndTileTestCase
        {
            Tile = new AxisAlignedRectangle(new Vector2(10, 10), new Vector2(10, 10)),
            Circle = new Circle(new Vector2(10, 20), 5),
            ExpectedCollisionNormal = new Vector2(0, -1),
            ExpectedPenetrationDepth = 0,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(10, 15), new Vector2(0, 5), new Vector2(0, -5))
        }).SetName($"08_{nameof(CircleKinematicBody_And_TileStaticBody)}"),
        // Diagonal vertex overlap
        new TestCaseData(new CircleAndTileTestCase
        {
            Tile = new AxisAlignedRectangle(new Vector2(10, 10), new Vector2(10, 10)),
            Circle = new Circle(new Vector2(18, 2), 5),
            ExpectedCollisionNormal = new Vector2(-0.707106, 0.707106),
            ExpectedPenetrationDepth = 0.757359,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(14.732233, 5.267766), new Vector2(4.732233, -4.732233), new Vector2(-3.267766, 3.267766))
        }).SetName($"17_{nameof(CircleKinematicBody_And_TileStaticBody)}"),
        new TestCaseData(new CircleAndTileTestCase
        {
            Tile = new AxisAlignedRectangle(new Vector2(10, 10), new Vector2(10, 10)),
            Circle = new Circle(new Vector2(2, 2), 5),
            ExpectedCollisionNormal = new Vector2(0.707106, 0.707106),
            ExpectedPenetrationDepth = 0.757359,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(5.267766, 5.267766), new Vector2(-4.732233, -4.732233), new Vector2(3.267766, 3.267766))
        }).SetName($"18_{nameof(CircleKinematicBody_And_TileStaticBody)}"),
        new TestCaseData(new CircleAndTileTestCase
        {
            Tile = new AxisAlignedRectangle(new Vector2(10, 10), new Vector2(10, 10)),
            Circle = new Circle(new Vector2(2, 18), 5),
            ExpectedCollisionNormal = new Vector2(0.707106, -0.707106),
            ExpectedPenetrationDepth = 0.757359,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(5.267766, 14.732233), new Vector2(-4.732233, 4.732233), new Vector2(3.267766, -3.267766))
        }).SetName($"19_{nameof(CircleKinematicBody_And_TileStaticBody)}"),
        new TestCaseData(new CircleAndTileTestCase
        {
            Tile = new AxisAlignedRectangle(new Vector2(10, 10), new Vector2(10, 10)),
            Circle = new Circle(new Vector2(18, 18), 5),
            ExpectedCollisionNormal = new Vector2(-0.707106, -0.707106),
            ExpectedPenetrationDepth = 0.757359,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(14.732233, 14.732233), new Vector2(4.732233, 4.732233), new Vector2(-3.267766, -3.267766))
        }).SetName($"20_{nameof(CircleKinematicBody_And_TileStaticBody)}"),
        // Different sizes
        new TestCaseData(new CircleAndTileTestCase
        {
            Tile = new AxisAlignedRectangle(new Vector2(5, 5), new Vector2(5, 5)),
            Circle = new Circle(new Vector2(12, 5), 5),
            ExpectedCollisionNormal = new Vector2(-1, 0),
            ExpectedPenetrationDepth = 0.5,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(7.25, 5), new Vector2(2.25, 0), new Vector2(-4.75, 0))
        }).SetName($"21_{nameof(CircleKinematicBody_And_TileStaticBody)}"),
        new TestCaseData(new CircleAndTileTestCase
        {
            Tile = new AxisAlignedRectangle(new Vector2(20, 20), new Vector2(20, 20)),
            Circle = new Circle(new Vector2(33, 20), 5),
            ExpectedCollisionNormal = new Vector2(-1, 0),
            ExpectedPenetrationDepth = 2,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(29, 20), new Vector2(9, 0), new Vector2(-4, 0))
        }).SetName($"22_{nameof(CircleKinematicBody_And_TileStaticBody)}"),
        // Circle rotated
        new TestCaseData(new CircleAndTileTestCase
        {
            Tile = new AxisAlignedRectangle(new Vector2(10, 10), new Vector2(10, 10)),
            Circle = new Circle(new Vector2(19, 10), 5),
            CircleRotation = Angle.Deg2Rad(45),
            ExpectedCollisionNormal = new Vector2(-1, 0),
            ExpectedPenetrationDepth = 1,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(14.5, 10), new Vector2(4.5, 0), new Vector2(-3.181981, 3.181981))
        }).SetName($"25_{nameof(CircleKinematicBody_And_TileStaticBody)}")
    };

    [TestCaseSource(nameof(CircleAndTileTestCases))]
    public void CircleKinematicBody_And_TileStaticBody(CircleAndTileTestCase testCase)
    {
        // Arrange
        var physicsConfiguration = new PhysicsConfiguration
        {
            TileSize = new SizeD(testCase.Tile.Width, testCase.Tile.Height),
            EnableDebugRendering = true
        };
        var physicsSystem = GetPhysicsSystem(physicsConfiguration);
        var kinematicBody = CreateCircleKinematicBody(testCase.Circle, testCase.CircleRotation);
        var staticBody = CreateTileStaticBody(testCase.Tile.Center);

        SaveVisualOutput(physicsSystem, 0, 10);

        // Assume
        Assert.That(kinematicBody.GetComponent<CircleColliderComponent>().IsColliding, Is.False);
        Assert.That(staticBody.GetComponent<TileColliderComponent>().IsColliding, Is.False);

        // Act
        physicsSystem.ProcessPhysics();

        SaveVisualOutput(physicsSystem, 1, 10);

        // Assert
        Assert.That(staticBody.GetComponent<Transform2DComponent>().Translation, Is.EqualTo(testCase.Tile.Center), "Tile is misaligned.");

        var kinematicBodyCollider = kinematicBody.GetComponent<CircleColliderComponent>();
        var staticBodyCollider = staticBody.GetComponent<TileColliderComponent>();

        Assert.That(kinematicBodyCollider.IsColliding, Is.True);
        var kinematicBodyContacts = kinematicBodyCollider.GetContacts();
        Assert.That(kinematicBodyContacts, Has.Length.EqualTo(1));
        Assert.That(kinematicBodyContacts[0].ThisCollider, Is.EqualTo(kinematicBodyCollider));
        Assert.That(kinematicBodyContacts[0].OtherCollider, Is.EqualTo(staticBodyCollider));
        Assert.That(kinematicBodyContacts[0].CollisionNormal, Is.EqualTo(-testCase.ExpectedCollisionNormal).Using(Vector2Comparer));
        Assert.That(kinematicBodyContacts[0].PenetrationDepth, Is.EqualTo(testCase.ExpectedPenetrationDepth));
        Assert.That(kinematicBodyContacts[0].ContactPoints.Count, Is.EqualTo(1));

        var cp = testCase.ExpectedContactPoint;
        var reflectedContactPoint = new ContactPoint2D(cp.WorldPosition, cp.OtherLocalPosition, cp.ThisLocalPosition);
        Assert.That(kinematicBodyContacts[0].ContactPoints[0],
            Is.EqualTo(reflectedContactPoint).Using<ContactPoint2D, ContactPoint2D>(ContactPoint2DComparison));

        Assert.That(staticBodyCollider.IsColliding, Is.True);
        var staticBodyContacts = staticBodyCollider.GetContacts();
        Assert.That(staticBodyContacts, Has.Length.EqualTo(1));
        Assert.That(staticBodyContacts[0].ThisCollider, Is.EqualTo(staticBodyCollider));
        Assert.That(staticBodyContacts[0].OtherCollider, Is.EqualTo(kinematicBodyCollider));
        Assert.That(staticBodyContacts[0].CollisionNormal, Is.EqualTo(testCase.ExpectedCollisionNormal).Using(Vector2Comparer));
        Assert.That(staticBodyContacts[0].PenetrationDepth, Is.EqualTo(testCase.ExpectedPenetrationDepth));


        Assert.That(staticBodyContacts[0].ContactPoints.Count, Is.EqualTo(1));
        Assert.That(staticBodyContacts[0].ContactPoints[0],
            Is.EqualTo(testCase.ExpectedContactPoint).Using<ContactPoint2D, ContactPoint2D>(ContactPoint2DComparison));
    }

    #endregion
}