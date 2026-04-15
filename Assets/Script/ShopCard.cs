using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 상점에서 아이템을 구매할 때 사용하는 카드 클래스입니다.
/// </summary>
public class ShopCard : MonoBehaviour
{
    [Header("UI References")]
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI priceText;    // 상점 전용: 가격 표시

    [Header("State Settings")]
    public GameObject soldOutOverlay;    // 구매 후 '품절' 표시용

    private ItemData currentItem;
    private Button cardButton;
    private bool isSoldOut = false;

    void Awake()
    {
        cardButton = GetComponent<Button>();
        if (soldOutOverlay != null) soldOutOverlay.SetActive(false);
    }

    public void Setup(ItemData item)
    {
        currentItem = item;
        isSoldOut = false;
        if (cardButton != null) cardButton.interactable = true;
        if (soldOutOverlay != null) soldOutOverlay.SetActive(false);

        if (iconImage != null) iconImage.sprite = item.icon;
        if (nameText != null) nameText.text = item.itemName;
        if (descriptionText != null) descriptionText.text = item.description;
        
        // 가격 표시
        if (priceText != null) 
        {
            priceText.text = $"{item.price} G";
            
            // 돈이 부족하면 텍스트 색상을 빨간색으로 변경하는 등 시각적 피드백 가능
            PlayerStats player = GameObject.FindWithTag("Player")?.GetComponent<PlayerStats>();
            if (player != null && player.money < item.price)
            {
                priceText.color = Color.red;
            }
            else
            {
                priceText.color = Color.white;
            }
        }
    }

    /// <summary>
    /// 카드 클릭 시 호출 (구매 로직 실행)
    /// </summary>
    public void OnClick()
    {
        if (isSoldOut || currentItem == null) return;

        PlayerStats player = GameObject.FindWithTag("Player")?.GetComponent<PlayerStats>();
        if (player == null) return;

        // 1. 돈이 충분한지 확인
        if (player.money >= currentItem.price)
        {
            // 2. 돈 차감 및 아이템 추가
            player.money -= currentItem.price;
            bool success = PlayerInventory.Instance.AddItem(currentItem);

            if (success)
            {
                Debug.Log($"{currentItem.itemName} 구매 완료! 남은 돈: {player.money}");
                isSoldOut = true;
                
                if (cardButton != null) cardButton.interactable = false;
                if (soldOutOverlay != null) soldOutOverlay.SetActive(true);
            }
        }
        else
        {
            Debug.Log("돈이 부족합니다!");
            // TODO: UI 상에서 "돈 부족" 알림 연출 추가 가능
        }
    }
}
