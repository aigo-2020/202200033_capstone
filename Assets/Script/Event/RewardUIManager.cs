using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

/// <summary>
/// 1, 2, 3 스테이지 클리어 후 3개 중 1개의 무료 보상을 선택하는 UI를 관리합니다.
/// 플레이어의 카테고리 레벨 표시와 퇴장 버튼 기능이 추가되었습니다.
/// </summary>
public class RewardUIManager : MonoBehaviour
{
    public static RewardUIManager Instance { get; private set; }

    [Header("Event UI Settings")]
    public GameObject eventUIPanel; 
    public RewardCard[] rewardCards; 
    public Button exitButton;        // 추가: 퇴장 버튼

    [Header("Level Display")]
    public TextMeshProUGUI attackLevelText;
    public TextMeshProUGUI defenseLevelText;
    public TextMeshProUGUI agilityLevelText;

    [Header("SpecUp Pools")]
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

        // 퇴장 버튼 초기 설정
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(CloseEventUI);
            exitButton.interactable = false; // 보상 선택 전까지는 클릭 불가 (기본값)
        }
    }

    /// <summary>
    /// 스펙업 보상 UI를 활성화합니다.
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

        // 1. 레벨 텍스트 초기화
        UpdateLevelTexts(player);

        // 2. 각 카테고리에서 랜덤하게 하나씩 추출
        SpecUpData atkData = GetRandomFromPool(attackPool);
        SpecUpData defData = GetRandomFromPool(defensePool);
        SpecUpData agiData = GetRandomFromPool(agilityPool);

        // 3. 카드 설정
        SetupCard(0, atkData, player.GetCategoryLevel(SpecUpCategory.Attack));
        SetupCard(1, defData, player.GetCategoryLevel(SpecUpCategory.Defense));
        SetupCard(2, agiData, player.GetCategoryLevel(SpecUpCategory.Agility));

        // 4. 퇴장 버튼 리셋
        if (exitButton != null) exitButton.interactable = false;

        eventUIPanel.SetActive(true);
        Time.timeScale = 0f; 
    }

    /// <summary>
    /// 플레이어의 현재 카테고리 레벨을 UI에 갱신합니다.
    /// </summary>
    public void UpdateLevelTexts(PlayerStats player)
    {
        if (player == null) return;

        if (attackLevelText != null) attackLevelText.text = $"Lv. {player.attackLevel}";
        if (defenseLevelText != null) defenseLevelText.text = $"Lv. {player.defenseLevel}";
        if (agilityLevelText != null) agilityLevelText.text = $"Lv. {player.agilityLevel}";
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
        
        Button btn = rewardCards[index].GetComponent<Button>();
        if (btn != null)
        {
            btn.interactable = true;
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OnCardSelected(index));
        }
    }

    private void OnCardSelected(int selectedIndex)
    {
        // 1. 보상 적용
        rewardCards[selectedIndex].OnClick();

        // 2. 중복 선택 방지
        foreach (var card in rewardCards)
        {
            Button btn = card.GetComponent<Button>();
            if (btn != null) btn.interactable = false;
        }

        // 3. 레벨 텍스트 즉시 갱신
        PlayerStats player = PlayerInventory.Instance.GetComponent<PlayerStats>();
        UpdateLevelTexts(player);

        // 4. 퇴장 버튼 활성화 (보상을 골랐으니 나갈 수 있음)
        if (exitButton != null)
        {
            exitButton.interactable = true;
        }
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
