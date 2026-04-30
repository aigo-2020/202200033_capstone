using UnityEngine;
using StatSystem;
using System.Collections.Generic;

public class PlayerStats : MonoBehaviour
{
    [Header("Health")]
    public Stat maxHp = new Stat(70f);      // 최대 체력
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
    public int killCount = 0;               // 누적 적 처치 수

    [Header("SpecUp Levels")]
    public int attackLevel = 0;
    public int defenseLevel = 0;
    public int agilityLevel = 0;

    private PlayerInventory inventory;
    
    // 장착된 모든 효과 인스턴스들을 습득 순서대로 관리
    private List<ItemEffect> activeEffects = new List<ItemEffect>();

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
        for (int i = 0; i < activeEffects.Count; i++)
        {
            activeEffects[i].OnUpdate(this);
        }
    }

    // 아이템 효과 적용 (PlayerInventory에서 호출됨)
    public void ApplyItem(ItemData item)
    {
        Debug.Log($"[PlayerStats] 아이템 적용 시도: {item.itemName}");
        // 1. 단순 스탯 변화 적용
        foreach (var change in item.statChanges)
        {
            Stat targetStat = GetStatByType(change.statType);
            if (targetStat != null)
            {
                targetStat.AddModifier(new StatModifier(change.value, change.modifierType, item));
            }
        }

        // 2. 특수 효과 인스턴스 생성 및 리스트 추가 (습득 순서 보존)
        List<ItemEffect> instances = item.InitializeEffects(this);
        Debug.Log($"[PlayerStats] {item.itemName}으로부터 {instances.Count}개의 효과 인스턴스 생성됨");
        foreach (var instance in instances)
        {
            instance.SourceItem = item; // 소스 아이템 설정
            activeEffects.Add(instance);
            instance.OnEquip(this);
            Debug.Log($"[PlayerStats] 효과 등록 완료: {instance.name}");
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

        // 2. 해당 아이템으로부터 생성된 효과들 정리 (현재는 리스트 전체 초기화 시점이 아니므로 
        // 특정 아이템의 인스턴스만 찾아 제거하는 로직이 필요할 수 있음. 
        // 간단히 하기 위해 일단 스탯 위주로 처리하거나, 인스턴스 리스트를 순회하며 Source 체크 필요)
        
        currentHp = Mathf.Clamp(currentHp, 0, maxHp.GetValue());
    }

    public void TakeDamage(float incomingDamage)
    {
        // 1. 피격 컨텍스트 생성
        DamageContext context = new DamageContext(incomingDamage);

        // 2. 모든 효과 인스턴스에게 피격 신호 전달 및 데미지 수정 기회 제공
        for (int i = 0; i < activeEffects.Count; i++)
        {
            activeEffects[i].OnTakeDamage(this, context);
        }

        // 3. 최종 수정된 데미지에 방어력 계산 및 실제 체력 감소
        float finalDamage = Mathf.Max(1f, context.modifiedDamage - defense.GetValue());
        currentHp -= finalDamage;
        
        Debug.Log($"플레이어 피격! 원본 데미지: {incomingDamage}, 수정된 데미지: {context.modifiedDamage}, 최종 데미지: {finalDamage}");

        if (currentHp <= 0) Die();
    }

    /// <summary>
    /// 플레이어가 적을 처치했을 때 호출됩니다.
    /// </summary>
    public void OnKillEnemy()
    {
        killCount++;
        
        for (int i = 0; i < activeEffects.Count; i++)
        {
            activeEffects[i].OnKillEnemy(this);
        }
        
        Debug.Log($"적 처치! 현재 누적 킬수: {killCount}");
    }

    // 외부(PlayerAttack 등)에서 공격 성공 신호를 보낼 때 사용
    public void OnAttackSuccess(IDamageable target, float initialDamage)
    {
        Debug.Log($"[PlayerStats] OnAttackSuccess 트리거 발동! 대상: {target}, 초기 데미지: {initialDamage}, 활성화된 효과 수: {activeEffects.Count}");
        
        // 1. 공격 컨텍스트 생성
        AttackContext context = new AttackContext(target, initialDamage);

        // 2. 모든 효과 인스턴스에게 공격 신호 전달 및 데미지 수정 기회 제공
        for (int i = 0; i < activeEffects.Count; i++)
        {
            activeEffects[i].OnAttack(this, context);
        }

        // 3. 최종 수정된 데미지로 적 타격
        context.target.TakeDamage(context.damage);
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
        
        // 체력 회복 시 효과 트리거 호출
        for (int i = 0; i < activeEffects.Count; i++)
        {
            activeEffects[i].OnHeal(this, amount);
        }
    }

    public void OnStageStart()
    {
        for (int i = 0; i < activeEffects.Count; i++)
        {
            activeEffects[i].OnStageStart(this);
        }
    }

    public void OnStageClear()
    {
        for (int i = 0; i < activeEffects.Count; i++)
        {
            activeEffects[i].OnStageClear(this);
        }
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
