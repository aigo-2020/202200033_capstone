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
    public RewardCard[] rewardCards; // 0:공격, 1:수비, 2:민첩 순서로 배치 권장

    [Header("SpecUp Pools")]
    [Tooltip("각 카테고리에 해당하는 스펙업 데이터들을 넣어주세요.")]
    public List<SpecUpData> attackPool;
    public List<SpecUpData> defensePool;
    public List<SpecUpData> agilityPool;

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
    }

    /// <summary>
    /// 스펙업 보상 UI를 활성화합니다. 항상 공격, 수비, 민첩 선택지가 하나씩 등장합니다.
    /// </summary>
    public void ShowRewardUI()
    {
        if (eventUIPanel == null || rewardCards.Length < 3)
        {
            Debug.LogError("RewardUIManager: UI Panel이 없거나 RewardCard가 3개 미만입니다.");
            return;
        }

        PlayerStats player = PlayerInventory.Instance.GetComponent<PlayerStats>();
        if (player == null) return;

        // 1. 각 카테고리에서 랜덤하게 하나씩 추출
        SpecUpData atkData = GetRandomFromPool(attackPool);
        SpecUpData defData = GetRandomFromPool(defensePool);
        SpecUpData agiData = GetRandomFromPool(agilityPool);

        // 2. 고정된 순서(공격, 수비, 민첩)로 카드 설정
        SetupCard(0, atkData, player.GetCategoryLevel(SpecUpCategory.Attack));
        SetupCard(1, defData, player.GetCategoryLevel(SpecUpCategory.Defense));
        SetupCard(2, agiData, player.GetCategoryLevel(SpecUpCategory.Agility));

        eventUIPanel.SetActive(true);
        Time.timeScale = 0f; // 게임 일시 정지
    }

    private SpecUpData GetRandomFromPool(List<SpecUpData> pool)
    {
        if (pool == null || pool.Count == 0) return null;
        return pool[Random.Range(0, pool.Count)];
    }

    private void SetupCard(int index, SpecUpData data, int currentLevel)
    {
        if (data == null)
        {
            rewardCards[index].gameObject.SetActive(false);
            return;
        }

        rewardCards[index].gameObject.SetActive(true);
        rewardCards[index].Setup(data, currentLevel);
        
        // 버튼 클릭 이벤트 초기화 및 연결
        Button btn = rewardCards[index].GetComponent<Button>();
        if (btn != null)
        {
            btn.interactable = true;
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OnCardSelected(index));
        }
    }

    /// <summary>
    /// 카드 하나가 선택되었을 때의 처리
    /// </summary>
    private void OnCardSelected(int selectedIndex)
    {
        // 실제 스펙업 적용 (RewardCard 내부 로직 실행)
        rewardCards[selectedIndex].OnClick();

        // 모든 카드 비활성화 (중복 선택 방지)
        foreach (var card in rewardCards)
        {
            Button btn = card.GetComponent<Button>();
            if (btn != null) btn.interactable = false;
        }

        // 선택 후 일정 시간 뒤에 닫거나, '닫기' 버튼을 활성화할 수 있습니다.
        // 여기서는 즉시 혹은 약간의 딜레이 후 CloseEventUI 호출을 권장합니다.
        Invoke("CloseEventUI", 0.5f); // 0.5초 후 자동으로 UI 닫기 (선택 사항)
    }

    public void CloseEventUI()
    {
        if (eventUIPanel != null)
        {
            eventUIPanel.SetActive(false);
            Time.timeScale = 1f; // 게임 재개

            if (StageManager.Instance != null)
            {
                StageManager.Instance.NextStage();
            }
        }
    }
}
