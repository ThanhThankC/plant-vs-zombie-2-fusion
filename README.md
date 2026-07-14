# PVZ2 Fusion

A fan-made **Plants vs. Zombies 2** reimagining built in Unity, featuring a **Fusion system** that lets players combine two plants into a stronger hybrid.

## Demo
 [Watch Demo on YouTube](https://www.youtube.com/watch?v=v3ae7nTWjh8)

---

## Timeline

4/2026 - 7/2026

---

## Gameplay Overview

- Pick plants from a card selection screen, then defend against waves of zombies on a grid-based map.
- Place two compatible plants on the same cell to **fuse** them into a new plant.
- Zombies apply status effects on plants; plants apply status effects on zombies — effects interact and cancel each other.

---

## Fusion System

Plants are fused by placing one on top of another. The result is looked up in `FusionDatabase` via a symmetric `FusionKey(A, B)`.

| Ingredients | Result |
|---|---|
| Peashooter + Peashooter | Repeater |
| Peashooter + Repeater | Splitpea |
| Peashooter + IceStorm | Snowpea |
| Peashooter + Jalapeno | Fireshooter |
| Peashooter + BooShroom | GooPeashooter |
| Sunflower + Sunflower | Twinflower |
| Iceberg + Iceberg | IceStorm |
| Wallnut + Wallnut | Tallnut |
| CherryBomb + Wallnut | Exonut |
| CabbagePult + Jalapeno | PepperPult |
| MelonPult + IceStorm | WinterMelon |
| Pumpkin + Peashooter | PeaVine |

Fusion rules:
- A **Normal** plant can fuse with any other Normal plant.
- A **Support** plant can only fuse with another plant of the same field type.
- If no recipe matches, the incoming plant cannot be placed.

---

## Status Effects

Zombies can carry multiple effects simultaneously. Effects interact via `EffectInteractionTable`.

| Effect | Behavior |
|---|---|
| **Freeze** | Stuns zombie; transitions to Chill on expiry (unless Burn is active) |
| **Chill** | Slows zombie (0.7× speed); blocked if Freeze is already active |
| **Burn (Instant)** | Deals damage; removes Freeze, Chill, Poison |
| **Burn (Infinite)** | Damage over time; removes Freeze, Chill |
| **Poison** | Damage over time; removed by Burn |
| **Butter** | Stuns zombie (lane-switch effect) |
| **Stinky** | Repels zombies to another lane |

**Effect interaction rules:**
- Chill is **blocked** when Freeze is active.
- Freeze removes Chill and Burn Infinite when applied.
- Burn (any) removes cold effects; Burn Instant also removes Poison.
- Butter, Stinky, and Poison **coexist** with cold effects.

---

## Architecture

```
Scripts/
├── Core/           # Singletons, scene loading, drag, sorting, time
├── Data/           # ScriptableObject-style data (PlantData, ZombieData, WaveData…)
├── Effects/        # IEffect, all effect types, ZombieEffectController, EffectInteractionTable
├── Events/         # ScriptableObject event channels (GameEvent / GameEventChannel)
├── Fusion/         # FusionDatabase, FusionKey, FusionDatabaseMono
├── GamePlay/       # GameFlowManager, WaveManager, WinCupController
├── Grid/           # Cell, Zone, GridManager
├── Input/          # DragController, ToolManager, canvas & world input receivers
├── Plants/         # PlantBase, PlantManager, individual plant entities
├── Pool/           # Generic ObjectPool + PoolManager
├── Projectiles/    # ProjectileBase, pea & pult variants
├── Sun/            # Sun pickup, SunManager, SunSpawner
├── UI/             # Cards, deck, HUD, level select, shovel/glove tools
├── VFX/            # PlantEffect, ZombieAsh, SplatProjectile
└── Zombies/        # ZombieBase, movement, animation, visual, body parts
```

Key patterns used:
- **Singleton** — `GameFlowManager`, `PlantManager`, `WaveManager`, etc.
- **Object Pool** — all plants, zombies, projectiles, and VFX reuse pooled instances.
- **ScriptableObject Events** — decoupled communication between systems (e.g. `ZombieDiedEvent`, `LevelClearedEvent`).
- **Spine** — zombie and plant animations via Spine skeletal animation.

---

## Game States

`GameFlowManager` drives the game through these states:

`Idle` → `CardSelection` → `BattleIntro` → `Playing` ⇄ `Paused` → `Win` / `Lose`

---

## Testing

EditMode unit tests (Unity Test Framework):

| Suite | Cases |
|---|---|
| `FusionDatabaseTests` | 35 |
| `EffectInteractionTableTests` | 37 |

Run via **Window → General → Test Runner → EditMode** in the Unity Editor.

---

## Tech Stack

- **Unity 2022.3.32f1** (2D, Android)
- **C#**
- **Spine-Unity 4.2** — skeletal animation
- **DOTween** — tweening & camera animations
- **TextMeshPro** — UI text
- **NUnit** — unit testing

---

## Getting Started

**Requirements:**
- Unity **2022.3.32f1**
- Spine-Unity package **4.2** — import via Unity Package Manager or from [esotericsoftware.com](http://esotericsoftware.com/spine-unity-download)
- Android Build Support module (nếu build cho Android)

**Run in Editor:**
1. Clone repo và mở bằng Unity Hub, chọn đúng version `2022.3.32f1`.
2. Mở scene chính trong `Assets/Scenes/`.
3. Nhấn **Play**.

**Build Android:**
1. Vào **File → Build Settings**, chọn platform **Android**.
2. **Switch Platform** → **Build & Run**.
