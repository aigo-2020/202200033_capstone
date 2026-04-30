using UnityEngine;
using StatSystem;

// [공격 컨텍스트] 공격자, 대상, 데미지 정보 (아이템 효과에 의해 수정 가능)
public class AttackContext
{
    public IDamageable target;
    public float damage;
    // 필요 시 크리티컬 여부 등을 확장 가능
    public AttackContext(IDamageable target, float damage) { this.target = target; this.damage = damage; }
}

// [피격 컨텍스트] 들어온 데미지와 수정된 데미지 정보
public class DamageContext
{
    public float rawDamage;      // 방어력 계산 전 원본 데미지
    public float modifiedDamage; // 아이템 효과에 의해 수정될 데미지
    public DamageContext(float damage) { this.rawDamage = damage; this.modifiedDamage = damage; }
}

/// <summary>
/// 모든 아이템 특수 효과의 부모가 되는 추상 클래스입니다.
/// 새로운 메커니즘을 만들고 싶다면 이 클래스를 상속받아 필요한 트리거만 구현하세요.
/// </summary>
public abstract class ItemEffect : ScriptableObject
{
    [Header("Base Settings")]
    [TextArea] public string effectDescription;

    /// <summary>
    /// 이 효과를 생성한 원본 아이템 데이터입니다. (제거 시 참조용)
    /// </summary>
    public ItemData SourceItem { get; set; }

    /// <summary>
    /// 아이템을 습득하여 효과 인스턴스가 생성될 때 호출됩니다.
    /// </summary>
    public virtual void OnEquip(PlayerStats player) { }

    /// <summary>
    /// 아이템을 버리거나 효과가 제거될 때 호출됩니다.
    /// </summary>
    public virtual void OnUnequip(PlayerStats player) { }

    /// <summary>
    /// 플레이어가 적을 공격했을 때 호출됩니다. context.damage를 수정하여 최종 데미지에 영향을 줄 수 있습니다.
    /// </summary>
    public virtual void OnAttack(PlayerStats player, AttackContext context) { }

    /// <summary>
    /// 플레이어가 적에게 맞았을 때 호출됩니다. context.modifiedDamage를 수정하여 받는 피해를 조절할 수 있습니다.
    /// </summary>
    public virtual void OnTakeDamage(PlayerStats player, DamageContext context) { }

    /// <summary>
    /// 플레이어가 적을 처치했을 때 호출됩니다.
    /// </summary>
    public virtual void OnKillEnemy(PlayerStats player) { }

    /// <summary>
    /// 플레이어가 체력을 회복했을 때 호출됩니다.
    /// </summary>
    public virtual void OnHeal(PlayerStats player, float amount) { }

    /// <summary>
    /// 매 프레임 업데이트가 필요할 때 사용합니다.
    /// </summary>
    public virtual void OnUpdate(PlayerStats player) { }

    /// <summary>
    /// 새로운 스테이지가 시작될 때 호출됩니다.
    /// </summary>
    public virtual void OnStageStart(PlayerStats player) { }

    /// <summary>
    /// 스테이지를 클리어했을 때 호출됩니다.
    /// </summary>
    public virtual void OnStageClear(PlayerStats player) { }
}
