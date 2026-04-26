using UnityEngine;
using StatSystem;
using System.Collections.Generic;

public class PlayerStats : MonoBehaviour
{
    [Header("Health")]
    public Stat maxHp = new Stat(50f);      // 최대 체력
    public float currentHp = 30f;           // 현재 체력
    public Stat defense = new Stat(0f);     // 방어력

    [Header("Combat Stats")]
    public Stat damage = new Stat(10f);     // 공격력
    public Stat fireRate = new Stat(1f);    // 연사력 (초당 공격 횟수)
    public Stat range = new Stat(10f);      // 사거리

    [Header("Movement Stats")]
    public Stat speed = new Stat(5f);       // 이동 속도

    [Header("Resources")]
    public int money = 0;                   // 인게임 재화

    [Header("SpecUp Levels")]
    public int attackLevel = 0;
    public int defenseLevel = 0;
    public int agilityLevel = 0;

    private PlayerInventory inventory;
    
    // 각 아이템 데이터별로 생성된 실제 효과 인스턴스들을 관리 (데이터 독립성 확보)
    private Dictionary<ItemData, List<ItemEffect>> itemEffectInstances = new Dictionary<ItemData, List<ItemEffect>>();

    void Awake()
    {
        inventory = GetComponent<PlayerInventory>();
    }

    void Start()
    {
        currentHp = Mathf.Clamp(currentHp, 0, maxHp.GetValue());
    }

    void Update()
    {
        // 소유한 모든 효과 인스턴스들의 Update 실행
        foreach (var effectList in itemEffectInstances.Values)
        {
            foreach (var effect in effectList)
            {
                effect.OnUpdate(this);
            }
        }
    }

    // 아이템 효과 적용 (PlayerInventory에서 호출됨)
    public void ApplyItem(ItemData item)
    {
        // 1. 단순 스탯 변화 적용
        foreach (var change in item.statChanges)
        {
            Stat targetStat = GetStatByType(change.statType);
            if (targetStat != null)
            {
                targetStat.AddModifier(new StatModifier(change.value, change.modifierType, item));
            }
        }

        // 2. 특수 효과 인스턴스 생성 및 초기화
        List<ItemEffect> instances = item.InitializeEffects(this);
        if (instances.Count > 0)
        {
            itemEffectInstances[item] = instances;
        }

        currentHp = Mathf.Clamp(currentHp, 0, maxHp.GetValue());
    }

    /// <summary>
    /// 슬롯을 차지하지 않는 스펙업 데이터를 적용하고 해당 카테고리 레벨을 올립니다.
    /// </summary>
    public void ApplySpecUp(SpecUpData data)
    {
        if (data == null) return;

        int currentLvl = GetCategoryLevel(data.category);
        float finalValue = data.GetCalculatedValue(currentLvl);

        Stat targetStat = GetStatByType(data.targetStat);
        if (targetStat != null)
        {
            // 스펙업은 영구적이므로 별도의 만료 없이 Modifier 추가
            targetStat.AddModifier(new StatModifier(finalValue, ModifierType.Flat, data));
        }

        // 해당 카테고리 성장 기록
        LevelUpCategory(data.category);

        currentHp = Mathf.Clamp(currentHp, 0, maxHp.GetValue());
        Debug.Log($"{data.category} 스펙업 적용: {data.specName} (+{finalValue}). 현재 {data.category} 레벨: {GetCategoryLevel(data.category)}");
    }

    public int GetCategoryLevel(SpecUpCategory category)
    {
        switch (category)
        {
            case SpecUpCategory.Attack: return attackLevel;
            case SpecUpCategory.Defense: return defenseLevel;
            case SpecUpCategory.Agility: return agilityLevel;
            default: return 0;
        }
    }

    private void LevelUpCategory(SpecUpCategory category)
    {
        switch (category)
        {
            case SpecUpCategory.Attack: attackLevel++; break;
            case SpecUpCategory.Defense: defenseLevel++; break;
            case SpecUpCategory.Agility: agilityLevel++; break;
        }
    }

    // 아이템 효과 제거 (PlayerInventory에서 호출됨)
    public void RemoveItem(ItemData item)
    {
        // 1. 단순 스탯 변화 제거
        foreach (var change in item.statChanges)
        {
            Stat targetStat = GetStatByType(change.statType);
            if (targetStat != null)
            {
                targetStat.RemoveAllModifiersFromSource(item);
            }
        }

        // 2. 관리 중인 특수 효과 인스턴스 정리 및 제거
        if (itemEffectInstances.TryGetValue(item, out List<ItemEffect> instances))
        {
            foreach (var effect in instances)
            {
                effect.OnUnequip(this);
            }
            itemEffectInstances.Remove(item);
        }

        currentHp = Mathf.Clamp(currentHp, 0, maxHp.GetValue());
    }

    public void TakeDamage(float incomingDamage)
    {
        // 1. 모든 효과 인스턴스에게 피격 신호 전달 (보호막, 골드 획득 등)
        foreach (var effectList in itemEffectInstances.Values)
        {
            foreach (var effect in effectList)
            {
                effect.OnTakeDamage(this, incomingDamage);
            }
        }

        // 2. 방어력 계산 및 실제 체력 감소
        float finalDamage = Mathf.Max(10f, incomingDamage - defense.GetValue());
        currentHp -= finalDamage;
        
        Debug.Log($"플레이어 피격! 데미지: {finalDamage}. 현재 체력: {currentHp}");

        if (currentHp <= 0) Die();
    }

    // 외부(PlayerAttack 등)에서 공격 성공 신호를 보낼 때 사용
    public void OnAttackSuccess(IDamageable target, float damageDealt)
    {
        foreach (var effectList in itemEffectInstances.Values)
        {
            foreach (var effect in effectList)
            {
                effect.OnAttack(this, target, damageDealt);
            }
        }
    }

    private Stat GetStatByType(StatType type)
    {
        switch (type)
        {
            case StatType.MaxHp: return maxHp;
            case StatType.Damage: return damage;
            case StatType.Speed: return speed;
            case StatType.FireRate: return fireRate;
            case StatType.Range: return range;
            case StatType.Defense: return defense;
            default: return null;
        }
    }

    public void Heal(float amount)
    {
        currentHp += amount;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp.GetValue());
    }

    void Die()
    {
        Debug.Log("플레이어 사망!");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
    }
}
