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
        // 코드에서 버튼 클릭 이벤트 직접 연결 (에디터 설정 누락 방지)
        if (cardButton != null)
        {
            cardButton.onClick.RemoveAllListeners();
            cardButton.onClick.AddListener(OnClick);
        }
        
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
        Debug.Log($"[ShopCard] {currentItem?.itemName} 카드 클릭됨");

        if (isSoldOut || currentItem == null) return;

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj == null)
        {
            Debug.LogError("[ShopCard] 'Player' 태그를 가진 오브젝트를 찾을 수 없습니다!");
            return;
        }

        PlayerStats player = playerObj.GetComponent<PlayerStats>();
        if (player == null) return;

        Debug.Log($"[ShopCard] 소지 골드: {player.money} / 필요 골드: {currentItem.price}");

        // 1. 돈이 충분한지 확인
        if (player.money >= currentItem.price)
        {
            // 2. 아이템 추가 시도
            bool success = PlayerInventory.Instance.AddItem(currentItem);

            if (success)
            {
                // 3. 구매 성공 시에만 돈 차감 및 상태 변경
                player.money -= currentItem.price;
                Debug.Log($"[ShopCard] {currentItem.itemName} 구매 완료! 남은 돈: {player.money}");
                
                isSoldOut = true;
                if (cardButton != null) cardButton.interactable = false;
                if (soldOutOverlay != null) soldOutOverlay.SetActive(true);
            }
            else
            {
                Debug.LogWarning("[ShopCard] 인벤토리가 가득 차서 아이템을 추가할 수 없습니다.");
            }
        }
        else
        {
            Debug.Log("[ShopCard] 돈이 부족합니다!");
        }
    }
}
