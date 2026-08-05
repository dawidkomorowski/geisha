using System;
using System.Collections.Generic;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Physics;
using Geisha.Engine.Physics.Components;
using Geisha.Engine.Physics.Systems;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Physics.Systems.PhysicsSystemTests;

// Broad phase narrows down the set of body pairs and query candidates before exact geometry is evaluated. It is an
// internal implementation detail, so the tests below do not observe it directly. Instead they pin the observable
// property it must preserve: results of queries and collision detection depend only on the current geometry of the
// scene, never on the history of how that geometry came to be. A broad phase that fails to keep up with a moving,
// rotating or resized body violates that property by silently omitting the body from results.
[TestFixture]
public class BroadPhaseTests : PhysicsSystemTestsBase
{
    #region Configuration

    [TestCase(0, 0)]
    [TestCase(0, 1)]
    [TestCase(1, 0)]
    [TestCase(-1, -1)]
    [TestCase(-1, 1)]
    [TestCase(1, -1)]
    public void Constructor_ShouldThrowException_GivenInvalidBroadPhaseGridCellSize(double width, double height)
    {
        // Arrange
        var physicsConfiguration = new PhysicsConfiguration
        {
            BroadPhaseGridCellSize = new SizeD(width, height)
        };

        // Act & Assert
        Assert.That(() => GetPhysicsSystem(physicsConfiguration), Throws.ArgumentException);
    }

    [Test]
    public void Constructor_ShouldSetCellSizeOnInternalPhysicsScene_GivenValidBroadPhaseGridCellSize()
    {
        // Arrange
        var physicsConfiguration = new PhysicsConfiguration
        {
            BroadPhaseGridCellSize = new SizeD(10, 20)
        };

        // Act
        var physicsSystem = GetPhysicsSystem(physicsConfiguration);

        // Assert
        Assert.That(physicsSystem.PhysicsScene2D.BroadPhaseGridCellSize, Is.EqualTo(physicsConfiguration.BroadPhaseGridCellSize));
    }

    #endregion

    #region History independence

    // Bodies are placed away from the origin on purpose. A broad-phase proxy is created before the collider of a body
    // is computed, so a proxy that is never updated degenerates into a single point at the origin. Tests targeting the
    // origin would then pass even with a completely broken proxy update.
    private static readonly Vector2 FinalPosition = new(100, 0);

    private const double BodyRadius = 10;

    // Points spanning the whole extent of a circle body of BodyRadius centered at the given position: the center and
    // four points just inside its edges.
    private static IEnumerable<Vector2> PointsAcrossBodyExtent(Vector2 center)
    {
        const double nearEdge = BodyRadius - 0.5;

        yield return center;
        yield return center + new Vector2(nearEdge, 0);
        yield return center + new Vector2(-nearEdge, 0);
        yield return center + new Vector2(0, nearEdge);
        yield return center + new Vector2(0, -nearEdge);
    }

    // Each test case below reaches the same final geometry through a different history. The final results must be
    // identical for all of them, so a single expected value is shared by all test cases. Bodies used with these offsets
    // have a radius of 10, and the distances are chosen to span the whole range of distances a body can travel:
    //   -   0 - the body is created at its final position and never moves; the reference case with no history at all.
    //   -  ±5 - a small move; the broad phase is expected to skip updating its internal state, so this case pins that
    //           skipping the update is safe.
    //   - ±25 - a medium move; the region of space the body occupied and the one it occupies now still overlap. This is
    //           the range where an update is already required but the body has not moved far enough to make that
    //           obvious, so the update condition is easy to get subtly wrong.
    //   - ±60 - a large move; the body is now far away from everything it used to overlap with.
    private static IEnumerable<double> StartOffsets
    {
        get
        {
            yield return 0;
            yield return 5;
            yield return -5;
            yield return 25;
            yield return -25;
            yield return 60;
            yield return -60;
        }
    }

    [Test]
    public void QueryPoint_ShouldReturnStaticCollider_AfterStaticBodyIsMoved()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var startPosition = FinalPosition + new Vector2(60, 0);
        var staticBody = CreateCircleStaticBody(startPosition.X, startPosition.Y, BodyRadius);
        var staticCollider = staticBody.GetComponent<CircleColliderComponent>();
        physicsSystem.SynchronizePhysicsState();

        var colliders = new List<Collider2DComponent>();

        // Act
        staticBody.GetComponent<Transform2DComponent>().Translation = FinalPosition;
        physicsSystem.SynchronizePhysicsState();

        // Assert
        foreach (var pointToQuery in PointsAcrossBodyExtent(FinalPosition))
        {
            Assert.That(physicsSystem.QueryPoint(pointToQuery, colliders), Is.EqualTo(1), $"Collider was not found at {pointToQuery}.");
            Assert.That(colliders, Is.EquivalentTo(new[] { staticCollider }));
        }

        Assert.That(physicsSystem.QueryPoint(startPosition, colliders), Is.Zero);
        Assert.That(colliders, Is.Empty);
    }

    [Test]
    public void QueryPoint_ShouldReturnKinematicCollider_RegardlessOfHowBodyReachedItsPosition([ValueSource(nameof(StartOffsets))] double startOffset)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var kinematicBody = CreateCircleKinematicBody(FinalPosition.X + startOffset, FinalPosition.Y, BodyRadius);
        var kinematicCollider = kinematicBody.GetComponent<CircleColliderComponent>();
        physicsSystem.SynchronizePhysicsState();

        var colliders = new List<Collider2DComponent>();

        // Act
        kinematicBody.GetComponent<Transform2DComponent>().Translation = FinalPosition;
        physicsSystem.SynchronizePhysicsState();

        // Assert
        // The body must be found across its whole extent, not just at its center. A body that moved but whose broad
        // phase state was not updated correctly is often still found at its center while the parts of it that entered
        // newly occupied space are missing, so probing the edges is what makes this test sensitive.
        foreach (var pointToQuery in PointsAcrossBodyExtent(FinalPosition))
        {
            var written = physicsSystem.QueryPoint(pointToQuery, colliders);

            Assert.That(written, Is.EqualTo(1), $"Collider was not found at {pointToQuery}.");
            Assert.That(colliders, Is.EquivalentTo(new[] { kinematicCollider }));
        }
    }

    [TestCase(25)]
    [TestCase(-25)]
    [TestCase(60)]
    [TestCase(-60)]
    public void QueryPoint_ShouldNotReturnKinematicCollider_AtPositionBodyMovedAwayFrom(double startOffset)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var startPosition = new Vector2(FinalPosition.X + startOffset, FinalPosition.Y);
        var kinematicBody = CreateCircleKinematicBody(startPosition.X, startPosition.Y, BodyRadius);
        var kinematicCollider = kinematicBody.GetComponent<CircleColliderComponent>();
        physicsSystem.SynchronizePhysicsState();

        var colliders = new List<Collider2DComponent>();

        // Assume
        Assert.That(physicsSystem.QueryPoint(startPosition, colliders), Is.EqualTo(1));
        Assert.That(colliders, Is.EquivalentTo(new[] { kinematicCollider }));

        // Act
        kinematicBody.GetComponent<Transform2DComponent>().Translation = FinalPosition;
        physicsSystem.SynchronizePhysicsState();

        var written = physicsSystem.QueryPoint(startPosition, colliders);

        // Assert
        Assert.That(written, Is.Zero);
        Assert.That(colliders, Is.Empty);
    }

    // The other body is placed so that it only just overlaps the moving body once it arrives. A barely overlapping pair
    // is what makes this test sensitive: it is detected only when the broad phase tracks the moving body accurately
    // enough at its edges, whereas a deeply overlapping pair is found even by a considerably lagging broad phase.
    //
    // The distance assumes the other body is a rectangle of 10x10 placed at this position, which leaves the two bodies
    // overlapping by a single unit. Using a different shape or size here changes the overlap and can make it deep enough
    // for the test to stop being sensitive - two circles of BodyRadius, for example, would overlap by 6 units.
    private static readonly Vector2 BarelyTouchingPosition = FinalPosition + new Vector2(BodyRadius + 4, 0);

    // Asserts that the two bodies touch exactly once and that they do so barely. The contact count rules out the same
    // pair being reported more than once when the bodies span several cells, while the penetration depth guards the
    // setup itself: it is the shallow overlap that makes these tests sensitive, and nothing else here would notice if a
    // change to the bodies deepened it. See BarelyTouchingPosition for how such a change can come about.
    //
    // Both colliders are checked, because each of them reports its contacts on its own. The two reports are separate
    // results and one of them being correct does not make the other one correct.
    private static void AssertBodiesBarelyTouchOnce(Collider2DComponent collider, Collider2DComponent otherCollider)
    {
        AssertColliderBarelyTouchesOnce(collider, otherCollider);
        AssertColliderBarelyTouchesOnce(otherCollider, collider);
    }

    private static void AssertColliderBarelyTouchesOnce(Collider2DComponent collider, Collider2DComponent otherCollider)
    {
        var contacts = new List<Contact2D>();
        Assert.That(collider.GetContacts(contacts), Is.EqualTo(1), $"Unexpected number of contacts of {collider}.");
        Assert.That(contacts[0].OtherCollider, Is.EqualTo(otherCollider));
        Assert.That(contacts[0].PenetrationDepth, Is.EqualTo(1).Within(Epsilon), "The bodies no longer overlap barely.");
    }

    [Test]
    public void ProcessPhysics_ShouldDetectCollisionWithStaticBody_RegardlessOfHowKinematicBodyReachedItsPosition(
        [ValueSource(nameof(StartOffsets))] double startOffset)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var kinematicBody = CreateCircleKinematicBody(FinalPosition.X + startOffset, FinalPosition.Y, BodyRadius);
        var kinematicCollider = kinematicBody.GetComponent<CircleColliderComponent>();
        var staticCollider = CreateRectangleStaticBody(BarelyTouchingPosition.X, BarelyTouchingPosition.Y, 10, 10)
            .GetComponent<RectangleColliderComponent>();
        physicsSystem.ProcessPhysics();

        // Act
        kinematicBody.GetComponent<Transform2DComponent>().Translation = FinalPosition;
        physicsSystem.ProcessPhysics();

        // Assert
        Assert.That(kinematicCollider.IsColliding, Is.True);
        Assert.That(staticCollider.IsColliding, Is.True);
        AssertBodiesBarelyTouchOnce(kinematicCollider, staticCollider);
    }

    [Test]
    public void ProcessPhysics_ShouldDetectCollisionWithOtherKinematicBody_RegardlessOfHowKinematicBodyReachedItsPosition(
        [ValueSource(nameof(StartOffsets))] double startOffset)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var movingBody = CreateCircleKinematicBody(FinalPosition.X + startOffset, FinalPosition.Y, BodyRadius);
        var movingCollider = movingBody.GetComponent<CircleColliderComponent>();
        var standingCollider = CreateRectangleKinematicBody(BarelyTouchingPosition.X, BarelyTouchingPosition.Y, 10, 10)
            .GetComponent<RectangleColliderComponent>();
        physicsSystem.ProcessPhysics();

        // Act
        movingBody.GetComponent<Transform2DComponent>().Translation = FinalPosition;
        physicsSystem.ProcessPhysics();

        // Assert
        Assert.That(movingCollider.IsColliding, Is.True);
        Assert.That(standingCollider.IsColliding, Is.True);
        AssertBodiesBarelyTouchOnce(movingCollider, standingCollider);
    }

    [TestCase(25)]
    [TestCase(-25)]
    [TestCase(60)]
    [TestCase(-60)]
    public void ProcessPhysics_ShouldNotDetectCollision_WhenKinematicBodyMovedAwayFromStaticBody(double targetOffset)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var kinematicBody = CreateCircleKinematicBody(FinalPosition.X, FinalPosition.Y, BodyRadius);
        var kinematicCollider = kinematicBody.GetComponent<CircleColliderComponent>();
        var staticCollider = CreateRectangleStaticBody(BarelyTouchingPosition.X, BarelyTouchingPosition.Y, 10, 10)
            .GetComponent<RectangleColliderComponent>();
        physicsSystem.ProcessPhysics();

        // Assume
        Assert.That(kinematicCollider.IsColliding, Is.True);
        Assert.That(staticCollider.IsColliding, Is.True);

        // Act
        kinematicBody.GetComponent<Transform2DComponent>().Translation = FinalPosition + new Vector2(0, targetOffset);
        physicsSystem.ProcessPhysics();

        // Assert
        Assert.That(kinematicCollider.IsColliding, Is.False);
        Assert.That(staticCollider.IsColliding, Is.False);
    }

    [Test]
    public void ProcessPhysics_ShouldDetectCollision_WhenKinematicBodyTravelsLongDistanceDrivenByLinearVelocity()
    {
        // Arrange
        TimeSystem.FixedDeltaTime.Returns(TimeSpan.FromSeconds(0.1));

        var physicsSystem = GetPhysicsSystem();
        var kinematicBody = CreateRectangleKinematicBody(FinalPosition.X, FinalPosition.Y, 10, 10);
        var kinematicCollider = kinematicBody.GetComponent<RectangleColliderComponent>();
        kinematicBody.GetComponent<KinematicRigidBody2DComponent>().LinearVelocity = new Vector2(100, 0);
        var staticCollider = CreateRectangleStaticBody(FinalPosition.X + 100, FinalPosition.Y, 10, 10).GetComponent<RectangleColliderComponent>();

        // Act & Assume - the body advances by 10 units per step, so it is still far away from the static body here.
        for (var i = 0; i < 5; i++)
        {
            physicsSystem.ProcessPhysics();
        }

        Assert.That(kinematicBody.GetComponent<Transform2DComponent>().Translation, Is.EqualTo(FinalPosition + new Vector2(50, 0)));
        Assert.That(kinematicCollider.IsColliding, Is.False);
        Assert.That(staticCollider.IsColliding, Is.False);

        // Act
        for (var i = 0; i < 5; i++)
        {
            physicsSystem.ProcessPhysics();
        }

        // Assert
        Assert.That(kinematicBody.GetComponent<Transform2DComponent>().Translation, Is.EqualTo(FinalPosition + new Vector2(100, 0)));
        Assert.That(kinematicCollider.IsColliding, Is.True);
        Assert.That(staticCollider.IsColliding, Is.True);
    }

    [Test]
    public void ProcessPhysics_ShouldDetectCollision_WhenKinematicBodyIsRotated()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();

        // A long and thin body has a very different bounding box depending on its rotation, so rotating it in place
        // changes the region of space it occupies without changing its position.
        var kinematicBody = CreateRectangleKinematicBody(FinalPosition.X, FinalPosition.Y, 40, 2);
        var kinematicCollider = kinematicBody.GetComponent<RectangleColliderComponent>();
        var staticCollider = CreateRectangleStaticBody(FinalPosition.X, FinalPosition.Y + 18, 4, 4).GetComponent<RectangleColliderComponent>();
        physicsSystem.ProcessPhysics();

        // Assume
        Assert.That(kinematicCollider.IsColliding, Is.False);
        Assert.That(staticCollider.IsColliding, Is.False);

        // Act
        kinematicBody.GetComponent<Transform2DComponent>().Rotation = Angle.DegreesToRadians(90);
        physicsSystem.ProcessPhysics();

        // Assert
        Assert.That(kinematicCollider.IsColliding, Is.True);
        Assert.That(staticCollider.IsColliding, Is.True);
    }

    [Test]
    public void ProcessPhysics_ShouldDetectCollision_WhenKinematicBodyColliderIsEnlarged()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var kinematicBody = CreateCircleKinematicBody(FinalPosition.X, FinalPosition.Y, 5);
        var kinematicCollider = kinematicBody.GetComponent<CircleColliderComponent>();
        var staticCollider = CreateRectangleStaticBody(FinalPosition.X + 30, FinalPosition.Y, 10, 10).GetComponent<RectangleColliderComponent>();
        physicsSystem.ProcessPhysics();

        // Assume
        Assert.That(kinematicCollider.IsColliding, Is.False);
        Assert.That(staticCollider.IsColliding, Is.False);

        // Act
        kinematicCollider.Radius = 30;
        physicsSystem.ProcessPhysics();

        // Assert
        Assert.That(kinematicCollider.IsColliding, Is.True);
        Assert.That(staticCollider.IsColliding, Is.True);
    }

    #endregion

    #region Cell size invariance

    // The broad phase partitions space into cells and the size of a cell is configurable. It must be a pure performance
    // knob: the same scene has to produce the same results no matter how space is partitioned. Each test below therefore
    // asserts a single expected result for every cell size it runs with.
    //
    // The cell sizes are chosen relative to the size of the bodies used in these tests, which is 20 units across:
    //   -         1x1 - much smaller than a body, so every body and every query covers a large block of cells and the
    //                   broad phase has to combine partial results from all of them.
    //   -         3x7 - also smaller than a body, and not square, so cell width and cell height cannot be swapped for
    //                   one another without changing the results.
    //   -     256x256 - the default, comparable to the size of the whole scene used here.
    //   - 10000x10000 - much larger than the whole scene, so all bodies fall into a single cell and the broad phase
    //                   degenerates into comparing everything with everything. This is the reference case in which
    //                   space is effectively not partitioned at all.
    private static IEnumerable<SizeD> CellSizes
    {
        get
        {
            yield return new SizeD(1, 1);
            yield return new SizeD(3, 7);
            yield return new SizeD(256, 256);
            yield return new SizeD(10000, 10000);
        }
    }

    private PhysicsSystem GetPhysicsSystem(SizeD cellSize) =>
        GetPhysicsSystem(new PhysicsConfiguration
        {
            PenetrationTolerance = 0d,
            BroadPhaseGridCellSize = cellSize
        });

    // A second body placed far enough from FinalPosition not to overlap a body standing there. The gap between the two
    // bodies is wider than the smaller cell sizes, so for those they occupy disjoint sets of cells and neither can be
    // found by looking at the cells of the other. For the larger cell sizes they share a cell instead, which is the case
    // where the broad phase has to reject a candidate it did gather.
    private static readonly Vector2 DistantPosition = FinalPosition + new Vector2(0, 60);

    [Test]
    public void QueryPoint_ShouldReturnCollidersAcrossTheirWholeExtent_RegardlessOfCellSize([ValueSource(nameof(CellSizes))] SizeD cellSize)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem(cellSize);
        var staticCollider = CreateCircleStaticBody(FinalPosition.X, FinalPosition.Y, BodyRadius).GetComponent<CircleColliderComponent>();
        var kinematicCollider = CreateCircleKinematicBody(DistantPosition.X, DistantPosition.Y, BodyRadius).GetComponent<CircleColliderComponent>();
        physicsSystem.SynchronizePhysicsState();

        var colliders = new List<Collider2DComponent>();

        // Act & Assert
        // A body is bigger than a cell for the smaller cell sizes, so points across its extent fall into different cells.
        // All of them must find the body, which they only do when the body is registered in every cell it covers.
        foreach (var pointToQuery in PointsAcrossBodyExtent(FinalPosition))
        {
            Assert.That(physicsSystem.QueryPoint(pointToQuery, colliders), Is.EqualTo(1), $"Static collider was not found at {pointToQuery}.");
            Assert.That(colliders, Is.EquivalentTo(new[] { staticCollider }));
        }

        foreach (var pointToQuery in PointsAcrossBodyExtent(DistantPosition))
        {
            Assert.That(physicsSystem.QueryPoint(pointToQuery, colliders), Is.EqualTo(1), $"Kinematic collider was not found at {pointToQuery}.");
            Assert.That(colliders, Is.EquivalentTo(new[] { kinematicCollider }));
        }

        var pointBetweenBodies = DistantPosition.Midpoint(FinalPosition);
        Assert.That(physicsSystem.QueryPoint(pointBetweenBodies, colliders), Is.Zero);
        Assert.That(colliders, Is.Empty);
    }

    [Test]
    public void QueryBounds_ShouldReturnEachOverlappedColliderExactlyOnce_RegardlessOfCellSize([ValueSource(nameof(CellSizes))] SizeD cellSize)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem(cellSize);
        var staticCollider = CreateCircleStaticBody(FinalPosition.X, FinalPosition.Y, BodyRadius).GetComponent<CircleColliderComponent>();
        var kinematicCollider = CreateCircleKinematicBody(DistantPosition.X, DistantPosition.Y, BodyRadius).GetComponent<CircleColliderComponent>();
        physicsSystem.SynchronizePhysicsState();

        // Bounds reaching from the center of one body to the center of the other and extending a little past both. The box
        // is narrower than the bodies, so it overlaps each of them only partially.
        const double margin = 5;
        var boundsToQuery = new AABB2D(
            new Vector2(FinalPosition.X - margin, FinalPosition.Y - margin),
            new Vector2(DistantPosition.X + margin, DistantPosition.Y + margin));

        var colliders = new List<Collider2DComponent>();

        // Act
        // Both the bounds and the bodies cover many cells for the smaller cell sizes, so a body is reachable through
        // several cells at once. Each of them must still be reported exactly once.
        var written = physicsSystem.QueryBounds(boundsToQuery, colliders);

        // Assert
        Assert.That(written, Is.EqualTo(2));
        Assert.That(colliders, Is.EquivalentTo(new Collider2DComponent[] { staticCollider, kinematicCollider }));
    }

    [Test]
    public void ProcessPhysics_ShouldDetectSingleCollisionBetweenKinematicAndStaticBody_RegardlessOfCellSize([ValueSource(nameof(CellSizes))] SizeD cellSize)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem(cellSize);
        var kinematicCollider = CreateCircleKinematicBody(FinalPosition.X, FinalPosition.Y, BodyRadius).GetComponent<CircleColliderComponent>();
        var staticCollider = CreateRectangleStaticBody(BarelyTouchingPosition.X, BarelyTouchingPosition.Y, 10, 10)
            .GetComponent<RectangleColliderComponent>();

        // Act
        physicsSystem.ProcessPhysics();

        // Assert
        Assert.That(kinematicCollider.IsColliding, Is.True);
        Assert.That(staticCollider.IsColliding, Is.True);

        // The two bodies share many cells for the smaller cell sizes. Reporting the collision once per shared cell would
        // produce duplicated contacts, so the exact contact count is what pins that down.
        AssertBodiesBarelyTouchOnce(kinematicCollider, staticCollider);
    }

    [Test]
    public void ProcessPhysics_ShouldDetectSingleCollisionBetweenTwoKinematicBodies_RegardlessOfCellSize([ValueSource(nameof(CellSizes))] SizeD cellSize)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem(cellSize);
        var collider1 = CreateCircleKinematicBody(FinalPosition.X, FinalPosition.Y, BodyRadius).GetComponent<CircleColliderComponent>();
        var collider2 = CreateRectangleKinematicBody(BarelyTouchingPosition.X, BarelyTouchingPosition.Y, 10, 10)
            .GetComponent<RectangleColliderComponent>();

        // Act
        physicsSystem.ProcessPhysics();

        // Assert
        Assert.That(collider1.IsColliding, Is.True);
        Assert.That(collider2.IsColliding, Is.True);
        AssertBodiesBarelyTouchOnce(collider1, collider2);
    }

    [Test]
    public void ProcessPhysics_ShouldDetectCollision_WhenKinematicBodyTravelsAcrossManyCells([ValueSource(nameof(CellSizes))] SizeD cellSize)
    {
        // Arrange
        TimeSystem.FixedDeltaTime.Returns(TimeSpan.FromSeconds(0.1));

        var physicsSystem = GetPhysicsSystem(cellSize);
        var kinematicBody = CreateRectangleKinematicBody(FinalPosition.X, FinalPosition.Y, 10, 10);
        var kinematicCollider = kinematicBody.GetComponent<RectangleColliderComponent>();
        kinematicBody.GetComponent<KinematicRigidBody2DComponent>().LinearVelocity = new Vector2(100, 0);
        var staticCollider = CreateRectangleStaticBody(FinalPosition.X + 100, FinalPosition.Y, 10, 10).GetComponent<RectangleColliderComponent>();

        // Act & Assume - the body advances by 10 units per step, so it is still far away from the static body here.
        for (var i = 0; i < 5; i++)
        {
            physicsSystem.ProcessPhysics();
        }

        Assert.That(kinematicCollider.IsColliding, Is.False);
        Assert.That(staticCollider.IsColliding, Is.False);

        // Act
        for (var i = 0; i < 5; i++)
        {
            physicsSystem.ProcessPhysics();
        }

        // Assert
        Assert.That(kinematicBody.GetComponent<Transform2DComponent>().Translation, Is.EqualTo(FinalPosition + new Vector2(100, 0)));
        Assert.That(kinematicCollider.IsColliding, Is.True);
        Assert.That(staticCollider.IsColliding, Is.True);
    }

    #endregion
}