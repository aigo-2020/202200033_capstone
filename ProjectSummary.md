# Project Context & Summary (for Gemini CLI)

## 🛠 Development Environment
*   **Unity Version:** 6000.3.13f1
*   **Target Platform:** PC / 2D

## Project Overview
*   **Genre:** Top-down 2D manual shooting roguelike.
*   **Core Loop:** Stage Clear -> Event Portal -> Reward Selection (Items/Stats) -> Inventory Update -> Stat Calculation -> Next Stage.

## 📂 System Architecture & Scripts

### 📂 Directory Convention (Important)
*   **`Assets/Script/`**: 일반적인 시스템 매니저, 플레이어 로직, UI 등 핵심 코드 위치.
*   **`Assets/ScriptableObjects/MonsterData/`**: **주의!** 몬스터 데이터(`MonsterData.cs`)뿐만 아니라, 개별 몬스터의 **MonoBehaviour 로직(`Monster1.cs`, `Monster2.cs` 등)이 함께 위치함.**
*   **`Assets/Script/Stat/`**: 스탯 시스템 관련 핵심 로직.

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
*   **`RewardUIManager.cs` (Refactored)**: 
    *   Exclusive to **Spec-Up Rewards**. 
    *   Ensures a fixed selection of [Attack, Defense, Agility] in each event.
    *   Picks random `SpecUpData` from category-specific pools.
*   **`ShopUIManager.cs` (Planned)**: Handles gold-based item purchasing (Stages 4-5).
*   **`RewardCard.cs`**: Handles both `ItemData` and `SpecUpData` setup and acquisition.

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
1.  **Monster Expansion**: Create more `MonsterData` assets and prefabs.
2.  **Gold Drop Logic**: Probability-based gold drops from monsters.
3.  **Combat Juice**: Hit Flash, Camera Shake, SFX.
