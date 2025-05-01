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
    public sealed class RectangleAndRectangleTestCase
    {
        public AxisAlignedRectangle Rectangle1 { get; init; }
        public AxisAlignedRectangle Rectangle2 { get; init; }

        public double Rotation1 { get; init; }
        public double Rotation2 { get; init; }

        public Vector2 ExpectedCollisionNormal { get; init; }
        public double ExpectedSeparationDepth { get; init; }
        public ReadOnlyFixedList2<ContactPoint2D> ExpectedContactPoints { get; init; }
    }

    public static IEnumerable<TestCaseData> RectangleAndRectangleTestCases => new[]
    {
        // Edges overlapping
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(-1, 2.5), new Vector2(10, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 2.5), new Vector2(10, 5)),
            ExpectedCollisionNormal = new Vector2(-1, 0),
            ExpectedSeparationDepth = 4,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(4, 5), new Vector2(5, 2.5), new Vector2(-1, 2.5)),
                new ContactPoint2D(new Vector2(4, 0), new Vector2(5, -2.5), new Vector2(-1, -2.5)))
        }).SetName($"01_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(5, 5), new Vector2(10, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 2.5), new Vector2(10, 5)),
            ExpectedCollisionNormal = new Vector2(0, 1),
            ExpectedSeparationDepth = 2.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(0, 2.5), new Vector2(-5, -2.5), new Vector2(-5, 0)),
                new ContactPoint2D(new Vector2(10, 2.5), new Vector2(5, -2.5), new Vector2(5, 0)))
        }).SetName($"02_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(11, 2.5), new Vector2(10, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 2.5), new Vector2(10, 5)),
            ExpectedCollisionNormal = new Vector2(1, 0),
            ExpectedSeparationDepth = 4,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(6, 5), new Vector2(-5, 2.5), new Vector2(1, 2.5)),
                new ContactPoint2D(new Vector2(6, 0), new Vector2(-5, -2.5), new Vector2(1, -2.5)))
        }).SetName($"03_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(5, 0), new Vector2(10, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 2.5), new Vector2(10, 5)),
            ExpectedCollisionNormal = new Vector2(0, -1),
            ExpectedSeparationDepth = 2.5,
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
            ExpectedSeparationDepth = 0,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(0, 5), new Vector2(5, 2.5), new Vector2(-5, 2.5)),
                new ContactPoint2D(new Vector2(0, 0), new Vector2(5, -2.5), new Vector2(-5, -2.5)))
        }).SetName($"05_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(5, 7.5), new Vector2(10, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 2.5), new Vector2(10, 5)),
            ExpectedCollisionNormal = new Vector2(0, 1),
            ExpectedSeparationDepth = 0,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(0, 5), new Vector2(-5, -2.5), new Vector2(-5, 2.5)),
                new ContactPoint2D(new Vector2(10, 5), new Vector2(5, -2.5), new Vector2(5, 2.5)))
        }).SetName($"06_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(15, 2.5), new Vector2(10, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 2.5), new Vector2(10, 5)),
            ExpectedCollisionNormal = new Vector2(1, 0),
            ExpectedSeparationDepth = 0,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(10, 5), new Vector2(-5, 2.5), new Vector2(5, 2.5)),
                new ContactPoint2D(new Vector2(10, 0), new Vector2(-5, -2.5), new Vector2(5, -2.5)))
        }).SetName($"07_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(5, -2.5), new Vector2(10, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 2.5), new Vector2(10, 5)),
            ExpectedCollisionNormal = new Vector2(0, -1),
            ExpectedSeparationDepth = 0,
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
            ExpectedSeparationDepth = 1.303300,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(1.303300, 2.767766), new Vector2(5, -2.5), new Vector2(-3.696699, 0.267766)))
        }).SetName($"09_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(5, 9), new Vector2(10, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 2.5), new Vector2(10, 5)),
            Rotation1 = Angle.Deg2Rad(45),
            ExpectedCollisionNormal = new Vector2(0, 1),
            ExpectedSeparationDepth = 1.303300,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(3.232233, 3.696699), new Vector2(-5, -2.5), new Vector2(-1.767766, 1.196699)))
        }).SetName($"10_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(14, 4), new Vector2(10, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 2.5), new Vector2(10, 5)),
            Rotation1 = Angle.Deg2Rad(45),
            ExpectedCollisionNormal = new Vector2(1, 0),
            ExpectedSeparationDepth = 1.303300,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(8.696699, 2.232233), new Vector2(-5, 2.5), new Vector2(3.696699, -0.267766)))
        }).SetName($"11_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(5, -4), new Vector2(10, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 2.5), new Vector2(10, 5)),
            Rotation1 = Angle.Deg2Rad(45),
            ExpectedCollisionNormal = new Vector2(0, -1),
            ExpectedSeparationDepth = 1.303300,
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
            ExpectedSeparationDepth = 2.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(0, 5), new Vector2(0, 0), new Vector2(-5, 2.5)))
        }).SetName($"13_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(10, 5), new Vector2(10, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 2.5), new Vector2(10, 5)),
            Rotation1 = Angle.Deg2Rad(-45),
            ExpectedCollisionNormal = new Vector2(0.707106, 0.707106),
            ExpectedSeparationDepth = 2.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(10, 5), new Vector2(0, 0), new Vector2(5, 2.5)))
        }).SetName($"14_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(10, 0), new Vector2(10, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 2.5), new Vector2(10, 5)),
            Rotation1 = Angle.Deg2Rad(45),
            ExpectedCollisionNormal = new Vector2(0.707106, -0.707106),
            ExpectedSeparationDepth = 2.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(10, 0), new Vector2(0, 0), new Vector2(5, -2.5)))
        }).SetName($"15_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(0, 0), new Vector2(10, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 2.5), new Vector2(10, 5)),
            Rotation1 = Angle.Deg2Rad(-45),
            ExpectedCollisionNormal = new Vector2(-0.707106, -0.707106),
            ExpectedSeparationDepth = 2.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(0, 0), new Vector2(0, 0), new Vector2(-5, -2.5)))
        }).SetName($"16_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        // Two contact points without clipping
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(-2, 5), new Vector2(5, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 5), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(-1, 0),
            ExpectedSeparationDepth = 0.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(0.5, 7.5), new Vector2(2.5, 2.5), new Vector2(-4.5, 2.5)),
                new ContactPoint2D(new Vector2(0.5, 2.5), new Vector2(2.5, -2.5), new Vector2(-4.5, -2.5)))
        }).SetName($"17_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(5, 12), new Vector2(5, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 5), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(0, 1),
            ExpectedSeparationDepth = 0.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(2.5, 9.5), new Vector2(-2.5, -2.5), new Vector2(-2.5, 4.5)),
                new ContactPoint2D(new Vector2(7.5, 9.5), new Vector2(2.5, -2.5), new Vector2(2.5, 4.5)))
        }).SetName($"18_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(12, 5), new Vector2(5, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 5), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(1, 0),
            ExpectedSeparationDepth = 0.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(9.5, 7.5), new Vector2(-2.5, 2.5), new Vector2(4.5, 2.5)),
                new ContactPoint2D(new Vector2(9.5, 2.5), new Vector2(-2.5, -2.5), new Vector2(4.5, -2.5)))
        }).SetName($"19_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(5, -2), new Vector2(5, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 5), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(0, -1),
            ExpectedSeparationDepth = 0.5,
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
            ExpectedSeparationDepth = 0.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(0.5, 3.5), new Vector2(2.5, 2.5), new Vector2(-4.5, -1.5)),
                new ContactPoint2D(new Vector2(0.5, 0), new Vector2(2.5, -1), new Vector2(-4.5, -5)))
        }).SetName($"21_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(-2, 9), new Vector2(5, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 5), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(-1, 0),
            ExpectedSeparationDepth = 0.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(0.5, 10), new Vector2(2.5, 1), new Vector2(-4.5, 5)),
                new ContactPoint2D(new Vector2(0.5, 6.5), new Vector2(2.5, -2.5), new Vector2(-4.5, 1.5)))
        }).SetName($"22_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(1, 12), new Vector2(5, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 5), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(0, 1),
            ExpectedSeparationDepth = 0.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(0, 9.5), new Vector2(-1, -2.5), new Vector2(-5, 4.5)),
                new ContactPoint2D(new Vector2(3.5, 9.5), new Vector2(2.5, -2.5), new Vector2(-1.5, 4.5)))
        }).SetName($"23_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(9, 12), new Vector2(5, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 5), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(0, 1),
            ExpectedSeparationDepth = 0.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(6.5, 9.5), new Vector2(-2.5, -2.5), new Vector2(1.5, 4.5)),
                new ContactPoint2D(new Vector2(10, 9.5), new Vector2(1, -2.5), new Vector2(5, 4.5)))
        }).SetName($"24_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(12, 9), new Vector2(5, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 5), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(1, 0),
            ExpectedSeparationDepth = 0.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(9.5, 10), new Vector2(-2.5, 1), new Vector2(4.5, 5)),
                new ContactPoint2D(new Vector2(9.5, 6.5), new Vector2(-2.5, -2.5), new Vector2(4.5, 1.5)))
        }).SetName($"25_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(12, 1), new Vector2(5, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 5), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(1, 0),
            ExpectedSeparationDepth = 0.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(9.5, 3.5), new Vector2(-2.5, 2.5), new Vector2(4.5, -1.5)),
                new ContactPoint2D(new Vector2(9.5, 0), new Vector2(-2.5, -1), new Vector2(4.5, -5)))
        }).SetName($"26_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(9, -2), new Vector2(5, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 5), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(0, -1),
            ExpectedSeparationDepth = 0.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(6.5, 0.5), new Vector2(-2.5, 2.5), new Vector2(1.5, -4.5)),
                new ContactPoint2D(new Vector2(10, 0.5), new Vector2(1, 2.5), new Vector2(5, -4.5)))
        }).SetName($"27_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(1, -2), new Vector2(5, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 5), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(0, -1),
            ExpectedSeparationDepth = 0.5,
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
            ExpectedSeparationDepth = 1,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(1, 10), new Vector2(10, 5), new Vector2(-4, 5)),
                new ContactPoint2D(new Vector2(1, 0), new Vector2(10, -5), new Vector2(-4, -5)))
        }).SetName($"29_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(5, 19), new Vector2(20, 20)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 5), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(0, 1),
            ExpectedSeparationDepth = 1,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(0, 9), new Vector2(-5, -10), new Vector2(-5, 4)),
                new ContactPoint2D(new Vector2(10, 9), new Vector2(5, -10), new Vector2(5, 4)))
        }).SetName($"30_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(19, 5), new Vector2(20, 20)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 5), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(1, 0),
            ExpectedSeparationDepth = 1,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(9, 10), new Vector2(-10, 5), new Vector2(4, 5)),
                new ContactPoint2D(new Vector2(9, 0), new Vector2(-10, -5), new Vector2(4, -5)))
        }).SetName($"31_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}"),
        new TestCaseData(new RectangleAndRectangleTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(5, -9), new Vector2(20, 20)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 5), new Vector2(10, 10)),
            ExpectedCollisionNormal = new Vector2(0, -1),
            ExpectedSeparationDepth = 1,
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
            ExpectedSeparationDepth = 3.169872,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(3.169872, 1.830127), new Vector2(-5, 5), new Vector2(-3.169873, -1.830127)),
                new ContactPoint2D(new Vector2(2.113248, 0), new Vector2(-5, 2.886751), new Vector2(-5, -2.886751)))
        }).SetName($"33_{nameof(RectangleKinematicBody_And_RectangleStaticBody)}")
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

    public sealed class CircleAndCircleTestCase
    {
        public Circle Circle1 { get; init; }
        public Circle Circle2 { get; init; }

        public double Rotation1 { get; init; }
        public double Rotation2 { get; init; }

        public Vector2 ExpectedCollisionNormal { get; init; }
        public double ExpectedSeparationDepth { get; init; }
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
            ExpectedSeparationDepth = 1,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(0.5, 5), new Vector2(4.5, 0), new Vector2(-4.5, 0))
        }).SetName($"01_{nameof(CircleKinematicBody_And_CircleStaticBody)}"),
        new TestCaseData(new CircleAndCircleTestCase
        {
            Circle1 = new Circle(new Vector2(5, 14), 5),
            Circle2 = new Circle(new Vector2(5, 5), 5),
            ExpectedCollisionNormal = new Vector2(0, 1),
            ExpectedSeparationDepth = 1,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(5, 9.5), new Vector2(0, -4.5), new Vector2(0, 4.5))
        }).SetName($"02_{nameof(CircleKinematicBody_And_CircleStaticBody)}"),
        new TestCaseData(new CircleAndCircleTestCase
        {
            Circle1 = new Circle(new Vector2(14, 5), 5),
            Circle2 = new Circle(new Vector2(5, 5), 5),
            ExpectedCollisionNormal = new Vector2(1, 0),
            ExpectedSeparationDepth = 1,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(9.5, 5), new Vector2(-4.5, 0), new Vector2(4.5, 0))
        }).SetName($"03_{nameof(CircleKinematicBody_And_CircleStaticBody)}"),
        new TestCaseData(new CircleAndCircleTestCase
        {
            Circle1 = new Circle(new Vector2(5, -4), 5),
            Circle2 = new Circle(new Vector2(5, 5), 5),
            ExpectedCollisionNormal = new Vector2(0, -1),
            ExpectedSeparationDepth = 1,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(5, 0.5), new Vector2(0, 4.5), new Vector2(0, -4.5))
        }).SetName($"04_{nameof(CircleKinematicBody_And_CircleStaticBody)}"),
        // Diagonal overlap
        new TestCaseData(new CircleAndCircleTestCase
        {
            Circle1 = new Circle(new Vector2(0, 10), 5),
            Circle2 = new Circle(new Vector2(5, 5), 5),
            ExpectedCollisionNormal = new Vector2(-0.707106, 0.707106),
            ExpectedSeparationDepth = 2.928932,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(2.5, 7.5), new Vector2(2.5, -2.5), new Vector2(-2.5, 2.5))
        }).SetName($"05_{nameof(CircleKinematicBody_And_CircleStaticBody)}"),
        new TestCaseData(new CircleAndCircleTestCase
        {
            Circle1 = new Circle(new Vector2(10, 10), 5),
            Circle2 = new Circle(new Vector2(5, 5), 5),
            ExpectedCollisionNormal = new Vector2(0.707106, 0.707106),
            ExpectedSeparationDepth = 2.928932,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(7.5, 7.5), new Vector2(-2.5, -2.5), new Vector2(2.5, 2.5))
        }).SetName($"06_{nameof(CircleKinematicBody_And_CircleStaticBody)}"),
        new TestCaseData(new CircleAndCircleTestCase
        {
            Circle1 = new Circle(new Vector2(10, 0), 5),
            Circle2 = new Circle(new Vector2(5, 5), 5),
            ExpectedCollisionNormal = new Vector2(0.707106, -0.707106),
            ExpectedSeparationDepth = 2.928932,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(7.5, 2.5), new Vector2(-2.5, 2.5), new Vector2(2.5, -2.5))
        }).SetName($"07_{nameof(CircleKinematicBody_And_CircleStaticBody)}"),
        new TestCaseData(new CircleAndCircleTestCase
        {
            Circle1 = new Circle(new Vector2(0, 0), 5),
            Circle2 = new Circle(new Vector2(5, 5), 5),
            ExpectedCollisionNormal = new Vector2(-0.707106, -0.707106),
            ExpectedSeparationDepth = 2.928932,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(2.5, 2.5), new Vector2(2.5, 2.5), new Vector2(-2.5, -2.5))
        }).SetName($"08_{nameof(CircleKinematicBody_And_CircleStaticBody)}"),
        // Touching
        new TestCaseData(new CircleAndCircleTestCase
        {
            Circle1 = new Circle(new Vector2(-5, 5), 5),
            Circle2 = new Circle(new Vector2(5, 5), 5),
            ExpectedCollisionNormal = new Vector2(-1, 0),
            ExpectedSeparationDepth = 0,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(0, 5), new Vector2(5, 0), new Vector2(-5, 0))
        }).SetName($"09_{nameof(CircleKinematicBody_And_CircleStaticBody)}"),
        new TestCaseData(new CircleAndCircleTestCase
        {
            Circle1 = new Circle(new Vector2(5, 15), 5),
            Circle2 = new Circle(new Vector2(5, 5), 5),
            ExpectedCollisionNormal = new Vector2(0, 1),
            ExpectedSeparationDepth = 0,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(5, 10), new Vector2(0, -5), new Vector2(0, 5))
        }).SetName($"10_{nameof(CircleKinematicBody_And_CircleStaticBody)}"),
        new TestCaseData(new CircleAndCircleTestCase
        {
            Circle1 = new Circle(new Vector2(15, 5), 5),
            Circle2 = new Circle(new Vector2(5, 5), 5),
            ExpectedCollisionNormal = new Vector2(1, 0),
            ExpectedSeparationDepth = 0,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(10, 5), new Vector2(-5, 0), new Vector2(5, 0))
        }).SetName($"11_{nameof(CircleKinematicBody_And_CircleStaticBody)}"),
        new TestCaseData(new CircleAndCircleTestCase
        {
            Circle1 = new Circle(new Vector2(5, -5), 5),
            Circle2 = new Circle(new Vector2(5, 5), 5),
            ExpectedCollisionNormal = new Vector2(0, -1),
            ExpectedSeparationDepth = 0,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(5, 0), new Vector2(0, 5), new Vector2(0, -5))
        }).SetName($"12_{nameof(CircleKinematicBody_And_CircleStaticBody)}"),
        // Different sizes
        new TestCaseData(new CircleAndCircleTestCase
        {
            Circle1 = new Circle(new Vector2(-5, 5), 10),
            Circle2 = new Circle(new Vector2(5, 5), 5),
            ExpectedCollisionNormal = new Vector2(-1, 0),
            ExpectedSeparationDepth = 5,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(2.5, 5), new Vector2(7.5, 0), new Vector2(-2.5, 0))
        }).SetName($"13_{nameof(CircleKinematicBody_And_CircleStaticBody)}"),
        new TestCaseData(new CircleAndCircleTestCase
        {
            Circle1 = new Circle(new Vector2(5, 15), 10),
            Circle2 = new Circle(new Vector2(5, 5), 5),
            ExpectedCollisionNormal = new Vector2(0, 1),
            ExpectedSeparationDepth = 5,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(5, 7.5), new Vector2(0, -7.5), new Vector2(0, 2.5))
        }).SetName($"14_{nameof(CircleKinematicBody_And_CircleStaticBody)}"),
        new TestCaseData(new CircleAndCircleTestCase
        {
            Circle1 = new Circle(new Vector2(15, 5), 10),
            Circle2 = new Circle(new Vector2(5, 5), 5),
            ExpectedCollisionNormal = new Vector2(1, 0),
            ExpectedSeparationDepth = 5,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(7.5, 5), new Vector2(-7.5, 0), new Vector2(2.5, 0))
        }).SetName($"15_{nameof(CircleKinematicBody_And_CircleStaticBody)}"),
        new TestCaseData(new CircleAndCircleTestCase
        {
            Circle1 = new Circle(new Vector2(5, -5), 10),
            Circle2 = new Circle(new Vector2(5, 5), 5),
            ExpectedCollisionNormal = new Vector2(0, -1),
            ExpectedSeparationDepth = 5,
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
            ExpectedSeparationDepth = 2.928932,
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
        Assert.That(kinematicBodyCollider.Contacts, Has.Count.EqualTo(1));
        Assert.That(kinematicBodyCollider.Contacts[0].ThisCollider, Is.EqualTo(kinematicBodyCollider));
        Assert.That(kinematicBodyCollider.Contacts[0].OtherCollider, Is.EqualTo(staticBodyCollider));
        Assert.That(kinematicBodyCollider.Contacts[0].CollisionNormal, Is.EqualTo(testCase.ExpectedCollisionNormal).Using(Vector2Comparer));
        Assert.That(kinematicBodyCollider.Contacts[0].SeparationDepth, Is.EqualTo(testCase.ExpectedSeparationDepth));
        Assert.That(kinematicBodyCollider.Contacts[0].ContactPoints.Count, Is.EqualTo(1));
        Assert.That(kinematicBodyCollider.Contacts[0].ContactPoints[0],
            Is.EqualTo(testCase.ExpectedContactPoint).Using<ContactPoint2D, ContactPoint2D>(ContactPoint2DComparison));

        Assert.That(staticBodyCollider.IsColliding, Is.True);
        Assert.That(staticBodyCollider.Contacts, Has.Count.EqualTo(1));
        Assert.That(staticBodyCollider.Contacts[0].ThisCollider, Is.EqualTo(staticBodyCollider));
        Assert.That(staticBodyCollider.Contacts[0].OtherCollider, Is.EqualTo(kinematicBodyCollider));
        Assert.That(staticBodyCollider.Contacts[0].CollisionNormal, Is.EqualTo(-testCase.ExpectedCollisionNormal).Using(Vector2Comparer));
        Assert.That(staticBodyCollider.Contacts[0].SeparationDepth, Is.EqualTo(testCase.ExpectedSeparationDepth));

        var cp = testCase.ExpectedContactPoint;
        var reflectedContactPoint = new ContactPoint2D(cp.WorldPosition, cp.OtherLocalPosition, cp.ThisLocalPosition);
        Assert.That(staticBodyCollider.Contacts[0].ContactPoints.Count, Is.EqualTo(1));
        Assert.That(staticBodyCollider.Contacts[0].ContactPoints[0],
            Is.EqualTo(reflectedContactPoint).Using<ContactPoint2D, ContactPoint2D>(ContactPoint2DComparison));
    }

    public sealed class RectangleAndCircleTestCase
    {
        public AxisAlignedRectangle Rectangle { get; init; }
        public Circle Circle { get; init; }

        public double RectangleRotation { get; init; }
        public double CircleRotation { get; init; }

        public Vector2 ExpectedCollisionNormal { get; init; }
        public double ExpectedSeparationDepth { get; init; }
        public ContactPoint2D ExpectedContactPoint { get; init; }
    }

    public static IEnumerable<TestCaseData> RectangleAndCircleTestCases => new[]
    {
        // Axis aligned edge overlap
        new TestCaseData(new RectangleAndCircleTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(-4, 5), new Vector2(10, 10)),
            Circle = new Circle(new Vector2(5, 5), 5),
            ExpectedCollisionNormal = new Vector2(-1, 0),
            ExpectedSeparationDepth = 1,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(0.5, 5), new Vector2(4.5, 0), new Vector2(-4.5, 0))
        }).SetName($"01_{nameof(RectangleKinematicBody_And_CircleStaticBody)}"),
        new TestCaseData(new RectangleAndCircleTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(5, 14), new Vector2(10, 10)),
            Circle = new Circle(new Vector2(5, 5), 5),
            ExpectedCollisionNormal = new Vector2(0, 1),
            ExpectedSeparationDepth = 1,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(5, 9.5), new Vector2(0, -4.5), new Vector2(0, 4.5))
        }).SetName($"02_{nameof(RectangleKinematicBody_And_CircleStaticBody)}"),
        new TestCaseData(new RectangleAndCircleTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(14, 5), new Vector2(10, 10)),
            Circle = new Circle(new Vector2(5, 5), 5),
            ExpectedCollisionNormal = new Vector2(1, 0),
            ExpectedSeparationDepth = 1,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(9.5, 5), new Vector2(-4.5, 0), new Vector2(4.5, 0))
        }).SetName($"03_{nameof(RectangleKinematicBody_And_CircleStaticBody)}"),
        new TestCaseData(new RectangleAndCircleTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(5, -4), new Vector2(10, 10)),
            Circle = new Circle(new Vector2(5, 5), 5),
            ExpectedCollisionNormal = new Vector2(0, -1),
            ExpectedSeparationDepth = 1,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(5, 0.5), new Vector2(0, 4.5), new Vector2(0, -4.5))
        }).SetName($"04_{nameof(RectangleKinematicBody_And_CircleStaticBody)}"),
        // Touching
        new TestCaseData(new RectangleAndCircleTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(-5, 5), new Vector2(10, 10)),
            Circle = new Circle(new Vector2(5, 5), 5),
            ExpectedCollisionNormal = new Vector2(-1, 0),
            ExpectedSeparationDepth = 0,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(0, 5), new Vector2(5, 0), new Vector2(-5, 0))
        }).SetName($"05_{nameof(RectangleKinematicBody_And_CircleStaticBody)}"),
        new TestCaseData(new RectangleAndCircleTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(5, 15), new Vector2(10, 10)),
            Circle = new Circle(new Vector2(5, 5), 5),
            ExpectedCollisionNormal = new Vector2(0, 1),
            ExpectedSeparationDepth = 0,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(5, 10), new Vector2(0, -5), new Vector2(0, 5))
        }).SetName($"06_{nameof(RectangleKinematicBody_And_CircleStaticBody)}"),
        new TestCaseData(new RectangleAndCircleTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(15, 5), new Vector2(10, 10)),
            Circle = new Circle(new Vector2(5, 5), 5),
            ExpectedCollisionNormal = new Vector2(1, 0),
            ExpectedSeparationDepth = 0,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(10, 5), new Vector2(-5, 0), new Vector2(5, 0))
        }).SetName($"07_{nameof(RectangleKinematicBody_And_CircleStaticBody)}"),
        new TestCaseData(new RectangleAndCircleTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(5, -5), new Vector2(10, 10)),
            Circle = new Circle(new Vector2(5, 5), 5),
            ExpectedCollisionNormal = new Vector2(0, -1),
            ExpectedSeparationDepth = 0,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(5, 0), new Vector2(0, 5), new Vector2(0, -5))
        }).SetName($"08_{nameof(RectangleKinematicBody_And_CircleStaticBody)}"),
        // Diagonal edge overlap
        new TestCaseData(new RectangleAndCircleTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(0, 10), new Vector2(10, 10)),
            Circle = new Circle(new Vector2(5, 5), 5),
            RectangleRotation = Angle.Deg2Rad(-45),
            ExpectedCollisionNormal = new Vector2(-1, 0),
            ExpectedSeparationDepth = 1,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(0.5, 5), new Vector2(4.5, 0), new Vector2(-4.5, 0))
        }).SetName($"09_{nameof(RectangleKinematicBody_And_CircleStaticBody)}"),
        new TestCaseData(new RectangleAndCircleTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(10, 10), new Vector2(10, 10)),
            Circle = new Circle(new Vector2(5, 5), 5),
            RectangleRotation = Angle.Deg2Rad(-45),
            ExpectedCollisionNormal = new Vector2(-1, 0),
            ExpectedSeparationDepth = 1,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(0.5, 5), new Vector2(4.5, 0), new Vector2(-4.5, 0))
        }).SetName($"10_{nameof(RectangleKinematicBody_And_CircleStaticBody)}"),
        new TestCaseData(new RectangleAndCircleTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(10, 0), new Vector2(10, 10)),
            Circle = new Circle(new Vector2(5, 5), 5),
            RectangleRotation = Angle.Deg2Rad(-45),
            ExpectedCollisionNormal = new Vector2(-1, 0),
            ExpectedSeparationDepth = 1,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(0.5, 5), new Vector2(4.5, 0), new Vector2(-4.5, 0))
        }).SetName($"11_{nameof(RectangleKinematicBody_And_CircleStaticBody)}"),
        new TestCaseData(new RectangleAndCircleTestCase
        {
            Rectangle = new AxisAlignedRectangle(new Vector2(0, 0), new Vector2(10, 10)),
            Circle = new Circle(new Vector2(5, 5), 5),
            RectangleRotation = Angle.Deg2Rad(-45),
            ExpectedCollisionNormal = new Vector2(-1, 0),
            ExpectedSeparationDepth = 1,
            ExpectedContactPoint = new ContactPoint2D(new Vector2(0.5, 5), new Vector2(4.5, 0), new Vector2(-4.5, 0))
        }).SetName($"12_{nameof(RectangleKinematicBody_And_CircleStaticBody)}")
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
        Assert.That(kinematicBodyCollider.Contacts, Has.Count.EqualTo(1));
        Assert.That(kinematicBodyCollider.Contacts[0].ThisCollider, Is.EqualTo(kinematicBodyCollider));
        Assert.That(kinematicBodyCollider.Contacts[0].OtherCollider, Is.EqualTo(staticBodyCollider));
        Assert.That(kinematicBodyCollider.Contacts[0].CollisionNormal, Is.EqualTo(testCase.ExpectedCollisionNormal).Using(Vector2Comparer));
        Assert.That(kinematicBodyCollider.Contacts[0].SeparationDepth, Is.EqualTo(testCase.ExpectedSeparationDepth));
        Assert.That(kinematicBodyCollider.Contacts[0].ContactPoints.Count, Is.EqualTo(1));
        Assert.That(kinematicBodyCollider.Contacts[0].ContactPoints[0],
            Is.EqualTo(testCase.ExpectedContactPoint).Using<ContactPoint2D, ContactPoint2D>(ContactPoint2DComparison));

        Assert.That(staticBodyCollider.IsColliding, Is.True);
        Assert.That(staticBodyCollider.Contacts, Has.Count.EqualTo(1));
        Assert.That(staticBodyCollider.Contacts[0].ThisCollider, Is.EqualTo(staticBodyCollider));
        Assert.That(staticBodyCollider.Contacts[0].OtherCollider, Is.EqualTo(kinematicBodyCollider));
        Assert.That(staticBodyCollider.Contacts[0].CollisionNormal, Is.EqualTo(-testCase.ExpectedCollisionNormal).Using(Vector2Comparer));
        Assert.That(staticBodyCollider.Contacts[0].SeparationDepth, Is.EqualTo(testCase.ExpectedSeparationDepth));

        var cp = testCase.ExpectedContactPoint;
        var reflectedContactPoint = new ContactPoint2D(cp.WorldPosition, cp.OtherLocalPosition, cp.ThisLocalPosition);
        Assert.That(staticBodyCollider.Contacts[0].ContactPoints.Count, Is.EqualTo(1));
        Assert.That(staticBodyCollider.Contacts[0].ContactPoints[0],
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
        Assert.That(kinematicBodyCollider.Contacts, Has.Count.EqualTo(1));
        Assert.That(kinematicBodyCollider.Contacts[0].ThisCollider, Is.EqualTo(kinematicBodyCollider));
        Assert.That(kinematicBodyCollider.Contacts[0].OtherCollider, Is.EqualTo(staticBodyCollider));
        Assert.That(kinematicBodyCollider.Contacts[0].CollisionNormal, Is.EqualTo(-testCase.ExpectedCollisionNormal).Using(Vector2Comparer));
        Assert.That(kinematicBodyCollider.Contacts[0].SeparationDepth, Is.EqualTo(testCase.ExpectedSeparationDepth));
        Assert.That(kinematicBodyCollider.Contacts[0].ContactPoints.Count, Is.EqualTo(1));

        var cp = testCase.ExpectedContactPoint;
        var reflectedContactPoint = new ContactPoint2D(cp.WorldPosition, cp.OtherLocalPosition, cp.ThisLocalPosition);
        Assert.That(kinematicBodyCollider.Contacts[0].ContactPoints[0],
            Is.EqualTo(reflectedContactPoint).Using<ContactPoint2D, ContactPoint2D>(ContactPoint2DComparison));

        Assert.That(staticBodyCollider.IsColliding, Is.True);
        Assert.That(staticBodyCollider.Contacts, Has.Count.EqualTo(1));
        Assert.That(staticBodyCollider.Contacts[0].ThisCollider, Is.EqualTo(staticBodyCollider));
        Assert.That(staticBodyCollider.Contacts[0].OtherCollider, Is.EqualTo(kinematicBodyCollider));
        Assert.That(staticBodyCollider.Contacts[0].CollisionNormal, Is.EqualTo(testCase.ExpectedCollisionNormal).Using(Vector2Comparer));
        Assert.That(staticBodyCollider.Contacts[0].SeparationDepth, Is.EqualTo(testCase.ExpectedSeparationDepth));


        Assert.That(staticBodyCollider.Contacts[0].ContactPoints.Count, Is.EqualTo(1));
        Assert.That(staticBodyCollider.Contacts[0].ContactPoints[0],
            Is.EqualTo(testCase.ExpectedContactPoint).Using<ContactPoint2D, ContactPoint2D>(ContactPoint2DComparison));
    }
}