using System.Collections.Generic;
using UnityEngine;
using StatSystem;

[CreateAssetMenu(fileName = "NewItem", menuName = "ScriptableObjects/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("Item Info")]
    public string itemName;
    [TextArea] public string description;
    public Sprite icon;
    public int price;

    [Header("Stat Changes (Simple)")]
    public List<StatChange> statChanges;

    [Header("Special Effects (Logic)")]
    public List<ItemEffect> specialEffects;

    // 인스펙터에서 설정을 편하게 하기 위한 구조체
    [System.Serializable]
    public struct StatChange
    {
        public StatType statType;
        public float value;
        public ModifierType modifierType;
    }

    /// <summary>
    /// 아이템 효과들을 복제하여 새로운 인스턴스 리스트로 반환합니다.
    /// (데이터 오염 방지를 위해 플레이어마다 개별 인스턴스를 가짐)
    /// </summary>
    public List<ItemEffect> InitializeEffects(PlayerStats player)
    {
        List<ItemEffect> instances = new List<ItemEffect>();

        if (specialEffects != null)
        {
            foreach (var effect in specialEffects)
            {
                if (effect != null)
                {
                    // SO 에셋을 복제하여 개별 인스턴스로 만듦
                    ItemEffect instance = Instantiate(effect);
                    instance.OnEquip(player);
                    instances.Add(instance);
                }
            }
        }

        return instances;
    }
}
