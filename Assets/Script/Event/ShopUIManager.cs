using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// 4, 5 스테이지 클리어 후 골드를 사용하여 아이템을 구매하는 상점 UI를 관리합니다.
/// </summary>
public class ShopUIManager : MonoBehaviour
{
    public static ShopUIManager Instance { get; private set; }

    [Header("Shop UI Settings")]
    public GameObject shopUIPanel;
    public ShopCard[] shopCards; // RewardCard 대신 ShopCard 사용
    public Button exitButton;      // 상점에서 나가는 버튼

    [Header("Item Database")]
    public List<ItemData> allItems;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (shopUIPanel != null)
        {
            shopUIPanel.SetActive(false);
        }

        // 나기기 버튼 설정
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(CloseShopUI);
        }
    }

    public void ShowShopUI()
    {
        if (shopUIPanel != null)
        {
            // 상점용 랜덤 아이템 뽑기
            List<ItemData> selectedItems = GetRandomItems(3);

            for (int i = 0; i < shopCards.Length; i++)
            {
                if (i < selectedItems.Count)
                {
                    shopCards[i].gameObject.SetActive(true);
                    shopCards[i].Setup(selectedItems[i]);
                    
                    // TODO: 상점 카드 클릭 시 가격 체크 로직이 RewardCard 내부에 있어야 함
                    // 현재는 RewardCard가 공짜로 인벤토리에 추가하는 방식이므로, 
                    // 나중에 Shop 전용 로직을 추가하거나 RewardCard를 수정해야 합니다.
                }
                else
                {
                    shopCards[i].gameObject.SetActive(false);
                }
            }

            shopUIPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    private List<ItemData> GetRandomItems(int count)
    {
        List<ItemData> result = new List<ItemData>();
        if (allItems == null || allItems.Count == 0) return result;

        List<ItemData> availableItems = new List<ItemData>();
        foreach (var item in allItems)
        {
            if (PlayerInventory.Instance != null && !PlayerInventory.Instance.OwnedItems.Contains(item))
            {
                availableItems.Add(item);
            }
        }
        
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
