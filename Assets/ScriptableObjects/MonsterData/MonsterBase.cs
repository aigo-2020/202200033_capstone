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

    // 넉백/반동 상태 관리
    protected bool isKnockingBack = false;
    protected float lastAttackTime = 0f;

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

        // [미래 구현 가이드: 피격 넉백 (Damage Knockback)]
        // 만약 화살이나 스킬에 맞았을 때 몬스터가 뒤로 밀리게 하고 싶다면 여기서 처리합니다.
        // 예: Vector2 knockbackDir = (transform.position - attackerPosition).normalized;
        //     ApplyKnockback(knockbackDir * force, duration);
        // 현재는 시각적/상태적 피드백만 처리하도록 설계되어 있습니다.

        // 피격 효과/애니메이션을 위한 후크 메서드 호출
        OnTakeDamage(finalDamage);

        if (currentHp <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// 플레이어와의 물리적 접촉 시 호출됩니다. (모든 몬스터 공통)
    /// </summary>
    protected virtual void OnCollisionStay2D(Collision2D collision)
    {
        if (!isInitialized || isKnockingBack) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            // 공격 속도에 따른 쿨타임 체크
            if (Time.time >= lastAttackTime + (1f / attackRate))
            {
                HandleContactAttack(collision.gameObject);
                lastAttackTime = Time.time;
            }
        }
    }

    /// <summary>
    /// 플레이어에게 데미지를 입히고 쌍방향 넉백을 적용하는 공통 로직입니다.
    /// </summary>
    protected virtual void HandleContactAttack(GameObject player)
    {
        // 1. 플레이어 데미지 입히기
        if (player.TryGetComponent<PlayerStats>(out var stats))
        {
            stats.TakeDamage(damage);
        }

        // 2. 넉백 방향 계산 (몬스터 -> 플레이어)
        Vector2 knockbackDir = (player.transform.position - transform.position).normalized;

        // 3. 플레이어 넉백 적용
        if (player.TryGetComponent<PlayerController>(out var controller))
        {
            controller.ApplyKnockback(knockbackDir * knockbackForce, 0.2f);
        }

        // 4. 몬스터 자신에게 반동(Recoil) 적용
        ApplyKnockback(-knockbackDir * recoilForce, 0.2f);

        OnAttack();
    }

    /// <summary>
    /// 몬스터에게 물리적인 힘을 가하고 잠시 동안 이동 AI를 중단시킵니다.
    /// </summary>
    public virtual void ApplyKnockback(Vector2 force, float duration)
    {
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(KnockbackCoroutine(force, duration));
        }
    }

    private System.Collections.IEnumerator KnockbackCoroutine(Vector2 force, float duration)
    {
        isKnockingBack = true;
        
        // 기존 속도 초기화 후 순간적인 힘 가함
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(force, ForceMode2D.Impulse);

        yield return new WaitForSeconds(duration);

        // 물리 상태 초기화 및 AI 재개
        rb.linearVelocity = Vector2.zero;
        isKnockingBack = false;
    }

    /// <summary>
    /// 피격 시 시각적 효과나 애니메이션 처리를 위해 자식 클래스에서 오버라이드합니다.
    /// </summary>
    protected virtual void OnTakeDamage(float damage) { }

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

        // 3. 사망 효과/애니메이션을 위한 후크 메서드 호출
        OnDie();

        Destroy(gameObject);
    }

    /// <summary>
    /// 사망 시 시각적 효과나 애니메이션 처리를 위해 자식 클래스에서 오버라이드합니다.
    /// </summary>
    protected virtual void OnDie() { }

    /// <summary>
    /// 공격 시 시각적 효과나 애니메이션 처리를 위해 자식 클래스에서 오버라이드합니다.
    /// </summary>
    protected virtual void OnAttack() { }

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
