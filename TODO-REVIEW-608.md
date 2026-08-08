# TODO Review — Branch `608-add-broad-phase-in-physics-engine-2d`

Scope: all `TODO` items created, modified or moved by the changes on this branch
(diff range `master...HEAD`, 95 commits, 32 files changed).

Generated: 2026-07-27 · Updated: 2026-08-07 · Open sections re-derived against `7e9090d8`

---

## New TODOs added on this branch (0 open, 5 postponed)

`PhysicsSceneData.cs:283` — "How to test that proxy is destroyed when body is destroyed?" — is
**resolved**, see
[Resolved: proxy destruction](#resolved-physicsscenedatacs283--proxy-destruction-2026-08-07).
`BroadPhase.cs:9` and `SceneQuery.cs:10` — the two "review related tests" markers — are
**resolved**, see [Resolved](#resolved-broadphasecs9-and-scenequerycs10--review-related-tests).
`SpatialGrid.cs:800`, `BroadPhase.cs:84`, `BroadPhase.cs:123`, `SceneQuery.cs:111` and
`SimulationPipeline.cs:25` were moved to
[Postponed to the future](#postponed-to-the-future-out-of-scope-for-this-branch).

No TODO added on this branch remains open.

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
   - `BroadPhase.cs:9` and `SceneQuery.cs:10` — "review related tests" markers, now
     **done**; both comments are gone. See Resolved.
   - `PhysicsSceneData.cs:283` — proxy destruction, now **done**; the comment is gone. See
     [Resolved: proxy destruction](#resolved-physicsscenedatacs283--proxy-destruction-2026-08-07).

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

## Resolved: `BroadPhase.cs:9` and `SceneQuery.cs:10` — review related tests (2026-08-03)

> Review and possibly update related tests to cover new implementation.
> Now collision detection / queries rely on spatial grid so bugs in updates of spatial grid
> should be captured in collision detection / query tests?

Both comments are removed. The answer to the question they pose turned out to be "yes for some
bugs, no for others", so three coverage gaps were identified and closed.

### The testing-layer decision

Broad phase and spatial grid are implementation details that are not directly observable through
the public physics API, which is what made the right test layer non-obvious. Three options were
weighed against verification power, brittleness under behaviour-preserving refactoring, and cost of
adapting when behaviour does change:

- **White box via `InternalsVisibleTo`** — precise, but pins `SpatialGrid`, `SpatialProxyId`,
  `BroadPhaseAABB` and `MoveProxy`, so any internal reshuffle breaks it. Rejected: the recent
  OOP → DOD rewrite of the internal engine was cheap precisely because the tests did not do this.
- **Internal physics engine public API** — the existing middle ground. Does not help here, because
  the broad phase is not observable at that level either.
- **Physics system level (chosen)** — entities, components, `ProcessPhysics`, `QueryPoint`,
  `QueryBounds`.

The unlock was reframing the contract so it never mentions grids, proxies or cells. Two properties
are stated purely in terms of scene geometry, which makes both fully observable through the public
API:

1. **History independence** — results depend only on the current geometry of the scene, never on
   the history of how that geometry came to be.
2. **Cell size invariance** — the same scene produces the same results for any
   `BroadPhaseGridCellSize`; it is a performance knob, not a behavioural one.

No test in the new fixture references any broad-phase type, so the whole spatial layer can be
replaced (grid → BVH, different fat-box factor, different update condition) without touching them.
Failures read as "collider was not found at (90.5, 0)" rather than as a proxy-bounds mismatch.

Deliberately **not** tested: the 2× fat-box inflation factor itself. Changing it to 3× alters no
observable result, only how often `MoveProxy` fires, so it is not behaviour. What is tested is the
consequence of getting the *update condition* wrong.

### Where the tests live

New dedicated fixture `test/…/PhysicsSystemTests/BroadPhaseTests.cs`, rather than spreading the
tests across the existing query and collision fixtures. Reasons: cell-size invariance would
otherwise have to be triplicated; shared scaffolding would have to be hoisted into a base class
inherited by 15 fixtures; scenario-shaped tests break `SceneQueryTests`' per-API idiom; and
`TileColliderTests` is existing precedent for a subject-scoped fixture that owns its own
configuration tests.

The two `BroadPhaseGridCellSize` constructor tests moved out of `TweakingParametersTests` into the
new fixture's `Configuration` region and lost their `BroadPhaseGridCellSizeTest_` prefix.
`TweakingParametersTests` is left holding only solver-tuning parameters, all following the uniform
"increasing X makes Y more accurate" idiom.

### Gap A — dual-grid query routing

Scene queries gather from `StaticGrid` **and** `DynamicGrid` (`SceneQuery.cs:25-26`, `:88-89`), but
the query fixtures only exercised static bodies, so dropping either gather went unnoticed for
kinematic bodies. Closed by adding kinematic-body cases to `SceneQueryTests` (`f12c0496`).

### Gap C — history independence (`c6d4f97c`, `2ba5c18e`)

Nine tests, region `History independence`. Every scenario reaches identical final geometry through
a different history and shares one expected result.

Two things had to be got right for these tests to have any power, and both were discovered by
mutation testing rather than by reading the code:

- **Bodies must be placed away from the origin.** A proxy is created with `bodyRef.AABB` *before*
  the collider is computed (`PhysicsSceneData.cs:257-262`), i.e. with `default` — a degenerate box
  at the origin. Deleting `MoveProxy` entirely collapses every kinematic proxy to a point at the
  origin, which is exactly where the first draft aimed its queries, so the mutation *healed* the
  tests. Fixed with `FinalPosition = (100, 0)`.
- **Queries must probe a body's edges, not its centre.** A lagging proxy still covers the body's
  centre; only the edges fall outside it. Fixed with `PointsAcrossBodyExtent`, which probes the
  centre plus four points at `BodyRadius - 0.5`.

The offset ladder `{0, ±5, ±25, ±60}` is provably load-bearing — each band is the *only* one that
catches a particular mutation. With radius 10 the fat box is ±20 and the tight AABB ±10:

| Offset | Why it is there |
|---|---|
| 0 | Reference case with no history at all. |
| ±5 | Inside the fat box, so the update is skipped. The only band that catches a stale proxy. |
| ±25 | Between 10 and 30, the band where `Contains` and `Overlaps` disagree. |
| ±60 | Far from everything the body used to overlap. |

Collision scenarios use `BarelyTouchingPosition` — the other body only just overlaps once the
moving body arrives. A deeply overlapping pair is found even by a badly lagging broad phase.

### Gap B — cell size invariance (2026-08-03)

Five tests, region `Cell size invariance`, each parameterized over four cell sizes chosen relative
to the 20-unit bodies used: `1×1` and `3×7` (both smaller than a body, so bodies and queries span
many cells), `256×256` (the default), and `10000×10000` (whole scene in one cell — the reference
case where space is effectively not partitioned).

`3×7` is non-square on purpose. `BroadPhaseGridCellSize` is a `SizeD` and non-square cells are a
supported configuration, but before this work nothing in the repo used one.

### Mutation testing

Every gap was validated by deliberately breaking a line and confirming the tests go red. This is
what found the three first-draft tests that passed for the wrong reason, plus one genuine bug in
the draft itself (collision tests were using the wrong origin, so the documented offsets did not
mean what the comment claimed). All mutations reverted; `src/` verified clean.

`RigidBodyData.RecomputeCollider` — the proxy update:

| Mutation | Caught by |
|---|---|
| Kinematic `MoveProxy` deleted | 11 tests |
| `Contains` → `Overlaps` (update fires too late) | 3 tests — only the ±25 cases |
| Static `MoveProxy` deleted | 15 tests |
| Proxy moved to tight `AABB` instead of `BroadPhaseAABB` | 3 tests — only the ±5 cases |

`SpatialGrid` — space partitioning. The "before" column is the physics suite as it stood after
Gap C; the point of Gap B is the two rows where it read *survives*:

| Mutation | Before Gap B | After Gap B |
|---|---|---|
| Canonical-cell check disabled (duplicate pairs) | 42 failures | 11 tests |
| `QueryBounds` `LastQueryId` dedup removed | — | 15 tests |
| `FindCells` collapsed to a single cell | 211 failures | caught |
| `FindCells` cell span clamped to 2×2 | **survives all 1495** | 8 tests (`1×1` and `3×7` only) |
| `FindCell` axis swap (`Width` ↔ `Height`) | **survives all 4177** | 2 tests (`3×7` only) |
| `FindCells` axis swap (`Width` ↔ `Height`) | **survives all 4177** | 2 tests (`3×7` only) |

The clamp mutation was caught by `SpatialGridTests` (5 tests) but was invisible to the entire
physics suite. Both axis swaps survived *every* test in the repo, `SpatialGridTests` included,
because all 80 of its tests use square cells. Both are now caught in `SpatialGridTests` as well —
see [Non-square cells in `SpatialGridTests`](#non-square-cells-in-spatialgridtests-2026-08-05).

Note on the two axis-swap rows: each mutation swaps `Width`/`Height` in *one* function. Swapping
both `FindCell` and `FindCells` together is an equivalent mutant that no test catches or should
catch — it merely transposes the whole grid while keeping placement and lookup in agreement.

One further mutation, `canonicalCell = FindCell(intersection.Max)` instead of `.Min`, survives the
full suite and was **not** treated as a gap. The canonical cell only has to be *some* cell both
proxies occupy; since each proxy's bounds contain both corners of the intersection and `FindCells`
is inclusive at `Max`, both corners always satisfy that. It is an equivalent mutant — a valid
alternative implementation — so no test should reject it.

### Result

`BroadPhaseTests` — 60 tests (2 configuration, 9 history independence with 33 instances, 5 cell
size invariance with 20 instances). Full unit suite green at 4197.

### Deliberately not pursued

- **A centralized white-box invariant check** in shared teardown — for every body, assert the proxy
  exists, lives in the grid matching its `BodyType`, and its bounds contain the body's `AABB`. Needs
  no new production API (`GetProxyData` and `GetBodiesSpan` exist, `InternalsVisibleTo` is in
  place). Not added because the behavioural tests already catch every mutation tried; it would be
  the white-box coupling that was explicitly rejected above, for no demonstrated gain.
- ~~**Non-square cell sizes below `SpatialGrid` level**~~ — **done**, see
  [Non-square cells in `SpatialGridTests`](#non-square-cells-in-spatialgridtests-2026-08-05).
- **The known blind spot of metamorphic testing** — a bug that corrupts every cell size equally is
  invisible to invariance tests. Mitigated by the ~4000 absolute assertions elsewhere in the suite,
  not by these tests.

### Non-square cells in `SpatialGridTests` (2026-08-05)

Both axis swaps were caught only at the physics level, three layers above the defect. `SpatialGrid`
has its own fixture, so they are now caught there too — directly, and with a failure that names the
grid rather than a collider position.

Three tests added, one per query API, each using cell size `10x30`:

| Test | Catches |
|---|---|
| `QueryPoint_ShouldReturnProxy_WhenCellsAreNonSquare` | axis swap in `FindCell` or `FindCells`, individually |
| `QueryOverlappingPairs_ShouldReturnPair_WhenCellsAreNonSquare` | axis swap in `FindCell` or `FindCells`, individually |
| `QueryBounds_ShouldReturnProxyOnlyOnce_WhenCellsAreNonSquare` | multi-cell dedup on non-square cells |

**What these tests actually detect is disagreement between `FindCell` and `FindCells`, not the axis
swap itself.** Applying the swap to *both* functions leaves all 87 tests green, because a uniformly
transposed grid is still a coherent grid — every proxy is stored and looked up under the same
addressing, so nothing observable changes. That is an equivalent mutant, the same category as the
`canonicalCell = .Max` case above. What the tests catch is a swap in *one* function, which
desynchronizes placement from lookup. Since that is the realistic typo, the tests are worth having —
but the property they pin is consistency, not orientation.

The mechanism differs per API and is worth stating precisely, because it is easy to get wrong:

- `QueryPoint` — proxies are placed by `FindCells`, the point is located by `FindCell`. A swap in
  either sends the query to a cell the proxy was never stored in.
- `QueryOverlappingPairs` — placement is by `FindCells`, but the pair is reported only from the cell
  matching `FindCell(intersection.Min)` (`SpatialGrid.cs:599-602`). Both proxies stay in the same
  cell under a swap; what breaks is that the canonical cell moves elsewhere, so the pair is reported
  from a cell it is not stored in — that is, never.
- `QueryBounds` — uses `FindCells` for placement *and* lookup, so any swap applied there is
  self-consistent by construction and cancels out. Confirmed: both a full swap and a `Max`-only
  partial swap leave this test green while failing the other two.

The `QueryBounds` test is kept on its own merit rather than for the axis swaps: it is the only one
covering `QueryBounds` dedup when a proxy spans several non-square cells, and it fails when the
`LastQueryId` dedup is removed.

`SpatialGridTests` — 87 tests. Full unit suite green at 4200.

---

## Resolved: `PhysicsSceneData.cs:283` — proxy destruction (2026-08-07)

The TODO asked how to test that a body's spatial proxy is destroyed together with the body. It is
removed; the behaviour is now covered by
`RigidBody_ShouldPreserveContactsAndBodiesLayoutIntegrity_WhenMultipleCollidingBodiesAreCreatedAndDestroyed`
(`7e9090d8`).

### Why the existing coverage was not enough

Two mutations sit in this `switch`. The first — deleting `StaticGrid.DestroyProxy` — was already
caught by 368 tests, so it looked covered. The second was not, and the reason is structural:

```csharp
if (_kinematicBodyCount > 0 && denseIndex < _staticBodyCount)
{
    SwapBodies(_staticBodyCount, denseIndex);
    denseIndex = _staticBodyCount;
    body = ref bodiesSpan[denseIndex];   // dropping this line destroys the wrong proxy
}
```

Both conditions must hold to reach the rebind: kinematic bodies must exist, *and* the destroyed
static must not be the last one. The integrity test had four kinematic bodies and **no static bodies**,
so `case BodyType.Static` never ran there at all.

Dropping the rebind did fail 12 tests, but all 12 failed in `PhysicsSystemTestsBase.TearDown` —
`Scene.RemoveObserver` tears down every body, and some fixtures happen to have a static-plus-kinematic
population. That is incidental coverage: it reports `TearDown : Invalid proxy id` from tests about
ghost collisions and normal filtering, none of which destroy a body in their own act. It would
disappear with any change to teardown ordering.

### What was added

Four static bodies on the diagonals of the existing four-kinematic layout, at `(±75, ±75)` with
rotations 0.5–0.8, each straddling two adjacent kinematic bodies. Every kinematic body goes from 2 to
4 contacts; every static has 2. Eight new acts (9–16) cycle all four statics through
destroy → assert → `ProcessPhysics` → assert → recreate → assert, mirroring the four kinematic rounds.

The overlap property the test was built on is preserved and strengthened: because each static touches
two kinematic bodies, destroying one splices two contact lists rather than one.

**Cycling all four statics is what makes the test insensitive to ordering.** Only destroying the
last-created static skips the swap branch, so 3 of the 4 rounds reach it regardless of the order
chosen. A single round would have to destroy a specific static — a precondition a later edit could
silently break.

| Mutation | Before | After |
|---|---|---|
| `StaticGrid.DestroyProxy` deleted | caught, but not by this test's own acts | fails in Act 0 |
| `body = ref bodiesSpan[denseIndex]` rebind dropped | **passed this test**; caught only in shared teardown elsewhere | fails in Act 9.2 |

Full unit suite green at 4200.

### Caveat

Both mutations surface as a thrown `ArgumentException: Invalid Rigid Body ID` from
`BroadPhase.DetectCollisions_Kinematic_Vs_Static`, not as a failed assertion. That is inherent to how
a leaked or misdirected proxy becomes observable through the public API: the contact assertions pin
contact bookkeeping, while proxy lifetime rides on the simulation not throwing. The alternative — a
white-box `IsValidProxy` teardown check — is the coupling rejected under
[Deliberately not pursued](#deliberately-not-pursued).

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
- **Test** — `BroadPhaseGridCellSizeTest_Constructor_ShouldThrowException_GivenInvalidBroadPhaseGridCellSize`
  with 6 `[TestCase]`s covering zero on each axis, zero on both, negative on each axis, and
  negative on both. Originally in `TweakingParametersTests`, following the naming and
  Arrange/Act-Assert shape of the sibling `Substeps`, `VelocityIterations`, `PositionIterations`
  and `PenetrationTolerance` constructor tests.

`TweakingParametersTests` green (25 tests passed).

Both changes are now committed as `479ea05a` ("Add test for broad-phase grid cell size"); the
earlier note about them being uncommitted no longer applies.

Later moved to `BroadPhaseTests` and renamed to
`Constructor_ShouldThrowException_GivenInvalidBroadPhaseGridCellSize` when that fixture took
ownership of the option — see
[Resolved: `BroadPhase.cs:9` and `SceneQuery.cs:10`](#resolved-broadphasecs9-and-scenequerycs10--review-related-tests).

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
