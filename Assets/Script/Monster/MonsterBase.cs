using UnityEngine;

/// <summary>
/// 모든 몬스터의 공통 기반이 되는 추상 클래스입니다.
/// 체력 관리, 피격 판정, 골드 드랍 등의 핵심 수명 주기를 담당합니다.
/// </summary>
public abstract class MonsterBase : MonoBehaviour, IDamageable
{
    [Header("Base Stats (Initialized from MonsterData)")]
    [SerializeField] protected float currentHp;
    [SerializeField] protected float defense;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float damage;
    [SerializeField] protected float attackRate;
    [SerializeField] protected float knockbackForce;
    [SerializeField] protected float recoilForce;

    protected Rigidbody2D rb;
    protected Transform playerTransform;
    protected MonsterData data;
    protected bool isInitialized = false;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Start()
    {
        // 씬 내 플레이어 탐색
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    /// <summary>
    /// MonsterSpawner로부터 데이터를 전달받아 몬스터를 초기화합니다.
    /// </summary>
    public virtual void Initialize(MonsterData data)
    {
        this.data = data;
        currentHp = data.baseHp;
        defense = data.baseDefense;
        moveSpeed = data.baseMoveSpeed;
        damage = data.baseDamage;
        attackRate = data.baseAttackRate;
        knockbackForce = data.knockbackForce;
        recoilForce = data.recoilForce;
        
        isInitialized = true;
    }

    /// <summary>
    /// 피격 데미지 처리 (IDamageable 구현)
    /// </summary>
    public virtual void TakeDamage(float incomingDamage)
    {
        if (!isInitialized) return;

        // 최소 데미지 보장 로직 (예: 방어력이 아무리 높아도 최소 1의 데미지는 입음)
        float finalDamage = Mathf.Max(incomingDamage - defense, 1f);
        
        currentHp -= finalDamage;
        Debug.Log($"{gameObject.name} 피격! (데미지: {finalDamage}) 남은 체력: {currentHp}");

        if (currentHp <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// 몬스터 사망 시 호출됩니다. 골드 드랍 및 스테이지 관리자 해제 처리를 합니다.
    /// </summary>
    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name} 처치됨!");

        // 1. 골드 드랍 로직 실행
        DropGold();

        // 2. 스테이지 매니저에게 마리 수 감소 알림
        if (StageManager.Instance != null)
        {
            StageManager.Instance.UnregisterEnemy();
        }

        Destroy(gameObject);
    }

    /// <summary>
    /// MonsterData에 설정된 확률과 범위에 따라 플레이어에게 골드를 지급합니다.
    /// </summary>
    protected virtual void DropGold()
    {
        if (data == null) return;

        // 확률 체크
        float randomValue = Random.value; // 0.0 ~ 1.0
        if (randomValue <= data.dropChance)
        {
            // 골드량 결정
            int amount = Random.Range(data.minGold, data.maxGold + 1);
            
            // 플레이어의 money 스탯 직접 증가 (추후 골드 아이템 프리팹 드랍으로 확장 가능)
            if (playerTransform != null && playerTransform.TryGetComponent<PlayerStats>(out var stats))
            {
                stats.money += amount;
                Debug.Log($"{amount} 골드 획득! (현재 총액: {stats.money})");
            }
        }
    }
}
