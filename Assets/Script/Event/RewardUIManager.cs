using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// 1, 2, 3 스테이지 클리어 후 3개 중 1개의 무료 보상을 선택하는 UI를 관리합니다.
/// </summary>
public class RewardUIManager : MonoBehaviour
{
    public static RewardUIManager Instance { get; private set; }

    [Header("Event UI Settings")]
    public GameObject eventUIPanel; 
    public RewardCard[] rewardCards; 

    [Header("Item Database")]
    [Tooltip("보상으로 등장할 수 있는 모든 아이템 리스트")]
    public List<ItemData> allItems;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (eventUIPanel != null)
        {
            eventUIPanel.SetActive(false);
        }

        // 각 카드 버튼에 클릭 이벤트 연결
        foreach (var card in rewardCards)
        {
            Button btn = card.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(() => card.OnClick());
            }
        }
    }

    public void ShowRewardUI()
    {
        if (eventUIPanel != null)
        {
            // 1. 랜덤하게 아이템 3개 뽑기 (중복 없이)
            List<ItemData> selectedItems = GetRandomItems(3);

            // 2. 각 카드에 데이터 주입
            for (int i = 0; i < rewardCards.Length; i++)
            {
                if (i < selectedItems.Count)
                {
                    rewardCards[i].gameObject.SetActive(true);
                    rewardCards[i].Setup(selectedItems[i]);
                    
                    // 각 카드의 버튼 클릭 시 '모든 카드 비활성화' 처리를 위해 리스너 재설정
                    int index = i;
                    Button btn = rewardCards[index].GetComponent<Button>();
                    if (btn != null)
                    {
                        btn.onClick.RemoveAllListeners();
                        btn.onClick.AddListener(() => OnCardSelected(index));
                    }
                }
                else
                {
                    rewardCards[i].gameObject.SetActive(false);
                }
            }

            eventUIPanel.SetActive(true);
            Time.timeScale = 0f; 
        }
    }

    /// <summary>
    /// 카드 하나가 선택되었을 때 다른 카드들도 누르지 못하게 함 (1개만 선택 규칙)
    /// </summary>
    private void OnCardSelected(int selectedIndex)
    {
        // 1. 실제 아이템 획득 로직 실행
        rewardCards[selectedIndex].OnClick();

        // 2. 모든 카드 비활성화 (이미 하나 골랐으므로)
        foreach (var card in rewardCards)
        {
            Button btn = card.GetComponent<Button>();
            if (btn != null) btn.interactable = false;
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

    public void CloseEventUI()
    {
        if (eventUIPanel != null)
        {
            eventUIPanel.SetActive(false);
            Time.timeScale = 1f; 

            if (StageManager.Instance != null)
            {
                StageManager.Instance.NextStage();
            }
        }
    }
}
