using UnityEngine;

[CreateAssetMenu(fileName = "NewMonsterData", menuName = "ScriptableObjects/MonsterData")]
public class MonsterData : ScriptableObject
{
    [Header("몬스터 설정")]
    public GameObject monsterPrefab;      // 실제로 소환될 몬스터 프리팹
    public int difficultyScore = 2;       // 이 몬스터의 난이도 점수 (소환 비용)
    
    [Header("등장 조건 (라운드 내 스테이지 기준 1~5)")]
    [Range(1, 5)] public int minStage = 1; // 등장하기 시작하는 최소 스테이지
    [Range(1, 5)] public int maxStage = 5; // 등장이 끝나는 최대 스테이지
    
    
    [Header("제한")]
    public int maxCountPerStage = 5;      // 한 스테이지에서 이 몬스터가 최대 몇 마리까지 나올 수 있는지

    [Header("기본 스탯")]
    public float baseHp = 50f;            // 기본 체력
    public float baseDefense = 0f;        // 기본 방어력
    public float baseMoveSpeed = 2f;      // 기본 이동 속도
    public float baseDamage = 10f;        // 기본 공격력
    public float baseAttackRate = 1.0f;   // 초당 공격 횟수 (1.0 = 1초에 한 번)
    public float knockbackForce = 5f;     // 플레이어를 밀어내는 힘
    public float recoilForce = 3f;        // 공격 후 자신이 튕겨나가는 힘

    [Header("골드 드랍 설정")]
    [Range(0f, 1f)] public float dropChance = 1f; // 골드 드랍 확률 (1.0 = 100%)
    public int minGold = 1;              // 최소 골드량
    public int maxGold = 5;              // 최대 골드량
}
