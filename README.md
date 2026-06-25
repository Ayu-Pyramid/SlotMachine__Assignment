# Slot Machine Game — Unity Assignment

## Game Overview

A classic 3-reel slot machine built in Unity. Each reel spins independently with a randomized stop time, creating a staggered left-to-right finish. The player wins when all three reels land on the same symbol on the pay line.

**Features implemented:**
- Object-oriented reel and symbol architecture (`Reel.cs`, `SymbolData.cs`, `SlotMachine.cs`)
- Randomized spin duration and stagger per reel for a natural, non-mechanical feel
- ScriptableObject-based symbol definitions (easy to add/edit symbols without touching code)
- Win detection and payout system
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

## Bonus Features

The win-checking logic (`CheckWinCondition` in `SlotMachine.cs`) is written to support a Wild symbol that substitutes for any other symbol on the pay line — the architecture is already in place (`SymbolData.isWild` flag, substitution logic in the win check), but a Wild symbol asset was not included in this submission's final symbol set. This was a deliberate scope decision to keep the five-symbol set simple and consistent; adding a Wild symbol later only requires creating one more `SymbolData` asset with `isWild` checked and adding it to the symbol pool — no code changes needed.

## How to Run

### Option A: WebGL Build
1. Navigate to `/Build` in this repository.
2. Download/clone the repo, then open `index.html` (inside `/Build`) in a browser.
3. Click **Spin** to play.

*(Note: WebGL builds need to be served by a local web server to run correctly in a browser — opening `index.html` directly from a file browser, or viewing it on GitHub's file viewer, will not execute it. Running it via Unity Editor, per Option B, is the most reliable way to verify gameplay.)*

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
 ├─ ScriptableObjects/# Symbol data assets (Cherry, Lemon, Bell, Bar, Seven)
 ├─ Sprites/          # Symbol artwork
 ├─ Scenes/           # Main game scene
 └─ UI/                # Canvas and UI elements
Build/                # WebGL build output
```

## Thought Process & Approach

The goal was a clean separation between **reel behavior** and **game logic**:

- `Reel.cs` only knows how to spin and report back a result — it has no concept of winning, payouts, or balance.
- `SymbolData.cs` is a ScriptableObject so new symbols (or rebalanced payouts) can be added entirely from the Unity Editor, without code changes.
- `SlotMachine.cs` is the single source of truth for game state — it owns the balance, listens for each reel to finish, and only evaluates a win once every reel has reported in. This avoids race conditions between reels finishing at different times (since each reel has its own randomized stop delay).

The win-check logic treats the first symbol on the pay line as the "target" and requires every other symbol to match it — this scales naturally if more reels or a Wild symbol are added later (the substitution logic for Wilds is already written, see Bonus Features above).

