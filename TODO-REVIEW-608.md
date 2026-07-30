# TODO Review — Branch `608-add-broad-phase-in-physics-engine-2d`

Scope: all `TODO` items created, modified or moved by the changes on this branch
(diff range `master...HEAD`, 95 commits, 32 files changed).

Generated: 2026-07-27 · Updated: 2026-07-30 · Open sections re-derived against `9bd3f53d`

---

## New TODOs added on this branch (3 open, 5 postponed)

### `src/Geisha.Engine/Physics/PhysicsEngine2D/Internal/BroadPhase.cs` *(new file)*

| Line | TODO |
|---|---|
| 9 | Review and possibly update related tests to cover new implementation. |

### `src/Geisha.Engine/Physics/PhysicsEngine2D/Internal/SceneQuery.cs` *(effectively rewritten)*

The file exists in `master` (80 lines, no TODOs) but was rewritten on this branch to query the
spatial grid, so both TODOs are new.

| Line | TODO |
|---|---|
| 10 | Review and possibly update related tests to cover new implementation. |

### `src/Geisha.Engine/Physics/PhysicsEngine2D/Internal/PhysicsSceneData.cs`

| Line | TODO |
|---|---|
| 283 | How to test that proxy is destroyed when body is destroyed? |

All 3 line numbers above verified against `9bd3f53d`. `SpatialGrid.cs:800`, `BroadPhase.cs:84`,
`BroadPhase.cs:123`, `SceneQuery.cs:111` and `SimulationPipeline.cs:25` were moved to
[Postponed to the future](#postponed-to-the-future-out-of-scope-for-this-branch).

---

## Moved TODOs (1)

`CollisionDetection.cs:123` → `NarrowPhase.cs:46` — the file was renamed (git reports `R051`,
51% similarity) as part of the broad/narrow phase split. Text unchanged:

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
| `Physics/PhysicsEngine2D/Internal/SimulationPipeline.cs` | 16 → 16, 17 → 17, 32 → 33, 38 → 39 |
| `Physics/Systems/PhysicsSystem.cs` | 319 → 326, 392 → 399, 406 → 413, 501 → 508, 504 → 511 |

The two `SimulationPipeline.cs` entries that did not move (16, 17) are listed for completeness,
so the file's full TODO inventory is visible in one place: 5 at `HEAD`, of which 4 are
pre-existing and only line 25 is new.

---

## Themes / loose ends introduced by this branch

1. **Missing docs, tests and validation**
   - `PhysicsConfiguration.BroadPhaseGridCellSize` — docs, tests and validation are all
     **done**; the file now contains no TODOs at all. See Resolved.
   - `BroadPhase.cs:9` and `SceneQuery.cs:10` — "review related tests" markers.
   - `PhysicsSceneData.cs:283` — open question about testing proxy destruction.

2. **Blocked on .NET 9 / C# 13 (ref fields + ref struct interfaces)**
   Three new `ProxyQueryHandler` TODOs (`BroadPhase.cs:84`, `BroadPhase.cs:123`,
   `SceneQuery.cs:111`) join three pre-existing ones of the same flavour in
   `PhysicsSystem.cs` (508, 511) and `PhysicsSceneData.cs:50` (`System.Threading.Lock`),
   so **six** TODOs are now gated on a single framework upgrade. Worth tracking as one
   umbrella item. The three new ones are **postponed** — see
   [Postponed to the future](#postponed-to-the-future-out-of-scope-for-this-branch).

3. **Performance follow-ups** — both **postponed**, see
   [Postponed to the future](#postponed-to-the-future-out-of-scope-for-this-branch).
   - `SimulationPipeline.cs:25` — only recompute bodies that actually moved.
   - `SpatialGrid.cs:800` — extract `GrowArrayExp` to a shared helper if reused.

---

## Postponed to the future (out of scope for this branch)

TODOs intentionally left open. They are acknowledged, deliberately **not** worked on in scope of
`608-add-broad-phase-in-physics-engine-2d`, and are expected to be picked up later.

| File | Line | TODO | Reason for postponing |
|---|---|---|---|
| `src/Geisha.Engine/Core/Spatial/SpatialGrid.cs` | 800 | This might be useful helper in other places. If so, move to `ArrayEx`? | Conditional by its own wording: the move to `ArrayEx` is only worth doing *if* the exponential array-growth pattern is needed elsewhere. `SpatialGrid` is currently the only user, so there is nothing to share yet and extracting now would be speculative generality. Revisit when a second caller appears. |
| `src/Geisha.Engine/Physics/PhysicsEngine2D/Internal/SimulationPipeline.cs` | 25 | Recomputation is only needed for bodies that actually moved. | Optimization opportunity, not a correctness gap: recomputing every body's proxy is correct, just wasteful. Skipping unmoved bodies needs a reliable moved/dirty signal, which is a separate design step. The broad phase works without it, so it is deferred to a dedicated performance pass. |
| `src/Geisha.Engine/Physics/PhysicsEngine2D/Internal/BroadPhase.cs` | 84 | To implement `ProxyQueryHandler` properly it requires ref fields and ref struct interfaces features of .NET 9 (C# 13). | Gated on a framework upgrade — see the umbrella note below. |
| `src/Geisha.Engine/Physics/PhysicsEngine2D/Internal/BroadPhase.cs` | 123 | To implement `ProxyQueryHandler` properly it requires ref fields and ref struct interfaces features of .NET 9 (C# 13). | Gated on a framework upgrade — see the umbrella note below. |
| `src/Geisha.Engine/Physics/PhysicsEngine2D/Internal/SceneQuery.cs` | 111 | To implement `ProxyQueryHandler` properly it requires ref fields and ref struct interfaces features of .NET 9 (C# 13). | Gated on a framework upgrade — see the umbrella note below. |

### Umbrella: .NET 9 / C# 13 upgrade

The three `ProxyQueryHandler` TODOs above cannot be actioned on this branch — they need
**ref fields** and **ref struct interfaces**, which arrive with .NET 9 (C# 13). Upgrading the
target framework is a repo-wide change, unrelated to adding the broad phase, and is therefore
out of scope here.

They are not alone: three pre-existing TODOs are blocked on the same upgrade
(`PhysicsSystem.cs:508`, `PhysicsSystem.cs:511`, and `PhysicsSceneData.cs:50` for
`System.Threading.Lock`), so **six** TODOs unblock together the moment the framework moves.
Best tracked as one upgrade task rather than six independent items.

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

Both changes are now committed as `479ea05a` ("Add test for broad-phase grid cell size"); the
earlier note about them being uncommitted no longer applies.

### `src/Geisha.Engine/Physics/PhysicsConfiguration.cs` — all TODOs cleared (2026-07-30)

> `85` Add documentation. · `86` Add tests. *(both for `BroadPhaseGridCellSize`)*

The file now contains no TODOs at all.

**Tests** — removed in `ccd16842` ("Add BroadPhaseGridCellSize to configuration"), which wired the
property through the JSON config file and covered it end to end:

- `Configuration.LoadFromFile` parsing (`Configuration.cs`), plus the `BroadPhaseGridCellSize`
  entry in `full-configuration.json` test data.
- `ConfigurationTests.Overwrite_ShouldCreateNewConfigurationOverwrittenByGame` — asserts
  `new SizeD(12, 34)` survives `Game.Configure` overwrite.
- `ConfigurationIntegrationTests` — two assertions: the default `new SizeD(256, 256)` when the
  config file omits the value, and `new SizeD(12, 34)` when it is present. This covers the
  default-value and valid-value gaps flagged in the earlier review.
- Also added to the `engine-config.json` of Benchmark, Demo, Sandbox and E2EApp (`e145da7a`).

`ConfigurationTests` green (1 test passed).

**Docs** — removed in `5a2f67b6` ("Document broad-phase grid cell size config"), then extended by
`41f85e86` and `8893837b`:

- `<summary>` for `BroadPhaseGridCellSize` — what the cell size is, units (meters), and the
  default, plus `<remarks>` in four `<para>` blocks: what the broad phase does, the
  large-vs-small-cells trade-off framed as guidance rather than prescription, the
  positive-dimensions requirement pointing at the `ArgumentException`, and that the value is
  fixed at physics-system creation.
- **Default values documented across all four configuration types** (`41f85e86`) — 12 properties
  gained a `Default is <c>X</c>.` sentence, so `AudioConfiguration` (2), `CoreConfiguration` (15),
  `PhysicsConfiguration` (7) and `RenderingConfiguration` (4) are now at full coverage, 28/28.
  The `<summary>` placement was chosen over `<value>` to match the existing 28-vs-4 convention
  in the repo.
- **`EnableSound` / `EnableVSync` reworded** (`8893837b`) from `If true, …` to the
  `Specifies whether …` form used by their siblings, so the appended default sentence follows a
  declarative summary.

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
