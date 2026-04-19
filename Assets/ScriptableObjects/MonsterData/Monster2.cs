using UnityEngine;
using System.Collections;

/// <summary>
/// 원거리 카이팅형 몬스터입니다.
/// 플레이어와의 거리를 일정하게 유지하며(Elastic Band), 조준 후 투사체를 발사합니다.
/// </summary>
public class Monster2 : MonsterBase
{
    [Header("Ranged AI Settings")]
    public GameObject projectilePrefab;  // 발사할 투사체 프리팹
    public float minSafeDistance = 4f;    // 이보다 가까우면 도망
    public float maxAttackDistance = 7f;  // 이보다 멀면 접근
    public float preAttackDelay = 0.5f;   // 공격 전 조준 시간
    public float projectileSpeed = 8f;    // 투사체 속도

    private bool isAttacking = false;
    private float lastAttackTime = 0f;

    protected override void Start()
    {
        base.Start();
    }

    void Update()
    {
        // 초기화 완료 및 플레이어가 생존해 있을 때만 행동
        if (!isInitialized || playerTransform == null || isAttacking) return;

        HandleElasticMovement();
        CheckAttack();
    }

    /// <summary>
    /// 플레이어와의 거리에 따라 전진/후퇴를 결정하는 고무줄 이동 로직
    /// </summary>
    void HandleElasticMovement()
    {
        float distance = Vector2.Distance(transform.position, playerTransform.position);
        Vector2 direction = (playerTransform.position - transform.position).normalized;

        if (distance > maxAttackDistance)
        {
            // 너무 멀면 접근
            rb.MovePosition(rb.position + direction * moveSpeed * Time.deltaTime);
        }
        else if (distance < minSafeDistance)
        {
            // 너무 가까우면 후퇴 (접근보다 조금 느리게 도망)
            rb.MovePosition(rb.position - direction * (moveSpeed * 0.8f) * Time.deltaTime);
        }
        // 적정 거리(4~7) 내에 있으면 정지하여 공격 집중
    }

    /// <summary>
    /// 공격 조건(사거리, 쿨타임)을 체크합니다.
    /// </summary>
    void CheckAttack()
    {
        float distance = Vector2.Distance(transform.position, playerTransform.position);
        
        // 사거리 내에 있고 쿨타임이 지났을 때 공격 시작
        if (distance <= maxAttackDistance && Time.time >= lastAttackTime + (1f / attackRate))
        {
            StartCoroutine(AttackRoutine());
            lastAttackTime = Time.time;
        }
    }

    /// <summary>
    /// 조준 후 발사하는 공격 시퀀스
    /// </summary>
    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        
        // 1. 조준 단계 (정지 상태에서 대기)
        // 이 지점에 나중에 애니메이션 'Aim' 트리거를 추가하세요.
        yield return new WaitForSeconds(preAttackDelay);

        // 2. 발사 단계
        if (playerTransform != null)
        {
            LaunchProjectile();
            OnAttack(); // MonsterBase의 애니메이션 후크 호출
        }

        isAttacking = false;
    }

    /// <summary>
    /// 플레이어 방향으로 투사체를 생성하고 초기화합니다.
    /// </summary>
    void LaunchProjectile()
    {
        if (projectilePrefab == null) return;

        // 플레이어 방향 계산 및 회전값 도출
        Vector2 shootDir = (playerTransform.position - transform.position).normalized;
        float angle = Mathf.Atan2(shootDir.y, shootDir.x) * Mathf.Rad2Deg;

        // 투사체 생성
        GameObject projObj = Instantiate(projectilePrefab, transform.position, Quaternion.AngleAxis(angle, Vector3.forward));
        
        // 투사체 초기화
        if (projObj.TryGetComponent<MonsterProjectile>(out var proj))
        {
            proj.Initialize(damage, projectileSpeed);
        }
    }

    protected override void OnAttack()
    {
        // MonsterBase의 Hook 오버라이드: 나중에 여기서 공격 애니메이션 트리거 가능
        base.OnAttack(); 
    }
}
