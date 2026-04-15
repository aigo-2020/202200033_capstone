# Project Context & Summary (for Gemini CLI)

## Project Overview
*   **Genre:** Top-down 2D manual shooting roguelike.
*   **Core Loop:** Stage Clear -> Event Portal -> Reward Selection (Items/Stats) -> Inventory Update -> Stat Calculation -> Next Stage.

## 📂 System Architecture & Scripts

### 1. Stat System (Assets/Script/Stat/) - **Core Logic**
*   **`StatType.cs`**: `MaxHp`, `Damage`, `Speed`, `FireRate`, `Range`, `Defense`.
*   **`StatModifier.cs`**: `Value`, `Type`, `Source`. (Value updated to be modifiable for real-time effects).
*   **`Stat.cs`**: `GetValue()` for final calculation. Added `OnValueChanged` event for optimization.

### 2. Item & Effect System (Refactored)
*   **`ItemData.cs` (Container)**: Defines identity (Name, Icon, Price). Creates effect instances for each player (`InitializeEffects`).
*   **`ItemEffect.cs` (Base)**: Abstract blueprint for special mechanics (`OnEquip`, `OnUpdate`, `OnTakeDamage`, `OnAttack`).
*   **`EffectScript/`**: Individual logic scripts (e.g., `Effect_AtkByMaxHp`, `Effect_GoldOnHit`).
*   **`PlayerStats.cs`**: Manages own item effect instances using a `Dictionary`. Dispatches signals (TakeDamage, Attack) to these instances.

### 3. UI System
*   **`StageEventManager.cs` (Planned)**: Central hub to trigger different event UIs based on stage numbers (Reward/Shop).
*   **`RewardUIManager.cs` (Refactored from UIManager)**: Handles free 1-out-of-3 reward selection (Stages 1-3).
*   **`ShopUIManager.cs` (Planned)**: Handles gold-based item purchasing (Stages 4-5).
*   **`RewardCard.cs`**: Individual reward logic. Handles `ItemData` addition.

## 🔄 Interaction Map
1.  **Stage Clear** -> **Portal Collision** -> **`StageEventManager.CheckAndTriggerEvent()`** -> **Selective UI (Reward or Shop)**.
2.  **Stat Change** -> `Stat.OnValueChanged` -> `ItemEffect.HandleStatChange` (Optimization).
2.  **TakeDamage** -> `PlayerStats.TakeDamage` -> `ItemEffect.OnTakeDamage`.
3.  **AttackSuccess** -> `PlayerStats.OnAttackSuccess` -> `ItemEffect.OnAttack`.

## 💡 Future Gameplay Flow (Planned)
1.  **Stages 1-3**: Stat Upgrade Reward (Pick 1 from 3: Offense/Defense/Agility).
2.  **Stage 4**: Shop Event (Buy items with gold).
3.  **Stage 5**: Boss Stage & Reward.
4.  **Rounds**: 3 Rounds (15 stages) + Final Boss (16th stage) = Clear.

## Future Sessions: Priorities
1.  **Monster Expansion**: Create more `MonsterData` assets and prefabs.
2.  **Gold Drop Logic**: Probability-based gold drops (random amount) from monsters.
3.  **Reward Logic (Refinement)**: Introduce simple stat-up SOs for stages 1-3 (separate from ItemData).
4.  **Combat Juice**: Hit Flash, Camera Shake, SFX.
