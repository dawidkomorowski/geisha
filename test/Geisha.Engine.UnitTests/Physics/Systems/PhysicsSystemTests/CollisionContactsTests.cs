using Geisha.Engine.Core.Collections;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Physics.Components;
using Geisha.Engine.Physics;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Geisha.Engine.UnitTests.Physics.Systems.PhysicsSystemTests;

[TestFixture]
public class CollisionContactsTests : PhysicsSystemTestsBase
{
    public sealed class ContactsTestCase
    {
        public AxisAlignedRectangle Rectangle1 { get; init; }
        public AxisAlignedRectangle Rectangle2 { get; init; }

        public Vector2 ExpectedCollisionNormal { get; init; }
        public double ExpectedSeparationDepth { get; init; }
        public ReadOnlyFixedList2<ContactPoint2D> ExpectedContactPoints { get; init; }
    }

    public static IEnumerable<TestCaseData> ContactsTestCases => new[]
    {
        // Rectangles deeply interpenetrating
        new TestCaseData(new ContactsTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(-1, 2.5), new Vector2(10, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 2.5), new Vector2(10, 5)),
            ExpectedCollisionNormal = new Vector2(-1, 0),
            ExpectedSeparationDepth = 4,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(4, 5), new Vector2(5, 2.5), new Vector2(-1, 2.5)),
                new ContactPoint2D(new Vector2(4, 0), new Vector2(5, -2.5), new Vector2(-1, -2.5)))
        }).SetName($"1_{nameof(RectangleKinematicBody_vs_RectangleStaticBody)}"),
        new TestCaseData(new ContactsTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(5, 5), new Vector2(10, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 2.5), new Vector2(10, 5)),
            ExpectedCollisionNormal = new Vector2(0, 1),
            ExpectedSeparationDepth = 2.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(0, 2.5), new Vector2(-5, -2.5), new Vector2(-5, 0)),
                new ContactPoint2D(new Vector2(10, 2.5), new Vector2(5, -2.5), new Vector2(5, 0)))
        }).SetName($"2_{nameof(RectangleKinematicBody_vs_RectangleStaticBody)}"),
        new TestCaseData(new ContactsTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(11, 2.5), new Vector2(10, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 2.5), new Vector2(10, 5)),
            ExpectedCollisionNormal = new Vector2(1, 0),
            ExpectedSeparationDepth = 4,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(6, 5), new Vector2(-5, 2.5), new Vector2(1, 2.5)),
                new ContactPoint2D(new Vector2(6, 0), new Vector2(-5, -2.5), new Vector2(1, -2.5)))
        }).SetName($"3_{nameof(RectangleKinematicBody_vs_RectangleStaticBody)}"),
        new TestCaseData(new ContactsTestCase
        {
            Rectangle1 = new AxisAlignedRectangle(new Vector2(5, 0), new Vector2(10, 5)),
            Rectangle2 = new AxisAlignedRectangle(new Vector2(5, 2.5), new Vector2(10, 5)),
            ExpectedCollisionNormal = new Vector2(0, -1),
            ExpectedSeparationDepth = 2.5,
            ExpectedContactPoints = new ReadOnlyFixedList2<ContactPoint2D>(
                new ContactPoint2D(new Vector2(0, 2.5), new Vector2(-5, 2.5), new Vector2(-5, 0)),
                new ContactPoint2D(new Vector2(10, 2.5), new Vector2(5, 2.5), new Vector2(5, 0)))
        }).SetName($"4_{nameof(RectangleKinematicBody_vs_RectangleStaticBody)}")
    };

    [TestCaseSource(nameof(ContactsTestCases))]
    public void RectangleKinematicBody_vs_RectangleStaticBody(ContactsTestCase testCase)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var kinematicBody = CreateRectangleKinematicBody(testCase.Rectangle1);
        var staticBody = CreateRectangleStaticBody(testCase.Rectangle2);

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
        Assert.That(kinematicBodyCollider.Contacts[0].CollisionNormal, Is.EqualTo(testCase.ExpectedCollisionNormal));
        Assert.That(kinematicBodyCollider.Contacts[0].SeparationDepth, Is.EqualTo(testCase.ExpectedSeparationDepth));
        Assert.That(kinematicBodyCollider.Contacts[0].ContactPoints.ToArray(), Is.EquivalentTo(testCase.ExpectedContactPoints.ToArray()));

        Assert.That(staticBodyCollider.IsColliding, Is.True);
        Assert.That(staticBodyCollider.Contacts, Has.Count.EqualTo(1));
        Assert.That(staticBodyCollider.Contacts[0].ThisCollider, Is.EqualTo(staticBodyCollider));
        Assert.That(staticBodyCollider.Contacts[0].OtherCollider, Is.EqualTo(kinematicBodyCollider));
        Assert.That(staticBodyCollider.Contacts[0].CollisionNormal, Is.EqualTo(-testCase.ExpectedCollisionNormal));
        Assert.That(staticBodyCollider.Contacts[0].SeparationDepth, Is.EqualTo(testCase.ExpectedSeparationDepth));

        var reflectedExpectedContactPoints = testCase.ExpectedContactPoints.ToArray()
            .Select(cp => new ContactPoint2D(cp.WorldPosition, cp.OtherLocalPosition, cp.ThisLocalPosition)).ToArray();
        Assert.That(staticBodyCollider.Contacts[0].ContactPoints.ToArray(), Is.EquivalentTo(reflectedExpectedContactPoints));
    }
}