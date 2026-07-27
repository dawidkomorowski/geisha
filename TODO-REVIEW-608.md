# TODO Review — Branch `608-add-broad-phase-in-physics-engine-2d`

Scope: all `TODO` items created, modified or moved by the changes on this branch
(diff range `master...HEAD`, 80 commits, 20 files changed).

Generated: 2026-07-27

---

## New TODOs added on this branch (13)

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

### `src/Geisha.Engine/Core/Math/AABB2D.cs`

| Line | TODO |
|---|---|
| 206 | Add documentation. *(for new `IsValid` property)* |
| 238 | Add documentation. *(for new `Intersect` method)* |

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

### `src/Geisha.Engine/Physics/Systems/PhysicsSystem.cs`

| Line | TODO |
|---|---|
| 55 | Add tests and validation for `BroadPhaseGridCellSize`. |

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
   - `AABB2D.IsValid` and `AABB2D.Intersect` — XML docs.
   - `PhysicsConfiguration.BroadPhaseGridCellSize` — docs, tests and validation
     (validation tracked separately in `PhysicsSystem.cs:55`).
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
