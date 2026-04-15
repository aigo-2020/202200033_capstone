using UnityEngine;
using StatSystem;

/// <summary>
/// 최대 체력에 비례하여 공격력을 실시간으로 증가시키는 효과입니다.
/// 이벤트 기반으로 작동하여 성능 최적화를 꾀했습니다.
/// </summary>
[CreateAssetMenu(fileName = "Effect_AtkByMaxHp", menuName = "ScriptableObjects/ItemEffects/AtkByMaxHp")]
public class Effect_AtkByMaxHp : ItemEffect
{
    [Header("Effect Settings")]
    [Range(0f, 1f)]
    public float hpToAtkRatio = 0.1f; // 최대 체력의 10%를 공격력으로 전환

    private StatModifier currentModifier;
    private PlayerStats cachedPlayer;

    public override void OnEquip(PlayerStats player)
    {
        cachedPlayer = player;
        currentModifier = new StatModifier(0f, ModifierType.Flat, this);
        player.damage.AddModifier(currentModifier);

        // 이벤트 구독: 최대 체력이 바뀔 때만 재계산함 (성능 최적화)
        player.maxHp.OnValueChanged += HandleHpChanged;
        
        UpdateBonus();
    }

    public override void OnUnequip(PlayerStats player)
    {
        // 이벤트 해제 중요! (메모리 누수 및 에러 방지)
        if (player.maxHp != null)
        {
            player.maxHp.OnValueChanged -= HandleHpChanged;
        }

        player.damage.RemoveAllModifiersFromSource(this);
        currentModifier = null;
        cachedPlayer = null;
    }

    // OnUpdate는 이제 필요 없으므로 비워두거나 구현하지 않음
    public override void OnUpdate(PlayerStats player) { }

    private void HandleHpChanged()
    {
        if (cachedPlayer != null) UpdateBonus();
    }

    private void UpdateBonus()
    {
        if (currentModifier == null || cachedPlayer == null) return;

        float currentMaxHp = cachedPlayer.maxHp.GetValue();
        float newBonus = currentMaxHp * hpToAtkRatio;

        currentModifier.Value = newBonus;
        // 주의: 여기서 player.damage.NotifyChange()가 자동으로 호출되므로 
        // 최종 공격력 수치를 사용하는 다른 곳들도 연쇄적으로 업데이트됩니다.
    }
}
