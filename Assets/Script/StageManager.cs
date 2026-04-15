using UnityEngine;
using System.Collections.Generic;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    [Header("Stage Info")]
    public int currentStage = 1;
    private int aliveEnemyCount = 0;

    [Header("Round Management")]
    public List<RoundData> roundDataList; // 각 라운드의 데이터 에셋 리스트

    [Header("References")]
    public GameObject portalPrefab;
    public Transform portalSpawnPoint; // 포탈이 생성될 위치 (없으면 맵 중앙 0,0)

    void Awake()
    {
        // 싱글톤 패턴 설정: 다른 스크립트에서 StageManager.Instance 로 쉽게 접근 가능하도록 함
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 스테이지 전환 시 파괴되지 않도록 설정 (필요 시)
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 현재 스테이지 번호를 기반으로 어떤 라운드인지 데이터를 반환합니다.
    /// </summary>
    public RoundData GetCurrentRoundData()
    {
        if (roundDataList == null || roundDataList.Count == 0) return null;

        int roundIndex = (currentStage - 1) / 5;
        if (roundIndex >= roundDataList.Count) roundIndex = roundDataList.Count - 1; 
        
        return roundDataList[roundIndex];
    }

    /// <summary>
    /// 현재 스테이지에 할당된 난이도 예산(Budget)을 계산하여 반환합니다.
    /// </summary>
    public int GetCurrentStageBudget()
    {
        RoundData currentRound = GetCurrentRoundData();
        if (currentRound == null) return 0;

        int stageInRound = (currentStage - 1) % 5; // 0, 1, 2, 3, 4
        return currentRound.baseBudget + (stageInRound * currentRound.budgetIncrement);
    }

    void Start()
    {
        // 게임 시작 시 첫 스테이지 몬스터 소환
        if (MonsterSpawner.Instance != null)
        {
            MonsterSpawner.Instance.SpawnEnemies();
        }
    }

    // 몬스터가 생성될 때 호출 (Enemy.cs에서 호출)
    public void RegisterEnemy()
    {
        aliveEnemyCount++;
    }

    // 몬스터가 죽었을 때 호출 (Enemy.cs에서 호출)
    public void UnregisterEnemy()
    {
        aliveEnemyCount--;
        
        Debug.Log($"남은 몬스터 수: {aliveEnemyCount}");

        if (aliveEnemyCount <= 0)
        {
            StageClear();
        }
    }

    void StageClear()
    {
        Debug.Log($"스테이지 {currentStage} 클리어! 포탈을 생성합니다.");
        
        if (portalPrefab != null)
        {
            Vector3 spawnPos = portalSpawnPoint != null ? portalSpawnPoint.position : Vector3.zero;
            Instantiate(portalPrefab, spawnPos, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("StageManager: Portal Prefab이 등록되지 않았습니다.");
        }
    }

    // 다음 스테이지로 넘어가는 로직 (이벤트 UI에서 "완료" 버튼을 누를 때 호출될 예정)
    public void NextStage()
    {
        currentStage++;
        Debug.Log($"스테이지 {currentStage} 시작!");

        // 플레이어 위치 초기화 (0, -2.5)
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            player.transform.position = new Vector3(0f, -2.5f, 0f);
        }

        // 새로운 스테이지 몬스터 소환
        if (MonsterSpawner.Instance != null)
        {
            MonsterSpawner.Instance.SpawnEnemies();
        }
    }
}
