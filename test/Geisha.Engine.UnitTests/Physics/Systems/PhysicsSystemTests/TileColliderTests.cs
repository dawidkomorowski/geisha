using Geisha.Engine.Core;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Physics;
using Geisha.Engine.Physics.Components;
using Geisha.Engine.Physics.PhysicsEngine2D;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.UnitTests.Physics.Systems.PhysicsSystemTests;

public class TileColliderTests : PhysicsSystemTestsBase
{
    [TestCase(0, 0)]
    [TestCase(0, 1)]
    [TestCase(1, 0)]
    [TestCase(-1, -1)]
    [TestCase(1, -1)]
    [TestCase(-1, 1)]
    public void Constructor_ShouldThrowException_GivenInvalidTileSize(double width, double height)
    {
        // Arrange
        var physicsConfiguration = new PhysicsConfiguration
        {
            TileSize = new SizeD(width, height)
        };

        // Act & Assert
        Assert.That(() => GetPhysicsSystem(physicsConfiguration), Throws.ArgumentException);
    }

    [Test]
    public void Constructor_ShouldNotThrowException_GivenValidTileSize()
    {
        // Arrange
        var physicsConfiguration = new PhysicsConfiguration
        {
            TileSize = new SizeD(1, 1)
        };

        // Act & Assert
        Assert.That(() => GetPhysicsSystem(physicsConfiguration), Throws.Nothing);
    }

    #region Collision Normal Filter

    public static IEnumerable<SizeD> TileSizes
    {
        get
        {
            yield return new SizeD(1, 1);
            yield return new SizeD(2.5, 3.5);
        }
    }

    public static IEnumerable<Vector2> TilePositions
    {
        get
        {
            yield return Vector2.Zero;
            yield return new Vector2(2, 3);
        }
    }

    public sealed record TileLayout
    {
        public Flag Layout { get; init; }
        internal CollisionNormalFilter Expected { get; init; }

        // ExpectedFilter allows to have Expected in the name of the test case, as Expected property is inaccessible to test runner.
        // ReSharper disable once UnusedMember.Global
        public object ExpectedFilter => Expected;

        [Flags]
        public enum Flag
        {
            None = 0b00000000,
            TopLeft = 0b10000000,
            Top = 0b01000000,
            TopRight = 0b00100000,
            Left = 0b00010000,
            Right = 0b00001000,
            BottomLeft = 0b00000100,
            Bottom = 0b00000010,
            BottomRight = 0b00000001,
            All = TopLeft | Top | TopRight | Left | Right | BottomLeft | Bottom | BottomRight
        }
    }

    public static IEnumerable<TileLayout> TileLayouts
    {
        get
        {
            yield return new TileLayout
            {
                Layout = TileLayout.Flag.None,
                Expected = CollisionNormalFilter.All,
            };
            yield return new TileLayout
            {
                Layout = TileLayout.Flag.TopLeft,
                Expected = CollisionNormalFilter.All
            };
            yield return new TileLayout
            {
                Layout = TileLayout.Flag.Top,
                Expected = CollisionNormalFilter.NegativeHorizontal | CollisionNormalFilter.PositiveHorizontal |
                           CollisionNormalFilter.NegativeVertical
            };
            yield return new TileLayout
            {
                Layout = TileLayout.Flag.TopRight,
                Expected = CollisionNormalFilter.All
            };
            yield return new TileLayout
            {
                Layout = TileLayout.Flag.Left,
                Expected = CollisionNormalFilter.PositiveHorizontal | CollisionNormalFilter.NegativeVertical |
                           CollisionNormalFilter.PositiveVertical
            };
            yield return new TileLayout
            {
                Layout = TileLayout.Flag.Right,
                Expected = CollisionNormalFilter.NegativeHorizontal | CollisionNormalFilter.NegativeVertical |
                           CollisionNormalFilter.PositiveVertical
            };
            yield return new TileLayout
            {
                Layout = TileLayout.Flag.BottomLeft,
                Expected = CollisionNormalFilter.All
            };
            yield return new TileLayout
            {
                Layout = TileLayout.Flag.Bottom,
                Expected = CollisionNormalFilter.NegativeHorizontal | CollisionNormalFilter.PositiveHorizontal |
                           CollisionNormalFilter.PositiveVertical
            };
            yield return new TileLayout
            {
                Layout = TileLayout.Flag.BottomRight,
                Expected = CollisionNormalFilter.All
            };
            yield return new TileLayout
            {
                Layout = TileLayout.Flag.All,
                Expected = CollisionNormalFilter.None
            };
            yield return new TileLayout
            {
                Layout = TileLayout.Flag.Left | TileLayout.Flag.Right,
                Expected = CollisionNormalFilter.NegativeVertical | CollisionNormalFilter.PositiveVertical
            };
            yield return new TileLayout
            {
                Layout = TileLayout.Flag.Top | TileLayout.Flag.Bottom,
                Expected = CollisionNormalFilter.NegativeHorizontal | CollisionNormalFilter.PositiveHorizontal
            };
            yield return new TileLayout
            {
                Layout = TileLayout.Flag.Left | TileLayout.Flag.Top,
                Expected = CollisionNormalFilter.PositiveHorizontal | CollisionNormalFilter.NegativeVertical
            };
            yield return new TileLayout
            {
                Layout = TileLayout.Flag.Right | TileLayout.Flag.Bottom,
                Expected = CollisionNormalFilter.NegativeHorizontal | CollisionNormalFilter.PositiveVertical
            };
            yield return new TileLayout
            {
                Layout = TileLayout.Flag.Left | TileLayout.Flag.Right | TileLayout.Flag.Top | TileLayout.Flag.Bottom,
                Expected = CollisionNormalFilter.None
            };
            yield return new TileLayout
            {
                Layout = TileLayout.Flag.TopLeft | TileLayout.Flag.TopRight | TileLayout.Flag.BottomLeft | TileLayout.Flag.BottomRight,
                Expected = CollisionNormalFilter.All
            };
        }
    }

    [Test]
    public void TileBody_CollisionNormalFilterIsUpdated_WhenTileBodyPositionIsChanged(
        [ValueSource(nameof(TileSizes))] SizeD tileSize,
        [ValueSource(nameof(TilePositions))] Vector2 tilePosition,
        [ValueSource(nameof(TileLayouts))] TileLayout tileLayout)
    {
        // Arrange
        var physicsConfiguration = new PhysicsConfiguration
        {
            TileSize = tileSize
        };
        var physicsSystem = GetPhysicsSystem(physicsConfiguration);

        var centerTilePosition = new Vector2(tilePosition.X * tileSize.Width, tilePosition.Y * tileSize.Height);

        CreateTileLayout(tileLayout.Layout, tileSize, centerTilePosition);

        var entity = CreateTileStaticBody(centerTilePosition);
        Assert.That(entity.GetComponent<Transform2DComponent>().Translation, Is.EqualTo(centerTilePosition), "Tile is misaligned.");

        // Act
        physicsSystem.ProcessPhysics();

        // Assert
        var body = GetBodyForEntity(physicsSystem, entity);
        Assert.That(body.CollisionNormalFilter, Is.EqualTo(tileLayout.Expected));
    }

    [Test]
    public void TileBody_CollisionNormalFilterIsUpdated_WhenNeighbouringTileBodyAppearsDueToPositionUpdate(
        [ValueSource(nameof(TileSizes))] SizeD tileSize,
        [ValueSource(nameof(TilePositions))] Vector2 tilePosition,
        [ValueSource(nameof(TileLayouts))] TileLayout tileLayout)
    {
        // Arrange
        var physicsConfiguration = new PhysicsConfiguration
        {
            TileSize = tileSize
        };
        var physicsSystem = GetPhysicsSystem(physicsConfiguration);

        var centerTilePosition = new Vector2(tilePosition.X * tileSize.Width, tilePosition.Y * tileSize.Height);

        var entity = CreateTileStaticBody(centerTilePosition);
        Assert.That(entity.GetComponent<Transform2DComponent>().Translation, Is.EqualTo(centerTilePosition), "Tile is misaligned.");

        CreateTileLayout(tileLayout.Layout, tileSize, centerTilePosition);

        // Act
        physicsSystem.ProcessPhysics();

        // Assert
        var body = GetBodyForEntity(physicsSystem, entity);
        Assert.That(body.CollisionNormalFilter, Is.EqualTo(tileLayout.Expected));
    }

    [Test]
    public void TileBody_CollisionNormalFilterIsUpdated_WhenNeighbouringTileBodyDisappearsDueToPositionUpdate(
        [ValueSource(nameof(TileSizes))] SizeD tileSize,
        [ValueSource(nameof(TilePositions))] Vector2 tilePosition,
        [ValueSource(nameof(TileLayouts))] TileLayout tileLayout)
    {
        // Arrange
        var physicsConfiguration = new PhysicsConfiguration
        {
            TileSize = tileSize
        };
        var physicsSystem = GetPhysicsSystem(physicsConfiguration);

        var centerTilePosition = new Vector2(tilePosition.X * tileSize.Width, tilePosition.Y * tileSize.Height);

        var (_, complementEntities) = CreateTileLayout(tileLayout.Layout, tileSize, centerTilePosition, true);

        var entity = CreateTileStaticBody(centerTilePosition);
        Assert.That(entity.GetComponent<Transform2DComponent>().Translation, Is.EqualTo(centerTilePosition), "Tile is misaligned.");

        physicsSystem.ProcessPhysics();

        // Assume
        var body = GetBodyForEntity(physicsSystem, entity);
        Assert.That(body.CollisionNormalFilter, Is.EqualTo(CollisionNormalFilter.None));

        // Act
        foreach (var complementEntity in complementEntities)
        {
            complementEntity.GetComponent<Transform2DComponent>().Translation = new Vector2(-100, -100);
        }

        physicsSystem.ProcessPhysics();

        // Assert
        Assert.That(body.CollisionNormalFilter, Is.EqualTo(tileLayout.Expected));
    }

    [Test]
    public void TileBody_CollisionNormalFilterIsUpdated_WhenNeighbouringTileBodyDisappearsDueToBeingRemoved(
        [ValueSource(nameof(TileSizes))] SizeD tileSize,
        [ValueSource(nameof(TilePositions))] Vector2 tilePosition,
        [ValueSource(nameof(TileLayouts))] TileLayout tileLayout)
    {
        // Arrange
        var physicsConfiguration = new PhysicsConfiguration
        {
            TileSize = tileSize
        };
        var physicsSystem = GetPhysicsSystem(physicsConfiguration);

        var centerTilePosition = new Vector2(tilePosition.X * tileSize.Width, tilePosition.Y * tileSize.Height);

        var (_, complementEntities) = CreateTileLayout(tileLayout.Layout, tileSize, centerTilePosition, true);

        var entity = CreateTileStaticBody(centerTilePosition);
        Assert.That(entity.GetComponent<Transform2DComponent>().Translation, Is.EqualTo(centerTilePosition), "Tile is misaligned.");

        physicsSystem.ProcessPhysics();

        // Assume
        var body = GetBodyForEntity(physicsSystem, entity);
        Assert.That(body.CollisionNormalFilter, Is.EqualTo(CollisionNormalFilter.None));

        // Act
        foreach (var complementEntity in complementEntities)
        {
            Scene.RemoveEntity(complementEntity);
        }

        physicsSystem.ProcessPhysics();

        // Assert
        Assert.That(body.CollisionNormalFilter, Is.EqualTo(tileLayout.Expected));
    }

    [Test]
    public void TileBody_CollisionNormalFilterIsUpdated_WhenMultipleBodiesShareATileAndLastNeighbouringTileBodyDisappearsDueToPositionUpdate(
        [ValueSource(nameof(TileSizes))] SizeD tileSize,
        [ValueSource(nameof(TilePositions))] Vector2 tilePosition,
        [ValueSource(nameof(TileLayouts))] TileLayout tileLayout)
    {
        // Arrange
        var physicsConfiguration = new PhysicsConfiguration
        {
            TileSize = tileSize
        };
        var physicsSystem = GetPhysicsSystem(physicsConfiguration);

        var centerTilePosition = new Vector2(tilePosition.X * tileSize.Width, tilePosition.Y * tileSize.Height);

        var (layoutEntities1, complementEntities1) = CreateTileLayout(tileLayout.Layout, tileSize, centerTilePosition, true);
        var (layoutEntities2, complementEntities2) = CreateTileLayout(tileLayout.Layout, tileSize, centerTilePosition, true);

        var entity = CreateTileStaticBody(centerTilePosition);
        Assert.That(entity.GetComponent<Transform2DComponent>().Translation, Is.EqualTo(centerTilePosition), "Tile is misaligned.");

        physicsSystem.ProcessPhysics();

        // Assume
        var body = GetBodyForEntity(physicsSystem, entity);
        Assert.That(body.CollisionNormalFilter, Is.EqualTo(CollisionNormalFilter.None));

        // Act 1
        foreach (var complementEntity in complementEntities1)
        {
            complementEntity.GetComponent<Transform2DComponent>().Translation = new Vector2(-100, -100);
        }

        physicsSystem.ProcessPhysics();

        // Assert 1
        Assert.That(body.CollisionNormalFilter, Is.EqualTo(CollisionNormalFilter.None));

        // Act 2
        foreach (var layoutEntity in layoutEntities1)
        {
            layoutEntity.GetComponent<Transform2DComponent>().Translation = new Vector2(-100, -100);
        }

        physicsSystem.ProcessPhysics();

        // Assert 2
        Assert.That(body.CollisionNormalFilter, Is.EqualTo(CollisionNormalFilter.None));

        // Act 3
        foreach (var complementEntity in complementEntities2)
        {
            complementEntity.GetComponent<Transform2DComponent>().Translation = new Vector2(-100, -100);
        }

        physicsSystem.ProcessPhysics();

        // Assert 3
        Assert.That(body.CollisionNormalFilter, Is.EqualTo(tileLayout.Expected));

        // Act 4
        foreach (var layoutEntity in layoutEntities2)
        {
            layoutEntity.GetComponent<Transform2DComponent>().Translation = new Vector2(-100, -100);
        }

        physicsSystem.ProcessPhysics();

        // Assert 4
        Assert.That(body.CollisionNormalFilter, Is.EqualTo(CollisionNormalFilter.All));
    }

    [Test]
    public void TileBody_CollisionNormalFilterIsUpdated_WhenMultipleBodiesShareATileAndLastNeighbouringTileBodyDisappearsDueToBeingRemoved(
        [ValueSource(nameof(TileSizes))] SizeD tileSize,
        [ValueSource(nameof(TilePositions))] Vector2 tilePosition,
        [ValueSource(nameof(TileLayouts))] TileLayout tileLayout)
    {
        // Arrange
        var physicsConfiguration = new PhysicsConfiguration
        {
            TileSize = tileSize
        };
        var physicsSystem = GetPhysicsSystem(physicsConfiguration);

        var centerTilePosition = new Vector2(tilePosition.X * tileSize.Width, tilePosition.Y * tileSize.Height);

        var (layoutEntities1, complementEntities1) = CreateTileLayout(tileLayout.Layout, tileSize, centerTilePosition, true);
        var (layoutEntities2, complementEntities2) = CreateTileLayout(tileLayout.Layout, tileSize, centerTilePosition, true);

        var entity = CreateTileStaticBody(centerTilePosition);
        Assert.That(entity.GetComponent<Transform2DComponent>().Translation, Is.EqualTo(centerTilePosition), "Tile is misaligned.");

        physicsSystem.ProcessPhysics();

        // Assume
        var body = GetBodyForEntity(physicsSystem, entity);
        Assert.That(body.CollisionNormalFilter, Is.EqualTo(CollisionNormalFilter.None));

        // Act 1
        foreach (var complementEntity in complementEntities1)
        {
            Scene.RemoveEntity(complementEntity);
        }

        physicsSystem.ProcessPhysics();

        // Assert 1
        Assert.That(body.CollisionNormalFilter, Is.EqualTo(CollisionNormalFilter.None));

        // Act 2
        foreach (var layoutEntity in layoutEntities1)
        {
            Scene.RemoveEntity(layoutEntity);
        }

        physicsSystem.ProcessPhysics();

        // Assert 2
        Assert.That(body.CollisionNormalFilter, Is.EqualTo(CollisionNormalFilter.None));

        // Act 3
        foreach (var complementEntity in complementEntities2)
        {
            Scene.RemoveEntity(complementEntity);
        }

        physicsSystem.ProcessPhysics();

        // Assert 3
        Assert.That(body.CollisionNormalFilter, Is.EqualTo(tileLayout.Expected));

        // Act 4
        foreach (var layoutEntity in layoutEntities2)
        {
            Scene.RemoveEntity(layoutEntity);
        }

        physicsSystem.ProcessPhysics();

        // Assert 4
        Assert.That(body.CollisionNormalFilter, Is.EqualTo(CollisionNormalFilter.All));
    }

    [Test]
    public void TileBody_CollisionNormalFilterIsUpdated_WhenTileColliderBecomesEnabled(
        [ValueSource(nameof(TileSizes))] SizeD tileSize,
        [ValueSource(nameof(TilePositions))] Vector2 tilePosition,
        [ValueSource(nameof(TileLayouts))] TileLayout tileLayout)
    {
        // Arrange
        var physicsConfiguration = new PhysicsConfiguration
        {
            TileSize = tileSize
        };
        var physicsSystem = GetPhysicsSystem(physicsConfiguration);

        var centerTilePosition = new Vector2(tilePosition.X * tileSize.Width, tilePosition.Y * tileSize.Height);

        CreateTileLayout(tileLayout.Layout, tileSize, centerTilePosition);

        var entity = CreateTileStaticBody(centerTilePosition);
        Assert.That(entity.GetComponent<Transform2DComponent>().Translation, Is.EqualTo(centerTilePosition), "Tile is misaligned.");

        var tileCollider = entity.GetComponent<TileColliderComponent>();
        tileCollider.Enabled = false;
        physicsSystem.ProcessPhysics();

        // Act
        tileCollider.Enabled = true;
        physicsSystem.ProcessPhysics();

        // Assert
        var body = GetBodyForEntity(physicsSystem, entity);
        Assert.That(body.CollisionNormalFilter, Is.EqualTo(tileLayout.Expected));
    }

    [Test]
    public void TileBody_CollisionNormalFilterIsUpdated_WhenNeighbouringTileColliderBecomesEnabled(
        [ValueSource(nameof(TileSizes))] SizeD tileSize,
        [ValueSource(nameof(TilePositions))] Vector2 tilePosition,
        [ValueSource(nameof(TileLayouts))] TileLayout tileLayout)
    {
        // Arrange
        var physicsConfiguration = new PhysicsConfiguration
        {
            TileSize = tileSize
        };
        var physicsSystem = GetPhysicsSystem(physicsConfiguration);

        var centerTilePosition = new Vector2(tilePosition.X * tileSize.Width, tilePosition.Y * tileSize.Height);

        var entity = CreateTileStaticBody(centerTilePosition);
        Assert.That(entity.GetComponent<Transform2DComponent>().Translation, Is.EqualTo(centerTilePosition), "Tile is misaligned.");

        var (layoutEntities, _) = CreateTileLayout(tileLayout.Layout, tileSize, centerTilePosition);
        foreach (var layoutEntity in layoutEntities)
        {
            layoutEntity.GetComponent<TileColliderComponent>().Enabled = false;
        }

        physicsSystem.ProcessPhysics();

        // Assume
        var body = GetBodyForEntity(physicsSystem, entity);
        Assert.That(body.CollisionNormalFilter, Is.EqualTo(CollisionNormalFilter.All));

        // Act
        foreach (var layoutEntity in layoutEntities)
        {
            layoutEntity.GetComponent<TileColliderComponent>().Enabled = true;
        }

        physicsSystem.ProcessPhysics();

        // Assert
        Assert.That(body.CollisionNormalFilter, Is.EqualTo(tileLayout.Expected));
    }

    [Test]
    public void TileBody_CollisionNormalFilterIsUpdated_WhenNeighbouringTileColliderBecomesDisabled(
        [ValueSource(nameof(TileSizes))] SizeD tileSize,
        [ValueSource(nameof(TilePositions))] Vector2 tilePosition,
        [ValueSource(nameof(TileLayouts))] TileLayout tileLayout)
    {
        // Arrange
        var physicsConfiguration = new PhysicsConfiguration
        {
            TileSize = tileSize
        };
        var physicsSystem = GetPhysicsSystem(physicsConfiguration);

        var centerTilePosition = new Vector2(tilePosition.X * tileSize.Width, tilePosition.Y * tileSize.Height);

        var (_, complementEntities) = CreateTileLayout(tileLayout.Layout, tileSize, centerTilePosition, true);

        var entity = CreateTileStaticBody(centerTilePosition);
        Assert.That(entity.GetComponent<Transform2DComponent>().Translation, Is.EqualTo(centerTilePosition), "Tile is misaligned.");

        physicsSystem.ProcessPhysics();

        // Assume
        var body = GetBodyForEntity(physicsSystem, entity);
        Assert.That(body.CollisionNormalFilter, Is.EqualTo(CollisionNormalFilter.None));

        // Act
        foreach (var complementEntity in complementEntities)
        {
            complementEntity.GetComponent<TileColliderComponent>().Enabled = false;
        }

        physicsSystem.ProcessPhysics();

        // Assert
        Assert.That(body.CollisionNormalFilter, Is.EqualTo(tileLayout.Expected));
    }

    [Test]
    public void TileBody_CollisionNormalFilterIsUpdated_WhenMultipleBodiesShareATileAndLastNeighbouringTileColliderBecomesDisabled(
        [ValueSource(nameof(TileSizes))] SizeD tileSize,
        [ValueSource(nameof(TilePositions))] Vector2 tilePosition,
        [ValueSource(nameof(TileLayouts))] TileLayout tileLayout)
    {
        // Arrange
        var physicsConfiguration = new PhysicsConfiguration
        {
            TileSize = tileSize
        };
        var physicsSystem = GetPhysicsSystem(physicsConfiguration);

        var centerTilePosition = new Vector2(tilePosition.X * tileSize.Width, tilePosition.Y * tileSize.Height);

        var (layoutEntities1, complementEntities1) = CreateTileLayout(tileLayout.Layout, tileSize, centerTilePosition, true);
        var (layoutEntities2, complementEntities2) = CreateTileLayout(tileLayout.Layout, tileSize, centerTilePosition, true);

        var entity = CreateTileStaticBody(centerTilePosition);
        Assert.That(entity.GetComponent<Transform2DComponent>().Translation, Is.EqualTo(centerTilePosition), "Tile is misaligned.");

        physicsSystem.ProcessPhysics();

        // Assume
        var body = GetBodyForEntity(physicsSystem, entity);
        Assert.That(body.CollisionNormalFilter, Is.EqualTo(CollisionNormalFilter.None));

        // Act 1
        foreach (var complementEntity in complementEntities1)
        {
            complementEntity.GetComponent<TileColliderComponent>().Enabled = false;
        }

        physicsSystem.ProcessPhysics();

        // Assert 1
        Assert.That(body.CollisionNormalFilter, Is.EqualTo(CollisionNormalFilter.None));

        // Act 2
        foreach (var layoutEntity in layoutEntities1)
        {
            layoutEntity.GetComponent<TileColliderComponent>().Enabled = false;
        }

        physicsSystem.ProcessPhysics();

        // Assert 2
        Assert.That(body.CollisionNormalFilter, Is.EqualTo(CollisionNormalFilter.None));

        // Act 3
        foreach (var complementEntity in complementEntities2)
        {
            complementEntity.GetComponent<TileColliderComponent>().Enabled = false;
        }

        physicsSystem.ProcessPhysics();

        // Assert 3
        Assert.That(body.CollisionNormalFilter, Is.EqualTo(tileLayout.Expected));

        // Act 4
        foreach (var layoutEntity in layoutEntities2)
        {
            layoutEntity.GetComponent<TileColliderComponent>().Enabled = false;
        }

        physicsSystem.ProcessPhysics();

        // Assert 4
        Assert.That(body.CollisionNormalFilter, Is.EqualTo(CollisionNormalFilter.All));
    }

    [Test]
    public void TileBody_CollisionNormalFilterIsNotUpdated_WhenTileColliderIsDisabledAndItsPositionIsChanged()
    {
        // Arrange
        var physicsConfiguration = new PhysicsConfiguration
        {
            TileSize = new SizeD(1, 1)
        };
        var physicsSystem = GetPhysicsSystem(physicsConfiguration);

        var enabledTile = CreateTileStaticBody(0, 0);
        var disabledTile = CreateTileStaticBody(-1, 0);

        disabledTile.GetComponent<TileColliderComponent>().Enabled = false;
        var disabledTransform = disabledTile.GetComponent<Transform2DComponent>();

        physicsSystem.ProcessPhysics();

        // Assume
        var enabledBody = GetBodyForEntity(physicsSystem, enabledTile);
        Assert.That(enabledBody.CollisionNormalFilter, Is.EqualTo(CollisionNormalFilter.All));

        // Act
        disabledTransform.Translation = new Vector2(1, 0);
        physicsSystem.ProcessPhysics();

        // Assert
        Assert.That(disabledTransform.Translation, Is.EqualTo(new Vector2(1, 0)));
        Assert.That(enabledBody.CollisionNormalFilter, Is.EqualTo(CollisionNormalFilter.All));
    }

    private (List<Entity> LayoutEntities, List<Entity> ComplementEntities) CreateTileLayout(TileLayout.Flag layout, SizeD tileSize, Vector2 centerTilePosition,
        bool complementLayout = false)
    {
        var layoutEntities = new List<Entity>();
        var complementEntities = new List<Entity>();

        // Map each flag to its offset from the center tile
        var flagOffsets = new Dictionary<TileLayout.Flag, Vector2>
        {
            { TileLayout.Flag.TopLeft, new Vector2(-tileSize.Width, tileSize.Height) },
            { TileLayout.Flag.Top, new Vector2(0, tileSize.Height) },
            { TileLayout.Flag.TopRight, new Vector2(tileSize.Width, tileSize.Height) },
            { TileLayout.Flag.Left, new Vector2(-tileSize.Width, 0) },
            { TileLayout.Flag.Right, new Vector2(tileSize.Width, 0) },
            { TileLayout.Flag.BottomLeft, new Vector2(-tileSize.Width, -tileSize.Height) },
            { TileLayout.Flag.Bottom, new Vector2(0, -tileSize.Height) },
            { TileLayout.Flag.BottomRight, new Vector2(tileSize.Width, -tileSize.Height) }
        };

        foreach (var (flag, offset) in flagOffsets)
        {
            var position = centerTilePosition + offset;

            if (layout.HasFlag(flag))
            {
                layoutEntities.Add(CreateTileStaticBody(position));
            }
            else if (complementLayout)
            {
                complementEntities.Add(CreateTileStaticBody(position));
            }
        }

        return (layoutEntities, complementEntities);
    }

    #endregion

    #region Ghost collision

    [Test]
    public void KinematicBody_RectangleCollider_ShouldAvoidGhostCollisionWithTileCollider_WhenMovingRight()
    {
        // Arrange
        GameTime.FixedDeltaTime = TimeSpan.FromSeconds(0.1);
        var physicsConfiguration = new PhysicsConfiguration
        {
            TileSize = new SizeD(10, 10),
            PenetrationTolerance = 1,
            EnableDebugRendering = true
        };
        var physicsSystem = GetPhysicsSystem(physicsConfiguration);

        CreateTileStaticBody(0, 0);
        CreateTileStaticBody(10, 0);
        var kinematicBody = CreateRectangleKinematicBody(-0.5, 9, 10, 10);
        kinematicBody.GetComponent<KinematicRigidBody2DComponent>().EnableCollisionResponse = true;
        kinematicBody.GetComponent<KinematicRigidBody2DComponent>().LinearVelocity = new Vector2(10, 0);

        // Act
        SaveVisualOutput(physicsSystem, 0, 10);
        physicsSystem.ProcessPhysics();
        SaveVisualOutput(physicsSystem, 1, 10);
        physicsSystem.ProcessPhysics();
        SaveVisualOutput(physicsSystem, 2, 10);

        // Assert
        Assert.That(kinematicBody.GetComponent<KinematicRigidBody2DComponent>().LinearVelocity, Is.EqualTo(new Vector2(10, 0)));
        Assert.That(kinematicBody.GetComponent<Transform2DComponent>().Translation, Is.EqualTo(new Vector2(1.5, 9)));
    }

    [Test]
    public void KinematicBody_RectangleCollider_ShouldAvoidGhostCollisionWithTileCollider_WhenMovingLeft()
    {
        // Arrange
        GameTime.FixedDeltaTime = TimeSpan.FromSeconds(0.1);
        var physicsConfiguration = new PhysicsConfiguration
        {
            TileSize = new SizeD(10, 10),
            PenetrationTolerance = 1,
            EnableDebugRendering = true
        };
        var physicsSystem = GetPhysicsSystem(physicsConfiguration);

        CreateTileStaticBody(0, 0);
        CreateTileStaticBody(10, 0);
        var kinematicBody = CreateRectangleKinematicBody(10.5, 9, 10, 10);
        kinematicBody.GetComponent<KinematicRigidBody2DComponent>().EnableCollisionResponse = true;
        kinematicBody.GetComponent<KinematicRigidBody2DComponent>().LinearVelocity = new Vector2(-10, 0);

        // Act
        SaveVisualOutput(physicsSystem, 0, 10);
        physicsSystem.ProcessPhysics();
        SaveVisualOutput(physicsSystem, 1, 10);
        physicsSystem.ProcessPhysics();
        SaveVisualOutput(physicsSystem, 2, 10);

        // Assert
        Assert.That(kinematicBody.GetComponent<KinematicRigidBody2DComponent>().LinearVelocity, Is.EqualTo(new Vector2(-10, 0)));
        Assert.That(kinematicBody.GetComponent<Transform2DComponent>().Translation, Is.EqualTo(new Vector2(8.5, 9)));
    }

    [Test]
    public void KinematicBody_RectangleCollider_ShouldAvoidGhostCollisionWithTileCollider_WhenMovingUp()
    {
        // Arrange
        GameTime.FixedDeltaTime = TimeSpan.FromSeconds(0.1);
        var physicsConfiguration = new PhysicsConfiguration
        {
            TileSize = new SizeD(10, 10),
            PenetrationTolerance = 1,
            EnableDebugRendering = true
        };
        var physicsSystem = GetPhysicsSystem(physicsConfiguration);

        CreateTileStaticBody(0, 0);
        CreateTileStaticBody(0, 10);
        var kinematicBody = CreateRectangleKinematicBody(-9, -0.5, 10, 10);
        kinematicBody.GetComponent<KinematicRigidBody2DComponent>().EnableCollisionResponse = true;
        kinematicBody.GetComponent<KinematicRigidBody2DComponent>().LinearVelocity = new Vector2(0, 10);

        // Act
        SaveVisualOutput(physicsSystem, 0, 10);
        physicsSystem.ProcessPhysics();
        SaveVisualOutput(physicsSystem, 1, 10);
        physicsSystem.ProcessPhysics();
        SaveVisualOutput(physicsSystem, 2, 10);

        // Assert
        Assert.That(kinematicBody.GetComponent<KinematicRigidBody2DComponent>().LinearVelocity, Is.EqualTo(new Vector2(0, 10)));
        Assert.That(kinematicBody.GetComponent<Transform2DComponent>().Translation, Is.EqualTo(new Vector2(-9, 1.5)));
    }

    [Test]
    public void KinematicBody_RectangleCollider_ShouldAvoidGhostCollisionWithTileCollider_WhenMovingDown()
    {
        // Arrange
        GameTime.FixedDeltaTime = TimeSpan.FromSeconds(0.1);
        var physicsConfiguration = new PhysicsConfiguration
        {
            TileSize = new SizeD(10, 10),
            PenetrationTolerance = 1,
            EnableDebugRendering = true
        };
        var physicsSystem = GetPhysicsSystem(physicsConfiguration);

        CreateTileStaticBody(0, 0);
        CreateTileStaticBody(0, 10);
        var kinematicBody = CreateRectangleKinematicBody(-9, 10.5, 10, 10);
        kinematicBody.GetComponent<KinematicRigidBody2DComponent>().EnableCollisionResponse = true;
        kinematicBody.GetComponent<KinematicRigidBody2DComponent>().LinearVelocity = new Vector2(0, -10);

        // Act
        SaveVisualOutput(physicsSystem, 0, 10);
        physicsSystem.ProcessPhysics();
        SaveVisualOutput(physicsSystem, 1, 10);
        physicsSystem.ProcessPhysics();
        SaveVisualOutput(physicsSystem, 2, 10);

        // Assert
        Assert.That(kinematicBody.GetComponent<KinematicRigidBody2DComponent>().LinearVelocity, Is.EqualTo(new Vector2(0, -10)));
        Assert.That(kinematicBody.GetComponent<Transform2DComponent>().Translation, Is.EqualTo(new Vector2(-9, 8.5)));
    }

    [Test]
    public void KinematicBody_CircleCollider_ShouldAvoidGhostCollisionWithTileCollider_WhenMovingRight()
    {
        // Arrange
        GameTime.FixedDeltaTime = TimeSpan.FromSeconds(0.1);
        var physicsConfiguration = new PhysicsConfiguration
        {
            TileSize = new SizeD(10, 10),
            PenetrationTolerance = 1,
            EnableDebugRendering = true
        };
        var physicsSystem = GetPhysicsSystem(physicsConfiguration);

        CreateTileStaticBody(0, 0);
        CreateTileStaticBody(10, 0);
        var kinematicBody = CreateCircleKinematicBody(1.5, 9, 5);
        kinematicBody.GetComponent<KinematicRigidBody2DComponent>().EnableCollisionResponse = true;
        kinematicBody.GetComponent<KinematicRigidBody2DComponent>().LinearVelocity = new Vector2(10, 0);

        // Act
        SaveVisualOutput(physicsSystem, 0, 10);
        physicsSystem.ProcessPhysics();
        SaveVisualOutput(physicsSystem, 1, 10);
        physicsSystem.ProcessPhysics();
        SaveVisualOutput(physicsSystem, 2, 10);

        // Assert
        Assert.That(kinematicBody.GetComponent<KinematicRigidBody2DComponent>().LinearVelocity, Is.EqualTo(new Vector2(10, 0)));
        Assert.That(kinematicBody.GetComponent<Transform2DComponent>().Translation, Is.EqualTo(new Vector2(3.5, 9)));
    }

    [Test]
    public void KinematicBody_CircleCollider_ShouldAvoidGhostCollisionWithTileCollider_WhenMovingLeft()
    {
        // Arrange
        GameTime.FixedDeltaTime = TimeSpan.FromSeconds(0.1);
        var physicsConfiguration = new PhysicsConfiguration
        {
            TileSize = new SizeD(10, 10),
            PenetrationTolerance = 1,
            EnableDebugRendering = true
        };
        var physicsSystem = GetPhysicsSystem(physicsConfiguration);

        CreateTileStaticBody(0, 0);
        CreateTileStaticBody(10, 0);
        var kinematicBody = CreateCircleKinematicBody(8.5, 9, 5);
        kinematicBody.GetComponent<KinematicRigidBody2DComponent>().EnableCollisionResponse = true;
        kinematicBody.GetComponent<KinematicRigidBody2DComponent>().LinearVelocity = new Vector2(-10, 0);

        // Act
        SaveVisualOutput(physicsSystem, 0, 10);
        physicsSystem.ProcessPhysics();
        SaveVisualOutput(physicsSystem, 1, 10);
        physicsSystem.ProcessPhysics();
        SaveVisualOutput(physicsSystem, 2, 10);

        // Assert
        Assert.That(kinematicBody.GetComponent<KinematicRigidBody2DComponent>().LinearVelocity, Is.EqualTo(new Vector2(-10, 0)));
        Assert.That(kinematicBody.GetComponent<Transform2DComponent>().Translation, Is.EqualTo(new Vector2(6.5, 9)));
    }

    [Test]
    public void KinematicBody_CircleCollider_ShouldAvoidGhostCollisionWithTileCollider_WhenMovingUp()
    {
        // Arrange
        GameTime.FixedDeltaTime = TimeSpan.FromSeconds(0.1);
        var physicsConfiguration = new PhysicsConfiguration
        {
            TileSize = new SizeD(10, 10),
            PenetrationTolerance = 1,
            EnableDebugRendering = true
        };
        var physicsSystem = GetPhysicsSystem(physicsConfiguration);

        CreateTileStaticBody(0, 0);
        CreateTileStaticBody(0, 10);
        var kinematicBody = CreateCircleKinematicBody(-9, 1.5, 5);
        kinematicBody.GetComponent<KinematicRigidBody2DComponent>().EnableCollisionResponse = true;
        kinematicBody.GetComponent<KinematicRigidBody2DComponent>().LinearVelocity = new Vector2(0, 10);

        // Act
        SaveVisualOutput(physicsSystem, 0, 10);
        physicsSystem.ProcessPhysics();
        SaveVisualOutput(physicsSystem, 1, 10);
        physicsSystem.ProcessPhysics();
        SaveVisualOutput(physicsSystem, 2, 10);

        // Assert
        Assert.That(kinematicBody.GetComponent<KinematicRigidBody2DComponent>().LinearVelocity, Is.EqualTo(new Vector2(0, 10)));
        Assert.That(kinematicBody.GetComponent<Transform2DComponent>().Translation, Is.EqualTo(new Vector2(-9, 3.5)));
    }

    [Test]
    public void KinematicBody_CircleCollider_ShouldAvoidGhostCollisionWithTileCollider_WhenMovingDown()
    {
        // Arrange
        GameTime.FixedDeltaTime = TimeSpan.FromSeconds(0.1);
        var physicsConfiguration = new PhysicsConfiguration
        {
            TileSize = new SizeD(10, 10),
            PenetrationTolerance = 1,
            EnableDebugRendering = true
        };
        var physicsSystem = GetPhysicsSystem(physicsConfiguration);

        CreateTileStaticBody(0, 0);
        CreateTileStaticBody(0, 10);
        var kinematicBody = CreateCircleKinematicBody(-9, 8.5, 5);
        kinematicBody.GetComponent<KinematicRigidBody2DComponent>().EnableCollisionResponse = true;
        kinematicBody.GetComponent<KinematicRigidBody2DComponent>().LinearVelocity = new Vector2(0, -10);

        // Act
        SaveVisualOutput(physicsSystem, 0, 10);
        physicsSystem.ProcessPhysics();
        SaveVisualOutput(physicsSystem, 1, 10);
        physicsSystem.ProcessPhysics();
        SaveVisualOutput(physicsSystem, 2, 10);

        // Assert
        Assert.That(kinematicBody.GetComponent<KinematicRigidBody2DComponent>().LinearVelocity, Is.EqualTo(new Vector2(0, -10)));
        Assert.That(kinematicBody.GetComponent<Transform2DComponent>().Translation, Is.EqualTo(new Vector2(-9, 6.5)));
    }

    #endregion

    [Test]
    public void TileBody_ShouldNotFilterCollisionNormal_WhenTileCollisionNormalFilterIsNone_Or_ItIsNotNoneButFilteredCollisionNormalIsZeroVector()
    {
        // Arrange
        var physicsConfiguration = new PhysicsConfiguration
        {
            TileSize = new SizeD(10, 10),
            PenetrationTolerance = 1,
            EnableDebugRendering = true
        };
        var physicsSystem = GetPhysicsSystem(physicsConfiguration);

        CreateTileStaticBody(-10, 10);
        CreateTileStaticBody(0, 10);
        CreateTileStaticBody(10, 10);
        CreateTileStaticBody(-10, 0);
        var tileWithNoneFilter = CreateTileStaticBody(0, 0);
        var tileWithZeroVector = CreateTileStaticBody(10, 0);
        CreateTileStaticBody(-10, -10);
        CreateTileStaticBody(0, -10);
        CreateTileStaticBody(10, -10);

        CreateRectangleKinematicBody(5, 0, 10, 10);

        // Act
        SaveVisualOutput(physicsSystem, 0, 10);
        physicsSystem.ProcessPhysics();
        SaveVisualOutput(physicsSystem, 1, 10);

        // Assert
        Assert.That(tileWithNoneFilter.GetComponent<TileColliderComponent>().GetContacts().Single().CollisionNormal, Is.EqualTo(-Vector2.UnitX));
        Assert.That(tileWithZeroVector.GetComponent<TileColliderComponent>().GetContacts().Single().CollisionNormal, Is.EqualTo(Vector2.UnitX));
    }

    [Test]
    public void TileBody_ShouldGenerateRegularContactPoints_ButShouldAdjustCollisionNormal()
    {
        // Arrange
        var physicsConfiguration = new PhysicsConfiguration
        {
            TileSize = new SizeD(50, 50),
            PenetrationTolerance = 1,
            EnableDebugRendering = true
        };
        var physicsSystem = GetPhysicsSystem(physicsConfiguration);

        CreateTileStaticBody(0, -50);
        CreateTileStaticBody(50, -50);

        var kinematicBody = CreateRectangleKinematicBody(5.329070518200751E-15, 36.42682999151367, 100, 100, -0.4450571790227168);

        // Act
        SaveVisualOutput(physicsSystem, 0, 3);
        physicsSystem.ProcessPhysics();
        SaveVisualOutput(physicsSystem, 1, 3);

        // Assert
        var contacts = kinematicBody.GetComponent<RectangleColliderComponent>().GetContacts();
        Assert.That(contacts.Count, Is.EqualTo(2));
        Assert.That(contacts[0].CollisionNormal, Is.EqualTo(Vector2.UnitY));
        Assert.That(contacts[0].PenetrationDepth, Is.EqualTo(5.2279470461191693d));
        Assert.That(contacts[1].CollisionNormal, Is.EqualTo(Vector2.UnitY));
        Assert.That(contacts[1].PenetrationDepth, Is.EqualTo(0.99051547947736651d));
    }
}