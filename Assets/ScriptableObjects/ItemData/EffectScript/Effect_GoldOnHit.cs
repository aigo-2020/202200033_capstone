using UnityEngine;

/// <summary>
/// 플레이어가 피격되었을 때 플레이어의 최대 체력에 비례하여 골드를 획득하는 효과입니다.
/// </summary>
[CreateAssetMenu(fileName = "Effect_GoldOnHit", menuName = "ScriptableObjects/ItemEffects/GoldOnHit")]
public class Effect_GoldOnHit : ItemEffect
{
    [Header("Effect Settings")]
    [Range(0f, 1f)]
    public float hpToGoldRatio = 0.05f; // 최대 체력의 5%를 골드로 전환 (기본값)
    public float chance = 1.0f;           // 획득 확률 (0~1)

    public override void OnTakeDamage(PlayerStats player, float incomingDamage)
    {
        // 1. 확률 체크
        if (Random.value <= chance)
        {
            // 2. 플레이어의 현재 "최종 최대 체력"을 가져옴
            float currentMaxHp = player.maxHp.GetValue();
            
            // 3. 골드 양 계산 (최대 체력 * 비율)
            int goldAmount = Mathf.FloorToInt(currentMaxHp * hpToGoldRatio);
            
            // 4. 플레이어 재화 증가
            player.money += goldAmount;
            
            Debug.Log($"[아이템 효과] 피격! 최대 체력({currentMaxHp})의 {hpToGoldRatio * 100}%인 {goldAmount} 골드 획득! (현재 골드: {player.money})");
        }
    }
}
