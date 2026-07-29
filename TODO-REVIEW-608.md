# TODO Review — Branch `608-add-broad-phase-in-physics-engine-2d`

Scope: all `TODO` items created, modified or moved by the changes on this branch
(diff range `master...HEAD`, 80 commits, 20 files changed).

Generated: 2026-07-27 · Updated: 2026-07-29

---

## New TODOs added on this branch (10 open)

### `src/Geisha.Engine/Core/Spatial/SpatialGrid.cs` *(new file)*

| Line | TODO |
|---|---|
| 800 | This might be useful helper in other places. If so, move to `ArrayEx`? |

### `src/Geisha.Engine/Physics/PhysicsEngine2D/Internal/BroadPhase.cs` *(new file)*

| Line | TODO |
|---|---|
| 9 | Review and possibly update related tests to cover new implementation. |
| 84 | To implement `ProxyQueryHandler` properly it requires ref fields and ref struct interfaces features of .NET 9 (C# 13). |
| 123 | To implement `ProxyQueryHandler` properly it requires ref fields and ref struct interfaces features of .NET 9 (C# 13). |

### `src/Geisha.Engine/Physics/PhysicsConfiguration.cs`

| Line | TODO |
|---|---|
| 85 | Add documentation. *(for new `BroadPhaseGridCellSize`)* |
| 86 | Add tests. *(same)* |

### `src/Geisha.Engine/Physics/PhysicsEngine2D/Internal/SceneQuery.cs`

| Line | TODO |
|---|---|
| 10 | Review and possibly update related tests to cover new implementation. |
| 111 | To implement `ProxyQueryHandler` properly it requires ref fields and ref struct interfaces features of .NET 9 (C# 13). |

### `src/Geisha.Engine/Physics/PhysicsEngine2D/Internal/PhysicsSceneData.cs`

| Line | TODO |
|---|---|
| 283 | How to test that proxy is destroyed when body is destroyed? |

### `src/Geisha.Engine/Physics/PhysicsEngine2D/Internal/SimulationPipeline.cs`

| Line | TODO |
|---|---|
| 25 | Recomputation is only needed for bodies that actually moved. |

---

## Moved TODOs (1)

`CollisionDetection.cs:123` → `NarrowPhase.cs:46` — the file was renamed as part of the
broad/narrow phase split. Text unchanged:

> Once broad phase is implemented in scope of <https://github.com/dawidkomorowski/geisha/issues/608>
> the collider type switch logic could be investigated…

Note: this TODO references issue #608 — the very issue this branch implements — so it is
now actionable rather than blocked.

---

## Deleted or reworded TODOs

None. Every TODO present in `master` for the touched files still exists verbatim at `HEAD`.
The following shifted line numbers only:

| File | master → HEAD |
|---|---|
| `Core/Math/Vector2.cs` | 293 → 296 |
| `Physics/PhysicsEngine2D/Internal/PhysicsSceneData.cs` | 10 → 11, 49 → 50 |
| `Physics/PhysicsEngine2D/Internal/RigidBodyData.cs` | 7 → 8, 30 → 31 |
| `Physics/PhysicsEngine2D/Internal/SimulationPipeline.cs` | 38 → 39 |
| `Physics/Systems/PhysicsSystem.cs` | 319 → 322, 392 → 395, 406 → 409, 501 → 504, 504 → 507 |

---

## Themes / loose ends introduced by this branch

1. **Missing docs, tests and validation**
   - `PhysicsConfiguration.BroadPhaseGridCellSize` — docs (`PhysicsConfiguration.cs:85`) and
     tests (`PhysicsConfiguration.cs:86`) still open. Validation is **done** — see Resolved.
   - `BroadPhase.cs:9` and `SceneQuery.cs:10` — "review related tests" markers.
   - `PhysicsSceneData.cs:283` — open question about testing proxy destruction.

2. **Blocked on .NET 9 / C# 13 (ref fields + ref struct interfaces)**
   Three new `ProxyQueryHandler` TODOs join three pre-existing ones of the same flavour
   in `PhysicsSystem.cs` (504, 507) and `PhysicsSceneData.cs:50` (`System.Threading.Lock`),
   so **six** TODOs are now gated on a single framework upgrade. Worth tracking as one
   umbrella item.

3. **Performance follow-ups**
   - `SimulationPipeline.cs:25` — only recompute bodies that actually moved.
   - `SpatialGrid.cs:800` — extract `GrowArrayExp` to a shared helper if reused.

---

## Resolved (2026-07-29)

### `src/Geisha.Engine/Physics/Systems/PhysicsSystem.cs:55` — `BroadPhaseGridCellSize` tests and validation

> Add tests and validation for `BroadPhaseGridCellSize`.

The TODO comment is gone; the line now holds the validation itself.

What was delivered:

- **Validation** in the `PhysicsSystem` constructor (`PhysicsSystem.cs:55-59`): throws
  `ArgumentException` when `BroadPhaseGridCellSize.Width <= 0 || Height <= 0`, with the message
  "`Configuration is invalid. BroadPhaseGridCellSize must have positive dimensions.`" and
  `paramName` of `physicsConfiguration`. Mirrors the existing `TileSize` check immediately above
  it, so the whole config-validation block stays uniform.
- **Test** — `TweakingParametersTests.BroadPhaseGridCellSizeTest_Constructor_ShouldThrowException_GivenInvalidBroadPhaseGridCellSize`
  with 6 `[TestCase]`s covering zero on each axis, zero on both, negative on each axis, and
  negative on both. Follows the naming and Arrange/Act-Assert shape of the sibling `Substeps`,
  `VelocityIterations`, `PositionIterations` and `PenetrationTolerance` constructor tests.

`TweakingParametersTests` green (25 tests passed).

**Note:** both changes are still uncommitted in the working tree (`PhysicsSystem.cs` and
`TweakingParametersTests.cs` show as modified) — they are not yet part of the 80-commit branch
history.

### Still open for `BroadPhaseGridCellSize`

The two TODOs in `PhysicsConfiguration.cs` (85: docs, 86: tests) are **not** covered by this
work and remain open:

- **Docs** — `BroadPhaseGridCellSize` is the only public `PhysicsConfiguration` property without
  an XML doc comment. Every sibling has `<summary>` plus `<remarks>` explaining the
  performance/behaviour trade-off; this one needs the same treatment (what the grid cell size
  means, units, and how tuning it trades broad-phase cell count against pairs per cell).
- **Tests** — the new test asserts only the *rejection* of invalid values. Not covered: the
  default value (`new SizeD(256, 256)`), that valid values are accepted (there is no
  `ShouldNotThrowException` counterpart, unlike the other four parameters), and that the
  configured value is actually propagated into `PhysicsScene2DDefinition` (`PhysicsSystem.cs:73`).
  The propagation path is partially covered one layer down by
  `PhysicsEngine2DTests.cs:37`, which asserts `scene.BroadPhaseGridCellSize` matches the scene
  definition, but nothing ties `PhysicsConfiguration` to that definition.

### `src/Geisha.Engine/Core/Math/AABB2D.cs` — all TODOs cleared

Both original "Add documentation." TODOs (for `IsValid` and `Intersect`) are done, and the
file now contains no TODOs at all.

What was delivered:

- **XML docs for `IsValid`** — states the well-formedness contract, that degenerate boxes
  (line, point) are valid, and points at `Intersect` as the main producer of invalid boxes.
- **XML docs for `Intersect`** — documents the invalid result for non-overlapping boxes,
  valid degenerate results for edge/corner touching, and invalidity propagation.
- **Type-level `<remarks>`** — added a paragraph establishing that boxes are not guaranteed
  well-formed (constructors and factories neither validate nor normalize), and a paragraph
  stating that containment/overlap results are *unspecified* for invalid operands, with
  `Intersect`'s propagation as the one guaranteed exception.
- **`Intersect` invalidity propagation codified by tests** —
  `Intersect_ShouldReturnInvalidAABB_WhenEitherBoxIsInvalid` with 6 cases (each operand
  invalid on each axis, both invalid on both axes, and each invalid on a different axis).
- **`FromPoints` / `FromAABBs` empty-input ambiguity resolved.** Previously both returned
  `default` — a *valid* degenerate box at the origin — making an empty-input result
  indistinguishable from a single point at the origin. Both now seed the fold with
  `Min = +infinity`, `Max = -infinity`, so empty input yields an invalid box and `IsValid`
  is the discriminator. The `if (length == 0)` early returns were removed; the sentinel is
  simply the identity element of the union. Docs updated from "`default`" to "an invalid
  bounding box". Tests: the empty `[TestCase]`s were converted to degenerate
  single-point-at-origin regression cases, `IsValid` is now asserted in both parameterized
  methods, and two dedicated tests cover empty input.

Full unit test suite green (4122 tests) after the change. Only in-repo caller is
`Rectangle.ComputeAABB` (`Rectangle.cs:209`), which always passes 4 vertices and is
unaffected.

### Deliberately not pursued

- **Invalid-operand behaviour of `Contains` / `Overlaps`** — documented as "unspecified"
  rather than pinned down. `Contains(in Vector2)` happens to return `false` for an invalid
  box (empty-set-correct), but `Overlaps` can return `true` for one, so the predicates are
  not mutually consistent. Deciding whether an invalid box is a supported input or caller
  error is a design question, not a documentation one. "Unspecified" leaves room for either
  answer.
- **NaN coordinate behaviour** — untested and undocumented. With the new fold, a single NaN
  point makes both `Min` and `Max` NaN, so the result is invalid ("NaN in, invalid out").
  Reasonable, but not pinned by a test.
- **`IsValid` style inconsistency** — it is the only computed property using a body-block
  getter with `[MethodImpl(AggressiveInlining)]`; `Center`, `Size`, `Width` and `Height` are
  plain expression-bodied members. Cosmetic only.
