using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

/// <summary>
/// 4, 5 스테이지 클리어 후 골드를 사용하여 아이템을 구매하는 상점 UI를 관리합니다.
/// 리롤(Reroll) 기능을 포함합니다.
/// </summary>
public class ShopUIManager : MonoBehaviour
{
    public static ShopUIManager Instance { get; private set; }

    [Header("Shop UI Settings")]
    public GameObject shopUIPanel;
    public ShopCard[] shopCards; 
    public Button exitButton;

    [Header("Reroll Settings")]
    public Button rerollButton;
    public TextMeshProUGUI rerollCostText;
    public int baseRerollCost = 100;         // 초기 리롤 비용
    [Range(0f, 1f)]
    public float rerollCostIncreaseRate = 0.2f; // 비용 증가율 (0.2 = 20%)

    private int currentRerollCost;

    [Header("Item Database")]
    public List<ItemData> allItems;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        ResetRerollCost(); // 게임 시작 시 비용 초기화
    }

    void Start()
    {
        if (shopUIPanel != null)
        {
            shopUIPanel.SetActive(false);
        }

        if (exitButton != null)
        {
            exitButton.onClick.AddListener(CloseShopUI);
        }

        // 리롤 버튼 이벤트 연결
        if (rerollButton != null)
        {
            rerollButton.onClick.AddListener(OnRerollButtonClicked);
        }
    }

    /// <summary>
    /// 상점 UI를 활성화하고 아이템을 채웁니다.
    /// </summary>
    public void ShowShopUI()
    {
        if (shopUIPanel != null)
        {
            RefreshShopItems();
            UpdateRerollUI();
            
            shopUIPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    /// <summary>
    /// 현재 상점의 아이템들을 새로운 랜덤 아이템으로 교체합니다.
    /// </summary>
    public void RefreshShopItems()
    {
        // shopCards 배열의 길이에 맞춰 아이템을 뽑습니다 (예: 6개)
        List<ItemData> selectedItems = GetRandomItems(shopCards.Length);

        for (int i = 0; i < shopCards.Length; i++)
        {
            if (i < selectedItems.Count)
            {
                shopCards[i].gameObject.SetActive(true);
                shopCards[i].Setup(selectedItems[i]);
            }
            else
            {
                // 뽑을 수 있는 아이템이 부족하면 카드를 비활성화
                shopCards[i].gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 리롤 버튼 클릭 시 호출됩니다.
    /// </summary>
    private void OnRerollButtonClicked()
    {
        PlayerStats player = GameObject.FindWithTag("Player")?.GetComponent<PlayerStats>();
        if (player == null) return;

        if (player.money >= currentRerollCost)
        {
            // 1. 비용 차감
            player.money -= currentRerollCost;
            
            // 2. 아이템 새로고침
            RefreshShopItems();

            // 3. 리롤 비용 상승 (복리: 현재 가격 * 1.2)
            currentRerollCost = Mathf.CeilToInt(currentRerollCost * (1f + rerollCostIncreaseRate));
            
            // 4. UI 갱신
            UpdateRerollUI();

            Debug.Log($"상점 리롤 완료! 다음 비용: {currentRerollCost}");
        }
        else
        {
            Debug.Log("리롤 비용이 부족합니다!");
        }
    }

    /// <summary>
    /// 리롤 버튼의 텍스트와 활성화 상태를 갱신합니다.
    /// </summary>
    private void UpdateRerollUI()
    {
        if (rerollCostText != null)
        {
            rerollCostText.text = $"{currentRerollCost} G";
        }

        // 돈이 부족하면 버튼 비활성화 (시각적 피드백)
        PlayerStats player = GameObject.FindWithTag("Player")?.GetComponent<PlayerStats>();
        if (player != null && rerollButton != null)
        {
            rerollButton.interactable = (player.money >= currentRerollCost);
            
            // 텍스트 색상 변경 (선택 사항)
            rerollCostText.color = (player.money >= currentRerollCost) ? Color.white : Color.red;
        }
    }

    /// <summary>
    /// 리롤 비용을 초기값으로 되돌립니다. (게임 클리어/시작 시 호출)
    /// </summary>
    public void ResetRerollCost()
    {
        currentRerollCost = baseRerollCost;
    }

    private List<ItemData> GetRandomItems(int count)
    {
        List<ItemData> result = new List<ItemData>();
        if (allItems == null || allItems.Count == 0) return result;

        // 인벤토리에 없는 아이템만 후보군으로 생성
        List<ItemData> availableItems = new List<ItemData>();
        foreach (var item in allItems)
        {
            if (PlayerInventory.Instance != null && !PlayerInventory.Instance.OwnedItems.Contains(item))
            {
                availableItems.Add(item);
            }
        }
        
        // 중복 없이 랜덤 추출
        for (int i = 0; i < count && availableItems.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, availableItems.Count);
            result.Add(availableItems[randomIndex]);
            availableItems.RemoveAt(randomIndex);
        }
        return result;
    }

    public void CloseShopUI()
    {
        if (shopUIPanel != null)
        {
            shopUIPanel.SetActive(false);
            Time.timeScale = 1f;

            if (StageManager.Instance != null)
            {
                StageManager.Instance.NextStage();
            }
        }
    }
}
