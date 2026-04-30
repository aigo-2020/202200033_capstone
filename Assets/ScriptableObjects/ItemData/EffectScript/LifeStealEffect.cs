using UnityEngine;

/// <summary>
/// 적을 맞출 때마다(OnAttack 트리거) 체력을 고정으로 1 회복하는 효과입니다.
/// </summary>
[CreateAssetMenu(fileName = "LifeStealEffect", menuName = "ItemEffects/LifeSteal")]
public class LifeStealEffect : ItemEffect
{
    public override void OnAttack(PlayerStats player, AttackContext context)
    {
        // 적을 맞추면 무조건 체력 1 회복
        player.Heal(1f);
        
        // 디버그 로그로 발동 확인
        Debug.Log($"[아이템 효과] 흡혈! 체력 1 회복 (현재 체력: {player.currentHp})");
    }
}
