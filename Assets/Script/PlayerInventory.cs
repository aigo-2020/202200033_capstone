using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 플레이어가 획득한 아이템 목록을 관리하는 클래스입니다.
/// </summary>
public class PlayerInventory : MonoBehaviour
{
    // 싱글톤 패턴 (어디서든 아이템 추가를 쉽게 호출하기 위함)
    public static PlayerInventory Instance { get; private set; }

    [Header("Inventory Settings")]
    public int maxSlotCount = 3; // 최대 아이템 보유 개수
    private List<ItemData> ownedItems = new List<ItemData>();
    public List<ItemData> OwnedItems => ownedItems;

    private PlayerStats playerStats;

    // 아이템이 추가되거나 변경되었을 때 UI에 알리기 위한 이벤트
    public event Action OnInventoryChanged;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        playerStats = GetComponent<PlayerStats>();
    }

    /// <summary>
    /// 새로운 아이템을 획득하고 스탯에 반영합니다.
    /// </summary>
    public bool AddItem(ItemData newItem)
    {
        if (ownedItems.Count >= maxSlotCount)
        {
            Debug.LogWarning("인벤토리가 가득 찼습니다!");
            return false;
        }

        // 1. 데이터 저장
        ownedItems.Add(newItem);

        // 2. 실제 플레이어 스탯에 반영
        if (playerStats != null)
        {
            playerStats.ApplyItem(newItem);
        }

        // 3. UI 갱신 신호 발송
        OnInventoryChanged?.Invoke();

        Debug.Log($"아이템 획득: {newItem.itemName}");
        return true;
    }

    /// <summary>
    /// 아이템을 제거하고 스탯에서 뺍니다. (나중에 교체 기능을 위해 미리 작성)
    /// </summary>
    public void RemoveItem(ItemData item)
    {
        if (ownedItems.Contains(item))
        {
            ownedItems.Remove(item);
            if (playerStats != null)
            {
                playerStats.RemoveItem(item);
            }
            OnInventoryChanged?.Invoke();
        }
    }
}
