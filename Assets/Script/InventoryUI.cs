using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 화면에 고정된 아이템 슬롯(3개)의 이미지를 관리하는 클래스입니다.
/// </summary>
public class InventoryUI : MonoBehaviour
{
    [Header("Slot UI Elements")]
    [Tooltip("에디터에서 미리 만들어둔 3개의 Image 컴포넌트를 순서대로 넣어주세요.")]
    public Image[] slotImages;

    [Tooltip("슬롯이 비었을 때 보여줄 기본 스프라이트 (없으면 투명하게 처리)")]
    public Sprite emptySlotSprite;

    void Start()
    {
        // 인벤토리 변경 이벤트 구독
        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.OnInventoryChanged += UpdateInventoryDisplay;
        }

        // 초기 상태 업데이트
        UpdateInventoryDisplay();
    }

    /// <summary>
    /// 인벤토리 데이터에 맞춰 슬롯 이미지를 갱신합니다.
    /// </summary>
    public void UpdateInventoryDisplay()
    {
        if (PlayerInventory.Instance == null || slotImages == null) return;

        var ownedItems = PlayerInventory.Instance.OwnedItems;

        for (int i = 0; i < slotImages.Length; i++)
        {
            // i번째 슬롯에 아이템이 있다면
            if (i < ownedItems.Count)
            {
                slotImages[i].sprite = ownedItems[i].icon;
                slotImages[i].enabled = true; // 이미지가 보이게 설정
                
                // 아이콘의 색상을 원래대로 (투명도 방지)
                Color c = slotImages[i].color;
                c.a = 1f;
                slotImages[i].color = c;
            }
            else
            {
                // i번째 슬롯에 아이템이 없다면
                if (emptySlotSprite != null)
                {
                    slotImages[i].sprite = emptySlotSprite;
                    slotImages[i].enabled = true;
                }
                else
                {
                    // 빈 슬롯 이미지가 없으면 투명하게 숨김
                    slotImages[i].enabled = false;
                }
            }
        }
    }

    private void OnDestroy()
    {
        // 메모리 누수 방지를 위한 이벤트 구독 해제
        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.OnInventoryChanged -= UpdateInventoryDisplay;
        }
    }
}
