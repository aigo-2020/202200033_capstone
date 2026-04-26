using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 스테이지 진행, 몬스터 스폰, 그리고 전투 중 발생하는 UI 이벤트를 관리합니다.
/// </summary>
public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    [Header("Stage Info")]
    public int currentStage = 1;
    private int aliveEnemyCount = 0;

    [Header("Round Management")]
    public List<RoundData> roundDataList; // 각 라운드의 데이터 에셋 리스트

    [Header("UI References")]
    public InventoryUI inventoryUI;
    public RewardUIManager rewardUI;
    public ShopUIManager shopUI;

    [Header("Stage Event Settings")]
    public List<int> rewardEventStages = new List<int> { 1, 2, 3 };
    public List<int> shopEventStages = new List<int> { 4, 5 };

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // GameManager가 Playing 상태일 때만 Tab 키로 인벤토리 열기 가능
        if (GameManager.Instance != null && GameManager.Instance.currentState == GameManager.GameState.Playing)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (inventoryUI != null) inventoryUI.ToggleStatusWindow();
            }
        }
    }

    public RoundData GetCurrentRoundData()
    {
        if (roundDataList == null || roundDataList.Count == 0) return null;
        int roundIndex = (currentStage - 1) / 5;
        if (roundIndex >= roundDataList.Count) roundIndex = roundDataList.Count - 1; 
        return roundDataList[roundIndex];
    }

    public int GetCurrentStageBudget()
    {
        RoundData currentRound = GetCurrentRoundData();
        if (currentRound == null) return 0;
        int stageInRound = (currentStage - 1) % 5;
        return currentRound.baseBudget + (stageInRound * currentRound.budgetIncrement);
    }

    void Start()
    {
        if (MonsterSpawner.Instance != null) MonsterSpawner.Instance.SpawnEnemies();
    }

    public void RegisterEnemy() => aliveEnemyCount++;

    public void UnregisterEnemy()
    {
        aliveEnemyCount--;
        Debug.Log($"남은 몬스터 수: {aliveEnemyCount}");
        if (aliveEnemyCount <= 0) StageClear();
    }

    void StageClear()
    {
        Debug.Log($"스테이지 {currentStage} 클리어! 이벤트를 결정합니다.");
        
        if (rewardEventStages.Contains(currentStage))
        {
            if (rewardUI != null) rewardUI.ShowRewardUI();
        }
        else if (shopEventStages.Contains(currentStage))
        {
            if (shopUI != null) shopUI.ShowShopUI();
        }
        else
        {
            NextStage();
        }
    }

    public void NextStage()
    {
        currentStage++;
        Debug.Log($"스테이지 {currentStage} 시작!");

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null) player.transform.position = new Vector3(0f, -2.5f, 0f);

        if (MonsterSpawner.Instance != null) MonsterSpawner.Instance.SpawnEnemies();
    }
}
