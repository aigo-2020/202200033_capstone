using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 1, 2, 3 스테이지 보상 선택 카드 하나를 관리합니다.
/// 나중에 '스펙업 데이터' 전용으로 확장될 예정입니다.
/// </summary>
public class RewardCard : MonoBehaviour
{
    [Header("UI References")]
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    
    [Header("State Settings")]
    public GameObject selectedOverlay; // 선택되었을 때 표시할 체크 표시 등 (선택 사항)

    private ItemData currentItem;
    private Button cardButton;
    private bool isSelected = false;

    void Awake()
    {
        cardButton = GetComponent<Button>();
        if (selectedOverlay != null) selectedOverlay.SetActive(false);
    }

    public void Setup(ItemData item)
    {
        currentItem = item;
        isSelected = false;
        if (cardButton != null) cardButton.interactable = true;
        if (selectedOverlay != null) selectedOverlay.SetActive(false);

        if (iconImage != null) iconImage.sprite = item.icon;
        if (nameText != null) nameText.text = item.itemName;
        if (descriptionText != null) descriptionText.text = item.description;
    }

    /// <summary>
    /// 카드 클릭 시 호출 (무료 보상이므로 즉시 획득)
    /// </summary>
    public void OnClick()
    {
        if (isSelected || currentItem == null || PlayerInventory.Instance == null) return;

        bool success = PlayerInventory.Instance.AddItem(currentItem);
        
        if (success)
        {
            Debug.Log($"{currentItem.itemName} 보상을 획득했습니다!");
            isSelected = true;
            
            // 한 번 선택하면 더 이상 누르지 못하게 함
            if (cardButton != null) cardButton.interactable = false;
            if (selectedOverlay != null) selectedOverlay.SetActive(true);

            // TODO: '3개 중 1개 선택' 규칙을 위해 다른 카드들도 비활성화해야 한다면 
            // RewardUIManager에서 이 처리를 해주는 것이 좋습니다.
        }
    }
}
