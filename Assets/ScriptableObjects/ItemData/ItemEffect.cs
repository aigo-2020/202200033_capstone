using UnityEngine;
using StatSystem;

/// <summary>
/// 모든 아이템 특수 효과의 부모가 되는 추상 클래스입니다.
/// 새로운 메커니즘을 만들고 싶다면 이 클래스를 상속받아 구현하세요.
/// </summary>
public abstract class ItemEffect : ScriptableObject
{
    [Header("Base Settings")]
    [TextArea] public string effectDescription; // 효과에 대한 간단한 설명 (디버깅/툴팁용)

    /// <summary>
    /// 아이템을 장착(인벤토리 추가)했을 때 최초 1회 호출됩니다.
    /// </summary>
    public virtual void OnEquip(PlayerStats player) { }

    /// <summary>
    /// 아이템을 해제(인벤토리 제거)했을 때 호출됩니다.
    /// </summary>
    public virtual void OnUnequip(PlayerStats player) { }

    /// <summary>
    /// 플레이어가 적을 공격했을 때(데미지를 입혔을 때) 호출됩니다.
    /// </summary>
    /// <param name="target">공격당한 대상</param>
    /// <param name="damage">입힌 데미지</param>
    public virtual void OnAttack(PlayerStats player, IDamageable target, float damage) { }

    /// <summary>
    /// 플레이어가 적에게 맞았을 때(데미지를 입었을 때) 호출됩니다.
    /// </summary>
    /// <param name="incomingDamage">받은 데미지 원본 수치</param>
    public virtual void OnTakeDamage(PlayerStats player, float incomingDamage) { }

    /// <summary>
    /// 플레이어가 적을 처치했을 때 호출됩니다.
    /// </summary>
    public virtual void OnKillEnemy(PlayerStats player) { }

    /// <summary>
    /// 매 프레임 업데이트가 필요할 때 사용합니다. (예: 펫 이동, 쿨타임 체크 등)
    /// </summary>
    public virtual void OnUpdate(PlayerStats player) { }
}
