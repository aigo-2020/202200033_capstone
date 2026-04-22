using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 게임 플레이 중 상시 노출되는 메인 HUD를 관리합니다.
/// 체력(HP), 재화(Money), 현재 장착된 아이템 슬롯만 표시합니다.
/// </summary>
public class PlayerUI : MonoBehaviour
{
    [Header("References")]
    public PlayerStats playerStats;

    [Header("HUD Elements")]
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI stageText;

    [Header("Item Slot HUD")]
    [Tooltip("상시 노출될 아이템 슬롯 이미지 배열입니다. (최대 슬롯 수만큼 설정)")]
    public Image[] itemSlotImages;
    public Sprite emptySlotSprite;

    void Start()
    {
        // 인벤토리 변경 이벤트 구독 (슬롯 UI 업데이트용)
        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.OnInventoryChanged += UpdateItemSlots;
        }
        
        UpdateItemSlots();
    }

    void Update()
    {
        if (playerStats == null) return;

        UpdateBasicHUD();
        UpdateStageDisplay();
    }

    /// <summary>
    /// 체력, 재화 등 기본적인 HUD 정보를 갱신합니다.
    /// </summary>
    void UpdateBasicHUD()
    {
        if (hpText != null)
            hpText.text = $"HP: {playerStats.currentHp:F0} / {playerStats.maxHp.GetValue():F0}";

        if (moneyText != null)
            moneyText.text = $"Gold: {playerStats.money}";
    }

    /// <summary>
    /// 현재 스테이지 정보를 갱신합니다.
    /// </summary>
    void UpdateStageDisplay()
    {
        if (stageText != null && StageManager.Instance != null)
        {
            stageText.text = $"Stage: {StageManager.Instance.currentStage}";
        }
    }

    /// <summary>
    /// 현재 소유한 아이템 아이콘으로 슬롯을 업데이트합니다.
    /// (기존 InventoryUI의 기능을 HUD로 통합)
    /// </summary>
    public void UpdateItemSlots()
    {
        if (PlayerInventory.Instance == null || itemSlotImages == null) return;

        var ownedItems = PlayerInventory.Instance.OwnedItems;

        for (int i = 0; i < itemSlotImages.Length; i++)
        {
            if (i < ownedItems.Count)
            {
                itemSlotImages[i].sprite = ownedItems[i].icon;
                itemSlotImages[i].enabled = true;
                
                Color c = itemSlotImages[i].color;
                c.a = 1f;
                itemSlotImages[i].color = c;
            }
            else
            {
                if (emptySlotSprite != null)
                {
                    itemSlotImages[i].sprite = emptySlotSprite;
                    itemSlotImages[i].enabled = true;
                    
                    // 빈 슬롯은 약간 투명하게 처리 가능
                    Color c = itemSlotImages[i].color;
                    c.a = 0.5f;
                    itemSlotImages[i].color = c;
                }
                else
                {
                    itemSlotImages[i].enabled = false;
                }
            }
        }
    }

    private void OnDestroy()
    {
        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.OnInventoryChanged -= UpdateItemSlots;
        }
    }
}
