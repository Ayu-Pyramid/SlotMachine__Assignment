# Slot Machine Game — Unity Assignment

## Game Overview

A classic 3-reel slot machine built in Unity. Each reel spins independently with a randomized stop time, creating a staggered left-to-right finish. The player wins when all three reels land on the same symbol on the pay line.

**Features implemented:**
- Object-oriented reel and symbol architecture (`Reel.cs`, `SymbolData.cs`, `SlotMachine.cs`)
- Randomized spin duration and stagger per reel for a natural, non-mechanical feel
- ScriptableObject-based symbol definitions (easy to add/edit symbols without touching code)
- Win detection and payout system, including Wild symbol substitution logic
- Balance tracking and bet deduction per spin
- Clean separation of concerns: each reel manages its own spin state; the `SlotMachine` manager only orchestrates and evaluates results

## Symbols & Payouts

| Symbol | Payout |
|---|---|
| Cherry | 10 |
| Lemon | 15 |
| Bell | 25 |
| Bar | 50 |
| Seven | 100 |
| Wild | 200 (substitutes for any symbol) |

## How to Run

### Option A: WebGL Build
1. Navigate to `/Build/WebGL` in this repository.
2. Open `index.html` in a browser (Chrome or Firefox recommended).
3. Click **Spin** to play.

*(Note: WebGL build to be added — see note below.)*

### Option B: Unity Editor
1. Clone this repository.
2. Open the project folder in Unity Hub (Unity 6000.3.10f1 or later recommended).
3. Open `Assets/Scenes/MainScene.unity`.
4. Press Play, click the Spin button.

## Project Structure

```
Assets/
 ├─ Scripts/          # Core gameplay scripts (Reel, SlotMachine, SymbolData)
 ├─ Prefabs/          # Reel prefab
 ├─ ScriptableObjects/# Symbol data assets (Cherry, Lemon, Bell, Bar, Seven, Wild)
 ├─ Sprites/          # Symbol artwork
 ├─ Scenes/           # Main game scene
 └─ UI/                # Canvas and UI elements
```

## Thought Process & Approach

The goal was a clean separation between **reel behavior** and **game logic**:

- `Reel.cs` only knows how to spin and report back a result — it has no concept of winning, payouts, or balance.
- `SymbolData.cs` is a ScriptableObject so new symbols (or rebalanced payouts) can be added entirely from the Unity Editor, without code changes.
- `SlotMachine.cs` is the single source of truth for game state — it owns the balance, listens for each reel to finish, and only evaluates a win once every reel has reported in. This avoids race conditions between reels finishing at different times (since each reel has its own randomized stop delay).

The win-check logic treats the first non-Wild symbol on the pay line as the "target" and requires every other symbol to either match it or be Wild — this scales naturally if more reels or Wild symbols are added later.

## Known Limitations / Next Steps

- WebGL build is still being finalized due to a local environment issue with the WebGL module install.
- Visual reel "blur" animation during spin is achieved via fast sprite-swapping rather than true motion blur — a future iteration could use a sprite-strip/scroll approach for a more authentic feel.
- No sound effects yet (`Assets/Sounds/` reserved for future use).
