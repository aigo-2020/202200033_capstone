# Project Context & Summary (for Gemini CLI)

## 🛠 Development Environment
*   **Unity Version:** 6000.3.13f1
*   **Target Platform:** PC / 2D
*   **Current Version:** ver 0.0.2

## Project Overview
*   **Genre:** Top-down 2D manual shooting roguelike.
*   **Core Loop:** Stage Clear -> Event UI (Reward/Shop) -> Inventory Update -> Stat Calculation -> Next Stage. (Portal removed for fast tempo)

## 📂 System Architecture & Scripts

### 📂 Directory Convention (Important)
*   **`Assets/Script/`**: Core code location including general system managers, player logic, and UI.
*   **`Assets/Script/GameManager.cs`**: **(New)** Global state management (Lobby, Playing, GameOver) and scene transitions.
*   **`Assets/Script/StageManager.cs`**: **(Enhanced)** Manages stage flow, monster spawning, and combat-related UIs (Inventory, Reward, Shop).
*   **`Assets/ScriptableObjects/MonsterData/`**: **Caution!** Contains not only `MonsterData.cs` but also individual monster **MonoBehaviour logic (`Monster1.cs`, `Monster2.cs`, etc.).**
*   **`Assets/Script/Stat/`**: Core logic related to the stat system.

### 1. Stat System (Assets/Script/Stat/) - **Core Logic**
*   **`StatType.cs`**: `MaxHp`, `Damage`, `Speed`, `FireRate`, `Range`, `Defense`.
*   **`StatModifier.cs`**: `Value`, `Type`, `Source`.
*   **`Stat.cs`**: `GetValue()` for final calculation. Added `OnValueChanged` event for optimization.

### 2. Item & Effect System (Refactored)
*   **`ItemData.cs` (Container)**: Defines identity (Name, Icon, Price). Creates effect instances for each player (`InitializeEffects`).
*   **`SpecUpData.cs` (New)**: Slots-independent stat upgrades. Categorized into Attack, Defense, and Agility. Scales with category levels.
*   **`ItemEffect.cs` (Base)**: Abstract blueprint for special mechanics.
*   **`PlayerStats.cs`**: 
    *   Manages item effect instances using a `Dictionary`. 
    *   Manages `attackLevel`, `defenseLevel`, `agilityLevel` for spec-up scaling.
    *   `ApplySpecUp()` handles permanent stat modifiers and level progression.

### 3. UI System
*   **`RewardUIManager.cs` (Enhanced)**: 
    *   Handles **Spec-Up Rewards** (Attack, Defense, Agility selection).
    *   Displays current category levels and requires an **Exit button** click to proceed.
*   **`ShopUIManager.cs` (Implemented)**: 
    *   Handles gold-based item purchasing and **Reroll** mechanics.
    *   Reroll cost increases exponentially (20%) per use and resets on new game.
*   **`RewardCard.cs` / `ShopCard.cs`**: Handle card-specific setup, acquisition/purchase logic, and visual feedback (Sold Out/Selected).

## 🔄 Interaction Map
1.  **Stage Clear** -> **Portal Collision** -> **`StageEventManager.CheckAndTriggerEvent()`** -> **Selective UI (Reward or Shop)**.
2.  **Stat Change** -> `Stat.OnValueChanged` -> `ItemEffect.HandleStatChange`.
3.  **TakeDamage** -> `PlayerStats.TakeDamage` -> `ItemEffect.OnTakeDamage`.
4.  **AttackSuccess** -> `PlayerStats.OnAttackSuccess` -> `ItemEffect.OnAttack`.

## 💡 Future Gameplay Flow (Planned)
1.  **Stages 1-3**: Stat Upgrade Reward (Pick 1 from 3: Attack/Defense/Agility).
2.  **Stage 4**: Shop Event (Buy items with gold).
3.  **Stage 5**: Boss Stage & Reward.
4.  **Rounds**: 3 Rounds (15 stages) + Final Boss (16th stage) = Clear.

## Future Priorities

### 👾 1. Round & Boss Flow
*   **Goal**: Create a structured game loop with increasing difficulty and climactic battles.
*   **Key Features**:
    *   **Round Loop**: 5 stages per round. Transition to the next round with increased difficulty after Stage 5.
    *   **Difficulty Scaling**: Apply multipliers to monster stats (HP, Damage) based on the current round number.
    *   **Boss Stage**: Spawn a unique boss monster at Stage 5 or at the end of the final round (e.g., Stage 16).
    *   **Victory Condition**: Implement a 'Game Clear' UI and transition to the result screen after defeating the final boss.
