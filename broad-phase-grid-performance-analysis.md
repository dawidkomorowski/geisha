# Broad-Phase Grid Performance Regression Analysis

## Summary

Commit `2d8a1f57` ("Use grid pair query in broad-phase collisions") replaced the
brute-force O(n²) kinematic-vs-kinematic loop with `SpatialGrid.QueryOverlappingPairs`.
This improved sparse scenes but **regressed dense scenes**, most notably
`IterationSetup_SimulatePhysics_10Seconds_638K_Fall_CR_Enabled`.

The regression is **not a bug** — it is the grid pair-query being applied to a
scenario that is pathological for it.

## The scenario

`IterationSetup_SimulatePhysics_10Seconds_638K_Fall_CR_Enabled`
(`benchmark/Geisha.MicroBenchmark/PhysicsSystemBenchmarks.cs:131-164`) creates a
**dense pile of ~638 small kinematic boxes** (20×20) packed into a ~1100-wide bin
bounded by 3 static walls, then drops them under gravity with collision *response*
enabled. Over 600 steps they settle into a tightly packed heap.

Crucially, the grid cell size is **250×250**
(`PhysicsSystemBenchmarks.cs:36`), while each body is only 20×20. So a single cell
holds dozens of bodies, and the whole pile spans only a handful of cells.

## The two implementations being compared

**Old (brute force)** — `DetectCollisions_Kinematic_Vs_Kinematic`, the O(n²) double
loop. For each of the `i<j` pairs it did cheap early-outs *first*:
`EnableCollisionDetection`, layer/mask, then `TestAABB`. Only surviving pairs reached
the expensive overlap test.

**New (grid pair query)** — `SpatialGrid.QueryOverlappingPairs`
(`src/Geisha.Engine/Core/Spatial/SpatialGrid.cs:271-317`) walks each cell's linked
list and does an `i<j` loop **within each cell's bucket**, then routes every
AABB-overlapping pair through `NarrowPhase.DetectCollision`.

## Why the grid version is slower here

### 1. With a 250-unit cell and 20-unit bodies, the grid degenerates into brute force — but a more expensive brute force.

All ~638 bodies collapse into a few buckets. The inner `node1×node2` loop in
`QueryOverlappingPairs` is still effectively O(k²) over the bucket, where k is nearly
n. So the grid gives essentially **zero broad-phase pruning** in this scene, while
adding overhead the plain double loop never had:

- **Per candidate pair, `Intersect` + `IsValid` + `FindCell` + a `BuildCellKey`
  comparison** (`SpatialGrid.cs:296-302`) for the canonical-cell dedup. `Intersect`
  (`AABB2D.cs:240`) does two `Vector2.Max/Min` (constructing a throwaway `AABB2D`),
  versus the old `TestAABB` which was a single `Overlaps` — 4 comparisons, no struct
  allocation, no floor/division. `FindCell` does two `Math.Floor` + two divisions on
  *every* overlapping pair.
- The old inner loop did the `EnableCollisionDetection` and layer/mask early-outs
  *before* touching AABBs. The grid does the AABB `Intersect` and canonical-cell math
  *first*, then hands off to `NarrowPhase.DetectCollision`, which **re-checks**
  `EnableCollisionDetection`, layer/mask, **and the AABB overlap again**
  (`NarrowPhase.cs:13-26`). So overlapping-AABB pairs get their AABB tested twice.

### 2. The gather-then-process split adds a materialization pass the old code didn't have.

The old loop tested and created contacts inline. The new path first pushes every
candidate pair into the `[ThreadStatic]` `_pairScratchBuffer` `List<Pair>`
(`BroadPhase.cs:162-166`), then iterates it again. In a dense heap the number of
AABB-overlapping pairs is large, so this is a big list to fill and re-scan. Each
processing-pass iteration also does **two `GetProxyData` calls** (each a bounds-check
+ version validate + struct copy of `ProxyData`, `SpatialGrid.cs:184-195`) plus
**two `GetBodyData` calls** (sparse→dense indirection with validation,
`PhysicsSceneData.cs:334-346`) — indirection the span-based double loop didn't need,
since it held `ref` to the bodies directly.

### 3. Settling makes it worse over the 10 s run.

As the pile compacts, more bodies share cells and more AABBs genuinely overlap, so
both the candidate-pair count and the redundant per-pair work grow — exactly where
`CR_Enabled` spends its time.

## Why other benchmarks didn't regress (or improved)

The `10000S` / `200K_1000S_Sparse` scenes spread bodies over a 10000×10000 area.
There the 250-cell grid actually partitions space, buckets are small, and the pair
query prunes the O(n²) explosion — that's the win. The dense fall scene is the
opposite regime: everything in a few oversized cells, so pruning ≈ 0 and only the
per-pair overhead remains.

## Root causes, ranked

1. **Cell size (250) is ~12× the body size (20) for this scene** → no spatial
   pruning; the grid replicates brute force with heavier per-pair cost.
2. **Redundant work per pair**: canonical-cell recomputation (`FindCell`/floor/div) +
   `Intersect` on every overlap, then AABB + filters re-tested inside
   `NarrowPhase.DetectCollision`.
3. **Extra indirection**: gather-into-list then `GetProxyData`×2 + `GetBodyData`×2 per
   pair, versus direct `ref` spans before.
4. **Ordering of early-outs**: cheap filters (collision-enabled, layer/mask) now run
   *after* the expensive geometric/cell work, not before.

## Suggested directions

- **Cheapest, most impactful**: cut redundant work in the hot loop. In
  `QueryOverlappingPairs`, skip the `Intersect`/`FindCell` canonical dedup when a
  cheaper key works (e.g. compare proxy indices or track a per-pair query id), and
  hoist the `EnableCollisionDetection`/layer-mask checks earlier so filtered pairs
  never reach `Intersect`.
- Avoid the double AABB test: the query already computes overlap via
  `Intersect().IsValid`, so `NarrowPhase` could skip its own AABB re-check when called
  from the pair path.
- **Tune the benchmark's cell size** relative to body size — a cell near the body size
  restores real pruning for the dense scene. Worth confirming this benchmark's cell
  size wasn't just left at the sparse-scene value.
- Consider passing `ref RigidBodyData` / proxy payload through the handler to avoid
  the `GetProxyData`+`GetBodyData` round trips (the code's own TODO notes this awaits
  .NET 9 ref-struct interfaces).
