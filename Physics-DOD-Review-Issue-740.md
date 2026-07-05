# Code Review — Refactor internal Physics Engine 2D to Data-Oriented Design (Issue #740)

**Branch:** `740-refactor-internal-physics-engine-2d-to-data-oriented-design-architecture`
**Base:** `master`
**Scope reviewed:** internal `PhysicsEngine2D`, `Physics/Systems`, supporting `Core/Math` and `Core/Memory` changes, tests and benchmarks.
**Diff size:** ~53 files, +3487 / −1128.

> This document is a working review to drive follow-up improvements. Items are tagged
> **[Bug]**, **[Perf]**, **[Arch]**, **[Nit]**, **[Docs]** with a rough priority (P1 highest).

---

## 1. Summary / Verdict

This is a high-quality, well-sequenced refactor that delivers on the core intent of the ticket.
The new architecture moves the 2D physics runtime to a genuinely data-oriented layout:

- **Handle/ID indirection** via `RigidBodyId` / `PhysicsSceneId` (generational, index + version).
- **Contiguous storage** for bodies and contacts (`List<T>` + `CollectionsMarshal.AsSpan`).
- **Unmanaged structs** for hot-path data (`RigidBodyData`, `ContactData`, `SimulationParameters`,
  `SensorOverlap`), enforced at compile time through the `IUnmanaged<T>` marker.
- **Unmanaged handle wrappers at the API boundary** (`RigidBody2D`, `PhysicsScene2D`) that resolve
  through the static `Physics2D` manager — exactly the pattern proposed in the issue.
- **Zero-allocation steady-state** simulation and pooled/struct-based query handlers.

The acceptance criteria are essentially met. The remaining gaps are (a) a couple of real but
low-severity defects, (b) a hot-path handle-resolution cost that partially offsets the cache-locality
win, and (c) documentation debt (`IUnmanaged`, `AABB2D`) plus allocation-documentation follow-ups
tracked separately. None of these block the
refactor; they are the natural next iteration.

### Acceptance-criteria checklist

| Criterion | Status | Note |
|---|---|---|
| Contiguous memory-oriented storage | ✅ | `List<RigidBodyData>` / `List<ContactData>` via spans |
| Handle/ID indirection for major resources | ✅ | sparse→dense with free list |
| Hot-path data as unmanaged structs | ✅ | compile-time enforced |
| API-boundary wrappers = unmanaged handle structs | ✅ | `RigidBody2D`, `PhysicsScene2D` |
| No allocation on normal simulate/update | ✅ | verify with `MemoryDiagnoser` benchmarks |
| No allocation when creating bodies in steady state | ⚠️ | amortized (List growth); tile creation allocates |
| Storage & allocation rules documented | ⚠️ | partial — thread-safety doc present; allocation behavior documentation tracked in R10/R11 |
| Existing behavior remains correct | ✅ | extensive test migration + new lifetime/validity tests |

---

## Review tracking checklist (living tracker)

Single source of truth for iteration. Update the **Status** cell as items are addressed:
⬜ open · 🟡 in progress · ✅ resolved · ⏸️ deferred / won't fix. Each `ID` links to the
detailed finding by section.

| ID | Status | Prio | Type | Item | Ref |
|----|--------|------|------|------|-----|
| R5 | ⬜ | P1 | Perf | Reduce hot-path handle resolution in solvers (store dense index in `ContactData`, add unchecked accessor, hoist spans) | §4.1 |
| R1 | ✅ | P2 | Bug | Remove duplicated `SetAngularVelocity` assignment | §3.1 |
| R2 | ⬜ | P2 | Bug | Include `Version` (and scene) in `SensorOverlapCache.CacheKey` | §3.2 |
| R3 | ⬜ | P2 | Arch | Document/guard `PhysicsSceneData` thread-affinity invariant | §3.3 |
| R6 | ⬜ | P2 | Perf | Revisit `RigidBodyData` layout: collapse dual transformed colliders / hot-cold split | §4.2 |
| R7 | ⏸️ | P2 | Perf | Spatial broadphase to replace O(n²) (likely separate issue) | §4.3 |
| R13 | ✅ | P2 | Docs | XML docs for public `AABB2D`; document `IUnmanaged<T>` purpose | §6 |
| R14 | ⬜ | P2 | Arch | Extract helpers + targeted tests for `DestroyContactsForBody` | §6 |
| R4 | ⬜ | P3 | Test | Test destroyed-sensor `End` event via removed-collider cache | §3.4 |
| R8 | ⬜ | P3 | Perf | Hoist repeated `AsSpan` materialization out of inner loops | §4.4 |
| R9 | ⬜ | P3 | Perf | Skip second `RecomputeCollider` when position solver made no correction | §4.5 |
| R10 | ⬜ | P3 | Alloc | Pre-size `_bodies` / `_bodyIndices` lists to avoid load-time churn | §5 |
| R11 | ⬜ | P3 | Docs | Document `TileMap` per-tile `List` allocation behavior | §5 |
| R12 | ⬜ | P3 | Test | Assert `Allocated == 0` in simulation benchmarks | §7 |
| R15 | ⏸️ | P3 | Nit | Remove dead `ColliderSpanQueryHandler` or link its TODO to an issue | §6 |
| R16 | ✅ | P3 | Nit | Consider constructing `BodiesView` on the fly instead of storing | §6 |
| R17 | ✅ | P3 | Nit | Note maintenance cost of duplicated collider-type dispatch chains | §6 |
| R18 | ⏸️ | P3 | Nit | Link remaining `// TODO` markers to tracking issues | §6 |
| R19 | ⬜ | P3 | Test | Same-frame index reuse during active sensor overlap | §7 |
| R20 | ⬜ | P3 | Test | Destroy body with multiple contacts; assert link consistency | §7 |

**Progress:** 4 / 20 resolved · P1: 0/1 · P2: 2/7 · P3: 2/12

---

## 2. Architecture assessment

The layering is clean and each stage has a single responsibility:

- `Physics2D` — static facade (`Scene`, `Body`) resolving handles to storage.
- `PhysicsSceneData` — the storage core: scene slot table, sparse body table, dense body array,
  contact array, sensor cache, tile map.
- `SimulationPipeline` — orchestrates the substep loop (solve velocity → integrate → recompute →
  detect → solve position → recompute → events).
- `CollisionDetection` / `ContactManager` / `ContactSolver` / `KinematicIntegration` / `SceneQuery` —
  independent stages operating on `ref PhysicsSceneData`.

Strong points:

- **Dense array partitioning** (`[static | kinematic]`) with `GetStaticBodiesSpan()` /
  `GetKinematicBodiesSpan()` lets broadphase iterate exactly the pairs it needs and keeps the
  narrowphase branch-predictable. Swap-on-insert / swap-on-remove maintains the invariant, guarded by
  `Debug.Assert(BodiesLayoutIsValid())`.
- **Intrusive doubly-linked contact list** stored inside a contiguous `List<ContactData>` with
  swap-remove — good DOD; avoids per-contact heap objects.
- **Generational handles** correctly invalidate stale references (validity tests confirm scene and
  body recreation cases).
- **Compile-time `unmanaged` enforcement**: `interface IUnmanaged<T> where T : unmanaged` used as a
  marker forces each implementing struct to remain blittable. This is an elegant, low-cost guard
  against accidentally introducing a managed field into hot data. (It just needs documenting — see
  §5.)

---

## 3. Bugs / correctness

### 3.1 · `R1` **[Bug][P2]** Duplicated assignment in `Physics2D.Body.SetAngularVelocity`
`src/Geisha.Engine/Physics/PhysicsEngine2D/Internal/Physics2D.cs`

```csharp
body.AngularVelocity = value;

body.AngularVelocity = value;   // <-- duplicated
```
Harmless functionally, but it is a copy-paste defect and should be removed. Its presence suggests the
setters were written quickly; worth a quick scan of the sibling setters for similar slips.

### 3.2 · `R2` **[Bug][P2]** `SensorOverlapCache.CacheKey` ignores handle version (and scene)
`src/Geisha.Engine/Physics/PhysicsEngine2D/Internal/SensorOverlapCache.cs`

`CacheKey` is built from `RigidBodyId.Index` only. Because sparse indices are recycled via the free
list, a destroyed sensor body and a newly created body can share the same index. If that reuse happens
in the same frame while overlapping the same partner, the stale entry is matched as `Updated`, which
can **suppress the destroyed body's `End` event and the new body's `Begin` event**. Cross-scene safety
happens to hold only because each scene owns its own cache instance.

This is an edge case (requires same-frame index reuse with an overlapping partner), but it is a real
correctness hole in the event stream. Recommend incorporating `Version` (and optionally the scene) into
the key, or asserting that recycled indices cannot alias a live overlap within a frame.

### 3.3 · `R3` **[Arch][P2]** Thread-safety relies on an undocumented-at-call-site invariant
`PhysicsSceneData` exposes a shared `static PhysicsSceneData[] Scenes`. `Create`/`Destroy` take a lock,
but `Get`/`IsValid` read the shared array with no lock or memory barrier. The class comment documents
the "one scene per thread" model, which makes this safe *in practice*, but:

- There is no runtime guard (e.g. owning-thread assert) — a misuse (sharing a scene across threads, or
  handing a freshly created scene to another thread without a barrier) can read torn/stale data
  silently.
- Consider an owning-thread id captured on `Create` plus a `Debug.Assert` in `Get`, or an explicit note
  in the public-facing docs that scene handles are thread-affine.

### 3.4 · `R4` **[Nit][P3]** Destroyed-body `End` events depend on the removed-collider cache
`PhysicsSystem.InvokeEventCallbacks` correctly falls back to `GetRemovedColliderByIdOrNull` when a body
was destroyed mid-overlap, and `ClearRemovedCollidersCache()` runs at end of `ProcessPhysics`. This is
a good design, but it is subtle and only lightly asserted (`Debug.Assert(collider1 is not null …)`).
Worth an explicit test that destroys a sensor body while overlapping and asserts a single `End` event
with a valid collider reference (the lifetime tests may already cover part of this — confirm).

---

## 4. Performance

### 4.1 · `R5` **[Perf][P1]** Hot-path handle resolution dominates the solver loops
`ContactSolver.SolveVelocityConstraints` / `SolvePositionConstraints` (and the debug/contact walk)
resolve bodies per contact, per iteration, per substep:

```csharp
ref var body1 = ref scene.GetBodyData(contact.Link1.BodyId);
ref var body2 = ref scene.GetBodyData(contact.Link2.BodyId);
```

`GetBodyData` performs, every call:
1. `IsValidBodyId` → `CollectionsMarshal.AsSpan(_bodyIndices)` + version compare (with a throw path),
2. a second span materialization for `_bodies`,
3. sparse→dense indirection.

Total cost scales as `Substeps × (VelocityIterations + PositionIterations) × contactCount × 2`. This
repeated validation + double indirection partially cancels the cache-locality benefit the refactor is
chasing. Suggestions (increasing effort):

- Store the **dense body index** (not just the `RigidBodyId`) in `ContactData.Link`, refreshed when
  bodies are swapped (the swap bookkeeping already touches these). Solver then indexes `bodiesSpan`
  directly.
- Provide an internal `GetBodyDataUnchecked` (no validity/throw) for trusted internal loops; keep the
  validated `GetBodyData` for the API boundary.
- Hoist `GetBodiesSpan()` once per solver pass instead of per contact.

### 4.2 · `R6` **[Perf][P2]** `RigidBodyData` is a large AoS struct mixing hot and cold fields
A single struct holds position/rotation/velocity **and** collision layer/mask, sensor flag, collider
sizes, contact bookkeeping, AABB, **and both** `TransformedCircleCollider` and
`TransformedRectangleCollider` (only one is ever valid for a given body). Consequences:

- Integration (`KinematicIntegration`) touches only position/rotation/velocity but pulls the whole
  struct through cache.
- Storing both transformed colliders wastes bytes per element in every iteration.

This is a pragmatic AoS choice and is fine as a first cut, but it leaves cache locality on the table.
Future options: split hot/cold into parallel arrays (SoA), or at least collapse the two transformed
colliders (they are mutually exclusive). Worth measuring the struct size and its effect on the
narrowphase sweep.

### 4.3 · `R7` **[Perf][P2]** Broadphase is O(n²) with no spatial acceleration
`DetectCollisions_Kinematic_Vs_Kinematic` (pairwise) and `_Kinematic_Vs_Static` (each kinematic × every
static) have no spatial partitioning. `TileMap` exists but is used only for collision-normal filtering,
not broadphase pruning. For scenes with many static tiles this is the dominant scaling cost.

This is arguably **out of scope** for #740 (which targets data layout, not the algorithm), but it is the
single biggest runtime-scaling risk and should be captured as a follow-up (e.g. reuse the tile grid, or
a uniform grid / sort-and-sweep on the dense arrays which are already contiguous and partitioned).

### 4.4 · `R8` **[Perf][P3]** Repeated span materialization
`GetBodiesSpan()` / `GetBodyIndicesSpan()` / `GetContactsSpan()` call `CollectionsMarshal.AsSpan` on
every invocation and are called very frequently inside loops. Individually cheap, but hoisting them out
of inner loops (or caching for the duration of a stage) removes redundant work.

### 4.5 · `R9` **[Perf][P3]** `RecomputeCollider` runs twice per substep for every kinematic body
Each call builds a `Transform2D`, converts to matrix, transforms the shape, and recomputes the AABB.
Necessary for correctness after integration and after position correction, but a candidate for
micro-optimization (e.g. skip the second recompute when the position solver made no correction — the
existing TODOs about early-out on solved constraints would enable this).

---

## 5. Allocations (vs. ticket goal of no runtime allocation)

- **Steady-state `Simulate`:** no allocations — spans, pooled arrays, and `stackalloc` throughout.
  Good, and covered by `[MemoryDiagnoser]` benchmarks. ✅ (Recommend asserting `Allocated == 0` in a
  benchmark/regression to lock this in.)
- **Query API:** span overloads use `ArrayPool<Collider2DComponent>.Shared.Rent/Return` and struct
  query handlers → no allocation and no boxing (generic constraint `where T : struct`). ✅
- **Body creation:** `R10` `_bodies` and `_bodyIndices` are created as `new List<T>()` (capacity 0), so early
  insertions reallocate until warmed; steady state is allocation-free (amortized doubling). Consider
  pre-sizing like `Contacts` (256) to avoid churn during scene load. ⚠️
- **Tile creation:** `R11` `TileMap` uses `Dictionary<TilePosition, List<RigidBodyId>>` and allocates a new
  `List` per newly occupied tile. Documented conceptually as an "explicit event", but tile-heavy level
  loading will allocate; document this behavior under `R11`. ⚠️

---

## 6. Code quality / maintainability

- `R13` **[Docs][P2]** `IUnmanaged<T>` and `AABB2D` both carry `// TODO: Add documentation`. `AABB2D` is now
  used in the **public** query API surface (`IPhysicsSystem`), so it should get XML docs to match the
  rest of the public math types before release. `IUnmanaged<T>`'s real purpose (compile-time `unmanaged`
  enforcement) is non-obvious for an empty interface and should be documented so it is not "cleaned up"
  as dead code later.
- `R14` **[Arch][P2]** `PhysicsSceneData.DestroyContactsForBody` is long and performs intricate manual
  linked-list surgery over the swapped-remove contact array. The commit history
  (`Fix contact link updates when destroying contacts`, `Update body contact indices after swap`,
  `Document bug: body indexes not updated after swap`) shows this was bug-prone. It is well-commented
  and asserted, but is a prime candidate for extracting small helpers (unlink-link, fix-links-after-swap)
  and dedicated unit tests targeting the swap-remap paths directly.
- `R15` **[Nit][P3]** Dead commented-out `ColliderSpanQueryHandler` block in `PhysicsSystem.cs`. Keep it only
  if the associated TODO (`.NET 9` ref-struct interfaces) is tracked in an issue; otherwise remove.
- `R16` **[Nit][P3]** `PhysicsScene2D` stores a `BodiesView` with a `// TODO: Construct it on the fly` — minor;
  the struct is cheap, but it means `PhysicsScene2D` is not purely `{ Id }`.
- `R17` **[Nit][P3]** Large collider-type `if/else` dispatch is duplicated across `TestOverlap`,
  `TestOverlapWithMtv`, and `ContactManager.ComputeManifold`. The `CollisionDetection` header comment
  explicitly warns this is intentional for inlining/perf — fine, but note the maintenance cost (adding a
  collider shape touches several parallel chains).
- `R18` **[Nit][P3]** Numerous `// TODO` markers remain (debug drawing relocation, `.NET 9/10` migrations,
  minimum-velocity threshold, early-out on solved constraints). All reasonable; ensure they are linked
  to tracking issues so they are not lost.

---

## 7. Tests & benchmarks

- Good new coverage of the handle model: `PhysicsEngine2DTests` exercises scene/body validity, destroy,
  and stale-handle-after-recreation (slot reuse) — the exact invariants the DOD model depends on.
- `RigidBodyLifetimeTests` grew substantially (+477) and contact assertions were added, which is the
  right place to catch the linked-list/swap regressions noted above.
- Query, sensor, tile, state-sync, and tweaking-parameter tests were migrated to the new API.
- `PhysicsSceneQueryBenchmarks` / `PhysicsSystemBenchmarks` use `[MemoryDiagnoser]` — please make the
  no-allocation guarantee explicit (assert `Allocated == 0`) so a future regression is caught
  automatically rather than by eyeballing benchmark output.

Suggested additional tests:
- `R12` Assert `Allocated == 0` in the simulation `[MemoryDiagnoser]` benchmarks to lock in the
  no-allocation guarantee automatically.
- `R19` Same-frame body destroy + create that reuses an index while a sensor overlap is active (guards §3.2).
- `R20` Destroy a body that is party to multiple contacts, asserting all peer link indices remain consistent
  (targets `DestroyContactsForBody`).

---

## 8. Prioritized follow-up list

> All follow-up items are consolidated into the **[Review tracking checklist](#review-tracking-checklist-living-tracker)**
> near the top of this document (single source of truth — update statuses there). This section keeps only
> the recommended ordering for a focused iteration.

1. **P1** — `R5` Reduce hot-path handle resolution in the solvers.
2. **P2** — `R1` duplicated `SetAngularVelocity` · `R2` `CacheKey` version · `R3` thread-affinity guard ·
   `R6` `RigidBodyData` layout · `R7` broadphase · `R13` docs · `R14` `DestroyContactsForBody` refactor+tests.
3. **P3** — `R4` `R8` `R9` `R10` `R11` `R12` `R15` `R16` `R17` `R18` `R19` `R20` (nits, micro-opts, extra tests).

---

*Reviewed against issue #740 goals: DOD layout, contiguous memory, unmanaged structs, handle-based
access, minimal GC pressure. Overall the change is a solid, well-tested foundation; the follow-ups above
are refinements, not blockers.*
