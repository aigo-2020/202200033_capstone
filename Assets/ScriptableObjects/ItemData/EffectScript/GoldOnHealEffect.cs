using UnityEngine;

/// <summary>
/// 체력이 회복될 때마다(OnHeal 트리거) 골드를 획득하는 효과입니다.
/// </summary>
[CreateAssetMenu(fileName = "GoldOnHealEffect", menuName = "ItemEffects/GoldOnHeal")]
public class GoldOnHealEffect : ItemEffect
{
    public int goldAmount = 1;

    public override void OnHeal(PlayerStats player, float amount)
    {
        // 체력이 얼마가 회복되든 상관없이 골드 획득
        player.money += goldAmount;
        
        Debug.Log($"[연쇄 효과] 회복 감지! 골드 {goldAmount} 획득 (현재 골드: {player.money})");
    }
}
