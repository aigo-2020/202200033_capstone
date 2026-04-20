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
        // 초기화 완료 및 플레이어 생존 시, 넉백 중이 아닐 때만 이동
        if (isInitialized && playerTransform != null && !isKnockingBack)
        {
            MoveTowardsPlayer();
        }
    }

    /// <summary>
    /// 플레이어 방향으로 이동 로직
    /// </summary>
    void MoveTowardsPlayer()
    {
        Vector2 direction = (Vector2)playerTransform.position - rb.position;
        direction.Normalize();
        rb.MovePosition(rb.position + direction * moveSpeed * Time.deltaTime);
    }
}
