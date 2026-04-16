using UnityEngine;
using StatSystem;

public enum SpecUpCategory
{
    Attack,     // 공격
    Defense,    // 수비
    Agility     // 민첩
}

/// <summary>
/// 슬롯을 차지하지 않는 단순 스탯 강화 데이터를 정의합니다.
/// </summary>
[CreateAssetMenu(fileName = "NewSpecUp", menuName = "ScriptableObjects/SpecUpData")]
public class SpecUpData : ScriptableObject
{
    [Header("SpecUp Info")]
    public string specName;
    [TextArea] public string description;
    public Sprite icon;

    [Header("Category & Stat")]
    public SpecUpCategory category;
    public StatType targetStat;

    [Header("Value Scaling")]
    public float baseValue;         // 레벨 0일 때 기본 증가 수치
    public float growthPerLevel;    // 레벨당 추가 증가치 (레벨업 시 가산됨)

    /// <summary>
    /// 현재 카테고리 레벨에 따른 최종 증가 수치를 계산합니다.
    /// 공식: 기본 수치 + (현재 카테고리 레벨 * 레벨당 증가치)
    /// </summary>
    public float GetCalculatedValue(int currentCategoryLevel)
    {
        return baseValue + (currentCategoryLevel * growthPerLevel);
    }
}
