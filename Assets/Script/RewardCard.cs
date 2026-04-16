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
    private SpecUpData currentSpecUp;
    private Button cardButton;
    private bool isSelected = false;

    void Awake()
    {
        cardButton = GetComponent<Button>();
        if (selectedOverlay != null) selectedOverlay.SetActive(false);
    }

    /// <summary>
    /// 일반 아이템 데이터를 설정합니다.
    /// </summary>
    public void Setup(ItemData item)
    {
        currentItem = item;
        currentSpecUp = null;
        isSelected = false;
        
        if (cardButton != null) cardButton.interactable = true;
        if (selectedOverlay != null) selectedOverlay.SetActive(false);

        if (iconImage != null) iconImage.sprite = item.icon;
        if (nameText != null) nameText.text = item.itemName;
        if (descriptionText != null) descriptionText.text = item.description;
    }

    /// <summary>
    /// 스펙업 데이터를 설정합니다. 현재 카테고리 레벨을 받아 수치를 계산해 표시합니다.
    /// </summary>
    public void Setup(SpecUpData specUp, int currentCategoryLevel)
    {
        currentItem = null;
        currentSpecUp = specUp;
        isSelected = false;

        if (cardButton != null) cardButton.interactable = true;
        if (selectedOverlay != null) selectedOverlay.SetActive(false);

        if (iconImage != null) iconImage.sprite = specUp.icon;
        if (nameText != null) nameText.text = specUp.specName;
        
        // 최종 증가 수치를 포함하여 설명 표시
        float finalValue = specUp.GetCalculatedValue(currentCategoryLevel);
        descriptionText.text = $"{specUp.description} (+{finalValue})";
    }

    /// <summary>
    /// 카드 클릭 시 호출 (무료 보상이므로 즉시 획득)
    /// </summary>
    public void OnClick()
    {
        if (isSelected || PlayerInventory.Instance == null) return;

        bool success = false;

        // 1. 일반 아이템인 경우
        if (currentItem != null)
        {
            success = PlayerInventory.Instance.AddItem(currentItem);
        }
        // 2. 스펙업 데이터인 경우
        else if (currentSpecUp != null)
        {
            PlayerStats player = PlayerInventory.Instance.GetComponent<PlayerStats>();
            if (player != null)
            {
                player.ApplySpecUp(currentSpecUp);
                success = true;
            }
        }
        
        if (success)
        {
            string gainedName = currentItem != null ? currentItem.itemName : currentSpecUp.specName;
            Debug.Log($"{gainedName} 보상을 획득했습니다!");
            isSelected = true;
            
            if (cardButton != null) cardButton.interactable = false;
            if (selectedOverlay != null) selectedOverlay.SetActive(true);
        }
    }
}
