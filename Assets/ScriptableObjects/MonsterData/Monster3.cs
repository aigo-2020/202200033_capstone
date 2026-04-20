using UnityEngine;
using System.Collections;

/// <summary>
/// 돌진형 몬스터입니다. (Isaac's Dingle / L4D Charger 컨셉)
/// 평소에는 아주 느리게 추격하다가, 사거리 내에 들어오면 직선으로 빠르게 돌진합니다.
/// 돌진 중 충돌 시 더 강력한 데미지와 넉백을 줍니다.
/// </summary>
public class Monster3 : MonsterBase
{
    private enum State { Chase, Ready, Dashing, Recovery }
    [SerializeField] private State currentState = State.Chase;

    [Header("Dash Settings (Prefab Only)")]
    [SerializeField] private float dashSpeed = 15f;          // 돌진 속도
    [SerializeField] private float dashDistance = 6f;       // 최대 돌진 거리
    [SerializeField] private float detectionRange = 8f;     // 돌진 시작 사거리

    [Header("Timing (Prefab Only)")]
    [SerializeField] private float preDashDelay = 1.0f;     // 선딜레이 (조준 시간)
    [SerializeField] private float postDashDelay = 0.8f;    // 후딜레이 (멈춤 시간)

    [Header("Multipliers (Prefab Only)")]
    [SerializeField] private float dashDamageMultiplier = 2.0f;    // 돌진 중 데미지 배율
    [SerializeField] private float dashKnockbackMultiplier = 2.5f; // 돌진 중 넉백 배율

    private Vector2 dashDirection;
    private Vector2 dashStartPosition;

    protected override void Start()
    {
        base.Start();
    }

    void Update()
    {
        // 공통 초기화 확인 및 플레이어 생존 확인
        // 넉백 중일 때는 모든 행동 중단
        if (!isInitialized || playerTransform == null || isKnockingBack) return;

        switch (currentState)
        {
            case State.Chase:
                HandleChaseState();
                break;
            case State.Ready:
            case State.Dashing:
            case State.Recovery:
                // 코루틴에서 제어하므로 Update에서는 별도 이동 로직 없음
                break;
        }
    }

    /// <summary>
    /// 플레이어를 아주 느릿하게 추격하다가 사거리 내에 들어오면 돌진 준비를 시작합니다.
    /// </summary>
    private void HandleChaseState()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= detectionRange)
        {
            StartCoroutine(DashRoutine());
        }
        else
        {
            // 평상시 느린 추격 (baseMoveSpeed 사용)
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            rb.MovePosition(rb.position + direction * moveSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// 선딜 -> 돌진 -> 후딜로 이어지는 핵심 시퀀스
    /// </summary>
    private IEnumerator DashRoutine()
    {
        // 1. 선딜레이 (Ready)
        currentState = State.Ready;
        rb.linearVelocity = Vector2.zero;
        
        // 조준: 돌진 시작 시점의 플레이어 방향 고정
        dashDirection = (playerTransform.position - transform.position).normalized;
        
        // [시각적 경고(Telegraphing) 구현 지점]
        // 주석: 여기서 몬스터의 색상을 붉게 바꾸거나, 애니메이션 'Ready' 트리거를 호출할 수 있습니다.
        Debug.Log($"{gameObject.name}: 돌진 준비 시작 (선딜레이)");
        
        yield return new WaitForSeconds(preDashDelay);

        // 2. 돌진 (Dashing)
        currentState = State.Dashing;
        dashStartPosition = transform.position;
        Debug.Log($"{gameObject.name}: 돌진 시작!");

        while (currentState == State.Dashing)
        {
            // 거리 체크: 설정된 돌진 거리를 넘었는지 확인
            float traveledDistance = Vector2.Distance(dashStartPosition, transform.position);
            if (traveledDistance >= dashDistance)
            {
                break;
            }

            // 직선 이동 (방향 전환 불가)
            rb.MovePosition(rb.position + dashDirection * dashSpeed * Time.deltaTime);
            yield return null;
        }

        // 3. 후딜레이 (Recovery)
        if (currentState == State.Dashing) // 중간에 충돌로 상태가 변하지 않았을 때만 실행
        {
            yield return StartCoroutine(FinishDashRoutine());
        }
    }

    /// <summary>
    /// 돌진을 정상적 또는 조기에 종료하고 후딜레이를 적용합니다.
    /// </summary>
    private IEnumerator FinishDashRoutine()
    {
        currentState = State.Recovery;
        rb.linearVelocity = Vector2.zero;
        Debug.Log($"{gameObject.name}: 돌진 종료 (후딜레이)");

        yield return new WaitForSeconds(postDashDelay);

        currentState = State.Chase;
    }

    /// <summary>
    /// 부모 클래스의 충돌 공격을 오버라이드하여 돌진 중 가산점을 부여합니다.
    /// </summary>
    protected override void HandleContactAttack(GameObject player)
    {
        // 돌진 중인지 여부에 따라 스탯 조정
        float finalDamage = damage;
        float finalKnockback = knockbackForce;

        bool wasDashing = (currentState == State.Dashing);

        if (wasDashing)
        {
            finalDamage *= dashDamageMultiplier;
            finalKnockback *= dashKnockbackMultiplier;
            Debug.Log($"{gameObject.name}: 돌진 충돌! 강화된 데미지({finalDamage}) 및 넉백 적용");
        }

        // 1. 플레이어 데미지 입히기
        if (player.TryGetComponent<PlayerStats>(out var stats))
        {
            stats.TakeDamage(finalDamage);
        }

        // 2. 넉백 방향 계산 (몬스터 -> 플레이어)
        Vector2 kDir = (player.transform.position - transform.position).normalized;

        // 3. 플레이어 넉백 적용
        if (player.TryGetComponent<PlayerController>(out var controller))
        {
            controller.ApplyKnockback(kDir * finalKnockback, 0.2f);
        }

        // 4. 몬스터 자신에게 반동 적용 (돌진 중이면 더 크게 밀려남)
        float finalRecoil = wasDashing ? recoilForce * 1.5f : recoilForce;
        ApplyKnockback(-kDir * finalRecoil, 0.2f);

        OnAttack();

        // 돌진 중 충돌했다면 즉시 돌진 중단하고 후딜레이로 전환
        if (wasDashing)
        {
            StopAllCoroutines();
            StartCoroutine(FinishDashRoutine());
        }
    }
}
