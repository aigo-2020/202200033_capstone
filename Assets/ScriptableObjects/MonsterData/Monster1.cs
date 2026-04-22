using UnityEngine;

/// <summary>
/// 가장 기초적인 근접형 몬스터입니다.
/// MonsterBase를 상속받아 플레이어를 추격합니다.
/// 접촉 공격 및 넉백은 부모 클래스(MonsterBase)에서 공통 처리합니다.
/// </summary>
public class Monster1 : MonsterBase
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    void Update()
    {
        // 초기화 완료 및 플레이어 생존 시, 넉백 중이 아닐 때만 이동 로직 수행
        if (isInitialized && playerTransform != null && !isKnockingBack)
        {
            MoveTowardsPlayer();
        }
        else if (isInitialized && !isKnockingBack)
        {
            // 이동 조건이 아니거나 플레이어가 없을 때는 멈춤 (넉백 중에는 건드리지 않음)
            rb.linearVelocity = Vector2.zero;
        }
    }

    /// <summary>
    /// 플레이어 방향으로 물리 속도를 부여하여 이동합니다.
    /// </summary>
    void MoveTowardsPlayer()
    {
        Vector2 direction = (Vector2)playerTransform.position - rb.position;
        direction.Normalize();

        // rb.MovePosition(좌표 이동) 대신 rb.linearVelocity(물리 속도)를 사용합니다.
        // 이렇게 하면 물리 엔진이 속도를 감지하여 애니메이션 스크립트가 정상 작동합니다.
        rb.linearVelocity = direction * moveSpeed;
    }
}
