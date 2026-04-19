using UnityEngine;
using System.Collections.Generic; // 리스트 사용을 위해 추가

public class MonsterSpawner : MonoBehaviour
{
    public static MonsterSpawner Instance { get; private set; }

    [Header("Spawn Area")]
    public Vector2 spawnAreaMin = new Vector2(-8f, -4f); // 스폰 영역의 좌하단
    public Vector2 spawnAreaMax = new Vector2(8f, 4f);   // 스폰 영역의 우상단
    public float enemyRadius = 0.5f; // 몬스터가 겹치지 않도록 체크할 반지름
    public int maxSpawnAttempts = 10; // 겹치지 않는 위치를 찾기 위한 최대 시도 횟수


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 트리거: 스테이지가 시작될 때 호출됨 (이벤트가 끝난 후, 또는 게임 첫 시작 시)
    public void SpawnEnemies()
    {
        if (StageManager.Instance == null)
        {
            Debug.LogWarning("MonsterSpawner: StageManager를 찾을 수 없습니다.");
            return;
        }

        RoundData currentRound = StageManager.Instance.GetCurrentRoundData();
        if (currentRound == null)
        {
            Debug.LogWarning("MonsterSpawner: RoundData가 설정되지 않았습니다!");
            return;
        }

        int currentStage = StageManager.Instance.currentStage;
        int stageInRound = ((currentStage - 1) % 5) + 1; // 1, 2, 3, 4, 5

        // 일반 스테이지: 예산 기반 랜덤 소환 (보스도 이제 5스테이지 전용 몬스터로 처리 가능)
        int budget = StageManager.Instance.GetCurrentStageBudget();
        
        // 이번 스테이지에 등장 가능한 몬스터 필터링
        List<MonsterData> availableMonsters = currentRound.possibleMonsters.FindAll(m => 
            stageInRound >= m.minStage && stageInRound <= m.maxStage);

        if (availableMonsters.Count == 0)
        {
            Debug.LogWarning("현재 스테이지에서 소환 가능한 몬스터가 없습니다!");
            return;
        }

        // 몬스터별 소환 횟수 추적
        Dictionary<MonsterData, int> spawnCounts = new Dictionary<MonsterData, int>();
        foreach (var m in availableMonsters) spawnCounts[m] = 0;

        Debug.Log($"스테이지 {currentStage}: 예산 {budget}으로 몬스터 소환 시작!");

        // 쇼핑 루프
        while (budget > 0)
        {
            // 남은 예산으로 살 수 있고, 소환 한도를 넘지 않은 몬스터들
            List<MonsterData> affordableList = availableMonsters.FindAll(m => 
                m.difficultyScore <= budget && spawnCounts[m] < m.maxCountPerStage);

            // 더 이상 살 수 있는 몬스터가 없으면 종료
            if (affordableList.Count == 0) break;

            // 무작위 선택
            MonsterData selected = affordableList[Random.Range(0, affordableList.Count)];

            // 소환 실행
            SpawnSingleEnemy(selected); // 매개변수를 MonsterData로 변경

            // 데이터 갱신
            budget -= selected.difficultyScore;
            spawnCounts[selected]++;
        }
    }

    void SpawnSingleEnemy(MonsterData data)
    {
        GameObject prefabToSpawn = data.monsterPrefab;
        if (prefabToSpawn == null) return;

        Vector2 spawnPos = Vector2.zero;
        bool canSpawn = false;

        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            float randomX = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
            float randomY = Random.Range(spawnAreaMin.y, spawnAreaMax.y);
            spawnPos = new Vector2(randomX, randomY);

            Collider2D hit = Physics2D.OverlapCircle(spawnPos, enemyRadius);
            
            if (hit == null)
            {
                canSpawn = true;
                break;
            }
        }

        if (canSpawn)
        {
            GameObject enemyObj = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
            
            // 몬스터 초기화 (MonsterBase를 상속받는 모든 스크립트 대응)
            if (enemyObj.TryGetComponent<MonsterBase>(out var monster))
            {
                monster.Initialize(data);
            }

            // StageManager에 몬스터 등록
            if (StageManager.Instance != null)
            {
                StageManager.Instance.RegisterEnemy();
            }
        }
        else
        {
            Debug.LogWarning("MonsterSpawner: 빈 공간을 찾지 못해 소환을 건너뜁니다.");
        }
    }

    // 인스펙터에서 스폰 영역을 씬 뷰에 초록색 사각형으로 그려주는 기능 (개발 편의성)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 center = new Vector3((spawnAreaMin.x + spawnAreaMax.x) / 2, (spawnAreaMin.y + spawnAreaMax.y) / 2, 0);
        Vector3 size = new Vector3(spawnAreaMax.x - spawnAreaMin.x, spawnAreaMax.y - spawnAreaMin.y, 0);
        Gizmos.DrawWireCube(center, size);
    }
}
